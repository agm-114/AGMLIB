public class DynamicButtonState : ActiveSettings
{
    // Start is called before the first frame update
    public enum ButtonLockState
    {
        Ignore,
        Lock,
        Unlock
    }


    class ForcedButtonState
    {
        public String ButtonName = "";
        public int Index = -1;
        public String State = "";
        public ButtonLockState LockState = ButtonLockState.Ignore;
    }

    [Space]
    [Header("Button State Settings")]
    [Space]
    public List<string> LockButtonNames = new();
    public List<int> SetStateIndexes = new();
    public List<string> SetStateStrings = new();
    public List<ButtonLockState> LockState = new();

    private List<ForcedButtonState> forcedButtonStates = new();

    protected void Start()
    {

        for (int i = 0; i < LockButtonNames.Count; i++)
        {
            ForcedButtonState newstate = new();
            forcedButtonStates.Add(newstate);
            newstate.ButtonName = LockButtonNames.ElementAt(i);
            if (i < SetStateIndexes.Count)
            {
                newstate.Index = SetStateIndexes.ElementAt(i); ;
            }
            if (i < SetStateStrings.Count)
            {
                newstate.State = SetStateStrings.ElementAt(i); ;
            }
            if (i < LockState.Count)
            {
                newstate.LockState = LockState.ElementAt(i); ;
            }
        }
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {

        base.FixedUpdate();
        if (!active)
            return;
        foreach(ForcedButtonState forcedButtonState in forcedButtonStates)
        {
            ShipInfoButton button = ShipInfoButton.FindButton(ShipController, forcedButtonState.ButtonName);

            if (button == null)
                continue;
            switch (forcedButtonState.LockState)
            {
                case ButtonLockState.Lock:
                    button.Override = true;
                    break;
                case ButtonLockState.Unlock:
                    button.Override = false;
                    break;
                default:
                    break;
            }
            if (forcedButtonState.Index < 0 && forcedButtonState.State.Length > 0)
                forcedButtonState.Index = button.States.FindIndex(a => a == forcedButtonState.State);
            button.ForceButtonChange(forcedButtonState.Index);
        }


    }
}
