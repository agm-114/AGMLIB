﻿using Game.Map;
public class PulsedThrusterPart : ThrusterPart
{
    private bool EffectPlaying => Common.GetVal<bool>(this, "_effectPlaying");


    public BaseSoundEffect PulsedSimpleSoundEffect;

    public AudioSource PulsedSimpleAudioSource;

    private VisibleObject Visible => base.gameObject.GetComponentInParent<VisibleObject>();

    protected override void Awake()
    {
        base.Awake();
        if (PulsedSimpleAudioSource == null)
            PulsedSimpleAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayPulse()
    {
        if (EffectPlaying && Visible.IsVisible)
            PulsedSimpleSoundEffect.PlayFromSource(PulsedSimpleAudioSource);
    }
}