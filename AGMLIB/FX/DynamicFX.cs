public class DynamicFX : ActiveSettings
{
    // Start is called before the first frame update

    [Space]
    [Header("Target FX")]
    [Space]
    public VisualEffect[] effects;
    public List<DynamicVisibleParticles> dynamicVisibleParticles = new();
    public BookendedAudioPlayer[] audioeffects;
    [Space]
    [Header("Event Settings")]
    [Space]
    public string[] events;
    public bool risingedge;
    public bool continuoustrigger;
    public bool fallingedge;
    bool laststate = false;

    // Update is called once per frame
    protected override void FixedUpdate()
    {

        base.FixedUpdate();

        if (active && continuoustrigger)
            SendTriggers();

        if(laststate != active)
        {
            risingedge = active && !laststate;
            fallingedge = !active && laststate;
        }
        else
        {
            risingedge = false;
            fallingedge = false;
        }
        if (risingedge)
        {
            SendTriggers();
            foreach (BookendedAudioPlayer player in audioeffects)
                player?.Play();
            foreach (DynamicVisibleParticles particle in dynamicVisibleParticles)
                particle.Play();
        }
        else if (fallingedge)
        {
            SendTriggers();
            foreach (BookendedAudioPlayer player in audioeffects)
                player?.Stop();
            foreach (DynamicVisibleParticles particle in dynamicVisibleParticles)
                particle.Stop();
        }
        laststate = active;
    }

    void SendTriggers()
    {
        foreach (VisualEffect effect in effects)
            foreach (string eventi in events)
                effect.SendEvent(eventi);

    }
}

public class StaticFX : MonoBehaviour
{
    public List<DynamicVisibleParticles> dynamicVisibleParticles = new();
    public BookendedAudioPlayer[] audioeffects;
    void Start()
    {
        foreach (BookendedAudioPlayer player in audioeffects)
            player?.Play();
        foreach (DynamicVisibleParticles particle in dynamicVisibleParticles)
            particle.Play();
    }
}