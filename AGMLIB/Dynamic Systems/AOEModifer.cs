using Mirror;
using UnityEngine.UI.Extensions;
using static Utility.GameColors;
using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Runtime.Seekers;
using System.Runtime.InteropServices;
using Random = UnityEngine.Random;
using Munitions.ModularMissiles.Descriptors.Seekers;

public class AOEModifer : ActiveSettings, IJammingSource
{
    
    protected HashSet<Ship> _detectedships = new();
    protected HashSet<ModularMissile> _detectedmissiles = new();

    protected List<FreeModifierSource> _modifierssources = new();
    protected bool? _laststate = null;
    protected Guid Guid = Guid.NewGuid();

    public bool UseTrigger = true;
    public Collider Trigger;
    public float Radius = 100f;
    public bool AffectFriendlies = true;
    public bool AffectEnemies = true;
    public bool AffectSelf = true;
    public bool CustomVFX = true;
    public ColorName Color = ColorName.Orange;
    [SerializeField]
    protected List<StatModifier>  _modifiers = new(1);
    public bool UseFallOff = false;
    public bool ShowLOB = true;
    public List<SignatureType> SoftKillWavelengths = new(3) { SignatureType.PassiveRadar | SignatureType.Radar | SignatureType.NoSignature } ;
    public float SoftKillRecycle = 1;
    public float FailChance = 1;
    public bool FailValidators = true;
    public bool JamSeekers = true;
    public bool NoJammingFalloff = true;
    public bool SoftKillAntiJamVals = true;
    public bool JamAntiJam = true;
    public float EffectAreaRatio = 0.4f;
    public float RadiatedPower = 2000f;
    public float Gain = 2f;
    public AnimationCurve ModiferFalloff = new(new Keyframe(0f, 1f), new Keyframe(1f, 1f));
    public List<string> Whitelist = new();
    public List<string> Blacklist = new();
    public bool Default = true;
    private float _updateInterval = 0.5f;
    private bool _updateEveryFrame = true;
    private Dictionary<IJammable, JammedVolume> _volumes = new Dictionary<IJammable, JammedVolume>();
    private NetworkPoolable _followingInstance;
    private const string matproperty = "Color_67F96457";
    private Color? _oldcolor;
    private float _updateAccum = 0f;
    public NetworkIdentity NetID => Ship.netIdentity;
    public bool ShowJammingLOB => ShowLOB;
    public Vector3 Position => transform.position;
    public Vector3 PlatformPosition => Ship.transform.position;
    public ISensorTrackable Platform => ShipController.Trackable;
    public Material Material => _followingInstance.GetComponentInChildren<MeshRenderer>().material;

    private float _softkillAccum = 0;

    public void Fire()
    {
        StopFire();//_EmissionColor
        //Debug.LogError("Creating Sphere");
        string PrefabName = "Stock/E70 'Interruption' Jammer";
        HullComponent goodewar = BundleManager.Instance.AllComponents.FirstOrDefault(x => x.SaveKey == PrefabName);
        if (goodewar == null)
            Debug.LogError("Did not get following bundle");
        //Debug.LogError("Found Target " + goodewar.SaveKey);
        RezFollowingMuzzle goodmuzzel = goodewar.gameObject.GetComponentInChildren<RezFollowingMuzzle>();
        if (goodmuzzel == null)
            Debug.LogError("Did not get following muzzel");
        GameObject prefab = Common.GetVal<GameObject>(goodmuzzel, "_followingPrefab");
        if(prefab == null)
            Debug.LogError("Did not get following prefab");
        else
            _followingInstance = NetworkObjectPooler.Instance?.GetNextOrNew(prefab, transform.position, transform.rotation );
        if (_followingInstance is ISettableEWarParameters settableEWarParameters)
        {
            settableEWarParameters.SetParams(SignatureType.Radar, omni: true, 360f, Radius, 1, 0, 0, 0f, true);
            _followingInstance.GetComponent<IImbued>()?.Imbue(ShipController.NetID);
            _oldcolor = Material.GetColor(matproperty);
            Material.SetColor(matproperty, GetColor(Color));
            _updateAccum = 0f;
        }

    }

    public void StopFire()
    {
        if (_followingInstance != null)
        {
            if(_oldcolor != null)
                Material.SetColor(matproperty, _oldcolor.Value);
            _followingInstance.RepoolSelf();
            _followingInstance = null;
            _oldcolor = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        if(Trigger == null)
        {
            SphereCollider sphereCollider = gameObject.GetOrAddComponent<SphereCollider>();
            Trigger = sphereCollider;
            sphereCollider.radius = Radius;
        }
        else if(Trigger is SphereCollider sphere)
        {
            Radius = sphere.radius;
        }
        //Debug.LogError("Current layer:" + gameObject.layer);
        //gameObject.layer = 19;
        if(UseFallOff)
        {
            ModiferFalloff.preWrapMode = WrapMode.Clamp;
            ModiferFalloff.postWrapMode = WrapMode.Clamp;
        }

        Trigger.isTrigger = true;
        //Debug.LogError("INIT AOE");

        foreach(StatModifier modifier in _modifiers)
            _modifierssources.Add(new FreeModifierSource(Guid.ToString() + modifier.StatName, modifier));

    }
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    private struct ColliderComparer : IEqualityComparer<Collider>
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
    private static ColliderComparer _colliderComparer = new();

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        _softkillAccum += Time.fixedDeltaTime;
        if (_softkillAccum > SoftKillRecycle)
        {
            IEnumerable<Collider> array = Physics.OverlapSphere(transform.position, Radius, 524801)?.Distinct(_colliderComparer) ?? new List<Collider>();
            foreach (Collider collider in array)
                OnTrigger(collider.transform, true);
            _softkillAccum = 0;
        }


        if (_laststate == null || _laststate != active || UseFallOff)
        {
            foreach (Ship target in _detectedships)
            {
                //if (active)
                //    Debug.LogError(target.ShipName + $" Is being modified");
                //else
                //    Debug.LogError(target.ShipName + $" Is not being modified");
                foreach (FreeModifierSource freeModifierSource in _modifierssources)
                {
                    StatModifier statModifier = freeModifierSource.Modifier;
                    if (UseFallOff)
                    {
                        float falloff = ModiferFalloff.Evaluate(Vector3.Distance(ShipController.transform.position, target.transform.position) / Radius);
                        statModifier = new(statModifier.StatName, statModifier.Literal * falloff, statModifier.Modifier * falloff);
                    }
                    if (active)
                        target.AddStatModifier(freeModifierSource, statModifier);
                    else
                        target.RemoveStatModifier(freeModifierSource, statModifier.StatName);
                }
            }
            foreach(ModularMissile missile in _detectedmissiles)
            {
                foreach(RuntimeMissileSeeker seeker in missile.GetComponentsInChildren<RuntimeMissileSeeker>())
                {
                    bool antijam = seeker.Descriptor is PassiveSeekerDescriptor passive && passive.CanPursueJamming;
                    if (!SoftKillWavelengths.Contains(seeker.DecoySigType))
                        return;
                    //Debug.LogError("Softkilling " + missile.gameObject.name);
                    if(SoftKillAntiJamVals && FailChance >= Random.Range(0.0f, 1f))
                    {
                        if (antijam && !SoftKillAntiJamVals)
                            continue;
                        Common.SetVal(seeker, "_validationReliable", false);
                    }


                    if (JamSeekers)
                    {
                        if (antijam && !JamAntiJam)
                            continue;
                        ReceivedJamming jammming = Common.GetVal<ReceivedJamming>(seeker, "_jammingSources");
                        if(active)
                            jammming.AddSource(this);
                        else
                            jammming.RemoveSource(this);
                    }


                }
            }
        }
                
        _laststate = active;

        _updateAccum += Time.fixedDeltaTime;
        if(CustomVFX && InGame)
        {

            if (active && _followingInstance == null)
                Fire();
            else if (!active)
                StopFire();
            if ((_updateEveryFrame || _updateAccum >= _updateInterval) && _followingInstance != null)
            {
                _updateAccum = 0f;
                _followingInstance.transform.position = base.transform.position;
                _followingInstance.transform.rotation = base.transform.rotation;
            }
        }
    }

    bool ValidListTarget(Ship target)
    {
        if (target == null || Blacklist.Contains(target.Hull.SaveKey))
            return false;

        if (Whitelist.Contains(target.Hull.SaveKey))
            return true;
        return Default;
    }
    bool ValidIFFTarget(Ship target)
    {
        if (target == Ship)
            return AffectSelf;
        return ValidIFFTarget(target?.Controller?.OwnedBy);

    }
    bool ValidIFFTarget(ModularMissile target) => ValidIFFTarget(target?.OwnedBy);
    bool ValidIFFTarget(IPlayer owner)
    {
        Game.IFF IFFstatus = ShipController?.GetIFF(owner) ?? Game.IFF.None;
        if (!AffectFriendlies && IFFstatus != Game.IFF.Enemy)
            return false;
        else if (!AffectEnemies && IFFstatus == Game.IFF.Enemy)
            return false;
        return true;
    }
    bool ValidTarget(Ship target)
    {
        if (target == null)
            return false;
        return  ValidListTarget(target) && ValidIFFTarget(target);
    }

    void UpdateDetectedList(Ship target, bool applicationarg = true)
    {  
        //if(_detectedships.Contains(target) )
        if (ValidTarget(target) && applicationarg == true)
        {
            //if(!_detectedships.Contains(target))
            //    Debug.LogError(target.ShipName + $" is a valid target");
            _detectedships.Add(target);
            _laststate = null;
        }
        else if(target != null)
        {
            //Debug.LogError(target?.ShipName + $" is not a valid target");
            if (_detectedships.Contains(target))
                foreach (FreeModifierSource freeModifierSource in _modifierssources)
                        target.RemoveStatModifier(freeModifierSource, freeModifierSource.Modifier.StatName);
            _detectedships.Remove(target);
        }

    }
    void UpdateDetectedList(ModularMissile target, bool applicationarg = true)
    {
        if(target ==  null) return;
        //if(_detectedships.Contains(target) )
        if (applicationarg == true)
        {
            if (!_detectedmissiles.Contains(target))
                Debug.LogError(target.name + $" is a valid missile target");
            _detectedmissiles.Add(target);
            _laststate = null;
        }
        else if(target != null)
        {

            Debug.LogError(target.name + $" is not a valid target");
            _detectedmissiles.Remove(target);
        }

    }

    public void OnTriggerEnter(Collider other) => OnTrigger(other.transform, true);
    public void OnTriggerExit(Collider other) => OnTrigger(other.transform, false);
    void OnTrigger(Transform target, bool applicationarg = true)
    {
        //Debug.LogError(target.name);
        target = target?.root;
        if (target == null)
            return;
        //Debug.LogError(target.name);
        //foreach (MonoBehaviour behaviour in target.GetComponents<MonoBehaviour>())
        //    Debug.LogError(behaviour.GetType().Name);
        if (UseTrigger)
            UpdateDetectedList(target?.GetComponent<Ship>(), applicationarg);
        UpdateDetectedList(target?.GetComponent<ModularMissile>(), applicationarg);
    }

    public static void HandleJammer(ActiveEWarEffect eWarEffect, IEWarTarget target, bool applicationarg)
    {
        FratricidalWeapon weapon = eWarEffect.GetComponent<FratricidalWeapon>();
        WeaponComponent weaponcom = weapon?.Source;
        AOEModifer AOEModifer = weaponcom?.GetComponentInChildren<AOEModifer>();
        if (target is not MonoBehaviour targetmono)
            return;
        AOEModifer?.OnTrigger(targetmono?.transform, applicationarg);
    }

    public JammedVolume GetJammedVolume(IJammable target)
    {
        JammedVolume volume = null;
        if (!_volumes.TryGetValue(target, out volume))
        {
            volume = new JammedVolume(this, target, EffectAreaRatio);
            _volumes.Add(target, volume);
        }
        return volume;
    }
    public float JammingPowerAtPoint(Vector3 point) => GetPowerAtPoint(point);

    protected float GetPowerAtPoint(Vector3 point)
    {
        if (NoJammingFalloff)
            return RadiatedPower * Gain;
        float R = Vector3.Distance(base.transform.position, point) * 10f;
        float sG = SensorMath.PowerDensityAtDistance(RadiatedPower, R, Gain);
        return sG;
    }
}

[HarmonyPatch(typeof(ActiveJammingEffect), "TargetGained")]
class ActiveEWarEffectTargetGained
{
    static void Postfix(ActiveEWarEffect __instance, IEWarTarget newTarget) => AOEModifer.HandleJammer(__instance, newTarget, true);
}

[HarmonyPatch(typeof(ActiveJammingEffect), "TargetLost")]
class ActiveEWarEffectTargetLost
{
    static void Postfix(ActiveEWarEffect __instance, IEWarTarget lostTarget) => AOEModifer.HandleJammer(__instance, lostTarget, false);
}