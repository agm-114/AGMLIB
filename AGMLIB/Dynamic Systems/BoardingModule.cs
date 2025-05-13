using UnityEngine.UI.Extensions;
using Object = UnityEngine.Object;
using TooltipTrigger = UI.TooltipTrigger;
public class BoardingModule : MonoBehaviour
{

}

public class AssualtTeam : DamageControlTeam
{
    public AssualtTeam(DamageControlDispatcher dispatcher, DCLockerComponent fromLocker, int number, int crewSize) : base(dispatcher, fromLocker, number, crewSize)
    {
    }
}

public class AssultTeamPiece : MonoBehaviour
{
    private DamageControlBoard _board;
    private ShipController _shipController;

    [SerializeField]
    private Image _progressBar;

    [SerializeField]
    private TooltipTrigger _tooltip;

    [SerializeField]
    private GameColors.ColorName _idleColor;

    [SerializeField]
    private GameColors.ColorName _enRouteColor;

    [SerializeField]
    private GameColors.ColorName _workingColor;

    [SerializeField]
    private GameColors.ColorName _deadColor;

    public void Setup(DCTeamPiece source, ShipController ship, DamageControlBoard board)
    {
        Debug.LogError("Settingup team");
        _shipController = ship;
        _board = board;
        _progressBar = Common.GetVal<Image>(source, "_progressBar");
        _tooltip = Common.GetVal<TooltipTrigger>(source, "_tooltip");

        _idleColor = Common.GetVal<GameColors.ColorName>(source, "_idleColor");
        _enRouteColor = Common.GetVal<GameColors.ColorName>(source, "_enRouteColor");
        _workingColor = Common.GetVal<GameColors.ColorName>(source, "_workingColor");
        _deadColor = Common.GetVal<GameColors.ColorName>(source, "_deadColor");
        Component.Destroy(source);

        base.transform.localScale = new Vector3(1f, 1f, 1f);

        _progressBar.fillAmount = 1f;
        _progressBar.color = GameColors.Red;
    }

    public void Update()
    {
        ShipStatusDetailPart detailPart = _board.GetDetailPart(_shipController?.GetComponentsInChildren<HullPart>()?.ToList()?.First());

        if (detailPart != null)
        {
            Debug.LogError("Boarding " + detailPart.name);
            base.transform.SetParent(detailPart.transform);
        }
    }
}

[HarmonyPatch(typeof(DamageControlBoard), nameof(DamageControlBoard.LinkShip))]
class DamageControlBoardLinkShip
{
    static void Postfix(DamageControlBoard __instance, ShipController ship)
    {
        return;
        Common.LogPatch();
        DamageControlBoardResources _resources = __instance.transform.parent.gameObject.GetComponent<DamageControlBoardResources>();
        //var random = new System.Random();
        //DCLockerComponent dCLockerComponent = ship.GetComponentsInChildren<DCLockerComponent>().ToList()[0];
        //AssualtTeam allTeam = new(null, dCLockerComponent, 1, dCLockerComponent.GetCrewRequirement());
        //allTeam.SetLocation(ship.GetComponentsInChildren<HullPart>().ToList()[random.Next(10)]);
        DCTeamPiece component = Object.Instantiate(_resources.TeamTokenPrefab, __instance.transform).GetComponent<DCTeamPiece>();
        component.gameObject.GetOrAddComponent<AssultTeamPiece>().Setup(component, ship, __instance);
        //component.Set(__instance, allTeam);
    }
}