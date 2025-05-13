// Nebulous, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Munitions.MissileImpactWarhead
public class SimpleShellWarhead : MissileWarhead
{

    [SerializeField]
    private LightweightKineticShell _appliedTemplate;

    LightweightKineticMunitionContainer container;

    public override float TotalComponentDamagePotential => 0;

    public override void ResetWarhead()
    {
        if (_appliedTemplate == null)
            _appliedTemplate = (LightweightKineticShell)BundleManager.Instance.GetMunition("Stock/400mm Plasma Ampoule");

    }

    public override string GetTooltipText() => $"Type: {_appliedTemplate.name}\nArmor Penetration: {_appliedTemplate.ArmorPenetration} cm\n"; //Component Damage: {_appliedTemplate.ComponentDamage} hp\nBlast Angle: {_appliedTemplate.r * 2f}";

    public override HitResult Detonate(IDamageable hitObject, MunitionHitInfo hitInfo, out float damageDone, out bool destroyed)
    {
        Debug.LogError("Submuntion Fired!");
        //container.transform.position = transform.position;
        //container.transform.rotation = transform.rotation;
        container = NetworkObjectPooler.Instance.GetNextOrNewLWKContainer(hitInfo.Point, transform.rotation, Vector3.zero) as LightweightKineticMunitionContainer;
        container.ApplyTemplate(_appliedTemplate.NetworkSpawnKey);
        _ = _appliedTemplate.DamageableImpact(container, hitObject, hitInfo, true, out HitResult hitRes, out damageDone, out destroyed, out _);
        Destroy(container);
        destroyed = !destroyed;
        return hitRes;
    }
}
