using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Descriptors.Warheads;
public class DescribedWarhead : MissileWarhead
{
    [SerializeField]
    protected BaseWarheadDescriptor _warhead;
    protected MissileSocket _socket;

    protected int _size;

    public override float TotalComponentDamagePotential => _warhead.TotalComponentDamagePotential;

    public override HitResult Detonate(IDamageable hitObject, MunitionHitInfo hitInfo, out float damageDone, out bool destroyed)
    {
        damageDone = 0;
        destroyed = false;
        HitResult hitResult = HitResult.None;

        //float missiledamage;
        //bool missledestroyed;
        //hitResult = _warhead.Detonate(hitObject, hitInfo, out missiledamage, out missledestroyed);
        //damageDone += missiledamage;
        //destroyed = missledestroyed || destroyed;
        //Debug.LogError("Warhead " + hitResult + " at " + hitObject.GameObj.name + " dealing " + missiledamage + " damage");
        //ReportDamageDone(hitResult, missiledamage, missledestroyed);
        //DoImpactEffect(hitInfo, hitResult);

        return hitResult;
    }

    public override string GetTooltipText() => _warhead.GetFormattedDescription();

    public override void ResetWarhead()
    {
        //
        return;
    }

    public override void ArmWarhead()
    {
        //foreach (MissileWarhead warhead in _Warheads)
        //    warhead.ArmWarhead();
    }
}
