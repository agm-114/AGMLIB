using Game;
using Munitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AGMLIB.Munitions.LightweightMunition
{
    public class Burst : ScriptableObject
    {
        public int Count = 10;
    }

    public class LightweightKineticBurstContainer : LightweightKineticMunitionContainer
    {
        private LightweightKineticShell _appliedTemplate;
        private int _successiveHits = 0;

        protected override bool DamageableImpact(IDamageable hitObject, MunitionHitInfo hitInfo, bool trigger)
        {
            if (_appliedTemplate != null)
            {
                HitResult hitRes;
                float damageDone;
                bool targetDestroyed;
                Vector3 repoolPosition;
                bool flag = _appliedTemplate.DamageableImpact(this, hitObject, hitInfo, trigger, out hitRes, out damageDone, out targetDestroyed, out repoolPosition);
                ReportDamageDone(hitRes, damageDone, targetDestroyed);
                if (flag)
                {
                    RepoolSelfAfterDelay(_appliedTemplate.RepoolDelay, disableImmediately: true, repoolPosition);
                }
                else
                {
                    _body.velocity = _lastVelocity;
                }
                return flag;
            }
            RepoolSelf(hitInfo.Point);
            return true;
        }

        internal virtual void ReportDamageDone(HitResult hit, float damage, bool destroyed)
        {
            if (_launchedFrom != null)
            {
                _launchedFrom.ReportDamageDealt(damage, _successiveHits);
            }
            if (_reportTo != null)
            {
                _reportTo.ReportHit(hit, damage, destroyed);
            }
        }

    }
}
