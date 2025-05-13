using Mirror;
using UnityEngine.UI.Extensions;

//[CreateAssetMenu(fileName = "New LW Guided Shell", menuName = "Nebulous/LW Shells/Guided Shell")]
public class ModularShell : ShellMunition
{

    [ShowInInspector]
    private string _networkspawnkey = "1c902a31174348c9a34cdfa99c1d4bf4";
    public override Guid SavedNetworkSpawnKey => new(_networkspawnkey);

    [SerializeField]
    protected MissileSeeker _seeker;

    //[SerializeField]
    //protected LightweightKineticShell _appliedTemplate;

    [SerializeField]
    protected MissileWarhead _Warhead;

    [SerializeField]
    protected MissileWarhead _ImpactWarhead;

    [SerializeField]
    protected bool _doconventionaldamage = false;

    [SerializeField]
    protected MissileWarhead _FusedWarhead;

    [SerializeField]
    protected float _turnRate = 0.25f;
    //LightweightSplashingShell shell;
    //ProximityBurstShellMunition ProximityBurstShellMunition;

    [SerializeField]
    protected Avoider _avoider;

    [SerializeField]
    protected PID _pid;

    [SerializeField]
    protected float _fusedelay = 1;

    [SerializeField]
    protected Collider _trigger;

    protected override bool DamageableImpact(IDamageable hitObject, MunitionHitInfo hitInfo, bool trigger)
    {

        if (trigger)
            Debug.LogError("FUSED IMPACT!");
        else
            Debug.LogError("IMPACT!");
        float missiledamage = 0;
        float missiledamage2 = 0;
        bool missledestroyed = false;
        HitResult hitResult = HitResult.None;

        if (_Warhead != null)
            hitResult = _Warhead.Detonate(hitObject, hitInfo, out missiledamage, out missledestroyed);
        if (trigger)
        {
            Debug.LogError("FUSED IMPACT!");
            if (_FusedWarhead != null)
                hitResult = _FusedWarhead.Detonate(hitObject, hitInfo, out missiledamage2, out missledestroyed);
        }
        else
        {
            Debug.LogError("IMPACT!");
            if (_ImpactWarhead != null)
                hitResult = _ImpactWarhead.Detonate(hitObject, hitInfo, out missiledamage2, out missledestroyed);
        }

        missiledamage += missiledamage2;
        Debug.LogError("Warhead " + hitResult + " at " + hitObject.GameObj.name + " dealing " + missiledamage + " damage");

        if (!trigger && _doconventionaldamage)
            base.DamageableImpact(hitObject, hitInfo, trigger);

        //This is the normal return value
        return !missledestroyed;//|| shelldestroyed; base.DamageableImpact(hitObject, hitInfo, trigger) ||
    }

    protected void OnImbued(ShipController platform)
    {
        if (_seeker != null)
        {
            _seeker.SetOwner(platform.OwnedBy);
            _seeker.SetLaunchingPlatform(platform);
        }
    }

    protected override void Awake()
    {
        //gameObject.GetComponent<BoxCollider>().enabled = false;
        _pid = new PID(1, 0, 2, -10f, 10f);
        _seeker.SetOwner(this.OwnedBy);

        if (_avoider == null)
            _avoider = gameObject.GetOrAddComponent<Avoider>();
        ResetFuses();
        base.Awake();
    }
    //Cruise Missile Behavior
    protected override void FixedUpdate()
    {
        gameObject.transform.rotation = Quaternion.LookRotation(_body.velocity, transform.up);

        if (!_isDead && _flightAccumulator > _fusedelay)
        {
            _Warhead.ArmWarhead();
            StateSeek();
        }
        base.FixedUpdate();
    }

    private void StateSeek()
    {
        Vector3 vector = transform.position + _body.velocity;

        if (_seeker != null && _seeker.SearchForTarget(out var position, out var velocity, out var acceleration))
        {
            //Debug.LogError("Seeking");
            vector = MathHelpers.EstimateLeadPosition(base.transform.position, __flightSpeed, position, velocity, acceleration, out float flightTime);

            //PointTowards(vector, Vector3.up, _turnRate);

            //gameObject.GetComponentInChildren<Collider>().isTrigger = false;
            //_body.velocity = gameObject.transform.forward.normalized * FlightSpeed;

            //Vector3 idealPoint = base.transform.position.ClosestPointOnLine(_acquiredPosition, vector, out flightTime);
            //NavigateToPoint(vector, idealPoint, Vector3.up, position, _seeker.MaxLookAngle);
        }

        if (_avoider != null && _avoider.BroadphaseCheck(_body.velocity.normalized, __flightSpeed * 10, out var _))
        {

            //vector =  _avoider.FindAvoidanceDirection(_body.velocity.normalized, __flightSpeed * 10);// transform.position +
            //Debug.LogError(vector);
        }
        PointTowards(vector, Vector3.up, _turnRate);
    }

    protected void PointTowards(Vector3 targetDirection, Vector3 up, float turnRate)
    {
        //Quaternion oldlook = gameObject.transform.rotation;
        //gameObject.transform.LookAt(vector);
        //gameObject.transform.rotation = Quaternion.RotateTowards(oldlook, gameObject.transform.rotation, 0.1f);

        if (targetDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, Quaternion.LookRotation(targetDirection - transform.position), 0.1f);
            _body.velocity = gameObject.transform.forward.normalized * FlightSpeed;
        }
    }

    protected override void OnUnpooled()
    {
        if (_trigger != null)
            _trigger.enabled = false;
        //foreach (MeshRenderer mesh in gameObject.GetComponentsInChildren<MeshRenderer>())
        //    mesh.enabled = true;
        base.OnUnpooled();
        if (_collider != null)
            _collider.enabled = false;
        ResetFuses();
    }

    protected override void OnRepooled()
    {
        //foreach(MeshRenderer mesh in gameObject.GetComponentsInChildren<MeshRenderer>())
        //    mesh.enabled = false;
        if (_trigger != null)
            _trigger.enabled = false;

        base.OnRepooled();
        //gameObject.transform.rotation = Quaternion.LookRotation(_body.velocity, transform.up);

    }

    protected override void OnRepoolDelayed(bool disableImmediately)
    {
        if (disableImmediately)
        {
        }
        base.OnRepoolDelayed(disableImmediately);
    }

    void ResetFuses()
    {
        _Warhead?.ResetWarhead();
        _ImpactWarhead?.ResetWarhead();
        _FusedWarhead?.ResetWarhead();
    }

    protected bool GetComponentHits(Vector3 hitPosition, Vector3 penDirection, float penDistance, HitResult hitRes, ref ISubDamageable[] hits, out int hitCount)
    {
        Debug.LogError("ISSUE");
        hitCount = 0;
        return false;
    }

    protected int DamageComponents(IDamageable parent, IEnumerable<ISubDamageable> parts, MunitionHitInfo hitInfo, HitResult hitRes, out float damageDone)
    {
        Debug.LogError("ISSUE");
        damageDone = 0;
        return 0;
    }
}