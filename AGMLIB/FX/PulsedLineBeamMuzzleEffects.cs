public class PulsedLineBeamMuzzleEffects : ResizingLineBeamMuzzleEffects
{

    protected override void Awake()
    {
        PulseAudio = true;
        base.Awake();
    }
}