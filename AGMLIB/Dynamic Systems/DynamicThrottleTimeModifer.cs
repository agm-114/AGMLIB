using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.TimeZoneInfo;

public class DynamicThrottleTimeModifer : ActiveSettings, IModifierSource
{
    public enum TransitionType { Instant, Lerp, FixedRate }
    private string sourceName = Guid.NewGuid().ToString();

    string IModifierSource.SourceName => sourceName;

    public MovementSpeed SetSpeed => MovementSpeed;

    public string StatName = "powerplant-prodefficiency";
    
    public float DefaultModifierValue = 1f;

    public Dictionary<MovementSpeed, float> Modifers = new();
    public float TransitionSpeed = 5f; // Used for Lerp/Spring
    public float ChangeRate = 0.5f;    // Used for Fixed Rate (units per second)
    public TransitionType Transition = TransitionType.Lerp;
    public float CurrentModifier = 1f;
    public float TargetModifier
    {
        get
        {
            if (Modifers.TryGetValue(SetSpeed, out float value))
            {
                return value;
            }

            return DefaultModifierValue;
        }
    }

    public float PreviousModifer = float.NaN;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        switch (Transition)
        {
            case TransitionType.Instant:
                CurrentModifier = TargetModifier;
                break;

            case TransitionType.Lerp:
                CurrentModifier = Mathf.Lerp(CurrentModifier, TargetModifier, Time.fixedDeltaTime * TransitionSpeed);
                break;

            case TransitionType.FixedRate:
                CurrentModifier = Mathf.MoveTowards(CurrentModifier, TargetModifier, ChangeRate * Time.fixedDeltaTime);
                break;
            default:
                CurrentModifier = TargetModifier;
                break;
        }
        if (CurrentModifier == PreviousModifer)
        {
            return;
        }
        Ship.RemoveStatModifier(this, StatName);
        Ship.AddStatModifier(this, new StatModifier(StatName, 0, CurrentModifier));
        PreviousModifer = CurrentModifier;
    }

}