using Lib.Generic_Gameplay;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Descriptors.Warheads;
using Munitions.ModularMissiles.Runtime;

public abstract class AngleWarheadDescriptor : BaseWarheadDescriptor
{
    [Header("Warhead Costing")]
    public float FixedPointCost = 0;
    public enum ThresholdType
    {
        SocketWeight,
        SocketSize
    }
    public ThresholdType CostThresholdType = ThresholdType.SocketWeight;
    public float SizeCostPenaltyThreshold = 0;
    public float FixedSizeCostPenalty = 0;
    public float ScalingCostPenalty = 0;
    public float SizeCostPenaltyMultiplier = 1;
    public override float PointCost
    {
        get
        {
            float baseCost = _pointCost * base._weightedSocketCost + FixedPointCost;
            float socketSize = base._weightedSocketSize;
            if (CostThresholdType == ThresholdType.SocketSize)
            {
                socketSize = _socketSize;
            }

            if (socketSize >= SizeCostPenaltyThreshold)
            {
                return baseCost;
            }
            float penalty = FixedSizeCostPenalty;
            float stepsBelowThreshold = SizeCostPenaltyThreshold - socketSize;
            penalty += stepsBelowThreshold * ScalingCostPenalty;
            return SizeCostPenaltyMultiplier * (baseCost + penalty);
        }
    }

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
    public float EffectiveBeamCount => BeamCount * BeamCountScaling.Evaluate(WeightedSocketSize);

    private ContactClassification _targetType = ContactClassification.Unknown;

    public override void GetWarheadStatsBlock(ref List<(string, string)> rows)
    {
        base.GetWarheadStatsBlock(ref rows);
        rows.Add(("Angle", $"{EffectiveLaunchAngle:N2} degrees"));
    }

    public override void FinalSetup(ModularMissile missile)
    {
        base.FinalSetup(missile);
        if (ProxFuse)
            missile.SpawnProximityFuze(FuseDimensions);
    }

    public override void SetTrackTarget(ModularMissile missile, TrackIdentifier trackId)
    {
        base.SetTrackTarget(missile, trackId);
        if (missile.LaunchedFrom != null && missile.LaunchedFrom.Sensors != null)
        {
            ITrack sensorTrack = missile.LaunchedFrom.Sensors.GetSensorTrack(trackId);
            if (sensorTrack != null)
            {
                _targetType = sensorTrack.Trackable.ContactType;
            }
        }
    }
    public Vector3 RandomConeRay(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo)
    {
        // Existing warheads aim at impact/proximity targets even with target selection disabled.
        // The flag also allows target aiming when detonation has no hit information.
        if ((SelectRandomPointInTarget || hitInfo != null) && hitObject?.GameObj != null)
        {
            Vector3 direction = runtime.transform.position.To(hitObject.GameObj.transform.TransformPoint(hitObject.RandomPointInBounds()));
            if (direction.sqrMagnitude > 0f)
                return direction.normalized;
        }
        if (Omnidirectional)
            return UnityEngine.Random.onUnitSphere;

        return MathHelpers.RandomRayInCone(runtime.transform.forward, EffectiveLaunchAngle);
    }
    public abstract HitResult DoRay(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo, Vector3 direction, Ray ray);

    public abstract HitResult Explode(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo);
    public override HitResult TriggerDetonate(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo, out bool noVfx)
    {
        // Detonation runs on the server. Keep damage dealers authoritative here; presentation-only
        // effects are spawned on every client by AngleWarheadNetworkEffectsPatch.
        noVfx = true;
        if (!runtime.isServer)
            return HitResult.None;

        // Only marked impact events replay custom effects. Native collision/destruction events
        // can follow this one and must not spawn another explosion.
        ((LookaheadMunitionBase)runtime.Missile).Internals().RpcRollImpactEffect(HitResult.None, AngleWarheadNetworkEffectsPatch.ExplosionEffect,
            0, runtime.transform.position, runtime.transform.rotation, null!);
        SpawnExplosionEffects(runtime.transform.position, runtime.transform.rotation, runtime, true);
        HitResult result = Explode(runtime, hitObject, hitInfo);
        for (int i = 0; i < EffectiveBeamCount; i++)
        {
            Vector3 direction = RandomConeRay(runtime, hitObject, hitInfo);
            HitResult hitRes = DoRay(runtime, hitObject, hitInfo, direction, new Ray(runtime.transform.position, direction));
            if (hitRes > result)
                result = hitRes;
        }
        return result;
    }

    internal void SpawnPresentationExplosionEffects(Vector3 position, Quaternion rotation, RuntimeMissileWarhead runtime) =>
        SpawnExplosionEffects(position, rotation, runtime, false);

    private void SpawnExplosionEffects(
        Vector3 position,
        Quaternion rotation,
        RuntimeMissileWarhead runtime,
        bool damageDealers)
    {
        if (ExplosionPrefabs == null)
            return;

        foreach (GameObject explosionPrefab in ExplosionPrefabs)
        {
            if (explosionPrefab == null ||
                HasDamageDealer(explosionPrefab) != damageDealers)
            {
                continue;
            }

            SpawnEffect(explosionPrefab, position, rotation, runtime);
        }
    }

    protected static bool HasDamageDealer(GameObject prefab) => prefab.GetComponentsInChildren<IDamageDealer>(true).Length > 0;

    public override HitResult CollisionDetonate(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo)
        => TriggerDetonate(runtime, hitObject, hitInfo, out _);

    public Poolable? SpawnEffect(GameObject prefab, Vector3 position, Quaternion rotation, RuntimeMissileWarhead runtime)
    {
        if (prefab == null)
            return null;
        Poolable poolable = ObjectPooler.Instance.GetNextOrNew(prefab, position, rotation);
        if (poolable == null)
            return null;
        foreach (IWeightedEffect effect in poolable.GetComponentsInChildren<IWeightedEffect>())
        {
            effect.SetWeight(WeightedSocketSize);
        }

        var missile = ((LookaheadMunitionBase)runtime.Missile).Internals();
        foreach (ILocalImbued imbued in poolable.gameObject.GetComponentsInChildren<ILocalImbued>())
        {
            imbued.ImbueLocal(missile.LocalLaunchedFrom);
            imbued.SetWeaponReportPath(missile.ReportTo);
        }
        if (poolable.GetComponent<ModularEffect>() == null)
            foreach (IEffectModule effect in poolable.gameObject.GetComponentsInChildren<IEffectModule>())
                effect.Play();
        return poolable;
    }

    public override void ResetWarhead()
    {
        base.ResetWarhead();
        _targetType = ContactClassification.Unknown;
    }

    public override bool ShouldFuzeOnTarget(MunitionHitInfo hitInfo, bool trigger)
    {
        if (!trigger)
        {
            return true;
        }

        if (_targetType != 0)
        {
            ISensorTrackable component = hitInfo.HitObject.transform.root.GetComponent<ISensorTrackable>();
            if (component != null && (component.ContactType == ContactClassification.SmallCraft || component.ContactType == ContactClassification.Missile) && _targetType != component.ContactType)
            {
                return false;
            }
        }

        return true;
    }
}
