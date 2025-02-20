using Ships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Generic_Gameplay.Discrete
{
    public interface IMuzzleEffect
    {
        public void FireEffect();
        public void SpawnHit(MunitionHitInfo rayHit);
    }



    public class MuzzleEffects : MonoBehaviour
    {
        public static void FireEffects(Muzzle muzzle)
        {
            //Common.Trace("Spawning Flashs");


            foreach (BaseMuzzleEffects effect in muzzle.gameObject.GetComponentsInChildren<BaseMuzzleEffects>())
            {
                //Common.Trace("Spawning Flash");
                effect.FireEffect();
            }
        }

        public static void SpawnImpacts(Muzzle muzzle, MunitionHitInfo hit)
        {

            if (hit == null)
                return;
            //Common.Trace("Spawning Impacts");

            foreach (IMuzzleEffect effect in muzzle.gameObject.GetComponentsInChildren<IMuzzleEffect>())
            {
                //Common.Trace("Spawning Impact");

                effect.SpawnHit(hit);
            }
        }
    }

    public abstract class BaseMuzzleEffects : MonoBehaviour, IMuzzleEffect
    {
        public abstract void FireEffect();
        public abstract void SpawnHit(MunitionHitInfo rayHit);
    }



    public class MuzzleSpawnedEffects : BaseMuzzleEffects
    {
        [SerializeField] protected Transform _muzzleEffectEffectlocation;
        [SerializeField] protected GameObject _muzzleEffect;
        [SerializeField] protected GameObject _hitEffect;
        public override void FireEffect()
        {
            //Common.Trace("Spawn Muzzle");
            ObjectPooler.Instance.GetNextOrNew(_muzzleEffect, _muzzleEffectEffectlocation ?? transform);
            //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //sphere.transform.position = _muzzleEffectEffectlocation?.position ?? transform.position;
        }
        public override void SpawnHit(MunitionHitInfo rayHit)  
        {

            return;
            Common.Trace("Custom Hit Effect");

            Poolable hitob = ObjectPooler.Instance.GetNextOrNew(_hitEffect, rayHit.HitObject.transform);
            hitob.transform.position = rayHit.Point;
            //hitob = ObjectPooler.Instance.GetNextOrNew(_hitEffect, rayHit.HitObject.transform);

            //hitob = ObjectPooler.Instance.GetNextOrNew(_hitEffect);



            //hitob = ObjectPooler.Instance.GetNextOrNew(_hitEffect);

           
            //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //sphere.transform.position = rayHit.HitObject.transform.position;
        }
        public void SpwawnCustomHit(Vector3 pos)
        {
            Common.Trace("Very Custom Hit Effect");
            Poolable hitob = ObjectPooler.Instance.GetNextOrNew(_muzzleEffect);
            hitob.transform.position = pos;
        }
    }
    public class MuzzleSoundEffects : BaseMuzzleEffects
    {
        [SerializeField] protected VariedSoundEffect _muzzleSound;
        [SerializeField] protected VariedSoundEffect _impactSound;
        public override void FireEffect() => GlobalSFX.PlayOneShotSpatial(_muzzleSound, transform);
        public override void SpawnHit(MunitionHitInfo rayHit) => GlobalSFX.PlayOneShotSpatial(_impactSound, rayHit.HitObject.transform);
    }

    /*
    public class MuzzleSoundSource : MuzzleSoundEffects
    {
        public override void FireEffect() => GlobalSFX.PlayOneShotSpatial(_muzzleSound, transform);
        public override void SpawnHit(MunitionHitInfo rayHit) => GlobalSFX.PlayOneShotSpatial(_muzzleSound, rayHit.HitObject.transform);
    }
    */

    public class LongPulseRaycastMuzzle : SinglePulseRaycastMuzzle
    {


    }
}
