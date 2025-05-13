using UnityEngine.Profiling;

namespace Lib.Generic_Gameplay.Engines
{
    public class RuntimeThruster : HullPart, IThruster
    {

        private AttitudeControl _attitudeInfluence;
        private AttitudeControl _attitudeInverse;
        private Direction _thrustDirection;

        private AngledThruster _parent;
        float _localpower;

        private IThrustController _thrustController;


        bool IThruster.IsFunctional => _parent.IsFunctional;
        float IThruster.LateralPower => _parent.Power * _localpower;
        Direction IThruster.LateralDirection => _thrustDirection;
        AttitudeControl IThruster.AttitudeDirections => _attitudeInfluence;


        private Sides _lastLateral = Sides.None;
        private AttitudeControl _lastAngular = AttitudeControl.None;
        private enum Throttle
        {
            Idle = 0,
            Half = 1,
            Full = 2
        }
        private Throttle _currentThrottle = Throttle.Idle;
        //private Throttle _lastThrottle = Throttle.Idle;
        //public bool ThrottleChange => _currentThrottle != _lastThrottle;
        public float CurrentThottle => ((float)((int)_currentThrottle) / 2) * _localpower;

        public bool MainEngine => _parent._mainEngine;

        private bool _providingLateralThrust = false;

        public event HullPart.PartChanged OnIsFunctionalChanged;

        public void RuntimeUpdate()
        {
            Profiler.BeginSample("Update Thrusters");
            if (_thrustController.WarpMode)
                return;
            if (_lastLateral != _thrustController.LateralThrust || _lastAngular != _thrustController.AngularThrust)
            {
                //_lastThrottle = _currentThrottle;

                _lastAngular = _thrustController.AngularThrust;
                _lastLateral = _thrustController.LateralThrust;
                bool lateral = _lastLateral.IsSet(_thrustDirection);
                bool lateralOpposite = _lastLateral.IsSet(_thrustDirection.Flip());
                bool angular = (_lastAngular & _attitudeInfluence) != 0;
                bool angularOpposite = (_lastAngular & _attitudeInverse) != 0;

                _providingLateralThrust = lateral;
                if (lateral && !angular)
                {
                    if (angularOpposite)
                        _currentThrottle = Throttle.Half;
                    else
                        _currentThrottle = Throttle.Full;
                }
                else if (angular)
                {
                    if (lateralOpposite)
                        _currentThrottle = Throttle.Idle;
                    else
                        _currentThrottle = Throttle.Full;
                }
                else
                {
                    _currentThrottle = Throttle.Idle;
                }
                //Debug.LogError(gameObject.name + " " + _thrustDirection + " " + _currentThrottle + CurrentThottle);

            }
            Profiler.EndSample();
        }



        void IThruster.SetThrustController(IThrustController controller)
        {
            //Debug.LogError(gameObject.name + " child controller setup");
            _thrustController = controller;
            if (_parent == null)
            {
                //Debug.LogError("nullparent");
                //return;
            }
            _parent.SetThrustController(controller);
        }

        private void ParentDisabled(HullPart part)
        {
            OnIsFunctionalChanged?.Invoke(part);
        }


        public void Setup(AngledThruster parent, Vector3 componentVector)
        {
            _parent = parent;
            _parent.OnIsFunctionalChanged += ParentDisabled;
            _thrustDirection = componentVector.ClosestSide().Flip();
            //Debug.LogError("subthruster" + _thrustDirection);
            _localpower = componentVector.magnitude;

            Vector3 relToCenter = base.transform.localPosition;
            relToCenter.x = ((Mathf.Abs(relToCenter.x) < 0.1f) ? 0f : relToCenter.x);
            relToCenter.y = ((Mathf.Abs(relToCenter.y) < 0.1f) ? 0f : relToCenter.y);
            relToCenter.z = ((Mathf.Abs(relToCenter.z) < 0.1f) ? 0f : relToCenter.z);



            if (!_parent.ContributeAngular)
            {
                //Debug.LogError("thruster misconfigured");
                _attitudeInfluence = AttitudeControl.None;
                _attitudeInverse = AttitudeControl.None;
                return;
            }

            if (_thrustDirection == Direction.Backward)
            {
                if (relToCenter.x > 0f)
                {
                    _attitudeInfluence |= AttitudeControl.YawRight;
                }
                else if (relToCenter.x < 0f)
                {
                    _attitudeInfluence |= AttitudeControl.YawLeft;
                }
                if (relToCenter.y > 0f)
                {
                    _attitudeInfluence |= AttitudeControl.PitchUp;
                }
                else if (relToCenter.y < 0f)
                {
                    _attitudeInfluence |= AttitudeControl.PitchDown;
                }
            }
            else if (_thrustDirection == Direction.Downward)
            {
                if (relToCenter.x > 0f)
                {
                    _attitudeInfluence |= AttitudeControl.RollRight;
                }
                else if (relToCenter.x < 0f)
                {
                    _attitudeInfluence |= AttitudeControl.RollLeft;
                }
                if (relToCenter.z > 0f)
                {
                    _attitudeInfluence |= AttitudeControl.PitchDown;
                }
                else if (relToCenter.z < 0f)
                {
                    _attitudeInfluence |= AttitudeControl.PitchUp;
                }
            }
            else if (_thrustDirection == Direction.Forward)
            {
                if (relToCenter.x > 0f)
                {
                    _attitudeInfluence |= AttitudeControl.YawLeft;
                }
                else if (relToCenter.x < 0f)
                {
                    _attitudeInfluence |= AttitudeControl.YawRight;
                }
                if (relToCenter.y > 0f)
                {
                    _attitudeInfluence |= AttitudeControl.PitchDown;
                }
                else if (relToCenter.y < 0f)
                {
                    _attitudeInfluence |= AttitudeControl.PitchUp;
                }
            }
            else if (_thrustDirection == Direction.Leftward)
            {
                if (relToCenter.z > 0f)
                {
                    _attitudeInfluence |= AttitudeControl.YawLeft;
                }
                else if (relToCenter.z < 0f)
                {
                    _attitudeInfluence |= AttitudeControl.YawRight;
                }
                if (relToCenter.y > 0f)
                {
                    _attitudeInfluence |= AttitudeControl.RollLeft;
                }
                else if (relToCenter.y < 0f)
                {
                    _attitudeInfluence |= AttitudeControl.RollRight;
                }
            }
            else if (_thrustDirection == Direction.Rightward)
            {
                if (relToCenter.z > 0f)
                {
                    _attitudeInfluence |= AttitudeControl.YawRight;
                }
                else if (relToCenter.z < 0f)
                {
                    _attitudeInfluence |= AttitudeControl.YawLeft;
                }
                if (relToCenter.y > 0f)
                {
                    _attitudeInfluence |= AttitudeControl.RollRight;
                }
                else if (relToCenter.y < 0f)
                {
                    _attitudeInfluence |= AttitudeControl.RollLeft;
                }
            }
            else if (_thrustDirection == Direction.Upward)
            {
                if (relToCenter.x > 0f)
                {
                    _attitudeInfluence |= AttitudeControl.RollLeft;
                }
                else if (relToCenter.x < 0f)
                {
                    _attitudeInfluence |= AttitudeControl.RollRight;
                }
                if (relToCenter.z > 0f)
                {
                    _attitudeInfluence |= AttitudeControl.PitchUp;
                }
                else if (relToCenter.z < 0f)
                {
                    _attitudeInfluence |= AttitudeControl.PitchDown;
                }
            }
            _attitudeInverse = _attitudeInfluence.Invert();
        }
    }
}
