public class CraftKinematics
{
    [SerializeField]
    protected static float _springStrength = 10;
    [SerializeField]
    protected static float _damperStrength = 10;
    public static void MoveTarget(ICraft craft, MovementTarget target = null)
    {
        if (target == null)
            return;
        if (craft.Rigidbody.velocity.magnitude > craft.FlightSpeed)
            craft.Rigidbody.velocity = craft.Rigidbody.velocity.normalized * craft.FlightSpeed;
        else
            craft.Rigidbody.AddForce(10 * (target.transform.position - craft.Rigidbody.transform.position).normalized);//0.5 is p
        Vector3 springTorque = _springStrength * Vector3.Cross(craft.Rigidbody.transform.forward, target.transform.forward);
        springTorque += _springStrength * Vector3.Cross(craft.Rigidbody.transform.up, target.transform.up);
        var dampTorque = _damperStrength * -craft.Rigidbody.angularVelocity;
        craft.Rigidbody.AddTorque(springTorque + dampTorque, ForceMode.Acceleration);
    }
}
public interface ICraft
{
    public float FlightSpeed { get; }
    public Rigidbody Rigidbody { get; }
    public Transform SocketTransform { get; }
    public MovementTarget Target { get; set; }
    public float HealthPercentage { get; }

    public void SetLeadFactor(Vector3 pos, bool evasive = false);
}
public class SimpleCraft : MonoBehaviour, ICraft
{
    private MovementTarget _target;
    MovementTarget ICraft.Target { get => _target; set => _target = value; }
    public Rigidbody Rigidbody => _body;
    public Transform SocketTransform => sockets[0].transform;
    public float HealthPercentage => 1;
    public List<GameObject> sockets;
    [SerializeField]
    protected float _springStrength = 1;
    [SerializeField]
    protected float _damperStrength = 1;
    protected Rigidbody _body;
    protected ComponentHullPaint[] _paints;
    public ShipController LaunchedFrom => _target.FormationManager.ShipController;
    [SerializeField]
    protected float _flightspeed = 1;
    public float FlightSpeed => FlightSpeed;
    protected void Awake() => _paints = gameObject.GetComponentsInChildren<ComponentHullPaint>();
    protected void FixedUpdate()
    {
        foreach (ComponentHullPaint paint in _paints)
            paint.SetColors(LaunchedFrom.OwnedBy.Colors.BaseColor, LaunchedFrom.OwnedBy.Colors.StripeColor);
        if (_target != null)
            CraftKinematics.MoveTarget(this, _target);
    }

    public void SetLeadFactor(float leadFactor = 1) => throw new NotImplementedException();

    public void SetLeadFactor(float leadFactor = 1, bool evasive = false) => throw new NotImplementedException();

    public void SetLeadFactor(Vector3 pos, bool evasive = false) => throw new NotImplementedException();
}
