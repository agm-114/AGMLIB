using System.Collections;

public enum AreaTeamEffect
{
    None = 0,
    ReduceCrew = 1,
    KillTeam = 2,
    SuppressOnly = 3
}

[CreateAssetMenu(fileName = "New Area DC Team Profile", menuName = "Nebulous/Modules/Area Effects/DC Team Profile")]
public class AreaDamageControlTeamProfileModule : ScriptableObject, IOnDamageableImpact, IOnGenericImpact
{
    [SerializeField]
    protected float _effectRadius = 0f;

    [SerializeField]
    protected AreaTeamEffect _teamEffect = AreaTeamEffect.ReduceCrew;

    [SerializeField]
    protected int _crewRemoved = 1;

    [SerializeField]
    protected float _suppressionSeconds = 0f;

    public float EffectRadius => _effectRadius;
    public AreaTeamEffect TeamEffect => _teamEffect;
    public int CrewRemoved => _crewRemoved;
    public float SuppressionSeconds => _suppressionSeconds;

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
        DamageControlDispatcher dispatcher = ship?.GetComponent<DamageControlDispatcher>();
        if (hitInfo == null || ship == null || dispatcher == null)
        {
            return;
        }

        if (SuppressionSeconds > 0f)
        {
            ship.gameObject.GetOrAddComponent<AreaDamageControlSuppressionState>()
                .Apply(dispatcher, SuppressionSeconds);
        }

        foreach (DamageControlTeam team in dispatcher.AllTeams.Where(team => TeamInRadius(ship, team, hitInfo.Point, EffectRadius)))
        {
            switch (TeamEffect)
            {
                case AreaTeamEffect.ReduceCrew:
                    team.AdjustCrewCount(-Mathf.Max(0, CrewRemoved));
                    break;
                case AreaTeamEffect.KillTeam:
                    team.AdjustCrewCountAbsolute(0);
                    break;
                case AreaTeamEffect.SuppressOnly:
                case AreaTeamEffect.None:
                default:
                    break;
            }
        }
    }

    private static bool TeamInRadius(ShipController ship, DamageControlTeam team, Vector3 position, float radius)
    {
        if (team == null || team.IsDead)
        {
            return false;
        }

        Vector3 teamPosition = team.CurrentLocation != null
            ? team.CurrentLocation.transform.position
            : ship.transform.TransformPoint(team.CurrentPosition);
        return Vector3.Distance(teamPosition, position) <= Mathf.Max(0.1f, radius);
    }
}

public class AreaDamageControlSuppressionState : MonoBehaviour
{
    private DamageControlDispatcher _dispatcher;
    private float _endTime;

    public void Apply(DamageControlDispatcher dispatcher, float seconds)
    {
        if (dispatcher == null || seconds <= 0f)
        {
            return;
        }

        _dispatcher = dispatcher;
        _endTime = Mathf.Max(_endTime, Time.time + seconds);
        _dispatcher.FreezeAllDamageControl(true);
    }

    private void Update()
    {
        if (_dispatcher == null)
        {
            Destroy(this);
            return;
        }

        if (Time.time < _endTime)
        {
            _dispatcher.FreezeAllDamageControl(true);
            return;
        }

        _dispatcher.FreezeAllDamageControl(false);
        Destroy(this);
    }
}
