using Mirror;
using UnityEngine.Profiling;

public class AdvancedRadar : ShipState
{
    public bool PingTracksOnly = false;
    public bool ForceBurnthrough = false;
    public bool IgnoreLos = false;
    public bool RequireIllumination = false;
    public float CycleTime = 10f;
}
public static class AdvancedRadarHelpers
{
    public static AdvancedRadar GetAdvancedRadar(this ISensor sensor)
    {
        if (sensor is BaseActiveSensorComponent baseradar)
            return baseradar?.gameObject?.GetComponent<AdvancedRadar>();
        return null;
    }
}

[HarmonyPatch(typeof(SensorTrack), nameof(SensorTrack.AddSensor))]
class SensorTrackAddSensor
{
    static void Prefix(SensorTrack __instance, ISensor sensor, ref AcquisitionType acqType)
    {
        if (sensor.GetAdvancedRadar()?.PingTracksOnly ?? false)
            acqType = AcquisitionType.Ping;
    }
}

[HarmonyPatch(typeof(SensorTrack), nameof(SensorTrack.Update))]
class SensorTrackUpdate
{
    static void Prefix(SensorTrack __instance)
    {
        List<AdvancedRadar> _pingSensors = Common.
            GetVal<List<ISensor>>(__instance, "_pingSensors").
            ConvertAll(a => a?.GetAdvancedRadar()).
            Where(a => a?.PingTracksOnly ?? false)
            .ToList();
        
        if (_pingSensors.Count > 0)
        {
            if (__instance.Age > _pingSensors.ConvertAll(a => a.CycleTime).Min())
            {
                __instance.UpdatePing();
                __instance.Update();
                __instance.UpdateLocks();
            }

        }
    }
}

[HarmonyPatch(typeof(BaseActiveSensorComponent), nameof(BaseActiveSensorComponent.AcquireContacts))]
class BaseActiveSensorComponentAcquireContacts
{
    static void Prefix(BaseActiveSensorComponent __instance)
    {
        if (__instance.GetAdvancedRadar()?.ForceBurnthrough ?? false)
            __instance.MarkNextBurnthroughCycle();
    }
}

//[HarmonyPatch(typeof(SensorTrack), "UpdateMode")]
class SensorTrackUpdateMode
{
    static void Postfix(SensorTrack __instance)
    {
        //Common.SetVal(__instance, "Mode", TrackingMode.Ping);
    }
}

[HarmonyPatch(typeof(BaseActiveSensorComponent), "CanSeeSignature")]
class BaseActiveSensorComponentCanSeeSignature
{
    static bool Prefix(BaseActiveSensorComponent __instance, IActiveSignature sig, ref bool __result)
    {
        AdvancedRadar AR = __instance?.GetAdvancedRadar();
        if (AR == null)
            return true;

        if(AR.RequireIllumination && !sig.IsIlluminated)
        {
            __result = false;
            return false;
        }

        if(AR.IgnoreLos)
        {
            Vector3 toSig = __instance.transform.position.To(sig.Position);
            Vector3 localPosition = __instance.transform.root.InverseTransformDirection(toSig);
            StatValue _statMaxRange = Common.GetVal<StatValue>(__instance, "_statMaxRange");
            StatValue _statRadiatedPower = Common.GetVal<StatValue>(__instance, "_statRadiatedPower");
            StatValue _statBurnthroughPowerMultiplier = Common.GetVal<StatValue>(__instance, "_statBurnthroughPowerMultiplier");
            StatValue _statGain = Common.GetVal<StatValue>(__instance, "_statGain");
            StatValue _statAperture = Common.GetVal<StatValue>(__instance, "_statAperture");
            StatValue _statNoiseFiltering = Common.GetVal<StatValue>(__instance, "_statNoiseFiltering");
            StatValue _statSensitivity = Common.GetVal<StatValue>(__instance, "_statSensitivity");
            bool _runSweepCycle = Common.GetVal<bool>(__instance, "_runSweepCycle");
            ReceivedJamming _jammingSources = Common.GetVal<ReceivedJamming>(__instance, "_jammingSources");
            ISensorProvider _provider = Common.GetVal<ISensorProvider>(__instance, "_provider");
            if (toSig.magnitude - sig.SigRadius > _statMaxRange.Value)
            {
                Profiler.EndSample();
                return true;
            }
            Profiler.BeginSample("Signal Strength");
            float returnPower = sig.GetReturnPowerDensity(_runSweepCycle ? (_statRadiatedPower.Value * _statBurnthroughPowerMultiplier.Value) : _statRadiatedPower.Value, localPosition.magnitude, _statGain.Value, sig.GetCrossSection(toSig.normalized), toSig.normalized) * _statAperture.Value;
            Profiler.EndSample();
            if (SensorMath.SignalLoss(returnPower) < _statSensitivity.Value || returnPower < _provider.AmbientNoiseLevel)
            {
                Profiler.EndSample();
                return true;
            }
            float jammingNoise = (_jammingSources.AnyJamming ? _jammingSources.GetTotalJammingPower(sig.Position) : 0f);
            float totalNoise = SensorMath.CalculateNoiseLevel(_provider.AmbientNoiseLevel, jammingNoise, _statGain.Value, _statAperture.Value, _statNoiseFiltering.Value);
            if (totalNoise > returnPower)
            {
                Profiler.EndSample();
                return true;
            }
            Profiler.EndSample();
            __result = true;
            return false;
        }

        return true;
        //Common.SetVal(__instance, "Mode", TrackingMode.Ping);
    }
}

//[HarmonyPatch(typeof(BaseActiveSensorComponent), "AttemptToGain")]
class BaseActiveSensorComponentAttemptToGain
{
    static void Prefix(BaseActiveSensorComponent __instance, ref bool fromPing, NetworkIdentity targetNetID, ushort trackID, IReadOnlyDictionary<NetworkIdentity, SensorTrackableObject> allTrackable)
    {
        //Debug.LogError("AttemptToGain");
        //fromPing = true;
        /*
        return;
        if (targetNetID != null && allTrackable.TryGetValue(targetNetID, out var value))
        {
            SensorTrack sensorTrack = value.Acquire(__instance, trackID, AcquisitionType.Active);
            sensorTrack.Trackable.Release(__instance, AcquisitionType.Active);
            sensorTrack = value.Acquire(__instance, trackID, AcquisitionType.Ping);
            //sensorTrack.UpdatePing();
        }

        return;
        AdvancedRadar radar = __instance.gameObject.GetComponent<AdvancedRadar>();
        if(radar != null)
            fromPing = radar.PingTracksOnly;
        //fromPing = true;
        */
    }
}

//[HarmonyPatch(typeof(BaseActiveSensorComponent), nameof(BaseActiveSensorComponent.AcquireWithDelta))]
class BaseActiveSensorComponentAcquireWithDelta
{
    static void Prefix(BaseActiveSensorComponent __instance, SensorDelta delta, IReadOnlyDictionary<NetworkIdentity, SensorTrackableObject> allTrackable)
    {
        //Debug.LogError("AcquireWithDelta");
        //return;

    }
}