using Ships;
using Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;
using UnityEngine.Serialization;
using Utility;

[RequireComponent(typeof(VisualEffect))]

public class VFXThrusterEvent : VFXOutputEventAbstractHandler
{
    public override bool canExecuteInEditor => true;
    public PulsedThrusterPart PulsedThrusterPart;

    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute) => PulsedThrusterPart.PlayPulse();
}