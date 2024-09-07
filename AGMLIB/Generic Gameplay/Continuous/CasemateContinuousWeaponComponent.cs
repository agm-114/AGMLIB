using Ships;
using Ships.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Ships.DiscreteWeaponComponent;
using UnityEngine;
using Utility;

namespace AGMLIB.Generic_Gameplay.Continuous
{

    public class CasemateContinuousWeaponComponent : FixedContinuousWeaponComponent
    {
        [Serializable]
        public class CasemateContinuousWeaponState : ContinuousWeaponState
        {
            public CasemateController.CasemateControllerState CasemateState;
        }

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

        protected override Vector3 _aimLook => _aimCheckFrom.forward;

        public Direction FacingDirection => (this as IFixedWeapon).FacingDirection;

        public bool NeedsTightPID => _turretController == null;

        protected override void SocketSet()
        {
            base.SocketSet();

            if (_turretController != null)
            {
                _turretController.OnHitLimits += delegate
                {
                    FireActivityChangedEvent();
                };
            }
        }

        public override string GetFormattedStats(bool full, int groupSize = 1)
        {
            if (_turretController != null)
            {
                string baseStats = base.GetFormattedStats(full, groupSize);
                return baseStats + _statTraverseRate.FullTextWithLink + "\n" + _statElevationRate.FullTextWithLink + "\n" + _turretController.GetFormattedStats(full);
            }
            return base.GetFormattedStats(full, groupSize);
        }

        protected override void BearToTarget(Vector3 aimPoint)
        {
            _turretController?.FaceTarget(aimPoint, _statTraverseRate.Value + 10, _statElevationRate.Value + 10);

            base.BearToTarget(aimPoint);
        }

        protected override void OnTrackingChanged()
        {
            base.OnTrackingChanged();
            _turretController?.StopMovement();

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
