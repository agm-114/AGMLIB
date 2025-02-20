[System.Serializable]
public enum ConditionalState
{
    Ignore,
    Enabled,
    Disabled

}

public class InternalShipState : MonoBehaviour
{
    public ShipState ShipState;

    // Start is called before the first frame update
    public ConditionalState FlankState => ShipController.Throttle == MovementSpeed.Flank ? ConditionalState.Enabled : ConditionalState.Disabled;
    public ConditionalState BattleshortState => ShipController.BattleShortEnabled == true ? ConditionalState.Enabled : ConditionalState.Disabled;
    public ConditionalState ControlState => ShipController.CommandState == CommandFunctions.None ? ConditionalState.Disabled : ConditionalState.Enabled;
    public ConditionalState ElimnatedState => ShipController.IsEliminated ? ConditionalState.Disabled : ConditionalState.Enabled;



    protected Dictionary<string, ShipInfoButton> _buttons = new();

    public ShipInfoButton GetButton(string key)
    {
        if (!_buttons.TryGetValue(key, out ShipInfoButton button))
        {
            ShipInfoButton shipInfoButton = ShipInfoButton.FindButton(ShipController, key);
            _buttons.Add(key, shipInfoButton);
            return shipInfoButton;
        }
        return button;
    }

    public Ship Ship;
    public ShipController ShipController;
    public ResourceComponent ResourceComponent;
    public EditorShipController EditorShipController;
    public Hull Hull;
    private Rigidbody _rigidbody = null;
    public Rigidbody Rigidbody
    {
        get
        {
            if (ShipController != null && _rigidbody == null)
                _rigidbody = Common.GetVal<Rigidbody>(ShipController, "_rigidbody");
            return _rigidbody;
        }

    }
    public float Velocity => Rigidbody?.velocity.magnitude ?? 0;
    public bool InEditor => !ShipController.enabled;
    public bool InGame => EditorShipController == null;

    public void Awake()
    {
        if (EditorShipController == null)
            EditorShipController = transform.gameObject.GetComponentInParent<EditorShipController>();
        if (EditorShipController == null)
            EditorShipController = transform.gameObject.GetComponent<EditorShipController>();
        if (EditorShipController == null)
            EditorShipController = transform.gameObject.GetComponentInChildren<EditorShipController>();
        if (ShipController == null)
            ShipController = transform.gameObject.GetComponentInParent<ShipController>();
        if (ShipController == null)
            ShipController = transform.gameObject.GetComponent<ShipController>();
        if (ShipController == null)
            ShipController = transform.gameObject.GetComponentInChildren<ShipController>();
        if (Ship == null && ShipController != null && ShipController.Ship != null)
            Ship = ShipController.Ship;
        if (Ship == null)
            Ship = transform.gameObject.GetComponentInParent<Ship>();
        if (Ship == null)
            Ship = transform.gameObject.GetComponent<Ship>();

        if (ResourceComponent == null)
            ResourceComponent = transform.gameObject.GetComponentInParent<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = transform.gameObject.GetComponent<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = transform.gameObject.GetComponentInChildren<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = Ship.gameObject.GetComponentInChildren<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = Ship.gameObject.GetComponent<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = Ship.gameObject.GetComponentInParent<ResourceComponent>();



        if (Hull == null)
            Hull = transform.gameObject.GetComponentInParent<Hull>();
        if (Hull == null)
            Hull = transform.gameObject.GetComponent<Hull>();
        if (Hull == null)
            Hull = transform.gameObject.GetComponentInChildren<Hull>();

        ShipState = Ship.GetComponent<ShipState>();
        if (ShipState == null)
        {
            ShipState = Ship.gameObject.AddComponent<ShipState>();
            ShipState.Awake();
        }
        //module = gameObject.GetComponent<HullComponent>();
    }

    public static InternalShipState GetInternalShipState(Ship ship)
    {
        InternalShipState state = ship.gameObject.GetComponentInParent<InternalShipState>();
        if (state == null)
        {
            state = ship.gameObject.AddComponent<InternalShipState>();
            state.Awake();
        }
        return state;
    }
}

public class ShipState : MonoBehaviour
{
    public ConditionalState FlankState => InternalShipState.FlankState;
    public ConditionalState BattleshortState => InternalShipState.BattleshortState;
    public ConditionalState ControlState => InternalShipState.ControlState;
    public ConditionalState ElimnatedState => InternalShipState.ElimnatedState;

    private InternalShipState InternalShipState;
    public float Velocity => InternalShipState.Velocity;
    public ShipInfoButton GetButton(string key) => InternalShipState.GetButton(key);
    public Ship Ship => InternalShipState.Ship;
    public ShipController ShipController => InternalShipState.ShipController;
    public Hull Hull => InternalShipState.Hull;
    public ResourceComponent ResourceComponent => InternalShipState.ResourceComponent;
    public EditorShipController EditorShipController => InternalShipState.EditorShipController;
    public Rigidbody Rigidbody => InternalShipState.Rigidbody;
    public bool InEditor => InternalShipState.InEditor;
    public bool InGame => InternalShipState.InGame;


    public virtual void Awake()
    {
        Ship ship = transform.gameObject.GetComponentInParent<Ship>();
        InternalShipState = InternalShipState.GetInternalShipState(ship);
    }


    public static ShipState GetShipState(Ship ship)
    {
        InternalShipState state = InternalShipState.GetInternalShipState(ship);
        return state.ShipState;
    }
}
#pragma warning restore IDE1006 // Naming Styles
