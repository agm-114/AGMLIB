using Game.Sensors;
using Munitions;
using Ships;
using UnityEngine;

public class SignatureDecay : MonoBehaviour
{
    [Tooltip("The signature that will have its size multiplier increased or decreased.")]
    [SerializeField]
    private BaseSignature _signature;
    [Tooltip("The multiplier for the signature size divided by the lifetime of the missile.")]
    [SerializeField]
    private AnimationCurve _signatureOverLifetime = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0f));
    [Tooltip("The parent active decoy the signature is tied to. eg EA99 Active Decoy")]
    [SerializeField]
    private LookaheadMunition _lookaheadMunition;
    [Tooltip("The duration the signature decreases for, in seconds.")]
    [SerializeField]
    private int _duration = 1;
    [Tooltip("If enabled, it uses the duration you set instead of the munitions lifetime, in seconds.")]
    [SerializeField]
    private bool _useDurationInsteadOfMunitionLifetime = false;
    private LiteralStatValue _multiplierStat = new LiteralStatValue(StatTable.GetStatInfo("hull-sigmult-radar", out var _), null);
    private float _accumulator = 0;
    private bool foundflighttime = false;
    private float flighttime = 1;
    // Update is called once per frame
    protected virtual void Update()
    {
        if (base.isActiveAndEnabled)
        {
            if (!_useDurationInsteadOfMunitionLifetime)
            {
                if (!foundflighttime)
                {
                    try
                    {
                        flighttime = (float)typeof(LookaheadMunition).GetField("_maxFlightTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(_lookaheadMunition);
                        foundflighttime = true;
                    }
                    catch
                    {
                        Debug.LogError("ok so i think the reflection didnt work. im sorry this will keep throwing until the decoy expires, ping @themetabread and tell me what happened on the main neb discord.");
                    }
                }
            }
            _accumulator += Time.deltaTime;
            //Debug.LogError(_accumulator + " accumulated s");
            if (_useDurationInsteadOfMunitionLifetime)
            {
                _multiplierStat.Modifier = _signatureOverLifetime.Evaluate(_accumulator / _duration);
            }
            else
            {
                _multiplierStat.Modifier = _signatureOverLifetime.Evaluate(_accumulator / flighttime);
            }
            //Debug.LogError(_multiplierStat.Modifier + " multiplier");
        }
    }
}
