namespace Lib.FX
{
    public class SpatialSoundEffectModule : MonoBehaviour, IEffectModule
    {
        [SerializeField] protected VariedSoundEffect _sound;
        public void Play() { GlobalSFX.PlayOneShotSpatial(_sound, transform); }
        public void SetEffectParameter(string name, float value) { }
        public void Stop() { }
    }
}
