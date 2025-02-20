using Game.Sensors;
using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Lib.Generic_Gameplay.Ewar
{
    public abstract class BaseTrackLogic : MonoBehaviour
    {
        public abstract void UpdateTrack(ITrack track, out Vector3 position, out Vector3 velocity, CachedCrossSectionData? cachedCS = null);

        public static void DefaultUpdateTrack(ITrack track, out Vector3 position, out Vector3 velocity, CachedCrossSectionData? cachedCS = null)
        {
            position = track.KnownPosition;
            velocity = Vector3.zero; ;
        }
    }
    public class ListTrackLogic : BaseTrackLogic
    {
        public List<ContactClassification> classifications = new();
        public List<BaseTrackLogic> trackLogics = new List<BaseTrackLogic>();
        public BaseTrackLogic FallbackTrackLogic;

        private Dictionary<ContactClassification, BaseTrackLogic> contactClassifications = new();


        public void Awake()
        {
            HashSet<ContactClassification> seenKeys = new HashSet<ContactClassification>();
            foreach (var classification in classifications)
            {
                if (!seenKeys.Add(classification))
                {
                    throw new ArgumentException($"Duplicate key found in classifications: {classification}");
                }
            }

            // Check if the second list has fewer elements than the first
            if (trackLogics.Count < classifications.Count)
            {
                throw new ArgumentException("Second list has fewer elements than the first list.");
            }

            // Create the dictionary
            contactClassifications = new Dictionary<ContactClassification, BaseTrackLogic>();

            for (int i = 0; i < classifications.Count; i++)
            {
                contactClassifications.Add(classifications[i], trackLogics[i]);
            }

        }


        public override void UpdateTrack(ITrack track, out Vector3 position, out Vector3 velocity, CachedCrossSectionData? cachedCS = null)
        {
            if(contactClassifications.TryGetValue(track.Trackable.ContactType, out BaseTrackLogic? value))
            {
                value.UpdateTrack(track, out position, out velocity);
                return;
            }

            if (FallbackTrackLogic == null)
                BaseTrackLogic.DefaultUpdateTrack(track, out position, out velocity);
            else
                FallbackTrackLogic.UpdateTrack(track, out position, out velocity);
        }
    }

    public class RatioTrackLogic : BaseTrackLogic
    {
        public int TruePostionWeight = 10;
        public int KnownPositionWeight = 1;
        //public int TrueVelocityWeight = 10;
        //public int KnownPositionWeight = 1;
        public override void UpdateTrack(ITrack track, out Vector3 position, out Vector3 velocity, CachedCrossSectionData? cachedCS = null)
        {
            position = ((track.KnownPosition * KnownPositionWeight) + (track.TruePosition * TruePostionWeight)) / (TruePostionWeight + KnownPositionWeight);
            velocity = Vector3.zero;
        }
    }

    public class FirecontrolTrackLogic : BaseTrackLogic
    {
        public FireControlSensor FireControlSensor;

        public override void UpdateTrack(ITrack track, out Vector3 position, out Vector3 velocity, CachedCrossSectionData? cachedCS = null)
        {
            FireControlSensor.SetLockedTarget(track.ID);
            FireControlSensor.AcquireContacts(transform);
            FireControlSensor.UpdateTrack(track.Trackable, out position, out velocity);
        }
    }
}
