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


    private string _sourceName = Guid.NewGuid().ToString();
    public string SourceName = "";

    string IModifierSource.SourceName => string.IsNullOrEmpty(SourceName) ? _sourceName : SourceName;

    public MovementSpeed SetSpeed => MovementSpeed;

    public string StatName = "powerplant-prodefficiency";
    
    public float DefaultModifierValue = 1f;
    public float OneThird = 1f;
    public float TwoThirds = 1f;
    public float Full = 1f;
    public float Flank = 1f;
    public float FlightQuarters = 1f;
    public Dictionary<MovementSpeed, float> Modifers =>  new Dictionary<MovementSpeed, float>
    {
        { MovementSpeed.OneThird, OneThird },
        { MovementSpeed.TwoThirds, TwoThirds },
        { MovementSpeed.Full, Full },
        { MovementSpeed.Flank, Flank },
        { MovementSpeed.FlightQuarters, FlightQuarters }
    };
    public float LerpRate = 0.5f; // Used for Lerp/Spring
    public float FixedRate = 0.1f;    // Used for Fixed Rate (units per second)
    //public TransitionType Transition = TransitionType.Lerp;
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
    public bool Instant = false;
    public bool ApplyAtZero = true;


    public float PreviousModifer = float.NaN;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        CurrentModifier = Mathf.MoveTowards(CurrentModifier, TargetModifier, FixedRate * Time.fixedDeltaTime);
        CurrentModifier = Mathf.Lerp(CurrentModifier, TargetModifier, Time.fixedDeltaTime * LerpRate);
       
        if (Instant)
        {
            CurrentModifier = TargetModifier;
        }


        /*
        switch (Transition)
        {
            case TransitionType.Instant:
                CurrentModifier = TargetModifier;
                break;

            case TransitionType.Lerp:
                CurrentModifier = Mathf.Lerp(CurrentModifier, TargetModifier, Time.fixedDeltaTime * LerpRate);
                break;

            case TransitionType.FixedRate:
                CurrentModifier = Mathf.MoveTowards(CurrentModifier, TargetModifier, FixedRate * Time.fixedDeltaTime);
                break;
            default:
                CurrentModifier = TargetModifier;
                break;
        }
        */
        if (CurrentModifier == PreviousModifer)
        {
            return;
        }
        Ship.RemoveStatModifier(this, StatName);
        if (CurrentModifier == 0 && !ApplyAtZero)
        {
            return;
        }
        Ship.AddStatModifier(this, new StatModifier(StatName, 0, CurrentModifier));
        PreviousModifer = CurrentModifier;
    }

}