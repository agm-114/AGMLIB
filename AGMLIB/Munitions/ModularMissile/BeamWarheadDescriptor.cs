using Lib.Munitions.Damagers;
using Munitions.ModularMissiles.Runtime;

public class RuntimeTimeFuse : MonoBehaviour
{
    // A delayed fuse must not survive repooling into another launch.
    private void OnDisable() => Destroy(this);
}

[CreateAssetMenu(fileName = "New Beam Missile Warhead", menuName = "Nebulous/Missiles/Warhead/Beam Warhead")]
public class BeamWarheadDescriptor : AngleWarheadDescriptor, IFuse
{
    private const int DamageMask = 524801;
    public override float ArmorPenetration => DamageCharacteristic.ArmorPenetration;
    public override float ComponentDamage => DamageCharacteristic.ComponentDamage;
    public override float TotalComponentDamagePotential => EffectiveBeamCount * ComponentDamage;
    [Header("Beam Warhead FX")]
    public GameObject BeamPrefab;
    [Header("IDamageCharacteristic Damage Values")]
    public AnimationCurve BeamArmorPenetrationRangeScaling = new(new Keyframe(0f, 1f), new Keyframe(10f, 0.5f));
    public AnimationCurve BeamArmorPenetrationSizeScaling = new(new Keyframe(0f, 1000f), new Keyframe(10f, 100f));
    public AnimationCurve BeamComponentDamageRangeScaling = new(new Keyframe(0f, 1f), new Keyframe(10f, 0.5f));
    public AnimationCurve BeamComponentDamageSizeScaling = new(new Keyframe(0f, 1000f), new Keyframe(10f, 100f));
    [Header("Beam Specfic Values")]
    public float FuseDelay = 0f;
    public float BeamLength = 2000;
    public SingleRayDamagerSettings? DamagerSettings;
    public bool ConeFrag = false;

    public override void GetWarheadStatsBlock(ref List<(string, string)> rows)
    {
        base.GetWarheadStatsBlock(ref rows);
        rows.Add(("Beam Length", $"{BeamLength * 10f:N0} m"));
        rows.Add(("Armor", $"{ArmorPenetration:N0}cm"));
        rows.Add(("Component", $"{ComponentDamage:N0}hp"));
    }

    public override HitResult DoRay(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo, Vector3 direction, Ray ray)
    {
        bool hit = Physics.Raycast(ray, out RaycastHit rayHit, BeamLength, DamageMask, QueryTriggerInteraction.Ignore);
        float length = hit ? rayHit.distance : BeamLength;
        if (runtime.isServer && BeamPrefab != null)
        {
            // Preserve the server's direction and endpoint; clients do not roll another ray
            // or repeat its damage. The native surface index carries the float length bits.
            ((LookaheadMunitionBase)runtime.Missile).Internals().RpcRollImpactEffect(hit ? HitResult.Stopped : HitResult.None,
                AngleWarheadNetworkEffectsPatch.BeamEffect, BitConverter.ToInt32(BitConverter.GetBytes(length), 0),
                ray.origin, Quaternion.LookRotation(direction), null!);
        }
        if (DebugMode)
            DrawDebugRay(ray, length, hit);
        IDamageable? damageable = hit ? rayHit.collider.GetComponentInParent<IDamageable>() : null;
        if (damageable == null)
            return HitResult.None;

        MunitionHitInfo beamHit = MunitionsHelpers.RaycastHitToMunitionHit(rayHit, direction, damageable.GameObj);
        try
        {
            if (beamHit.HitCollider is MeshCollider mesh)
                beamHit.HitUV = damageable.SampleUV(mesh, beamHit.LocalPoint, beamHit.LocalNormal);
            HitResult result = damageable.DoDamage(beamHit, MakeDamageDealer(length), out float damage, out bool destroyed);
            runtime.ReportDamageDone(result, damage, destroyed);
            return result;
        }
        finally
        {
            beamHit.Dispose();
        }
    }

    internal void SpawnPresentationBeam(Vector3 position, Quaternion rotation, float length, bool hit, RuntimeMissileWarhead runtime)
    {
        if (BeamPrefab == null || HasDamageDealer(BeamPrefab))
            return;
        LineBeamMuzzleEffects? effects = SpawnEffect(BeamPrefab, position, rotation, runtime)?.GetComponentInChildren<LineBeamMuzzleEffects>();
        if (effects == null)
            return;
        effects.StartEffect();
        effects.SetBeamLength(length);
        effects.PositionHitEffect(hit, position + rotation * Vector3.forward * length, null);
    }

    private static void DrawDebugRay(Ray ray, float length, bool hit)
    {
        GameObject lineObject = new("BeamLine");
        Destroy(lineObject, 30);
        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.startWidth = line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = line.endColor = hit ? Color.red : Color.blue;
        line.positionCount = 2;
        line.SetPosition(0, ray.origin);
        line.SetPosition(1, ray.GetPoint(length));
    }

    public override HitResult Explode(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo)
    {
        if (!ConeFrag)
            return HitResult.None;
        HitResult result = HitResult.None;
        MunitionsHelpers.HitAllDamageableInArea(runtime.transform.position, runtime.Missile.gameObject, BeamLength, DamageMask,
            (collider, damageable) =>
            {
                Vector3 direction = runtime.transform.position.To(collider.transform.position).normalized;
                if (Vector3.Angle(runtime.transform.forward, direction) > EffectiveLaunchAngle)
                    return;
                Vector3 point = collider.ClosestPoint(runtime.transform.position);
                MunitionHitInfo fragmentHit = MunitionHitInfo.Take();
                try
                {
                    fragmentHit.HitObject = damageable.GameObj;
                    fragmentHit.HitCollider = collider;
                    fragmentHit.Point = point;
                    fragmentHit.LocalPoint = damageable.GameObj.transform.InverseTransformPoint(point);
                    fragmentHit.HitNormal = direction;
                    fragmentHit.Normal = -direction;
                    fragmentHit.LocalHitNormal = damageable.GameObj.transform.InverseTransformDirection(direction);
                    fragmentHit.LocalNormal = -fragmentHit.LocalHitNormal;
                    if (collider is MeshCollider mesh)
                        fragmentHit.HitUV = damageable.SampleUV(mesh, fragmentHit.LocalPoint, fragmentHit.LocalNormal);
                    HitResult hitResult = damageable.DoDamage(fragmentHit, MakeDamageDealer(Vector3.Distance(runtime.transform.position, point)),
                        out float damage, out bool destroyed);
                    runtime.ReportDamageDone(hitResult, damage, destroyed);
                    if (hitResult > result)
                        result = hitResult;
                }
                finally
                {
                    fragmentHit.Dispose();
                }
            });
        return result;
    }

    protected virtual IDamageDealer MakeDamageDealer(float range = 0) => DamageSettings.MakeDamageDealer(this, range);
    private SingleRayDamagerSettings DamageSettings => DamagerSettings ??= ScriptableObject.CreateInstance<SingleRayDamagerSettings>();
    private IDamageCharacteristic DamageCharacteristic => DamageSettings.MakeDamageCharacteristic(this);

    public IEnumerator CoroutineOnTriggerEnter(LookaheadMunitionBase missile, Collider other, float delay)
    {
        try
        {
            yield return new WaitForSeconds(delay);
            if (missile == null || missile.IsDead || !missile.isServer || other == null || other.isTrigger)
                yield break;
            GameObject hitObject = other.transform.root.gameObject;
            MunitionHitInfo hitInfo = MunitionHitInfo.Take();
            try
            {
                hitInfo.HitObject = hitObject;
                hitInfo.HitCollider = other;
                hitInfo.Point = other.ClosestPoint(missile.transform.position);
                hitInfo.LocalPoint = hitObject.transform.InverseTransformPoint(hitInfo.Point);
                hitInfo.Normal = other.transform.position.To(missile.transform.position).normalized;
                hitInfo.LocalNormal = hitObject.transform.InverseTransformDirection(hitInfo.Normal);
                hitInfo.HitNormal = missile.LastVelocity.normalized;
                hitInfo.LocalHitNormal = hitObject.transform.InverseTransformDirection(hitInfo.HitNormal);
                missile.ProcessCollision(hitInfo, trigger: true);
            }
            finally
            {
                hitInfo.Dispose();
            }
        }
        finally
        {
            if (missile != null)
                Destroy(missile.GetComponent<RuntimeTimeFuse>());
        }
    }
}

[HarmonyPatch(typeof(LookaheadMunitionBase), nameof(LookaheadMunitionBase.OnTriggerEnter))]
internal static class LookaheadMunitionOnTriggerEnter
{
    private static bool Prefix(LookaheadMunitionBase __instance, Collider other)
    {
        RuntimeMissileWarhead runtime = __instance.GetComponent<RuntimeMissileWarhead>();
        if (runtime == null || runtime.Internals().Descriptor is not BeamWarheadDescriptor descriptor ||
            descriptor.FuseDelay <= 0 || __instance.IsDead || !__instance.isServer || other == null || other.isTrigger)
            return true;
        if (__instance.GetComponent<RuntimeTimeFuse>() == null)
        {
            __instance.gameObject.AddComponent<RuntimeTimeFuse>();
            __instance.StartCoroutine(descriptor.CoroutineOnTriggerEnter(__instance, other, descriptor.FuseDelay));
        }
        return false;
    }
}
