public class PulsedLineBeamMuzzleEffects : LineBeamMuzzleEffects
{
    
    public override void StopEffect()
    {
        //RaycastMuzzle
        AudioSource holder = _fireSoundEffect;
        if (_fireSoundEffect?.isPlaying ?? false)
            _fireSoundEffect = null;
        base.StopEffect();
        _fireSoundEffect = holder;
    }
}