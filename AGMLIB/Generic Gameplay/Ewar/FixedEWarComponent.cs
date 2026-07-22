using Lib.Generic_Gameplay.Ewar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class FixedEWarComponent : FixedContinuousWeaponComponent, IEWarWeapon, IWeapon, IHullComponent, ITuneableEWar
{
    [Header("EWar Turret")]
    [SerializeField]
    protected SignatureType _sigType = SignatureType.Radar;

    [SerializeField]
    protected float _coneFov = 15f;

    [SerializeField]
    protected float _maxRange = 500f;

    [SerializeField]
    protected float _effectAreaRatio = 0.4f;

    [SerializeField]
    protected float _radiatedPower = 2f;

    [SerializeField]
    protected float _gain = 1f;

    [SerializeField]
    protected float _edgeFalloff = 0f;

    [SerializeField]
    private bool _showLOB = true;

    public override WeaponType WepType => WeaponType.EWar;

    public override ComponentCostClass CostBreakdownClass => ComponentCostClass.EWar;

    public override float? MaxEffectiveRange => _maxRange;

    protected override void Awake()
    {
        base.Awake();
        if (_ewType == EWarWeaponType.None)
        {
            _ewType = EWarWeaponType.Jammer;
        }

        Muzzle[] muzzles = _muzzles;
        foreach (Muzzle muzzle in muzzles)
        {
            if (muzzle is FollowingInstanceMuzzle following)
            {
                //Common.SetVal(following, "_matchRotation", true);
                following.OnInstanceSpawned += HandleInstanceSpawned;
            }
        }
    }

    public override void GetFormattedStats(List<(string, string)> rows, bool full, int groupSize = 1)
    {
        base.GetFormattedStats(rows, full, groupSize);
        rows.Add(("$SHIPSTAT_SIGNATURETYPE", _sigType.GetAbbrevWithColor()));
        rows.Add(("$SHIPSTAT_BEAMWIDTH", $"{_coneFov * 2f} $UNIT_DEGREES"));
        rows.Add(("$SHIPSTAT_RADIATEDPOWER", $"{_radiatedPower} $UNIT_KILOWATTS"));
        rows.Add(("$SHIPSTAT_GAIN", $"{_gain} $UNIT_DECIBELS"));
    }

    private void HandleInstanceSpawned(NetworkPoolable instance)
    {
        if (instance is ISettableEWarParameters settable)
        {
            settable.SetParams(_sigType, omni: false, _coneFov, _maxRange, _effectAreaRatio, _radiatedPower, _gain, _edgeFalloff, _showLOB);
        }
    }

    public bool CanJamAny(int sigMask)
    {
        return ((1 << (int)_sigType) & sigMask) != 0;
    }

    public bool WillDrawARAD(int sigMask)
    {
        SignatureType passive = _sigType.ToPassiveEquivalent();
        if (passive == SignatureType.NoSignature)
        {
            return false;
        }
        return ((1 << (int)passive) & sigMask) != 0;
    }

    public static bool WeaponGroupNeedsFacing(WeaponGroup group)
    {
        return group?.Members?.Any(member => member is FixedEWarComponent) == true;
    }
}

[HarmonyPatch]
class WeaponGroupFixedEWarNeedsFacing
{
    static System.Reflection.MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(WeaponGroup), "Ships.IFacingDriver.get_NeedsFacing");
    }

    static void Postfix(WeaponGroup __instance, ref bool __result)
    {
        __result = __result || FixedEWarComponent.WeaponGroupNeedsFacing(__instance);
    }
}
