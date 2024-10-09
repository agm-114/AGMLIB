using Munitions.ModularMissiles.Descriptors.Support;
using Munitions.ModularMissiles;
using Game.EWar;
using Munitions.ModularMissiles.Descriptors;
using Munitions.ModularMissiles.Runtime;
[CreateAssetMenu(fileName = "New Modular Jammer Support", menuName = "Nebulous/Missiles/Support/Modular Jammer")]
public class ModularJammerSupportDescriptor : JammerSupportDescriptor, IModular
{
    [Header("Modular Components")]
    [SerializeField]
    protected List<ScriptableObject> _modules = new();
    List<ScriptableObject> IModular.Modules => _modules;

    public string PrefabName = "Stock/E55 'Spotlight' Illuminator";
    public bool ConicalScan = false;
    public float ConicalScanInitialCenterTime = 1f;
    public float ConicalScanSpeed = 360f;
    public float MaxBeamSteer = 10f;
    public override void FinalSetup(ModularMissile missile)
    {
        if(ConicalScan)
            missile.AddRuntimeBehaviour<BeamRuntimeJammerSupport>(this);
        else
            base.FinalSetup(missile);
    }

}

public class BeamRuntimeJammerSupport : RuntimeMissileBehaviour
{
    private float _fullScanSeconds = 0; 
    private float _conicalScanStartDelay = 0f;
    private float _conicalScanProgress = 0f;
    private Vector3 _topVector = Vector3.up;
    protected Vector3 _localBeamDirection { get; private set; } = Vector3.forward;
    protected Vector3 _worldBeamDirection => base.transform.TransformDirection(_localBeamDirection);
    public class RuntimeJammerState : RuntimeMissileBehaviourState
    {
        public bool Active;
    }

    [HideInInspector]
    [SerializeField]
    private ModularJammerSupportDescriptor _supportDesc;

    private ActiveEWarEffect _effect = null;

    public override void OnAdded(ModularMissile missile, MissileComponentDescriptor descriptor)
    {
        base.OnAdded(missile, descriptor);
        _supportDesc = descriptor as ModularJammerSupportDescriptor;
    }

    public override void OnLaunched()
    {
        base.OnLaunched();
        if (base.isServer && _effect == null)
        {
            StartJamming();
        }
    }

    private void StartJamming()
    {
        _conicalScanStartDelay = _supportDesc.ConicalScanInitialCenterTime;
        _conicalScanProgress = 0f;
        _localBeamDirection = Vector3.forward;
        _fullScanSeconds = 360f / _supportDesc.ConicalScanSpeed;
        _topVector = Quaternion.Euler(_supportDesc.MaxBeamSteer, 0f, 0f) * Vector3.forward;
        _effect = _supportDesc.SpawnJammingEffect(base.Missile, base.transform.position, base.transform.rotation);
    }

    public override void OnDead()
    {
        base.OnDead();
        if (base.isServer && _effect != null)
        {
            _effect.RepoolSelf();
            _effect = null;
        }
    }

    private void FixedUpdate()
    {
        if (!base.isServer || _effect == null)
        {
            return;
        }

        _effect.transform.position = base.transform.position;
        if (_supportDesc.DirectAtTarget && base.Missile.CurrentTargetDirection is Vector3 currentTargetDirection)
        {
            _effect.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(currentTargetDirection.normalized), _supportDesc.MaxAimingAngle);
        }
        else
        {
            if(_supportDesc.ConicalScan)
                    DoConicalScan(Time.fixedDeltaTime);
            _effect.transform.rotation = Quaternion.LookRotation(_worldBeamDirection, Vector3.up);
        }
    }

    protected override RuntimeMissileBehaviourState NewSaveStateInstance()
    {
        return new RuntimeJammerState();
    }

    protected override void FillSaveState(RuntimeMissileBehaviourState state)
    {
        if (state is RuntimeJammerState runtimeJammerState)
        {
            runtimeJammerState.Active = _effect != null;
        }
    }

    protected override void RestoreSaveState(RuntimeMissileBehaviourState state)
    {
        if (state is RuntimeJammerState runtimeJammerState && runtimeJammerState.Active)
        {
            StartJamming();
        }
    }

    private void MirrorProcessed()
    {
    }


    protected void DoConicalScan(float time)
    {
        if (_conicalScanStartDelay <= 0f)
        {
            _conicalScanProgress += time / _fullScanSeconds;
            if (_conicalScanProgress > 1f)
            {
                _conicalScanProgress = 0f;
            }
            _localBeamDirection = (Quaternion.Euler(0f, 0f, _conicalScanProgress * 360f) * _topVector).normalized;
        }
        else
        {
            _conicalScanStartDelay -= time;
            _localBeamDirection = Vector3.forward;
        }
    }
}