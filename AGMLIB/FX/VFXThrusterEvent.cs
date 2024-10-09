using UnityEngine.VFX.Utility;

[RequireComponent(typeof(VisualEffect))]

public class VFXThrusterEvent : VFXOutputEventAbstractHandler
{
    public override bool canExecuteInEditor => true;
    public PulsedThrusterPart PulsedThrusterPart;

    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute) => PulsedThrusterPart.PlayPulse();
}