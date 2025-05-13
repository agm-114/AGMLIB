using Munitions.InstancedDamagers;
using Munitions.ModularMissiles.Descriptors.Warheads;
using Munitions.ModularMissiles.Runtime;
using System.Runtime.InteropServices;

public class RangeBasedDamageCharacteristic : IDamageCharacteristic
{
    BeamWarheadDescriptor Warhead;
    public float Range = 0;
    public RangeBasedDamageCharacteristic(BeamWarheadDescriptor warhead, float range = 0)
    {
        Warhead = warhead;
        Range = range;
    }
    float IDamageCharacteristic.ArmorPenetration => Warhead.BeamArmorPenetrationSizeScaling.Evaluate(Warhead.WeightedSocketSize) * Warhead.BeamArmorPenetrationRangeScaling.Evaluate(Range);
    float IDamageCharacteristic.ComponentDamage => Warhead.BeamComponentDamageSizeScaling.Evaluate(Warhead.WeightedSocketSize) * Warhead.BeamComponentDamageRangeScaling.Evaluate(Range);
    float IDamageCharacteristic.HeatDamage => Warhead.HeatDamage;
    float IDamageCharacteristic.DamageBrushSize => Warhead.DamageBrushSize;
    float IDamageCharacteristic.OverpenetrationDamageMultiplier => Warhead.OverpenetrationDamageMultiplier;
    float IDamageCharacteristic.RandomEffectMultiplier => Warhead.RandomEffectMultiplier;
    float IDamageCharacteristic.CrewVulnerabilityMultiplier => Warhead.CrewVulnerabilityMultiplier;
    float? IDamageCharacteristic.MaxPenetrationDepth => Warhead.InternalPenetrationDepth;
    bool IDamageCharacteristic.NeverCrit => Warhead.NeverCrit;
    bool IDamageCharacteristic.AlwaysSpreadThroughStructure => Warhead.AlwaysSpreadThroughStructure;
    bool IDamageCharacteristic.NeverRicochet => Warhead.NeverRicochet;
    bool IDamageCharacteristic.IgnoreEffectiveThickness => Warhead.IgnoreEffectiveThickness;

    bool IDamageCharacteristic.NeverOverpen => Warhead.NeverOverpen;
}


public class RuntimeTimeFuse : MonoBehaviour
{
    /*
    public class RuntimeTimeFuseState : RuntimeMissileBehaviourState
    {
        public int FuseTime;
    }
    protected override void FillSaveState(RuntimeMissileBehaviourState state) { return; }
    protected override RuntimeMissileBehaviourState NewSaveStateInstance() { return new RuntimeTimeFuseState(); }
    protected override void RestoreSaveState(RuntimeMissileBehaviourState state) { }
    */
}

[CreateAssetMenu(fileName = "New Beam Missile Warhead", menuName = "Nebulous/Missiles/Warhead/Beam Warhead")]
public class BeamWarheadDescriptor : AngleWarheadDescriptor, IFuse
{

    IDamageCharacteristic DamageCharacteristic => new RangeBasedDamageCharacteristic(this);
    public override float ArmorPenetration => DamageCharacteristic.ArmorPenetration;
    public override float ComponentDamage => DamageCharacteristic.ComponentDamage;
    public override float TotalComponentDamagePotential => BeamCount * ComponentDamage;
    [Header("Beam Warhead FX")]

    public GameObject BeamPrefab;


    //public bool SelectRandomPointInTarget = false;
    //public readonly bool _bulletLook = false;

    [Header("IDamageCharacteristic Damage Values")]
    public AnimationCurve BeamArmorPenetrationRangeScaling = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(10f, 0.5f));
    public AnimationCurve BeamArmorPenetrationSizeScaling = new AnimationCurve(new Keyframe(0f, 1000f), new Keyframe(10f, 100f));
    public float InternalPenetrationDepth = 1000;
    public AnimationCurve BeamComponentDamageRangeScaling = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(10f, 0.5f));
    public AnimationCurve BeamComponentDamageSizeScaling = new AnimationCurve(new Keyframe(0f, 1000f), new Keyframe(10f, 100f));
    public float HeatDamage = new();
    public float DamageBrushSize = new();
    public float OverpenetrationDamageMultiplier = new();
    public float RandomEffectMultiplier = new();
    public float CrewVulnerabilityMultiplier = new();
    [Header("IDamageCharacteristic Damage Flags")]
    public bool NeverCrit = new();
    public bool NeverOverpen = new();
    public bool IgnoreEffectiveThickness = new();
    public bool NeverRicochet = new();
    public bool AlwaysSpreadThroughStructure = new();
    public bool IgnoreDamageReduction = false;
    [Header("Beam Specfic Values")]
    public float FuseDelay = 0.0f;
    public float BeamLength = 2000;

    public float PKill = 1;

    public CastType CastType = CastType.Ray;
    public DamageSpreadingMethod SpreadingMethod => DamageSpreadingMethod.EvenSpread;

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
        //Vector3 toTarget = runtime.transform.position.To(hitInfo.Point).normalized;

        LineBeamMuzzleEffects? effects = SpawnEffect(BeamPrefab, runtime.transform.position, Quaternion.LookRotation(direction), runtime)?.GetComponentInChildren<LineBeamMuzzleEffects>();
        effects?.StartEffect();
        effects?.SetBeamLength(BeamLength);
        if (effects == null) { Debug.LogError("Beam Effect Failed to spawn"); }

        LineRenderer? lineRenderer = null;
        if (DebugMode)
        {
            //Debug.LogError("Beam Spawn");
            GameObject lineObj = new GameObject("BeamLine");
            Destroy(lineObj, 30);
            lineRenderer = lineObj.AddComponent<LineRenderer>();

            // Configure LineRenderer settings
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Use a simple shader
            lineRenderer.startColor = Color.blue;
            lineRenderer.endColor = Color.blue;

            // Set the positions to visualize the ray
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, runtime.transform.position);
            lineRenderer?.SetPosition(1, runtime.transform.position + direction * BeamLength);

        }

        if (Physics.Raycast(ray, out var rayHit, BeamLength, 524801, QueryTriggerInteraction.Ignore))
        {


            effects?.SetBeamLength(rayHit.distance);
            lineRenderer?.SetPosition(1, rayHit.point);
            if (lineRenderer != null)
            {
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;
            }

            effects?.PositionHitEffect(on: true, ray.GetPoint(rayHit.distance), null);

            MunitionHitInfo shrapnelHit = MunitionsHelpers.RaycastHitToMunitionHit(rayHit, runtime.transform.position.To(rayHit.point).normalized);

            if (shrapnelHit.HitCollider is MeshCollider)
            {
                shrapnelHit.HitUV = hitObject.SampleUV(shrapnelHit.HitCollider as MeshCollider, shrapnelHit.LocalPoint, shrapnelHit.LocalNormal);
            }
            HitResult hitRes = hitObject.DoDamage(shrapnelHit, MakeDamageDealer(rayHit.distance), out float damage, out bool destroyedreport);
            //if(damage > 0)
            //    Debug.LogError("Size " + this.WeightedSocketSize + "Beam at distance " + rayHit.distance + " does " + damage + " damage out of " + MakeDamageDealer(rayHit.distance).ComponentDamage);

            runtime.ReportDamageDone(hitRes, damage, destroyedreport);
            //DoImpactEffect(shrapnelHit, hitRes);
            shrapnelHit.Dispose();
            return hitRes;
        }
        else
        {
            //effects?.HitEffectPlaying = false;
            effects?.PositionHitEffect(on: true, runtime.transform.position + (direction * BeamLength), null);
            return HitResult.None;
        }
    }



    public override HitResult Explode(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo)
    {
        //Debug.LogError($"Explode {BeamCount}");
        HitResult finalresult = HitResult.None;
        if (BeamPrefab == null)
            Debug.LogError("No Beam Prefab");
        if (BeamPrefab?.GetComponentInChildren<LineBeamMuzzleEffects>() == null)
            Debug.LogError("Beam Prefab has no LineBeamMuzzleEffects");

        //steDebug.LogError("Explode");
        MunitionsHelpers.HitAllDamageableInArea(hitInfo.Point, runtime.Missile.transform.root.gameObject, BeamLength, 524801, delegate (Collider hit, IDamageable damageable)
        {

            if (Vector3.Angle(runtime.Missile.Velocity.normalized, hit.attachedRigidbody.velocity.normalized) > EffectiveLaunchAngle && ConeFrag)
            {
                float damageDone2;
                bool destroyed;

                HitResult hitResult = damageable.DoDamage(hitInfo, this as IDamageDealer, out damageDone2, out destroyed);
                if (hitResult != 0)
                {
                    runtime.ReportDamageDone(hitResult, damageDone2, destroyed);
                }
            }
        });


        return finalresult;
    }

    private IEnumerator CoroutineDelayedDamage(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo)
    {
        yield return new WaitForSeconds(FuseDelay);
        Explode(runtime, hitObject, hitInfo);

    }

    protected virtual IDamageDealer MakeDamageDealer(float range = 0)
    {
        int _rayAngle = 1;
        int _rayCount = 1;
        IDamageCharacteristic characteristic = new RangeBasedDamageCharacteristic(this, range);
        if (CastType == CastType.Sphere)
        {
            float _spherecastRadius = 1;
            return new SingleSpherecastDamager(characteristic, _spherecastRadius, SpreadingMethod, alwaysSpreadDamage: false, null, IgnoreDamageReduction);
        }
        if (CastType == CastType.SpallingRay)
        {
            return new SpallingRayDamager(characteristic, _rayAngle, _rayCount, SpreadingMethod, alwaysSpreadDamage: false, null, IgnoreDamageReduction);
        }
        if (CastType == CastType.RayCone)
        {
            return new MultiRayConeDamager(characteristic, ComponentDamage / (float)_rayCount, _rayAngle, null, IgnoreDamageReduction);
        }
        return new SingleRayDamager(characteristic, SpreadingMethod, alwaysSpreadDamage: false, null, IgnoreDamageReduction);
    }




    private static ColliderComparer _colliderComparer;

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    private struct ColliderComparer : IEqualityComparer<Collider>
    {
        public bool Equals(Collider x, Collider y)
        {
            return x.transform.root.gameObject == y.transform.root.gameObject;
        }

        public int GetHashCode(Collider obj)
        {
            return ((object)obj).GetHashCode();
        }
    }

    public IEnumerator CoroutineOnTriggerEnter(LookaheadMunitionBase __instance, Collider other, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(__instance.gameObject.GetComponent<RuntimeTimeFuse>());

        GameObject hitObject = other.transform.root.gameObject;
        MunitionHitInfo hitInfo = MunitionHitInfo.Take();
        hitInfo.HitObject = hitObject;
        hitInfo.HitCollider = other;
        hitInfo.Point = other.ClosestPoint(__instance.transform.position);
        hitInfo.LocalPoint = hitObject.transform.InverseTransformPoint(hitInfo.Point);
        hitInfo.Normal = other.transform.position.To(__instance.transform.position);
        hitInfo.LocalNormal = hitObject.transform.InverseTransformDirection(hitInfo.Normal);
        hitInfo.HitNormal = __instance.LastVelocity.normalized;
        hitInfo.LocalHitNormal = hitObject.transform.InverseTransformDirection(hitInfo.HitNormal);
        __instance.ProcessCollision(hitInfo, trigger: true);
        hitInfo.Dispose();

    }

}

[HarmonyPatch(typeof(LookaheadMunitionBase), nameof(LookaheadMunitionBase.OnTriggerEnter))]
class LookaheadMunitionOnTriggerEnter
{
    static bool Prefix(LookaheadMunitionBase __instance, Collider other)
    {
        Common.LogPatch();
        RuntimeMissileWarhead AR = __instance?.GetComponent<RuntimeMissileWarhead>();
        if (AR == null)
            return true;
        if (Common.GetVal<BaseWarheadDescriptor>(AR, "_descriptor") is not BeamWarheadDescriptor beamWarheadDescriptor)
            return true;




        if (beamWarheadDescriptor.FuseDelay > 0 && !__instance.IsDead && __instance.isServer && !other.isTrigger)
        {
            if (__instance.GetComponent<RuntimeTimeFuse>() != null)
                return false;
            __instance.gameObject.AddComponent<RuntimeTimeFuse>();
            __instance.StartCoroutine(beamWarheadDescriptor.CoroutineOnTriggerEnter(__instance, other, beamWarheadDescriptor.FuseDelay));
            return false;
        }
        return true;
    }
}

/*

*/