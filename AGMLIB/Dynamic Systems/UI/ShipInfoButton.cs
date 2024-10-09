using static UI.SequentialButton;
using TooltipTrigger = UI.TooltipTrigger;

public class ShipInfoButton : ShipState
{
    
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
    public void ForceButtonChange(int value)
    {
        if (value < 0)
            value = SelectedOption;
        HandleButtonChanged(value);
        Button?.SetOptionWithoutNotify(value);
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
    public ShipInfoButton ButtonState;
    public SequentialButton Button => GameObject.GetComponentInChildren<SequentialButton>();
    SequentialButtonEvent OnValueChanged => Button.OnValueChanged;
    //RectTransform parentTransform => GameObject.transform.parent.GetComponent<RectTransform>();
    LayoutElement Layout => GameObject.transform.parent.GetComponent<LayoutElement>();
    TextMeshProUGUI Text => GameObject.GetComponentInChildren<TextMeshProUGUI>();

    TooltipTrigger Tooltip => Common.GetVal<TooltipTrigger>(Button, "_tooltip") ?? GameObject.GetComponentInChildren<TooltipTrigger>();

    public ButtonData(SequentialButton template, ShipInfoButton buttondata)
    {
        ButtonState = buttondata;
        Transform templatetransform = template.gameObject.transform.parent;
        GameObject = UnityEngine.Object.Instantiate(templatetransform.gameObject, templatetransform.parent);
        Setup();
    }

    void Setup()
    {
        ButtonState.Data = this;
        Text.text       = ButtonState.ButtonName;
        GameObject.name = ButtonState.ButtonName;
        Tooltip.Text   =  ButtonState.TooltipString;
        Layout.minWidth = 550 / 3 * 4;
        OnValueChanged.RemoveAllListeners();
        Common.SetVal(Button, "_options", ButtonState.Options);
        Common.SetVal(Button, "_originalTooltip", ButtonState.TooltipString);
        ButtonState.ForceButtonChange(ButtonState.SelectedOption);
        //Button.SetOptionWithoutNotify(ButtonState.SelectedOption);
        OnValueChanged.AddListener(ButtonState.HandleButtonChanged);
    }
}

//Button
//-Label
//-Actual Button
[HarmonyPatch(typeof(ShipInfoBar), "MatchAllButtons")]
class ShipInfoBarMatchAllButtons
{
    static Dictionary<string, ButtonData> buttons = new();

    static void Prefix(ShipInfoBar __instance, SequentialButton ____battleshort)
    {
        ShipController _primaryShip = Common.GetVal<ShipController>(__instance, "_primaryShip");
        foreach (var button in buttons)
            if (button.Value != null && button.Value.GameObject != null)
                UnityEngine.Object.Destroy(button.Value.GameObject);
        buttons.Clear();
        foreach (ShipInfoButton infobut in _primaryShip.GetComponentsInChildren<ShipInfoButton>())
        {
            ButtonData newbutton = new(____battleshort, infobut);
            buttons.Add(newbutton.ButtonState.ButtonName, newbutton);
        }
    }
}