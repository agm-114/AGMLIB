using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Game.Map;
using UnityEngine;
using Sound;
using Ships;

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