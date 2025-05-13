using Game.Reports;
using Munitions.ModularMissiles.Runtime;
using Shapes;

public abstract class BaseShellWarheadDescriptor : AngleWarheadDescriptor
{
    public abstract IDamageCharacteristic AmmoCharacteristic { get; }
    public abstract string ShellText { get; }

    public override string GetFormattedDescription()
    {
        string output = "";
        output += ShellText + " ";
        return output;
    }


    public bool BulletLook = false;
    public bool DirectImpact = false;

    public override float ArmorPenetration => AmmoCharacteristic.ArmorPenetration;
    public override float ComponentDamage => AmmoCharacteristic.ComponentDamage;
    public override float TotalComponentDamagePotential => ComponentDamage * EffectiveBeamCount;

    public override HitResult Explode(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo)
    {
        return HitResult.None;
    }

    public void HandleImbue(ILocalImbued? localImbued, RuntimeMissileWarhead runtime)
    {
        if (localImbued == null)
            return;
        IWeaponStatReportReceiver _reportTo = Common.GetVal<IWeaponStatReportReceiver>(runtime.Missile, "_reportTo");
        localImbued.ImbueLocal(runtime.Missile.LaunchedFrom);
        localImbued.SetWeaponReportPath(_reportTo);
    }
}

[CreateAssetMenu(fileName = "New Shell Missile Warhead", menuName = "Nebulous/Missiles/Warhead/Shell Warhead")]
public class ShellWarheadDescriptor : BaseShellWarheadDescriptor
{
    public GameObject ShellMunitionPrefab;

    public ShellMunition ShellMunition => ShellMunitionPrefab.GetComponentInChildren<ShellMunition>();

    public override IDamageCharacteristic AmmoCharacteristic => ShellMunition;
    public override string ShellText => ShellMunition.GetDetailText();

    public override HitResult DoRay(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo, Vector3 direction, Ray ray)
    {

        ShellMunition NewShellMunition = (ShellMunition)NetworkObjectPooler.Instance.GetNextOrNew(ShellMunitionPrefab, hitInfo.Point, BulletLook ? Quaternion.LookRotation(direction) : Quaternion.identity, direction * ShellMunition.FlightSpeed);
        HandleImbue(NewShellMunition as ILocalImbued, runtime);
        if (!DirectImpact)
        {
            NewShellMunition.ProcessCollision(hitInfo, false);
        }
        return HitResult.Stopped;
    }

}


[CreateAssetMenu(fileName = "New Shell Missile Warhead", menuName = "Nebulous/Missiles/Warhead/LW Shell Warhead")]
public class LightWeightShellWarheadDescriptor : BaseShellWarheadDescriptor
{

    public LightweightKineticShell Ammo => BundleManager.Instance.GetMunition("Stock/120mm HE Shell") as LightweightKineticShell;

    public override IDamageCharacteristic AmmoCharacteristic => Ammo;
    public override string ShellText => Ammo.GetDetailText();

    public override HitResult DoRay(RuntimeMissileWarhead runtime, IDamageable hitObject, MunitionHitInfo hitInfo, Vector3 direction, Ray ray)
    {
        LightweightKineticMunitionContainer container;
        NetworkPoolable networkPoolable;
        if (DirectImpact)
        {
            networkPoolable = NetworkObjectPooler.Instance.GetNextOrNewLWKContainer(hitInfo.Point, BulletLook ? Quaternion.LookRotation(direction) : Quaternion.identity, direction * Ammo.FlightSpeed);
            container = networkPoolable as LightweightKineticMunitionContainer;
            container.ApplyTemplate(Ammo.NetworkSpawnKey);
        }
        else
        {
            networkPoolable = Ammo.InstantiateSelf(runtime.transform.position, BulletLook ? Quaternion.LookRotation(direction) : Quaternion.identity, direction * Ammo.FlightSpeed);
        }
        container = networkPoolable as LightweightKineticMunitionContainer;

        HandleImbue(container as ILocalImbued, runtime);
        if (DirectImpact)
        {
            _ = Ammo.DamageableImpact(container, hitObject, hitInfo, true, out HitResult hitRes, out _, out _, out _);
            Destroy(container);
            return hitRes;
        }
        return HitResult.Stopped;
    }



}


public class MultiShellWarheadDescriptor : ShellWarheadDescriptor
{




    public List<LightweightMunitionBase> AmmoTypes// = new List<LightweightMunitionBase>(1);
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
    public List<ScalingValue> Scalingvalues = new(1);





    private List<int> Weights => Scalingvalues.ConvertAll(scalingvalue => (int)Math.Round(scalingvalue.GetValue(base.WeightedSocketSize)));
    private IEnumerable<KeyValuePair<LightweightMunitionBase, int>> Weightedammotypes => AmmoTypes.Zip(Weights, (key, value) => new KeyValuePair<LightweightMunitionBase, int>(key, value));
    public override float ArmorPenetration => AmmoTypes.Zip(Weights, (ammo, value) => ammo.ArmorPenetration * value).Sum() / Weights.Count;
    public override float ComponentDamage => AmmoTypes.Zip(Weights, (ammo, value) => ((IDamageCharacteristic)ammo).ComponentDamage * value).Sum();
    public override float TotalComponentDamagePotential => ComponentDamage;


    public override string GetFormattedDescription()
    {
        string output = "";
        foreach (LightweightMunitionBase ammo in AmmoTypes)
        {
            output += ammo.GetDetailText() + " ";

        }
        return output;
    }


}