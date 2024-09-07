using Game.EWar;
using Game.Sensors;
using Mirror;
using Munitions;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Descriptors.Seekers;
using Munitions.ModularMissiles.Runtime.Seekers;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AGMLIB.Dynamic_Systems.Area
{
    public class JammingEffect<TargetObject> : FalloffEffect<TargetObject>, IJammingSource where TargetObject : MonoBehaviour
    {

        public float EffectAreaRatio = 0.4f;
        public float RadiatedPower = 2000f;
        public float Gain = 2f;

        public bool ShowLOB = true;
        private Dictionary<IJammable, JammedVolume> _volumes = new Dictionary<IJammable, JammedVolume>();

        public JammedVolume GetJammedVolume(IJammable target)
        {
            JammedVolume volume = null;
            if (!_volumes.TryGetValue(target, out volume))
            {
                volume = new JammedVolume(this, target, EffectAreaRatio);
                _volumes.Add(target, volume);
            }
            return volume;
        }
        public float JammingPowerAtPoint(Vector3 point) => GetPowerAtPoint(point);

        protected float GetPowerAtPoint(Vector3 point)
        {
            if (!UseFallOff)
                return RadiatedPower * Gain;
            float R = Vector3.Distance(base.transform.position, point) * 10f;
            float sG = SensorMath.PowerDensityAtDistance(RadiatedPower, R, Gain);
            return sG;
        }
        public NetworkIdentity NetID => Ship.netIdentity;
        public bool ShowJammingLOB => ShowLOB;
        public Vector3 Position => transform.position;
        public Vector3 PlatformPosition => Ship.transform.position;
        public ISensorTrackable Platform => ShipController.Trackable;
    }

    public class ShipJammingEffect : JammingEffect<Ship>
    {

    }

    public class MissileJammingEffect : JammingEffect<ModularMissile>
    {
        public List<SignatureType> SoftKillWavelengths = new(3) { SignatureType.PassiveRadar | SignatureType.Radar | SignatureType.NoSignature };
        public float SoftKillRecycle = 1;
        public float FailChance = 1;
        public bool FailValidators = true;
        public bool JamSeekers = true;
        public bool SoftKillAntiJamVals = true;
        public bool JamAntiJam = true;


        public override void AreaEffectState()
        {
            foreach (RuntimeMissileSeeker seeker in target.GetComponentsInChildren<RuntimeMissileSeeker>())
            {
                bool antijam = seeker.Descriptor is PassiveSeekerDescriptor passive && passive.CanPursueJamming;
                if (!SoftKillWavelengths.Contains(seeker.DecoySigType))
                    return;
                Debug.LogError("Softkilling " + target.gameObject.name);
                if (SoftKillAntiJamVals && FailChance >= Random.Range(0.0f, 1f))
                {
                    if (antijam && !SoftKillAntiJamVals)
                        continue;
                    Common.SetVal(seeker, "_validationReliable", false);
                }


                if (JamSeekers)
                {
                    if (antijam && !JamAntiJam)
                        continue;
                    ReceivedJamming jammming = Common.GetVal<ReceivedJamming>(seeker, "_jammingSources");
                    if (_active)
                        jammming.AddSource(this);
                    else
                        jammming.RemoveSource(this);
                }


            }
        }

    }
}
