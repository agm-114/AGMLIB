// Nebulous, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Munitions.ModularMissiles.Descriptors.Support.JammerSupportDescriptor
using Game;
using Game.EWar;
using Game.Reports;
using Game.Sensors;
using Mirror;
using Munitions;
using UnityEngine;
using Utility;

public class LegacyJammerModule : NetworkBehaviour
{
    [Tooltip("The parent missile. I dont know why I put a tooltip for this. It seems pretty obvious.")]
    [SerializeField]
    private Missile _missile;

    [SerializeField]
    private EWarPrefabCollection.EwarType _effectType = EWarPrefabCollection.EwarType.SensorJamming;

    [SerializeField]
    private SignatureType _sigType = SignatureType.Radar;

    [SerializeField]
    private bool _omnidirectional = false;

    [SerializeField]
    private float _coneFov = 15f;

    [SerializeField]
    private float _maxRange = 500f;

    [SerializeField]
    private float _effectAreaRatio = 0.4f;

    [SerializeField]
    private float _radiatedPower = 2f;

    [SerializeField]
    private float _gain = 1f;

    [SerializeField]
    private float _maxAimingAngle = 30f;

    public bool DirectAtTarget => !_omnidirectional;

    public float MaxAimingAngle => _maxAimingAngle;

    private ActiveEWarEffect _effect;

    protected virtual void Update()
    {
        if (_missile.IsDead)
        {
            if (base.isServer && _effect != null)
            {
                _effect.RepoolSelf();
                _effect = null;
                //Debug.LogError("killed");
                //Debug.LogError(_effect);
            }
        }
        else
        {
            if (base.isServer && _effect == null)
            {
                _effect = SpawnJammingEffect(_missile);
                //Debug.LogError("spawn");
            }
        }
    }

    public ActiveEWarEffect SpawnJammingEffect(Missile missile)
    {
        ActiveEWarEffect effect = NetworkObjectPooler.Instance.GetNextOrNew<ActiveEWarEffect>(SingletonMonobehaviour<EWarPrefabCollection>.Instance.GetPrefab(_effectType).gameObject, base.gameObject.transform.position, base.gameObject.transform.rotation);
        effect.Imbue(missile);
        effect.SetParams(_sigType, _omnidirectional, _coneFov, _maxRange, _effectAreaRatio, _radiatedPower, _gain, 0f, showLOB: false);
        effect.transform.SetParent(base.transform);
        return effect;

    }
}
