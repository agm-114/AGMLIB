using static Game.Orders.Tasks.OrderTask;
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
        if (value == SelectedOption)
            return;
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
    public SequentialButton? Button => FindButton();
    SequentialButtonEvent OnValueChanged => Button.OnValueChanged;
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

    public void Setup(SequentialButton template, ShipInfoButton buttondata)
    {
        ButtonState = buttondata;
        Transform templatetransform = template.gameObject.transform.parent;
        GameObject = UnityEngine.Object.Instantiate(templatetransform.gameObject, templatetransform.parent);
        ButtonState.Data = this;
        Text.text       = ButtonState.ButtonName;
        GameObject.name = ButtonState.ButtonName;
        Tooltip.Text   =  ButtonState.TooltipString;
        //Layout.minWidth = 550 / 3 * 4;
        OnValueChanged.RemoveAllListeners();
        Common.SetVal(Button, "_options", ButtonState.Options);
        Common.SetVal(Button, "_originalTooltip", ButtonState.TooltipString);
        ButtonState.ForceButtonChange(ButtonState.SelectedOption);
        //Button.SetOptionWithoutNotify(ButtonState.SelectedOption);
        OnValueChanged.AddListener(ButtonState.HandleButtonChanged);
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
        ShipController _primaryShip = Common.GetVal<ShipController>(__instance, "_primaryShip");
        foreach (var button in uibuttons)
            if (button.Value != null && button.Value.GameObject != null)
                UnityEngine.Object.Destroy(button.Value.GameObject);
        uibuttons.Clear();

        ShipInfoButton[] shipbuttons = _primaryShip.GetComponentsInChildren<ShipInfoButton>();

        ButtonData.Resize(____battleshort, shipbuttons.Count());

        foreach (ShipInfoButton infobut in shipbuttons)
        {
            ButtonData newbutton = new();
            newbutton.Setup(____battleshort, infobut);
            uibuttons.Add(newbutton.ButtonState.ButtonName, newbutton);
        }

    }
}