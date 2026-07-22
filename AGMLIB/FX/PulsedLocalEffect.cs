namespace Lib.FX
{
    public class PulsedLocalEffect : MonoBehaviour
    {
        [SerializeField] private VisualEffect _effect;
        [SerializeField] private string _localPositionParameter = "HitPosition";
        [SerializeField] private string _pulseEvent = "OnHit";

        public void Pulse(Vector3 worldPosition)
        {
            if (_effect == null)
            {
                Common.Hint($"PulsedLocalEffect on '{gameObject.name}' does not have a VisualEffect assigned");
                return;
            }

            Vector3 localPosition = _effect.transform.InverseTransformPoint(worldPosition);

            if (!string.IsNullOrEmpty(_localPositionParameter))
                _effect.SetVector3(_localPositionParameter, localPosition);

            if (!string.IsNullOrEmpty(_pulseEvent))
                _effect.SendEvent(_pulseEvent);
        }
    }
}
