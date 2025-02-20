using Munitions.ModularMissiles.Descriptors.Warheads;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Runtime;
using Game.Reports;
using Munitions.InstancedDamagers;
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
public class BeamWarheadDescriptor : BaseWarheadDescriptor, IFuse
{
    
    public float WeightedSocketSize => base._weightedSocketSize;
    IDamageCharacteristic DamageCharacteristic => new RangeBasedDamageCharacteristic(this);
    public override float ArmorPenetration => DamageCharacteristic.ArmorPenetration;
    public override float ComponentDamage => DamageCharacteristic.ComponentDamage;
    public override float TotalComponentDamagePotential => BeamCount * ComponentDamage;
    [Header("Beam Warhead FX")]
    public WeaponEffectSet WepEffects = null;
    public override WeaponEffectSet GetEffectSet(int setIndex) => WepEffects;
    public GameObject BeamPrefab;
    public List<GameObject> ExplosionPrefabs;

    //public bool SelectRandomPointInTarget = false;
    //public readonly bool _bulletLook = false;
    [Header("Flavor Blobs")]
    public string Summary = "BEAM";
    public string DetailedSummary = "Multiple Beam Warhead";
    [Header("IDamageCharacteristic Damage Values")]
    public AnimationCurve BeamArmorPenetrationRangeScaling = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(10f, 0.5f));
    public AnimationCurve BeamArmorPenetrationSizeScaling = new AnimationCurve(new Keyframe(0f, 1000f), new Keyframe(10f, 100f));
    public float InternalPenetrationDepth = 1000;
    public AnimationCurve BeamComponentDamageRangeScaling = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(10f, 0.5f));
    public AnimationCurve BeamComponentDamageSizeScaling = new AnimationCurve(new Keyframe(0f, 1000f), new Keyframe(10f, 100f));
    public float HeatDamage = new ();
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
    public Vector3 FuseDimensions = Vector3.one;
    public bool Omnidirectional = false;
    public float BeamLength = 2000;
    public float BeamCount = 100;
    public float PKill = 1;
    public CastType CastType = CastType.Ray;
    public DamageSpreadingMethod SpreadingMethod => DamageSpreadingMethod.EvenSpread;
    public float LaunchAngle = 5;    
    
    public override string GetFormattedDescription()
    {
        
        string output = "";
        output += $"Beam Length: {BeamLength * 10f:N0} m\n";
        output += $"Armor: {ArmorPenetration:N0}cm\n";
        output += $"Component: {ComponentDamage:N0}hp\n";
        output += $"Beam Length: {BeamLength   * 10f:N0} m\n";
        output += $"Angle:       {LaunchAngle:N0} degrees\n";
        return output;
    }

    Poolable SpawnEffect(GameObject prefab, Vector3 position, Quaternion rotation, RuntimeMissileWarhead runtime)
    {
        Poolable poolable = ObjectPooler.Instance.GetNextOrNew(prefab, position, rotation);
        //ShortDurationEffect sde = poolable?.GetComponent<ShortDurationEffect>();
        foreach (ILocalImbued imbued in poolable.gameObject.GetComponentsInChildren<ILocalImbued>())
        {
            imbued.ImbueLocal(Common.GetVal<ShipController>(runtime.Missile, "_localLaunchedFrom"));
            imbued.SetWeaponReportPath(Common.GetVal<IWeaponStatReportReceiver>(runtime.Missile, "_reportTo"));
        }
        if (poolable.GetComponent<ModularEffect>() == null)
            foreach (IEffectModule effect in poolable.gameObject.GetComponentsInChildren<IEffectModule>())
                effect.Play();
        return poolable;
    }

    public HitResult Explode(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo)
    {

        HitResult finalresult = HitResult.None;
        if (BeamPrefab == null)
            Debug.LogError("No Beam Prefab");
        if (BeamPrefab?.GetComponentInChildren<LineBeamMuzzleEffects>() == null)
            Debug.LogError("Beam Prefab has no LineBeamMuzzleEffects");

        //steDebug.LogError("Explode");
        foreach (GameObject ExplosionPrefab in ExplosionPrefabs)
            SpawnEffect(ExplosionPrefab, runtime.transform.position, runtime.transform.rotation, runtime);

        for (int i = 0; i < BeamCount; i++)
        {

            //Vector3 toTarget = runtime.transform.position.To(hitInfo.Point).normalized;
            Vector3 randomdirection = MathHelpers.RandomRayInCone(runtime.transform.forward, LaunchAngle);
            Ray r = new Ray(runtime.transform.position, randomdirection);
            LineBeamMuzzleEffects effects = SpawnEffect(BeamPrefab, runtime.transform.position, Quaternion.LookRotation(randomdirection), runtime)?.GetComponentInChildren<LineBeamMuzzleEffects>();
            effects?.StartEffect();
            effects?.SetBeamLength(BeamLength);
            //if (beameffect == null) { Debug.LogError("Beam Effect Failed to spawn"); }

            if (Physics.Raycast(r, out var rayHit, BeamLength, 524801, QueryTriggerInteraction.Ignore))
            {


                effects?.SetBeamLength(rayHit.distance);
                effects?.PositionHitEffect(on: true, r.GetPoint(rayHit.distance), null);

                MunitionHitInfo shrapnelHit = MunitionsHelpers.RaycastHitToMunitionHit(rayHit, runtime.transform.position.To(rayHit.point).normalized);

                if (shrapnelHit.HitCollider is MeshCollider)
                {
                    shrapnelHit.HitUV = hitObject.SampleUV(shrapnelHit.HitCollider as MeshCollider, shrapnelHit.LocalPoint, shrapnelHit.LocalNormal);
                }
                HitResult hitRes = hitObject.DoDamage(shrapnelHit, MakeDamageDealer(rayHit.distance), out float damage, out bool destroyedreport);
                //if(damage > 0)
                //    Debug.LogError("Size " + this.WeightedSocketSize + "Beam at distance " + rayHit.distance + " does " + damage + " damage out of " + MakeDamageDealer(rayHit.distance).ComponentDamage);
                if (finalresult < hitRes)
                    finalresult = hitRes;
                runtime.ReportDamageDone(hitRes, damage, destroyedreport);
                //DoImpactEffect(shrapnelHit, hitRes);
                shrapnelHit.Dispose();
            }
            else
            {
                //effects?.HitEffectPlaying = false;
                effects?.PositionHitEffect(on: true, runtime.transform.position + (randomdirection * BeamLength), null);
            }
        }

        runtime.Missile.DoImpactEffect(finalresult, runtime.transform.position, Quaternion.LookRotation(hitInfo.HitNormal), null);

        return finalresult;
    }

    private IEnumerator CoroutineDelayedDamage(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo)
    {
        yield return new WaitForSeconds(FuseDelay);
        Explode(runtime, hitObject, hitInfo);

    }


    public override HitResult TriggerDetonate(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo, out bool noVfx)
    {
        //Debug.LogError("POP");

        //GameObject cubes = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cubes.transform.position = runtime.transform.position;
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = hitInfo.Point;
        noVfx = false;
        return Explode(runtime, hitObject, hitInfo);
        
        /*
        if (FuseDelay > 0.01f)
        {
            runtime.StartCoroutine(CoroutineDelayedDamage(runtime, hitObject, hitInfo));
            return HitResult.None;

            for (int i = 0; i < 10; i++)
            {
                if(Vector3.Angle(runtime.transform.forward, hitObject.GameObj.transform.position - runtime.transform.position) < LaunchAngle)
                    return HitResult.Penetrated;
                Ray r = new Ray(runtime.transform.position, MathHelpers.RandomRayInCone(runtime.transform.forward, LaunchAngle));
                if (Physics.Raycast(r, out var rayHit, BeamLength, 524801, QueryTriggerInteraction.Ignore))
                    return HitResult.Penetrated;
            }

            return HitResult.None;
        }
        */

    }


    public override HitResult CollisionDetonate(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo) => TriggerDetonate(runtime, hitObject, hitInfo, out bool _);


    public override void FinalSetup(ModularMissile missile)
    {
        //Debug.LogError("Setup");
        base.FinalSetup(missile);
        //Debug.LogError("Dims: " + FuseDims + " Target: " + Vector3.one * 100);

        missile.SpawnProximityFuze(FuseDimensions);//Fusedimensions * 10 * (AoeRadius / 10) 100
        missile.AddRuntimeBehaviour<RuntimeMissileWarhead>(this);
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

    public override string GetSummarySegment() => Summary;
    public override string GetDetailSummarySegment() => DetailedSummary;


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