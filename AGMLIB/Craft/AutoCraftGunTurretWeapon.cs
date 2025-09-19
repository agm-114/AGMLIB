    using SmallCraft;
using SmallCraft.Components;
using SmallCraft.Components.Runtime;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Craft
{
    public class AutoCraftGunTurretWeapon : CraftGunTurretWeapon
    {
        public TurretController TurretControl => Common.GetVal<TurretController>(this, "_turretControl");



        
        public override void FinalSetup(Spacecraft craft)
        {
            RuntimeCraftAmmoWeapon newruntime;
            if (Mode == FiringMode.Continuous)
            {
                Debug.LogError("Continous Weapon");

                newruntime = craft.AddRuntimeBehaviour<AutoRuntimeCraftContinuousWeapon>(this);
            }
            else
            {
                Debug.LogError("Discrete Weapon");
                newruntime = craft.AddRuntimeBehaviour<AutoRuntimeCraftDiscreteWeapon>(this);
            }
            Common.SetVal(this, "_runtime", newruntime);
        }

    }
    public class AutoRuntimeCraftContinuousWeapon : RuntimeCraftContinuousWeapon
    {
        public TurretController TurretControl => Common.GetVal<TurretController>(this, "_turretControl");

        public ITrack Track => Common.GetVal<ITrack>(this, "_target");

        AutoCraftGunTurretWeapon TurretWeapon => _weapon as AutoCraftGunTurretWeapon;

        ITrack _secondaryTargetTrack = null;

        protected bool ValidTrack(ITrack track)
        {

            if (track == null || !track.IsValid)
            {
                Common.Hint(this, $"Track is invalid or null");
                return false;
            }
            Common.Trace(this, $"Checking Track {track.ContactName}");

            Vector3 _targetPIP = track.TruePosition;
            float trueMaxRange = CurrentAmmoType?.MaxRange ?? 100f;
            float trueMinRange = trueMaxRange * 0.05f;
            float aimCheckMaxRange = _weapon.RestrictTrueMaxRange ? MaximumRange : trueMaxRange;
            float distance = Vector3.Distance(_targetPIP, base.OnCraft.transform.position);
            if (distance >= aimCheckMaxRange || distance < trueMinRange)
            {
                Common.Hint(this, $"Track is invalid or null");
                return false;
            }
            if (!_weapon.OnTargetCheckAngle && Vector3.Dot(_weapon.AimCheckFrom.forward, _weapon.AimCheckFrom.position.To(_targetPIP).normalized) < 0f)
            {
                Common.Hint(this, $"Target is behind the weapon");
                return false;
            }
            else if (Vector3.Angle(_weapon.AimCheckFrom.forward, base.OnCraft.transform.position.To(_targetPIP).normalized) > _weapon.OnTargetAngle)
            {
                Common.Hint(this, $"Target is outside the on-target angle");
                return false;
            }
            if (!TurretWeapon.TurretControl.TargetWithinLimits(track.TruePosition))
            {
                Common.Hint(this, $"Target is outside the turret limits");
                return false;

            }
            RaycastHit obstacleCheck;
            if (_weapon.CheckObstaclesInWay && Physics.Linecast(base.OnCraft.Position, _targetPIP, out obstacleCheck, 512, QueryTriggerInteraction.Ignore))
            {
                Common.Hint(this, $"Obstacle in way: {obstacleCheck.collider.name}");
                return false;

            }
            if (_weapon.CheckFriendlyShipsInWay && MunitionsHelpers.CheckFriendlyShipsInWay(base.OnCraft.OwnedBy, _weapon.AimCheckFrom.position, _targetPIP))
            {
                Common.Hint(this, $"Friendly ship in way");
                return false;
            }
            return true;
        }

        private NoAllocEnumerable<ITrack> GetAllTracksInArea(Vector3 center, float radius, ContactClassification type = ContactClassification.Unknown, bool anytype = true)
        {
            return OnCraft.Group.Context.GetTrackList().WhereNoAlloc((ITrack x) => x.Trackable.ContactType != ContactClassification.Lifeboat && x.Trackable.ContactType != 0 && !x.Trackable.IsDecoy  && (type == ContactClassification.Unknown || x.Trackable.ContactType == type || anytype) && Vector3.Distance(x.TruePosition, center) <= radius);
        }

        protected override void FixedUpdate()
        {
            Common.Hint(this, $"Fixed Updated");
            Debug.LogError("Fixed Updated");

            if (ValidTrack(Track))
            {
                

                base.FixedUpdate();
                return;
            }
            if (_secondaryTargetTrack == null || !ValidTrack(_secondaryTargetTrack))
            {
                _secondaryTargetTrack = null;
                IEnumerable<ITrack> tracks = OnCraft.Sensors.Context.GetTrackList(false);
                Common.Hint(this, $"Searching for tracks");

                tracks = tracks.Where(t => ValidTrack(t));
                if (tracks.Any())
                    _secondaryTargetTrack = tracks.First();
            }
            if (_secondaryTargetTrack == null)
            {
                base.FixedUpdate();
                return;
            }
            ITrack Stashtrack = Track;

            Common.SetVal(this, "_target", _secondaryTargetTrack);
            Common.Hint(this, $"Switching Target to {_secondaryTargetTrack.ContactName}");
            base.FixedUpdate();
            Common.SetVal(this, "_target", Stashtrack);
            //SetTargetTrack();
        }
    }

    public class AutoRuntimeCraftDiscreteWeapon : RuntimeCraftDiscreteWeapon
    {

    }
}
