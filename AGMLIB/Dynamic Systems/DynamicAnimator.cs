public class DynamicAnimator : ActiveSettings
{
    [SerializeField]
    private List<Animator> _animations = new();

    [SerializeField]
    private bool _invertOutput;

    [SerializeField]
    private string _openTagName = "IsOpen";

    [SerializeField]
    private string _closedTagName = "IsClosed";

    [SerializeField]
    private string _openControlVariableName = "OpenControl";

    private bool _stateInitialized;

    public bool IsOpen => HasAnimationState(_openTagName);

    public bool IsClosed => HasAnimationState(_closedTagName);

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (_stateInitialized && active == lastactive)
        {
            return;
        }

        bool output = _invertOutput ? !active : active;
        foreach (Animator animation in _animations)
        {
            animation.SetBool(_openControlVariableName, output);
        }

        _stateInitialized = true;
    }

    private bool HasAnimationState(string tagName)
    {

        foreach (Animator animation in _animations)
        {
            if (!animation.GetCurrentAnimatorStateInfo(0).IsTag(tagName))
            {
                return false;
            }
        }

        return true;
    }
}
