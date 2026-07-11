using System.Reflection;

public enum AreaShipCommandDisruption
{
    None = 0,
    MovementOnly = 1,
    NoCommand = 2
}

[CreateAssetMenu(fileName = "New Area Ship Disable Profile", menuName = "Nebulous/Modules/Area Effects/Ship Disable Profile")]
public class AreaShipDisableProfileModule : ScriptableObject, IOnDamageableImpact, IOnGenericImpact
{
    [SerializeField]
    protected AreaShipCommandDisruption _commandDisruption = AreaShipCommandDisruption.None;

    [SerializeField]
    protected float _durationSeconds = 30f;

    [SerializeField]
    protected bool _requiresCommandComponentHit = true;

    [SerializeField]
    protected bool _disableDrives = true;

    [SerializeField]
    protected bool _disableReactors = true;

    [SerializeField]
    protected bool _disableWeapons = true;

    public AreaShipCommandDisruption CommandDisruption => _commandDisruption;
    public float DurationSeconds => _durationSeconds;
    public bool RequiresCommandComponentHit => _requiresCommandComponentHit;
    public bool DisableDrives => _disableDrives;
    public bool DisableReactors => _disableReactors;
    public bool DisableWeapons => _disableWeapons;

    public void OnDamageableImpact(LightweightKineticShell shell, LightweightKineticMunitionContainer attachedTo, IDamageable hitObject, MunitionHitInfo hitInfo, bool trigger, HitResult hitRes, float damageDone, bool targetDestroyed)
    {
        Apply(hitInfo);
    }

    public void OnGenericImpact(LightweightKineticShell shell, LightweightKineticMunitionContainer attachedTo, MunitionHitInfo hitInfo, bool trigger, HitResult hitRes)
    {
        Apply(hitInfo);
    }

    public void Apply(MunitionHitInfo hitInfo)
    {
        ShipController ship = AreaEffectRuntime.HitShip(hitInfo);
        if (hitInfo == null || ship == null || DurationSeconds <= 0f)
        {
            return;
        }

        ApplyCommandDisruption(ship, hitInfo);
        DynamicWorkingHelpers.ApplyTimed(ship, DurationSeconds, DisableDrives, DisableReactors, DisableWeapons);
    }

    private void ApplyCommandDisruption(ShipController ship, MunitionHitInfo hitInfo)
    {
        if (CommandDisruption == AreaShipCommandDisruption.None)
        {
            return;
        }

        if (RequiresCommandComponentHit && !AreaEffectRuntime.ComponentsInRadius(hitInfo.Point, 0f).Any(component => component is CommandComponent))
        {
            return;
        }

        ship.gameObject.GetOrAddComponent<AreaCommandDisruptionState>()
            .Apply(ship, CommandDisruption, DurationSeconds);
    }
}

public class AreaCommandDisruptionState : MonoBehaviour
{
    private ShipController _ship;
    private AreaShipCommandDisruption _disruption;
    private Utility.CommandFunctions _state;
    private float _endTime;

    public void Apply(ShipController ship, AreaShipCommandDisruption disruption, float durationSeconds)
    {
        if (ship == null || disruption == AreaShipCommandDisruption.None || durationSeconds <= 0f)
        {
            return;
        }

        _ship = ship;
        _disruption = MoreRestrictive(_disruption, disruption);
        _state = ToCommandFunctions(_disruption);
        _endTime = Mathf.Max(_endTime, Time.time + durationSeconds);
        ApplyState();
    }

    private void Update()
    {
        if (_ship == null)
        {
            Destroy(this);
            return;
        }

        if (Time.time >= _endTime)
        {
            typeof(ShipController).GetMethod("HandleCommandWorkingChanged", Common.FunFlags)?.Invoke(_ship, new object[] { null });
            Destroy(this);
            return;
        }

        ApplyState();
    }

    private void ApplyState()
    {
        if (_ship != null && _ship.CommandState < _state)
        {
            _ship.Network_commandState = _state;
        }
    }

    private static AreaShipCommandDisruption MoreRestrictive(AreaShipCommandDisruption current, AreaShipCommandDisruption incoming)
    {
        return (AreaShipCommandDisruption)Mathf.Max((int)current, (int)incoming);
    }

    private static Utility.CommandFunctions ToCommandFunctions(AreaShipCommandDisruption disruption)
    {
        return disruption == AreaShipCommandDisruption.NoCommand
            ? Utility.CommandFunctions.None
            : Utility.CommandFunctions.MovementOnly;
    }
}
