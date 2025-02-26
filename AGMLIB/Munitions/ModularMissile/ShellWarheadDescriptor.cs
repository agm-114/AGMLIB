﻿using Munitions.ModularMissiles.Descriptors.Warheads;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Runtime;
using Shapes;
using Bundles;
using Game.Reports;

[CreateAssetMenu(fileName = "New Shell Missile Warhead", menuName = "Nebulous/Missiles/Warhead/Shell Warhead")]
public class ShellWarheadDescriptor : BaseWarheadDescriptor, IModular
{
    [SerializeField]
    private Vector3 _fusedimensions = Vector3.one;
    [SerializeField]
    private readonly bool _omnidirectional = false;

    [SerializeField]
    private readonly float _launchAngle = 20f;

    [SerializeField]
    private readonly WeaponEffectSet _effects = null;

    [SerializeField]
    private bool _selectRandomPointInTarget = false;

    [SerializeField]
    private List<LightweightMunitionBase> Ammotypes// = new List<LightweightMunitionBase>(1);
    {
        get
        {
            List<LightweightMunitionBase> ammotypes = new()
            {
                BundleManager.Instance.GetMunition("Stock/120mm HE Shell") as LightweightMunitionBase,
                BundleManager.Instance.GetMunition("Stock/250mm AP Shell") as LightweightMunitionBase,
                BundleManager.Instance.GetMunition("Stock/250mm HE-RPF Shell") as LightweightMunitionBase,
                BundleManager.Instance.GetMunition("Stock/450mm HE Shell") as LightweightMunitionBase,
                BundleManager.Instance.GetMunition("Stock/450mm AP Shell") as LightweightMunitionBase
            };
            return ammotypes;
        }
    }
    [SerializeField]
    private readonly List<ScalingValue> _scalingvalues = new(1);

    [SerializeField]
    private readonly bool _bulletLook = false;
    [SerializeField]
    protected List<ScriptableObject> _modules = new();

    private List<int> Weights => _scalingvalues.ConvertAll(scalingvalue => (int)Math.Round(scalingvalue.GetValue(base._weightedSocketSize)));
    private IEnumerable<KeyValuePair<LightweightMunitionBase, int>> Weightedammotypes => Ammotypes.Zip(Weights, (key, value) => new KeyValuePair<LightweightMunitionBase, int>(key, value));
    public override float ArmorPenetration => Ammotypes.Zip(Weights, (ammo, value) => ammo.ArmorPenetration * value).Sum() / Weights.Count;
    public override float ComponentDamage => Ammotypes.Zip(Weights, (ammo, value) => ((IDamageCharacteristic)ammo).ComponentDamage * value).Sum();
    public override float TotalComponentDamagePotential => ComponentDamage;
    List<ScriptableObject> IModular.Modules => _modules;
    public override WeaponEffectSet GetEffectSet(int setIndex) => _effects;

    public override string GetSummarySegment()
    {
        string output = "";
        foreach(LightweightMunitionBase ammo in Ammotypes)
        {
            output += ammo.MunitionName + " ";
        }
        return output;
    }

    public override string GetDetailSummarySegment() => "TODO";

    public override string GetFormattedDescription()
    {
        string output = "";
        foreach (LightweightMunitionBase ammo in Ammotypes)
        {
            output += ammo.GetDetailText() + " ";
        }
        return output;
    }

    public override HitResult TriggerDetonate(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo, out bool noVfx)
    {
        noVfx = false;
        _selectRandomPointInTarget = true;

        foreach (KeyValuePair<LightweightMunitionBase, int> entry in Weightedammotypes)
        {
            for(int i = 0; i < entry.Value; i++)
            {
                Vector3 forward = runtime.transform.forward;// base.transform.forward;
                forward = _omnidirectional ? UnityEngine.Random.onUnitSphere : ((!_selectRandomPointInTarget || hitObject == null || hitInfo == null) ? MathHelpers.RandomRayInCone(forward, _launchAngle).normalized : runtime.transform.position.To(hitInfo.HitObject.transform.root.TransformPoint(hitObject.RandomPointInBounds())).normalized);
                Fire(entry.Key, forward, runtime);
            }
        }

        return HitResult.Stopped;
    }

    public void Fire(LightweightMunitionBase ammo, Vector3 shotDirection, RuntimeMissileWarhead runtime)
    {
        NetworkPoolable networkPoolable = ammo.InstantiateSelf(runtime.transform.position, _bulletLook ? Quaternion.LookRotation(shotDirection) : Quaternion.identity, shotDirection * ((IMunition)ammo).FlightSpeed);
        IWeaponStatReportReceiver _reportTo = Common.GetVal<IWeaponStatReportReceiver>(runtime.Missile, "_reportTo");

        if (networkPoolable is ILocalImbued localImbued)
        {
            localImbued.ImbueLocal(runtime.Missile.LaunchedFrom);
            localImbued.SetWeaponReportPath(_reportTo);
        }
    }
    public override HitResult CollisionDetonate(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo) => TriggerDetonate(runtime, hitObject, hitInfo, out bool _);

    public void Awake() => Debug.Log("Awake");

    public override void FinalSetup(ModularMissile missile)
    {
        base.FinalSetup(missile);
        missile.SpawnProximityFuze(_fusedimensions * 10);
        missile.AddRuntimeBehaviour<RuntimeMissileWarhead>(this);
    }
}
