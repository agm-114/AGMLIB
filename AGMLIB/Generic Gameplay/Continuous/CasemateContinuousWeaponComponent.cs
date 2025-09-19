namespace AGMLIB.Generic_Gameplay.Continuous
{

    public class CasemateContinuousWeaponComponent : FixedContinuousWeaponComponent, IFixedWeapon
    {
        [Serializable]
        public class CasemateContinuousWeaponState : ContinuousWeaponState
        {
            public CasemateController.CasemateControllerState CasemateState;
        }

        [Tooltip("Ships are hard to turn to exact targets.  If true, the shot fired will take its base vector from direct line to the target, and then inaccuracy is added on top.  If false, the shot will use the forward vector of the muzzle.")]
        [SerializeField] protected bool _fudgeShotVector = true;

        [SerializeField]
        private CasemateController _turretController;

        [SerializeField]
        private float _traverseRate;

        [SerializeField]
        private float _elevationRate;

        [ShipStat("casemate-traverse", "Casemate Traverse Rate", "deg/s", InitializeFrom = "_traverseRate")]
        protected StatValue _statTraverseRate;

        [ShipStat("casemate-elevate", "Casemate Elevation Rate", "deg/s", InitializeFrom = "_elevationRate")]
        protected StatValue _statElevationRate;

        //private Direction _facingDirection => Common.GetVal<Direction>(this, "_facingDirection");

        public override WeaponType WepType => WeaponType.Fixed;

        public override UnmaskVector UnmaskAxis
        {
            get
            {
                switch (FacingDirection)
                {
                    case Direction.Leftward:
                    case Direction.Rightward:
                        return UnmaskVector.Right;
                    case Direction.Upward:
                    case Direction.Downward:
                        return UnmaskVector.Up;
                    default:
                        return UnmaskVector.None;
                }
            }
        }

        public override UnmaskSide UnmaskSide => FacingDirection switch
        {
            Direction.Rightward => UnmaskSide.Positive,
            Direction.Leftward => UnmaskSide.Negative,
            Direction.Upward => UnmaskSide.Positive,
            Direction.Downward => UnmaskSide.Negative,
            _ => UnmaskSide.Either,
        };

        protected override Vector3 _aimPosition => _aimCheckFrom.position;
        private Direction _facingDirection = Direction.Forward;
        protected override Vector3 _aimLook => _aimCheckFrom.forward;

        public Direction FacingDirection => _facingDirection;

        //grab from the parent
        bool IFixedWeapon.NeedsTightPID => _turretController == null;
        
        protected override void SocketSet()
        {
            base.SocketSet();
            Vector3 facingDirection = base.Socket.MyHull.MyShip.transform.InverseTransformDirection(base.transform.up).normalized.RemoveTransients();
            _facingDirection = facingDirection.ClosestSide();
            if (_turretController != null)
            {
                _turretController.OnHitLimits += delegate
                {
                    FireActivityChangedEvent();
                };
            }
        }

        public override void GetFormattedStats(List<(string, string)> rows, bool full, int groupSize = 1)
        {
            base.GetFormattedStats(rows, full, groupSize);
            if (_turretController != null)
            {
                rows.Add(_statTraverseRate.FullTextWithLinkRow);
                rows.Add(_statElevationRate.FullTextWithLinkRow);
                _turretController.GetFormattedStats(rows, full);
            }
        }

        protected override void BearToTarget(Vector3 aimPoint)
        {
            if (_turretController != null)
            {
                _turretController.FaceTarget(aimPoint, _statTraverseRate.Value, _statElevationRate.Value);
            }

            base.BearToTarget(aimPoint);
        }

        protected override void OnTrackingChanged()
        {
            base.OnTrackingChanged();
            if (_turretController != null)
            {
                _turretController.StopMovement();
            }
        }

        protected override bool IsAimingBlocked()
        {
            if (_turretController != null)
            {
                return _turretController.HitLimits;
            }
            return base.IsAimingBlocked();
        }



        protected override PersistentComponentState NewSaveStateInstance()
        {
            return new CasemateContinuousWeaponState();
        }

        protected override void FillSaveState(PersistentComponentState state)
        {
            base.FillSaveState(state);
            if (state is CasemateContinuousWeaponState wepState && _turretController != null)
            {
                wepState.CasemateState = _turretController.GetSaveState();
            }
        }
        
        public override void RestoreSavedState(PersistentComponentState state)
        {
            base.RestoreSavedState(state);
            if (state is CasemateContinuousWeaponState wepState && _turretController != null)
            {
                _turretController.RestoreFromSave(wepState.CasemateState);
            }
        }
    }
}
