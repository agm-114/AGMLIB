public class SimpleDynamicModifer : ActiveSettings, IModifierSource
{
    //public float launcherindex = 0;
    [Space]
    [Header("Modifier Settings")]
    [Space]
    public StatModifier[] Modifiers;

    private string sourceName = Guid.NewGuid().ToString();

    string IModifierSource.SourceName => sourceName;


    // Update is called once per frame

    protected override void FixedUpdate()
    {
        //time += Time.deltaTime;
        base.FixedUpdate();
        if (!base.active)
        {


            foreach (StatModifier statModifier in Modifiers)
            {
                Ship.RemoveStatModifier(this, statModifier.StatName);
            }
        }
        else
        {
            foreach (StatModifier modifier in Modifiers)
            {
                Ship.AddStatModifier(this, modifier);
            }
        }

    }

}
/*

`
time = time - interpolationPeriod;

*/