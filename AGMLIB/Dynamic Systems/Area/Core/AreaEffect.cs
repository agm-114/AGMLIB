using UnityEngine.UI.Extensions;
using Game.EWar;
using Game.Sensors;
using Bundles;
using static Utility.GameColors;
using Munitions.ModularMissiles;
using System.Runtime.InteropServices;
using AGMLIB.Dynamic_Systems.Area;
using static Game.EWar.EWarPrefabCollection;


public class AreaEffect : ActiveSettings
{
    protected static List<Type> SupportedTypes = new() { typeof(ModularMissile), typeof(Ship) };
    protected HashSet<MonoBehaviour> _detectedmonobhaviors = new();




    //public bool UseTrigger = true;
    public Collider Trigger;
    public float Radius = 1000f;

    public bool CustomVFX = true;
    public ColorName Color = ColorName.Orange;
    public BaseFilter? Filter = null;
    public List<BasicEffect> Effects = new();
    protected bool? _laststate = null;
    protected Guid Guid = Guid.NewGuid();
    private float _updateInterval = 0.5f;
    private bool _updateEveryFrame = true;
    private NetworkPoolable? _followingInstance;
    private const string matproperty = "Color_67F96457";
    private Color? _oldcolor;
    private float _updateAccum = 0f;

    public Material? Material
    {
        get
        {
            //if (_followingInstance == null)
            //    Common.Hint("Following Instance Null");
            MeshRenderer? renderer = _followingInstance?.GetComponentInChildren<MeshRenderer>();
            //if (renderer == null)
            //    Common.Hint("Following mesh Null");
            //if(_followingInstance.GetComponentInChildren<MeshRenderer>(includeInactive:true).material == null)
            //    Common.Hint("Following mat Null");

            return _followingInstance?.GetComponentInChildren<MeshRenderer>(includeInactive: true)?.material;
        }
    }

    //public List<BasicEffect<Ship>> ShipEffects = new();
    //public List<BasicEffect<ModularMissile>> ModularMissileEffects = new();     

    

    public void Fire()
    {
        if (_followingInstance != null)
            return;
        //StopFire();//_EmissionColor
        //Debug.LogError("Creating Sphere");
        _followingInstance = NetworkObjectPooler.Instance.GetNextOrNew(SingletonMonobehaviour<EWarPrefabCollection>.Instance.GetPrefab(EwarType.CommsJamming).gameObject, base.transform.position, base.transform.rotation);
        if (_followingInstance is not ISettableEWarParameters settableEWarParameters)
            return;
        settableEWarParameters.SetParams(SignatureType.Radar, omni: true, 360f, Radius, 1, 0, 0, 0f, true);
        _followingInstance.GetComponent<IImbued>()?.Imbue(ShipController);
        _followingInstance.GetComponent<ActiveEWarEffect>().enabled = false;
        _oldcolor = Material?.GetColor(matproperty);
        Material?.SetColor(matproperty, GameColors.GetColor(Color));
        _updateAccum = 0f;
    }

    public void StopFire()
    {

        if (_followingInstance == null)
            return;
        //Debug.LogError("Stopping Sphere");
        _followingInstance.GetComponent<ActiveEWarEffect>().enabled = true;

        if (_oldcolor != null)
            Material.SetColor(matproperty, _oldcolor.Value);
        //DestroyImmediate(_followingInstance);
        _followingInstance.RepoolSelf();
        _followingInstance = null;
        _oldcolor = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (BasicEffect effect in Effects)
        {
            effect.AreaEffect = this;
            effect.Setup();
        }   
        if (Trigger == null)
        {
            SphereCollider sphereCollider = gameObject.GetOrAddComponent<SphereCollider>();
            Trigger = sphereCollider;
            sphereCollider.radius = Radius;
        }
        else if(Trigger is SphereCollider sphere)
        {
            Radius = sphere.radius;
        }

        Trigger.isTrigger = true;


  
    }

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //Debug.LogError($"{Buttonstate} current buttonstate | targetbuttonstate {activateButtonState}");
        if (InEditor)
            return;
        //Debug.LogError("Core Fixed Update");

        foreach(BasicEffect basicEffect in  Effects)
        {
            //if(!basicEffect.isActiveAndEnabled)
            //    Debug.LogError("Effect Type: " + basicEffect.GetType().Name + " Active: " + basicEffect.isActiveAndEnabled);
            //basicEffect.enabled = true;

            //basicEffect.FixedUpdate();
        }

        if (_laststate == null || _laststate != active)
        {
            foreach (BasicEffect basicEffect in Effects.Where(a => a != null))
            {
                basicEffect.enabled = true;
                basicEffect.AreaUpdate();
            }

        }

                
        _laststate = active;

        _updateAccum += Time.fixedDeltaTime;
        if(CustomVFX)
        {

            if (active)
                Fire();
            else
                StopFire();

            if (_followingInstance != null && (_updateEveryFrame || _updateAccum >= _updateInterval))
            {
                _updateAccum = 0f;
                _followingInstance.transform.position = base.transform.position;
                _followingInstance.transform.rotation = base.transform.rotation;
            }
        }
    }



    void UpdateDetectedList(Ship target, bool applicationarg = true)
    {
        if (target == null) return;
        //if(_detectedships.Contains(target) )

        if (applicationarg == true  && (Filter?.CheckShip(ShipController, target.GetComponent<ShipController>()) ?? true))
        {
            foreach(var effect in Effects)
                effect.Enter(target);
            _laststate = null;
        }
        else if(target != null)
        {
            foreach (var effect in Effects)
                effect.Exit(target);
        }

    }
    void UpdateDetectedList(ModularMissile target, bool applicationarg = true)
    {
        if(target ==  null) return;
        //if(_detectedships.Contains(target) )
        if (applicationarg == true && (Filter?.CheckMissile(ShipController, target) ?? true))
        {
            foreach (var effect in Effects)
                effect.Enter(target);
            _laststate = null;
        }
        else if(target != null)
        {
            foreach (var effect in Effects)
                effect.Exit(target);
        }

    }

    public void OnTriggerEnter(Collider other) => OnTrigger(other.transform, true);
    public void OnTriggerExit(Collider other) => OnTrigger(other.transform, false);
    public void OnTrigger(Transform? target, bool applicationarg = true)
    {
        //Debug.LogError(target.name);
        target = target?.root;
        if (target == null)
            return;
        //Debug.LogError(target.name);
        //foreach (MonoBehaviour behaviour in target.GetComponents<MonoBehaviour>())
        //    Debug.LogError(behaviour.GetType().Name);
        UpdateDetectedList(target.GetComponent<Ship>(), applicationarg);
        UpdateDetectedList(target.GetComponent<ModularMissile>(), applicationarg);
    }

    public static void HandleJammer(ActiveEWarEffect eWarEffect, IEWarTarget target, bool applicationarg)
    {
        FratricidalWeapon weapon = eWarEffect.GetComponent<FratricidalWeapon>();
        WeaponComponent? weaponcom = weapon?.Source;
        AreaEffect? areaeffect = weaponcom?.GetComponentInChildren<AreaEffect>();
        if (target is not MonoBehaviour targetmono)
            return;
        areaeffect?.OnTrigger(targetmono?.transform, applicationarg);
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    protected struct ColliderComparer : IEqualityComparer<Collider>
    {
        public bool Equals(Collider x, Collider y)
        {
            return x.transform.root.gameObject == y.transform.root.gameObject;
        }

        public int GetHashCode(Collider obj)
        {
            return ((object)obj).GetHashCode();
        }
    }
    protected static ColliderComparer _colliderComparer = new();


}

[HarmonyPatch(typeof(ActiveJammingEffect), "TargetGained")]
class ActiveEWarEffectTargetGained2
{
    static void Postfix(ActiveEWarEffect __instance, IEWarTarget newTarget) => AreaEffect.HandleJammer(__instance, newTarget, true);
}

[HarmonyPatch(typeof(ActiveJammingEffect), "TargetLost")]
class ActiveEWarEffectTargetLost2
{
    static void Postfix(ActiveEWarEffect __instance, IEWarTarget lostTarget) => AreaEffect.HandleJammer(__instance, lostTarget, false);
}