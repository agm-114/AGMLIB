namespace AGMLIB.DynamicSystems.Area
{
    public class FalloffEffect<TargetObject> : GenericBasicEffect<TargetObject> where TargetObject : MonoBehaviour
    {

        public AnimationCurve DistanceCurve = new(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
        public bool UseFallOff = false;
        void Start()
        {
            DistanceCurve.preWrapMode = WrapMode.Clamp;
            DistanceCurve.postWrapMode = WrapMode.Clamp;

        }
    }
}
