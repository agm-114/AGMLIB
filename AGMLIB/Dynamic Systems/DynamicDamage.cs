public class DynamicDamage : ActiveSettings
{
    //public float launcherindex = 0;

    public enum DamageTarget
    {
        Random,
        All,
        Self,
    }
    //public Hull hull;
    [Space]
    [Header("Damage Settings")]
    [Space]
    public DamageTarget damageTarget = DamageTarget.Random;
    public float damagepertick = 1;
    public float interval = 1;

    private float time;

    //public ModiferOverride modiferOverride;
    // Update is called once per frame
    protected override void FixedUpdate()
    {
        time += Time.deltaTime;
        if (time > interval)
            time = 0;
        else
            return;
        base.FixedUpdate();
        if (!base.active)
            return;

        //Debug.Log("Damage Tick");
        Debug.Log("Triggering Damage");
        HullPart[] parts = ShipController.gameObject.GetComponentsInChildren<HullPart>();
        if (damageTarget == DamageTarget.Random && !ShipController.IsEliminated)
        {
            try
            {
                parts[UnityEngine.Random.Range(0, parts.Length)].DoDamage(damagepertick);
            }
            catch (Exception)
            {
            }
            ShipController?.MarkAsDamaged();
        }
        else if(damageTarget == DamageTarget.All)
        {
            foreach(HullPart part in parts)
                part.DoDamage(damagepertick);
            ShipController?.MarkAsDamaged();
        }
        else
        {
            //module.DoDamage(damagepertick);
        }
        //Debug.LogError("Hull Part: " + part2.gameObject.name);
        //CommsAntennaPart //HullComponent //SectorSensorPart //ThrusterPart //HullPart

    }
}
