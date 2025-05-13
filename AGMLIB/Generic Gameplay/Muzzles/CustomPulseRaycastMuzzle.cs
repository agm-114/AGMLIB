using Lib.Generic_Gameplay.Discrete;

namespace Lib.Generic_Gameplay.Muzzles
{
    public class CustomPulseRaycastMuzzle : SinglePulseRaycastMuzzle, IDirectDamageMuzzle
    {


        [SerializeField] protected GameObject _rayhitEffect;
        [SerializeField] protected bool _cylinderRaycast = false;
        public override void Fire()
        {
            base.Fire();
            base._reportTo?.ReportFired(1);
            MuzzleSpawnedEffects texteffect = gameObject.GetComponent<MuzzleSpawnedEffects>();

            Vector3 position;
            Vector3 direction;
            if (_cylinderRaycast)
            {
                position = base.transform.position + MathHelpers.RandomRayInConeToCylinder(base.transform.forward, base._accuracy / 2f, _raycastRange);
                direction = base.transform.forward;
            }

            else
            {
                position = base.transform.position;
                direction = MathHelpers.RandomRayInCone(base.transform.forward, base._accuracy);
            }
            MunitionHitInfo rayHit = DoRaycast(position, direction, _raycastRange); ;
            Ray ray = new Ray(position, direction);


            //MunitionHitInfo rayHit = DoRaycast(base.transform.position, MathHelpers.RandomRayInCone(base.transform.forward, base._accuracy), _raycastRange);
            if (rayHit != null)
            {
                if (_cachedDamageable != null)
                {
                    float damage;
                    bool destroyed;
                    HitResult hit = _cachedDamageable.DoDamage(rayHit, this, out damage, out destroyed);
                    base._weapon.Platform.ReportDamageDealt(damage);
                    base._reportTo?.ReportHit(hit, damage, destroyed);
                    base._weapon.TriggerHitEffect(base._muzzleIndex, hit, rayHit.Point, Quaternion.LookRotation(rayHit.HitNormal));

                    if ((_castType == RaycastType.Ray) ? Physics.Raycast(ray, out var hitInfo, _raycastRange, 524801, QueryTriggerInteraction.Ignore) : Physics.SphereCast(ray, _castWidth, out hitInfo, _raycastRange, 524801))
                    {
                        Vector3 point = ray.GetPoint(hitInfo.distance);
                        base._weapon.TriggerHitEffect(base._muzzleIndex, hit, point, Quaternion.LookRotation(rayHit.HitNormal));
                        texteffect.SpwawnCustomHit(point);

                    }
                }
                rayHit.Dispose();


            }
        }


    }
}
