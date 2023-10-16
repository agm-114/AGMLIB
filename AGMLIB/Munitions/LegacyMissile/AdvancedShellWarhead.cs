

// Nebulous, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Munitions.MissileImpactWarhead
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Game;
using Game.Reports;
using Game.Units;
using Munitions;
using Ships;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

public class AdvancedShellWarhead : MissileWarhead
{
    [SerializeField]
    private readonly RezzingMuzzle _barrel = null;

    [SerializeField]
    private readonly SimpleMagazine _magazine = null;

    [SerializeField]
    private readonly string _detailSubmunitionType = "MINE";

    [SerializeField]
    private readonly int _spawnCount = 0;

    [SerializeField]
    private readonly bool _omnidirectional = false;

    [SerializeField]
    private readonly float _launchAngle = 20f;

    [SerializeField]
    private readonly float _launchDelay = 0;

    [SerializeField]
    private readonly bool _selectRandomPointInTarget = false;

    [SerializeField]
    private readonly Collider _trigger = null;

    public override string DetailTextSegment => _detailSubmunitionType;

    public override float TotalComponentDamagePotential => 0f;


    public override HitResult Detonate(IDamageable hitObject, MunitionHitInfo hitInfo, out float damageDone, out bool destroyed)
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 forward = base.transform.forward;
            forward = (_omnidirectional ? Random.onUnitSphere : ((!_selectRandomPointInTarget || hitObject == null || hitInfo == null) ? MathHelpers.RandomRayInCone(forward, _launchAngle).normalized : base.transform.position.To(hitInfo.HitObject.transform.root.TransformPoint(hitObject.RandomPointInBounds())).normalized));
            StartCoroutine(CoroutineLaunch(forward));
        }

        damageDone = 0f;
        destroyed = false;
        return HitResult.None;
    }

    private IEnumerator CoroutineLaunch(Vector3 forward)
    {
        yield return new WaitForSeconds(_launchDelay);
        _barrel.Fire(forward);
    }

    public override string GetTooltipText()
    {
        //IMunition component = _submunitionPrefab.GetComponent<IMunition>();
        //if (component == null)
        //{
        //    return "";
        //}

        //return $"Type: Submunition Delivery\nCount: {_spawnCount}\n\nSubmunition Information: {component.MunitionName}\n{component.GetDetailText()}";
        return "";
    }

    public override void ArmWarhead()
    {
        if (_trigger != null)
        {
            _trigger.enabled = true;
        }
    }

    public override void ResetWarhead()
    {
        if (_trigger != null)
        {
            _trigger.enabled = false;
        }
        _barrel.SetAmmoSource(_magazine);
    }


}
