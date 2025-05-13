public class CustomAimPoint : ShipState
{
    /*
    protected new Vector3? GetAimPoint(out bool ignoreRange)
    {

    }
    */

    public static Vector3 EstimateLeadPosition(Vector3 shotPos, float shotVel, Vector3 targetPos, Vector3 targetVel, Vector3 targetAccel, out float flightTime, float positionAge = 0f)
    {
        Vector3 rhs = shotPos.To(targetPos);
        float magnitude = rhs.magnitude;
        float sqrMagnitude = targetVel.sqrMagnitude;
        float num2 = shotVel * shotVel - sqrMagnitude;
        float num3 = -2f * Vector3.Dot(targetVel, rhs);
        float num4 = 0f - magnitude * magnitude;
        float num5 = num3 * num3 - 4f * num2 * num4;
        if (num5 < 0f)
        {
            flightTime = magnitude / shotVel;
            return targetPos;
        }

        flightTime = (0f - num3 + Mathf.Sqrt(num5)) / (2f * num2);
        return targetPos + targetVel * flightTime;
    }
}


[HarmonyPatch(typeof(BaseTurretedLauncherComponent), "FixedUpdate")]
class BaseTurretedLauncherComponentFixedUpdate
{


    public static bool Prefix(BaseTurretedLauncherComponent __instance)
    {
        Common.LogPatch();
        CustomAimPoint customaimpoint = __instance.GetComponentInChildren<CustomAimPoint>();
        if (customaimpoint == null)
            return true;
        ITrack _trainOnTrack = Common.GetVal<ITrack>(__instance, "_trainOnTrack");
        Vector3? _trainOnPosition = Common.GetVal<Vector3?>(__instance, "_trainOnPosition");
        float _missileFlightSpeed = Common.GetVal<float>(__instance, "_missileFlightSpeed"); ;
        Vector3 _trackOffset = Common.GetVal<Vector3>(__instance, "_trackOffset");
        TurretController _turretControl = Common.GetVal<TurretController>(__instance, "_turretControl");
        StatValue _statTraverseRate = Common.GetVal<StatValue>(__instance, "_statTraverseRate");
        StatValue _statElevationRate = Common.GetVal<StatValue>(__instance, "_statElevationRate");
        Vector3? _lastAimPoint = Common.GetVal<Vector3?>(__instance, "_lastAimPoint");
        bool _waitingForCell = Common.GetVal<bool>(__instance, "_waitingForCell"); ;
        float _timeBetweenCells = Common.GetVal<float>(__instance, "_timeBetweenCells"); ;
        float _cellAccum = Common.GetVal<float>(__instance, "_cellAccum"); ;


        if (_trainOnTrack == null && !_trainOnPosition.HasValue)
            return true;
        Vector3 aimPoint;
        if (_trainOnTrack != null)
        {
            if (_trainOnTrack.IsSuperseded)
                _trainOnTrack = _trainOnTrack.NewTrack;
            //Debug.LogError("Running Enhanced Lead");
            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

            aimPoint = MathHelpers.EstimateLeadPosition(__instance.transform.position, _missileFlightSpeed, _trainOnTrack.KnownPosition + _trackOffset, _trainOnTrack.AbsoluteVelocity - customaimpoint.Rigidbody.velocity, Vector3.zero, out var _, _trainOnTrack.Age);
            //cube.transform.position = aimPoint;
        }
        else
        {
            aimPoint = _trainOnPosition.Value;
        }
        _turretControl.FaceTarget(aimPoint, _statTraverseRate.Value, _statElevationRate.Value);
        _lastAimPoint = aimPoint;
        if (_waitingForCell)
        {
            _cellAccum += Time.fixedDeltaTime;
            if (_cellAccum > _timeBetweenCells)
            {
                _cellAccum = 0f;
                _waitingForCell = false;
            }
        }
        Common.SetVal(__instance, "_lastAimPoint", _lastAimPoint);
        Common.SetVal(__instance, "_cellAccum", _cellAccum);
        Common.SetVal(__instance, "_waitingForCell", _waitingForCell);
        return false;
    }
}

[HarmonyPatch(typeof(DiscreteWeaponComponent), "CalculateLead")]
class WeaponComponentGetAimPoint
{
    public static void Postfix(DiscreteWeaponComponent __instance, Vector3 target, Vector3 velocity, Vector3 acceleration, float age, ref Vector3 __result)
    {
        Common.LogPatch();
        CustomAimPoint customaimpoint = __instance.GetComponentInChildren<CustomAimPoint>();
        if (customaimpoint == null)
            return;
        __result = MathHelpers.EstimateLeadPosition((__instance as IWeapon).AimFromPosition, __instance.MuzzleVelocity, target, velocity - customaimpoint.Rigidbody.velocity, acceleration, out float flightTime, age);
    }
}