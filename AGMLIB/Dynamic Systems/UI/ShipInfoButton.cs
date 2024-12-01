using static Game.Orders.Tasks.OrderTask;
using static UI.SequentialButton;
using TooltipTrigger = UI.TooltipTrigger;

public class ShipInfoButton : ShipState
{
    public bool Unique  = false;
    public string ButtonName = "BTN";
    public StringFormatter? Tooltip;
    public String TooltipString => Tooltip?.ToString() ?? ""; 
    public List<string> States = new() { "ACTV", "DEAD"};
    public List<GameColors.ColorName> Colors = new() { GameColors.ColorName.Red, GameColors.ColorName.Green };
    public int SelectedOption = 1;
    public bool Override = false;
    public ButtonData? Data;
    public SequentialButton Button => Data?.Button;
    public static ShipInfoButton FindButton(ShipController shipController, string buttonname) => shipController.GetComponentsInChildren<ShipInfoButton>().Where(a => a.ButtonName == buttonname).First();
    public static SequenceOption CreateOption(string name, GameColors.ColorName color)
    {
        SequenceOption option = new()
        {
            Text = name,
            TextColor = color
        };
        return option;
    }
    public void HandleButtonChanged(int value)
        //Debug.LogError("Button Clicked" + value);
        => SelectedOption = value;
    public void ForceButtonChange(int value, bool mixed = false)
    {
        //if (value == SelectedOption)
        //    return;
        if (value < 0)
            value = SelectedOption;
        HandleButtonChanged(value);
        Button?.SetOptionWithoutNotify(value, mixed);
        if (Override)
            Button?.SetOverride(value);
        else
            Button?.SetOverride(null);
            Button?.SetEnabled(enabled: true);
    }
    public SequenceOption[] Options => States.Zip(Colors, (name, color) => CreateOption(name, color)).ToArray();
    public SequenceOption CurrentOption => Options[SelectedOption];
}

public class ButtonData
{
    public GameObject GameObject;
    private ShipInfoButton PrimaryInfoButton;
    public string ButtonName => PrimaryInfoButton.ButtonName;

    public List<ShipInfoButton> AuxButtonStates = new();
    public SequentialButton? Button => FindButton();
    public SequentialButtonEvent OnValueChanged => Button.OnValueChanged;
    //RectTransform parentTransform => GameObject.transform.parent.GetComponent<RectTransform>();
    //LayoutElement Layout => GameObject.transform.parent.GetComponent<LayoutElement>();
    TextMeshProUGUI Text => GameObject.GetComponentInChildren<TextMeshProUGUI>();

    TooltipTrigger Tooltip => Common.GetVal<TooltipTrigger>(Button, "_tooltip") ?? GameObject.GetComponentInChildren<TooltipTrigger>();

    public SequentialButton? FindButton()
    {
        if (GameObject == null)
            return null;
        return GameObject.GetComponentInChildren<SequentialButton>();
    }

    private bool mixed = false;
    public void Setup(SequentialButton template, ShipInfoButton buttondata, List<ShipController> shipGroup)
    {

        PrimaryInfoButton = buttondata;

        IEnumerable<ShipInfoButton>  newAuxButtonStates = shipGroup.ConvertAll(ship => ShipInfoButton.FindButton(ship, PrimaryInfoButton.ButtonName));
        newAuxButtonStates = newAuxButtonStates.Where(s => s != null);


        Transform templatetransform = template.gameObject.transform.parent;
        GameObject = UnityEngine.Object.Instantiate(templatetransform.gameObject, templatetransform.parent);

        Text.text       = ButtonName;
        GameObject.name = ButtonName;
        Tooltip.Text   = PrimaryInfoButton.TooltipString;
        //Layout.minWidth = 550 / 3 * 4;
        OnValueChanged.RemoveAllListeners();
        Common.GetVal<SequenceOption[]>(Button, "_options");

        Common.SetVal(Button, "_onValueChanged", new SequentialButtonEvent());
        Common.SetVal(Button, "_options", PrimaryInfoButton.Options);

        Common.SetVal(Button, "_originalTooltip", PrimaryInfoButton.TooltipString);
        //Debug.LogError(PrimaryInfoButton.ButtonName);
        //foreach(string state in ButtonState.States)
        //    Debug.LogError("state: " + state);
        //Debug.LogError("dflt: " + ButtonState.SelectedOption);
        mixed = newAuxButtonStates.ConvertAll(s => s.SelectedOption).Any(s => s != PrimaryInfoButton.SelectedOption);

        AddInfoButton(PrimaryInfoButton);


        //Button.SetOptionWithoutNotify(ButtonState.SelectedOption);
        foreach (ShipInfoButton auxbutton in newAuxButtonStates)
            AddInfoButton(PrimaryInfoButton);

    }
    public void AddInfoButton(ShipInfoButton infobutton)
    {
        if (AuxButtonStates.Contains(infobutton))
            return;
        infobutton.Data = this;
        infobutton.ForceButtonChange(infobutton.SelectedOption, mixed);
        OnValueChanged.AddListener(infobutton.HandleButtonChanged);
        AuxButtonStates.Add(infobutton);
    }

    public static void Resize(SequentialButton template, int extrabuttons = 0)
    {
        LayoutElement Layout = template.gameObject.transform.parent.GetComponent<LayoutElement>();
        if(Layout != null) 
            Layout.minWidth = 550 / 3 * (3 + (int)Math.Ceiling(extrabuttons / 2.0));
    }
}

//Button
//-Label
//-Actual Button
[HarmonyPatch(typeof(ShipInfoBar), "MatchAllButtons")]
class ShipInfoBarMatchAllButtons
{
    static Dictionary<string, ButtonData> uibuttons = new();

    static void Prefix(ShipInfoBar __instance, SequentialButton ____battleshort)
    {
        //Debug.LogError("buttonmatch");
        ShipController _primaryShip = Common.GetVal<ShipController>(__instance, "_primaryShip");
        List<ShipController> _shipGroup = Common.GetVal<List<ShipController>>(__instance, "_shipGroup") ?? new();
        foreach (var button in uibuttons)
            if (button.Value != null && button.Value.GameObject != null)
                UnityEngine.Object.Destroy(button.Value.GameObject);
        uibuttons.Clear();

        ShipInfoButton[] shipbuttons = _primaryShip.GetComponentsInChildren<ShipInfoButton>();

        ButtonData.Resize(____battleshort, shipbuttons.Count());

        foreach (ShipInfoButton infobut in shipbuttons)
        {
            if(uibuttons.TryGetValue(infobut.ButtonName, out ButtonData? value))
            {
                if(infobut.Unique)
                {
                    ButtonData newrandombutton = new();
                    newrandombutton.Setup(____battleshort, infobut, _shipGroup);
                    uibuttons.Add(newrandombutton.ButtonName, newrandombutton);
                }
                else
                    value.AddInfoButton(infobut);
                continue;

            }

            ButtonData newbutton = new();
            newbutton.Setup(____battleshort, infobut, _shipGroup);
            uibuttons.Add(newbutton.ButtonName, newbutton);
        }

    }
}