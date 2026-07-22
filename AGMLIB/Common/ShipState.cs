using Mirror;

[System.Serializable]
public enum ConditionalState
{
    Ignore,
    Enabled,
    Disabled

}

public class InternalShipState : NetworkBehaviour
{
    public ShipState ShipState;

    // Start is called before the first frame update
    public MovementSpeed MovementSpeed => ShipController?.Throttle ?? MovementSpeed.OneThird;
    public ConditionalState FlankState => ShipController?.Throttle == MovementSpeed.Flank ? ConditionalState.Enabled : ConditionalState.Disabled;
    public ConditionalState BattleshortState => ShipController?.BattleShortEnabled == true ? ConditionalState.Enabled : ConditionalState.Disabled;
    public ConditionalState ControlState => ShipController?.CommandState == CommandFunctions.None ? ConditionalState.Disabled : ConditionalState.Enabled;
    public ConditionalState ElimnatedState => ShipController?.IsEliminated ?? false ? ConditionalState.Disabled : ConditionalState.Enabled;
    public ConditionalState WarpingIn => ShipController?.WarpMode == true && !ShipController.WarpDeparting ? ConditionalState.Enabled : ConditionalState.Disabled;
    public ConditionalState WarpingOut => ShipController?.WarpMode == true && ShipController.WarpDeparting ? ConditionalState.Enabled : ConditionalState.Disabled;
    public ConditionalState Warping => ShipController?.WarpMode == true ? ConditionalState.Enabled : ConditionalState.Disabled;
    public ConditionalState InMapBounds => ShipController?.MapBoundary?.OutsideBounds == false ? ConditionalState.Enabled : ConditionalState.Disabled;



    protected Dictionary<string, ShipInfoButton> _buttons = new();

    public ShipInfoButton GetButton(string key)
    {
        if(ShipController == null)
        {
            return null;
        }
        if (!_buttons.TryGetValue(key, out ShipInfoButton button))
        {
            ShipInfoButton shipInfoButton = ShipInfoButton.FindButton(ShipController, key);
            _buttons.Add(key, shipInfoButton);
            return shipInfoButton;
        }
        return button;
    }

    public Ship? Ship => Hull.MyShip;
    public ShipController? ShipController = null;
    public ResourceComponent? ResourceComponent = null;
    public EditorShipController? EditorShipController = null;
    public Hull? Hull = null;
    private Rigidbody? _rigidbody = null;
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

        if (Hull == null)
            Hull = transform.gameObject.GetComponentInParent<Hull>();
        if (Hull == null)
            Hull = transform.gameObject.GetComponent<Hull>();
        if (Hull == null)
            Hull = transform.gameObject.GetComponentInChildren<Hull>();

        if (ResourceComponent == null)
            ResourceComponent = transform.gameObject.GetComponentInParent<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = transform.gameObject.GetComponent<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = transform.gameObject.GetComponentInChildren<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = Hull?.gameObject.GetComponentInChildren<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = Hull?.gameObject.GetComponent<ResourceComponent>();
        if (ResourceComponent == null)
            ResourceComponent = Hull?.gameObject.GetComponentInParent<ResourceComponent>();




        ShipState = Hull?.GetComponent<ShipState>();
        if (ShipState == null)
        {
            ShipState = Hull?.gameObject.AddComponent<ShipState>();
            ShipState?.Awake();
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

public class ShipState : NetworkBehaviour
{
    public MovementSpeed MovementSpeed => InternalShipState.MovementSpeed;
    public ConditionalState FlankState => InternalShipState.FlankState;
    public ConditionalState BattleshortState => InternalShipState.BattleshortState;
    public ConditionalState ControlState => InternalShipState.ControlState;
    public ConditionalState ElimnatedState => InternalShipState.ElimnatedState;
    public ConditionalState WarpingIn => InternalShipState.WarpingIn;
    public ConditionalState WarpingOut => InternalShipState.WarpingOut;
    public ConditionalState Warping => InternalShipState.Warping;
    public ConditionalState InMapBounds => InternalShipState.InMapBounds;

    private InternalShipState InternalShipState;
    public float Velocity => InternalShipState.Velocity;
    public ShipInfoButton GetButton(string key) => InternalShipState.GetButton(key);
    public Ship? Ship => Hull.MyShip;
    public ShipController? ShipController => InternalShipState.ShipController;
    public Hull? Hull => InternalShipState.Hull;
    public ResourceComponent? ResourceComponent => InternalShipState.ResourceComponent;
    public EditorShipController? EditorShipController => InternalShipState.EditorShipController;
    public Rigidbody Rigidbody => InternalShipState.Rigidbody;
    public bool InEditor => InternalShipState.InEditor;
    public bool InGame => InternalShipState.InGame;
    public GameObject Root => gameObject.transform.root.gameObject;


    public virtual void Awake()
    {
        if(InternalShipState != null)
            return;
        Ship ship = transform.gameObject.GetComponentInParent<Ship>();
        if (ship == null)
        {
            InternalShipState = Root.GetComponent<InternalShipState>() ?? Root.AddComponent<InternalShipState>(); ;
            return;
        }
            
        InternalShipState = InternalShipState.GetInternalShipState(ship);
    }


    public static ShipState GetShipState(Ship ship)
    {
        InternalShipState state = InternalShipState.GetInternalShipState(ship);
        return state.ShipState;
    }
}
#pragma warning restore IDE1006 // Naming Styles
