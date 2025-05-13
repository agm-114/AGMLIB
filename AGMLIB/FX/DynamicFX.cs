public class DynamicFX : ActiveSettings
{
    // Start is called before the first frame update

    [Space]
    [Header("Target FX")]
    [Space]
    public VisualEffect[] effects;
    public BookendedAudioPlayer[] audioeffects;
    [Space]
    [Header("Event Settings")]
    [Space]
    public string[] events;
    public bool risingedge;
    public bool continuoustrigger;
    public bool fallingedge;
    bool laststate;
    void Start() => laststate = active;

    // Update is called once per frame
    protected override void FixedUpdate()
    {

        base.FixedUpdate();

        if (active && continuoustrigger)
            SendTriggers();
        else if (laststate != active)
        {
            if (active && risingedge)
            {
                SendTriggers();
                foreach (BookendedAudioPlayer player in audioeffects)
                    player?.Play();
            }
            else if (fallingedge)
            {
                SendTriggers();
                foreach (BookendedAudioPlayer player in audioeffects)
                    player?.Stop();
            }
            laststate = active;
        }
    }

    void SendTriggers()
    {
        foreach (VisualEffect effect in effects)
            foreach (string eventi in events)
                effect.SendEvent(eventi);

    }
}
