
public class CompoundWarhead : MissileWarhead
{
    [SerializeField]
    protected MissileWarhead[] _Warheads;

    public override float TotalComponentDamagePotential
    {
        get
        {
            float damagepot = 0;
            foreach (MissileWarhead warhead in _Warheads)
                damagepot += warhead.TotalComponentDamagePotential;
            return damagepot;
        }
    }

    public override HitResult Detonate(IDamageable hitObject, MunitionHitInfo hitInfo, out float damageDone, out bool destroyed)
    {
        damageDone = 0;
        destroyed = false;
        HitResult hitResult = HitResult.None;
        foreach (MissileWarhead warhead in _Warheads)
        {
            hitResult = warhead.Detonate(hitObject, hitInfo, out float missiledamage, out bool missledestroyed);
            damageDone += missiledamage;
            destroyed = missledestroyed || destroyed;
            Debug.LogError("Warhead " + hitResult + " at " + hitObject.GameObj.name + " dealing " + missiledamage + " damage");
            //ReportDamageDone(hitResult, missiledamage, missledestroyed);
            //DoImpactEffect(hitInfo, hitResult);
        }
        return hitResult;
    }

    public override string GetTooltipText()
    {
        string tooltiptext = "";
        foreach (MissileWarhead warhead in _Warheads)
        {
            tooltiptext += warhead.GetTooltipText();
        }
        return tooltiptext;
    }

    public override void ResetWarhead()
    {
        foreach (MissileWarhead warhead in _Warheads)
            warhead.ResetWarhead();
    }

    public override void ArmWarhead()
    {
        foreach (MissileWarhead warhead in _Warheads)
            warhead.ArmWarhead();
    }
}
