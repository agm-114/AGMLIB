using Telepathy;

public class DynamicTransform : ActiveSettings
{
    // Start is called before the first frame update
    [Space]
    [Header("Animation Settings")]
    [Space]
    public Transform Start;
    public Transform End;
    public Transform Target;
    public float MaxTime = 1f;
    public AnimationCurve AnimationCurve = new(new Keyframe(0, 0), new Keyframe(1, 1));

    public float SlerpVlaue => Mathf.Clamp(AnimationCurve.Evaluate(accum), 0, 1);
    public float EndValue => 1 - SlerpVlaue;
    private float accum = 0f;
    
    //public GameObject target;

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (active)
        {
            //Common.Trace(this.gameObject, $"p+{accum}, {Target.transform.position}, {Target.transform.rotation}");

            accum += Time.fixedDeltaTime;
            //Common.Trace(this.gameObject, $"a+{accum}, {Target.transform.position}, {Target.transform.rotation}");

            //C
        }
        else
        {
            //Common.Trace(this.gameObject, $"p-{accum}, {Target.transform.position}, {Target.transform.rotation}");

            accum -= Time.fixedDeltaTime;
            //Common.Trace(this.gameObject, $"a-{accum}, {Target.transform.position}, {Target.transform.rotation}");

            //Common.Trace(this.gameObject, "accum dead");
        }

        if (accum < 0f)
            accum = 0f;
        else if(accum > MaxTime)
            accum = MaxTime;
        //Common.Trace(this.gameObject, $"{accum}, {Target.transform.position}, {Target.transform.rotation}");

        Target.transform.position = Vector3.Slerp(Start.transform.position, End.transform.position, SlerpVlaue);
        Target.transform.rotation = Quaternion.Slerp(Start.transform.rotation, End.transform.rotation, SlerpVlaue);
    }
}
