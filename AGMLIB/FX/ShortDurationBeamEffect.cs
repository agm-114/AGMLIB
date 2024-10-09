public class BeamEffectModule : MonoBehaviour, IEffectModule
{
    public ShortDurationEffect Effect => gameObject.GetComponentInChildren<ShortDurationEffect>();
    public LineBeamMuzzleEffects LineBeamMuzzleEffects => gameObject.GetComponentInChildren<LineBeamMuzzleEffects>();
    
    protected LineRenderer Beam => Common.GetVal<LineRenderer>(LineBeamMuzzleEffects, "_beam");
    private float _accumulator = 0;
    private float Accumulator
    {
        get 
        {
            if(Effect != null)
                return Common.GetVal<float>(Effect, "_accumulator");
            //Debug.LogError("using Internal Accum");
            return _accumulator;
        }   // get method
    }

    public AnimationCurve BeamWidth = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
    public float StopTime = 10f;


    private void Start()
    {
        BeamWidth.preWrapMode = WrapMode.Clamp;
        BeamWidth.postWrapMode = WrapMode.Clamp;
        _accumulator = 0;
        Beam.widthMultiplier = BeamWidth.Evaluate(Accumulator);
    }

    private void Update()
    {
        //Debug.LogError("Current Accum " + Accumulator);
        Beam.widthMultiplier = BeamWidth.Evaluate(Accumulator);
        _accumulator += Time.deltaTime;
        if (Accumulator > StopTime)
            Stop();

    }

    public void Play()
    {
        Start();
    }
    public void Stop()
    {
        //Debug.LogError("Stopping");
        LineBeamMuzzleEffects?.SetBeamLength(0);
        LineBeamMuzzleEffects?.StopEffect();
        Start();
    }
    public void SetEffectParameter(string name, float value) { }
}

public class ShortDurationBeamEffect : BeamEffectModule
{

}