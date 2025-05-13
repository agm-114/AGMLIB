namespace Lib.FX
{
    public class ShortDurationFollowingModularEffect : FollowingModularEffect
    {
        [SerializeField]
        private float _repoolDelay = 1f;

        public override void OnUnpooled()
        {
            base.OnUnpooled();
            RepoolSelfAfterDelay(_repoolDelay, disableImmediately: false);
        }
    }
}
