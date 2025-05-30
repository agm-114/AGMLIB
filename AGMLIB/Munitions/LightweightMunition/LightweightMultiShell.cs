using Lib.Munitions.LightweightMunition;
using UnityEngine;

namespace Lib.Munitions.LightweightMunition
{
    public class Fragment : MonoBehaviour
    {
        public VisualEffect effect;
        public static Queue<Fragment> pool = new Queue<Fragment>();
        public static Vector3 offsetRange = Vector3.one * 10;
        public float life = 0;
        public Vector3 shotDirection = Vector3.forward; // The direction the shell was fired in
        private float spawnTime = 0;
        Vector3 horizontalComponent = Vector3.zero;
        float elapsedTime => Time.time - spawnTime;
        // Create the child object at the position of the parent + random offset


        public static Fragment SpawnFrag(Fragments parent, LightweightMultiShell simmedshell)
        {
            Fragment frag = null;
            GameObject gameObject;
            if (pool.Count != 0)
            {
                frag = pool.Dequeue();
                gameObject = frag.gameObject;
            }
            else
            {
                gameObject = new GameObject("New Frag");
                frag = gameObject.AddComponent<Fragment>();

            }

            frag.SetupFrag(parent, simmedshell);
            return frag;
        }

        public void SetupFrag(Fragments parent, LightweightMultiShell simmedshell)
        {
            //shotDirection = parent.body.velocity;
            shotDirection = MathHelpers.RandomRayInCone(shotDirection, simmedshell.Angle);
            //transform.rotation = Quaternion.LookRotation(shotDirection, transform.up);
            spawnTime = Time.time;
            Vector3 deviation = shotDirection - parent.transform.forward.normalized;
            horizontalComponent = deviation * simmedshell.FlightSpeed;
            //randomOffset = Vector3.zero;
            transform.SetParent(parent.transform);  // Set the parent of the child object
            //transform.localPosition = randomOffset;
            if (effect == null)
                effect = gameObject.AddComponent<VisualEffect>();
            gameObject.SetActive(true);

            simmedshell.ChildShellTemplate.TracerEffect.ApplyToEffect(effect);
            effect.Play();
            FixedUpdate();
        }
        public void FixedUpdate()
        {
            transform.localPosition = elapsedTime * horizontalComponent;
        }
        public void Update()
        {
            transform.localPosition = elapsedTime * horizontalComponent;
        }

        public void Sleep()
        {
            effect.Stop();
            gameObject.SetActive(false);
            transform.SetParent(null);
            transform.position = Vector3.zero;
            pool.Enqueue(this);
        }
    }

    public class Fragments : MonoBehaviour
    {
        GameObject _childprefab;
        public List<Fragment> fragments = new List<Fragment>();
        //public List<Vector3> offset = new List<Vector3>();
        public LightweightKineticMunitionContainer Container;
        public Rigidbody body;
        LightweightMultiShell parent;

        [SerializeField]
        private bool _includeMunitions = true;

        public void SpawnFragments(LightweightMultiShell simmedshell, LightweightKineticMunitionContainer container)
        {
            parent = simmedshell;
            Container = container;
            body = container.GetComponent<Rigidbody>();
            fragments.Capacity = parent.Count;

            for (int i = 0; i < parent.Count; i++)
            {
                Fragment fragment = Fragment.SpawnFrag(this, simmedshell);
                fragments.Add(fragment);
                //offset.Add(fragment.transform.localPosition);
            }

        }

        public void KillFragments()
        {
            //Common.Hint("clear frag");
            foreach (Fragment fragment in fragments)
                fragment.Sleep();
            fragments.Clear();
            //offset.Clear();
        }

        public bool DoLookAhead(out RaycastHit hit, out bool isTrigger)
        {
            float raydistance = body.velocity.magnitude * Time.fixedDeltaTime;
            isTrigger = false;
            hit = new RaycastHit();
            /*
            if(!Physics.BoxCast(
                center:transform.position, 
                halfExtents:Fragment.offsetRange, 
                direction:transform.forward,
                orientation: transform.rotation,
                maxDistance: raydistance, 
                layerMask:_includeMunitions ? 524801 : 513,
                queryTriggerInteraction: QueryTriggerInteraction.Ignore
                ))
                return false;
            */


            //if (parent is LightweightClusterShell or parent is LightweightProximityShell)
            List<Fragment> deadfrags = new List<Fragment>();

            foreach (Fragment fragment in fragments)
            {
                transform.rotation = Quaternion.LookRotation(body.velocity, transform.up);
                Ray r = new Ray(fragment.transform.position, transform.forward);
                LineRenderer? lineRenderer = null;

                if (parent.DebugMode)
                {

                    //Debug.LogError("Beam Spawn");
                    GameObject lineObj = new GameObject("BeamLine");
                    Destroy(lineObj, 2);
                    lineRenderer = lineObj.AddComponent<LineRenderer>();

                    // Configure LineRenderer settings
                    lineRenderer.startWidth = 0.05f;
                    lineRenderer.endWidth = 0.05f;
                    lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Use a simple shader
                    lineRenderer.startColor = Color.white;
                    lineRenderer.endColor = Color.white;

                    // Set the positions to visualize the ray
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, r.origin); 
                    lineRenderer?.SetPosition(1, r.GetPoint(raydistance));

                }

                if (parent.CustomLookaheadMethod)
                {


                    bool lookaheadCheck = parent.DoLookAhead(r.origin, Quaternion.LookRotation(r.direction, transform.up), body.velocity, out hit, out isTrigger);
                    if (lookaheadCheck)
                    {
                        if (lineRenderer != null)
                        {
                            lineRenderer.startColor = Color.red;
                            lineRenderer.endColor = Color.red;
                        }
                        deadfrags.Add(fragment);
                        continue;

                    }


                }
                if (parent.LookaheadSphereRadius > 0.01 && Physics.SphereCast(r, parent.LookaheadSphereRadius, out hit, raydistance, _includeMunitions ? 524801 : 513, QueryTriggerInteraction.Ignore))
                {
                    if (lineRenderer != null)
                    {
                        lineRenderer.startColor = Color.green;
                        lineRenderer.endColor = Color.green;
                    }
                    deadfrags.Add(fragment);
                    continue;
                }
                if (Physics.Raycast(ray: r, out hit, maxDistance: raydistance, layerMask: _includeMunitions ? 524801 : 513, QueryTriggerInteraction.Ignore))
                {
                    //Common.Hint("hit frag");
                    if (lineRenderer != null)
                    {
                        lineRenderer.startColor = Color.blue;
                        lineRenderer.endColor = Color.blue;
                    }
                    deadfrags.Add(fragment);

                }




            }
            foreach (Fragment fragment in deadfrags)
            {
                parent.ChildShellTemplate.InstantiateSelf(fragment.transform.position, fragment.transform.rotation, body.velocity);
                fragments.Remove(fragment);
                fragment.Sleep();
            }

            //
            //return 

            return false;
        }

    }
    [CreateAssetMenu(fileName = "New LW Multi Shell", menuName = "Nebulous/LW Shells/Multi Shell")]
    public class LightweightMultiShell : LightweightKineticShell
    {
        [SerializeField] protected LightweightKineticShell _childShellTemplate;
        public LightweightKineticShell ChildShellTemplate => _childShellTemplate;
        public override bool CustomLookaheadMethod => true;

        public bool DebugMode = false;
        public float Angle = 30;
        public int Count = 30;
        public float LookaheadSphereRadius = 4;


        public override NetworkPoolable InstantiateSelf(Vector3 startPosition, Quaternion startRotation, Vector3 startVelocity)
        {
            //Common.Hint("test frag");


            Common.SetVal(this, "_tracerEffect", _childShellTemplate.TracerEffect);
            Common.SetVal(this, "_whine", _childShellTemplate.Whine);
            LightweightKineticMunitionContainer container = base.InstantiateSelf(startPosition, startRotation, startVelocity) as LightweightKineticMunitionContainer;
            if (container == null)
                return null;

            Fragments frags = container.GetComponent<Fragments>() ?? container.gameObject.AddComponent<Fragments>();
            frags.SpawnFragments(this, container);


            return container;
        }

        public override bool DoLookAhead(Vector3 position, Quaternion rotation, Vector3 velocity, out RaycastHit hit, out bool isTrigger)
        {
            isTrigger = false;



            //Ray r = new Ray(position + velocity.normalized * _lookaheadSphereRadius, velocity.normalized);
            //if (Physics.SphereCast(r, _lookaheadSphereRadius, out hit, velocity.magnitude * Time.fixedDeltaTime, 513, QueryTriggerInteraction.Ignore))
            //{

            //}




            Physics.SphereCast(new Ray(Vector3.zero, Vector3.zero), 0, out hit, 0, 513, QueryTriggerInteraction.Ignore);

            return Common.SkipFunction;
        }


    }

}


[HarmonyPatch(typeof(LightweightKineticMunitionContainer), "OnRepooled")]
class LightweightKineticMunitionContainerOnRepooled
{
    static void Postfix(LightweightKineticMunitionContainer __instance)
    {
        Common.LogPatch();

        __instance.GetComponent<Fragments>()?.KillFragments();
    }
}

[HarmonyPatch(typeof(LightweightKineticMunitionContainer), "DoLookAhead")]
class LightweightKineticMunitionContainerDoLookAhead
{
    static bool Prefix(out RaycastHit hit, out bool isTrigger, LightweightKineticMunitionContainer __instance, ref bool __result) //
    {
        Common.LogPatch();
        __result = false;
        hit = default;
        isTrigger = false;
        return __instance.GetComponent<Fragments>()?.DoLookAhead(out hit, out isTrigger) ?? Common.RunFunction;
        return false;
    }
}