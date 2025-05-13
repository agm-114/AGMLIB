namespace Lib.Generic_Gameplay.Muzzles
{
    public class DelayedPulseRaycastMuzzle : RaycastMuzzle, IDirectDamageMuzzle

    {
        [SerializeField]
        protected GameObject _beamEffect;

        [SerializeField]
        protected GameObject _hitEffect;

        [SerializeField]
        private bool _cylinderRaycast = false;


        private bool _isFiring = false;

        float IDirectDamageMuzzle.ArmorPenetration => _armorPenetration;

        float IDirectDamageMuzzle.ComponentDamage => _componentDamage;

        string IDirectDamageMuzzle.ArmorDamageUnit => "cm";

        string IDirectDamageMuzzle.ComponentDamageUnit => "hp";

        float IDirectDamageMuzzle.DamagePeriod => 1f;



        public override void Fire()
        {
            MunitionHitInfo munitionHitInfo = ((!_cylinderRaycast) ? DoRaycast(base.transform.position, MathHelpers.RandomRayInCone(base.transform.forward, base._accuracy), _raycastRange) : DoRaycast(base.transform.position + MathHelpers.RandomRayInConeToCylinder(base.transform.forward, base._accuracy / 2f, _raycastRange), base.transform.forward, _raycastRange));
            base._reportTo?.ReportFired(1);

            if (munitionHitInfo != null)
            {

                if (_cachedDamageable != null)
                {
                    float damageDone;
                    bool destroyed;
                    HitResult result = _cachedDamageable.DoDamage(munitionHitInfo, this, out damageDone, out destroyed);
                    base._weapon.Platform.ReportDamageDealt(damageDone);
                    base._reportTo?.ReportHit(result, damageDone, destroyed);
                    base._weapon.TriggerHitEffect(base._muzzleIndex, result, munitionHitInfo.Point, Quaternion.LookRotation(munitionHitInfo.HitNormal));

                }

                munitionHitInfo.Dispose();
            }
        }



        public override void FireEffect()
        {
            base.FireEffect();
            ObjectPooler.Instance.GetNextOrNew(_beamEffect, base.transform.position, base.transform.rotation);
        }

        public override void StopFireEffect()
        {
            base.StopFireEffect();
        }

        public override void TriggerHitEffect(HitResult hit, Vector3 position, Quaternion rotation)
        {
            base.TriggerHitEffect(hit, position, rotation);
            ObjectPooler.Instance.GetNextOrNew(_hitEffect, position, rotation);
        }

        void IDirectDamageMuzzle.GetExtraStatDetails(List<(string, string)> rows)
        {
        }
    }
}
