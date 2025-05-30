﻿using Game.Reports;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Descriptors.Warheads;
using Munitions.ModularMissiles.Runtime;

abstract public class AngleWarheadDescriptor : BaseWarheadDescriptor
{
    [Header("Flavor Blobs")]
    public string Summary = "CODE";
    public string DetailedSummary = "Description Below Code";
    public override string GetSummarySegment() => Summary;
    public override string GetDetailSummarySegment() => DetailedSummary;
    public WeaponEffectSet WepEffects = null;
    public override WeaponEffectSet GetEffectSet(int setIndex) => WepEffects;

    [Header("Angle / Spawn Cone")]
    public AnimationCurve BeamAngleScaling = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

    public float WeightedSocketSize => base._weightedSocketSize;
    public bool ProxFuse = true;
    public Vector3 FuseDimensions = Vector3.one;
    public bool Omnidirectional = false;
    public float LaunchAngle = 5;
    public float EffectiveLaunchAngle => LaunchAngle * BeamAngleScaling.Evaluate(WeightedSocketSize);
    public bool SelectRandomPointInTarget = false;
    public bool DebugMode = false;
    public List<GameObject> ExplosionPrefabs;

    public AnimationCurve BeamCountScaling = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
    public float BeamCount = 100;
    public float EffectiveBeamCount => LaunchAngle * BeamAngleScaling.Evaluate(WeightedSocketSize);


    public override void GetWarheadStatsBlock(ref List<(string, string)> rows)
    {
        base.GetWarheadStatsBlock(ref rows);
        rows.Add(("Angle", $"{EffectiveLaunchAngle:N2} degrees"));

    }
    public override void FinalSetup(ModularMissile missile)
    {
        //Debug.LogError("Setup");
        base.FinalSetup(missile);
        //Debug.LogError("Dims: " + FuseDims + " Target: " + Vector3.one * 100);
        if (ProxFuse)
            missile.SpawnProximityFuze(FuseDimensions);//Fusedimensions * 10 * (AoeRadius / 10) 100
        missile.AddRuntimeBehaviour<RuntimeMissileWarhead>(this);

    }

    public Vector3 RandomConeRay(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo)
    {
        if (SelectRandomPointInTarget && hitObject != null || hitInfo != null)
            return runtime.transform.position.To(hitInfo.HitObject.transform.root.TransformPoint(hitObject.RandomPointInBounds())).normalized;
        if (Omnidirectional)
            return UnityEngine.Random.onUnitSphere;

        return MathHelpers.RandomRayInCone(runtime.transform.forward, EffectiveLaunchAngle);
    }
    public abstract HitResult DoRay(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo, Vector3 direction, Ray ray);

    public abstract HitResult Explode(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo);
    public override HitResult TriggerDetonate(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo, out bool noVfx)
    {
        //Debug.LogError("Trigger");

        //GameObject cubes = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cubes.transform.position = runtime.transform.position;
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //cube.transform.position = hitInfo.Point;
        noVfx = false;
        foreach (GameObject ExplosionPrefab in ExplosionPrefabs)
            SpawnEffect(ExplosionPrefab, runtime.transform.position, runtime.transform.rotation, runtime);
        HitResult finalresult = Explode(runtime, hitObject, hitInfo);
        for (int i = 0; i < EffectiveBeamCount; i++)
        {
            Vector3 randomdirection = RandomConeRay(runtime, hitObject, hitInfo);
            Ray r = new Ray(runtime.transform.position, randomdirection);
            HitResult hitRes = DoRay(runtime, hitObject, hitInfo, randomdirection, r);
            if (finalresult < hitRes)
                finalresult = hitRes;
        }
        runtime.Missile.DoImpactEffect(finalresult, runtime.transform.position, Quaternion.LookRotation(hitInfo.HitNormal), null);
        return finalresult;

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

    public override HitResult CollisionDetonate(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo)
    {
        //Debug.LogError("Collide");
        return TriggerDetonate(runtime, hitObject, hitInfo, out bool _);
    }

    public Poolable SpawnEffect(GameObject prefab, Vector3 position, Quaternion rotation, RuntimeMissileWarhead runtime)
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
}