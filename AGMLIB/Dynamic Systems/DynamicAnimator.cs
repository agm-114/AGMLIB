public class DynamicAnimator : ActiveSettings
{
    [SerializeField]
    private Animator _animations = null!;

    [SerializeField]
    private string _openTagName = "IsOpen";

    [SerializeField]
    private string _closedTagName = "IsClosed";

    [SerializeField]
    private string _openControlVariableName = "OpenControl";

    private bool _stateInitialized;

    public bool IsOpen => _animations.GetCurrentAnimatorStateInfo(0).IsTag(_openTagName);

    public bool IsClosed => _animations.GetCurrentAnimatorStateInfo(0).IsTag(_closedTagName);

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (_stateInitialized && active == lastactive)
        {
            return;
        }

        _animations.SetBool(_openControlVariableName, active);
        _stateInitialized = true;
    }
}
