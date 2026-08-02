using System;
using System.Collections.Generic;
using System.Linq;
using Munitions;
using Ships;
using Sound;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace AGMLIB.Compatibility.HaloModernization;

/// <summary>
/// Modern fixed discrete weapon with ammo-tag-selected light, heavy, and
/// super-heavy timing profiles. Native weapon targeting, ammo feed, RPCs,
/// reporting, and save-state storage remain in the base classes.
/// </summary>
public class MultiModeFixedWeapon : FixedDiscreteWeaponComponent
{
    public enum AmmoMode
    {
        Light,
        Heavy,
        SuperHeavy,
    }

    [Header("Ammo classification")]
    [SerializeField]
    private MunitionTags[] _lightAmmoTags = [];

    [SerializeField]
    private MunitionTags[] _heavyAmmoTags = [];

    [Header("Light profile")]
    [SerializeField]
    private float _reloadTimeLight = 5f;

    [SerializeField]
    [FormerlySerializedAs("_timeBetweenMuzzlesLight")]
    private float _recycleTimeLight = 0.25f;

    [SerializeField]
    private int _magazineSizeLight = 1;

    [SerializeField]
    private float _chargingTimeLight = 2f;

    [SerializeField]
    private TemporaryFiringModifier[] _temporaryFiringEffectsLight = [];

    [Header("Heavy profile")]
    [SerializeField]
    private float _reloadTimeHeavy = 120f;

    [SerializeField]
    [FormerlySerializedAs("_timeBetweenMuzzlesHeavy")]
    private float _recycleTimeHeavy = 0.25f;

    [SerializeField]
    private int _magazineSizeHeavy = 1;

    [SerializeField]
    private float _chargingTimeHeavy = 2f;

    [SerializeField]
    private TemporaryFiringModifier[] _temporaryFiringEffectsHeavy = [];

    [Header("Super-heavy profile")]
    [SerializeField]
    private float _reloadTimeSuperHeavy = 320f;

    [SerializeField]
    [FormerlySerializedAs("_timeBetweenMuzzlesSuperHeavy")]
    private float _recycleTimeSuperHeavy = 0.25f;

    [SerializeField]
    private int _magazineSizeSuperHeavy = 1;

    [SerializeField]
    private float _chargingTimeSuperHeavy = 2f;

    [SerializeField]
    private TemporaryFiringModifier[] _temporaryFiringEffectsSuperHeavy = [];

    [ShipStat(
        "discreteweapon-reload",
        "$SHIPSTAT_WEPRELOADTIME",
        "$UNIT_SECONDS",
        InitializeFrom = "_reloadTimeLight",
        PositiveBad = true,
        LimitSubtypeModifiersOnly = true,
        NameSubtypeFrom = "_reloadStatSubtype"
    )]
    private StatValue _statReloadTimeLight;

    [ShipStat(
        "discreteweapon-reload",
        "$SHIPSTAT_WEPRELOADTIME",
        "$UNIT_SECONDS",
        InitializeFrom = "_reloadTimeHeavy",
        PositiveBad = true,
        LimitSubtypeModifiersOnly = true,
        NameSubtypeFrom = "_reloadStatSubtype"
    )]
    private StatValue _statReloadTimeHeavy;

    [ShipStat(
        "discreteweapon-reload",
        "$SHIPSTAT_WEPRELOADTIME",
        "$UNIT_SECONDS",
        InitializeFrom = "_reloadTimeSuperHeavy",
        PositiveBad = true,
        LimitSubtypeModifiersOnly = true,
        NameSubtypeFrom = "_reloadStatSubtype"
    )]
    private StatValue _statReloadTimeSuperHeavy;

    [ShipStat(
        "discreteweapon-recycle",
        "$SHIPSTAT_WEPRECYCLETIME",
        "$UNIT_SECONDS",
        InitializeFrom = "_recycleTimeLight",
        PositiveBad = true,
        LimitSubtypeModifiersOnly = true,
        NameSubtypeFrom = "_recycleStatSubtype"
    )]
    private StatValue _statRecycleTimeLight;

    [ShipStat(
        "discreteweapon-recycle",
        "$SHIPSTAT_WEPRECYCLETIME",
        "$UNIT_SECONDS",
        InitializeFrom = "_recycleTimeHeavy",
        PositiveBad = true,
        LimitSubtypeModifiersOnly = true,
        NameSubtypeFrom = "_recycleStatSubtype"
    )]
    private StatValue _statRecycleTimeHeavy;

    [ShipStat(
        "discreteweapon-recycle",
        "$SHIPSTAT_WEPRECYCLETIME",
        "$UNIT_SECONDS",
        InitializeFrom = "_recycleTimeSuperHeavy",
        PositiveBad = true,
        LimitSubtypeModifiersOnly = true,
        NameSubtypeFrom = "_recycleStatSubtype"
    )]
    private StatValue _statRecycleTimeSuperHeavy;

    [ShipStat(
        "mac-autoloader-capacity-light",
        "MAC Autoloader Capacity (Light)",
        "",
        AllowInitializeType = typeof(int),
        InitializeFrom = "_magazineSizeLight"
    )]
    private StatValue _statMagazineSizeLight;

    [ShipStat(
        "mac-autoloader-capacity-heavy",
        "MAC Autoloader Capacity (Heavy)",
        "",
        AllowInitializeType = typeof(int),
        InitializeFrom = "_magazineSizeHeavy"
    )]
    private StatValue _statMagazineSizeHeavy;

    [ShipStat(
        "mac-autoloader-capacity-superheavy",
        "MAC Autoloader Capacity (Super Heavy)",
        "",
        AllowInitializeType = typeof(int),
        InitializeFrom = "_magazineSizeSuperHeavy"
    )]
    private StatValue _statMagazineSizeSuperHeavy;

    private bool _chargingEffectsPlayed;
    private Coroutine[] _modeEffectCoroutines = [];
    private string[] _activeEffectStatNames = [];

    public AmmoMode CurrentMode
    {
        get
        {
            MunitionTags? tags = SelectedAmmoType?.Tags;
            if (tags.HasValue && MatchesAny(_lightAmmoTags, tags.Value))
                return AmmoMode.Light;
            if (tags.HasValue && MatchesAny(_heavyAmmoTags, tags.Value))
                return AmmoMode.Heavy;
            return AmmoMode.SuperHeavy;
        }
    }

    public override float? RoundsPerSecond
    {
        get
        {
            if (!NeedsExternalAmmoFeed)
                return null;

            int capacity = SelectedMagazineSize;
            float cycleDuration =
                Math.Max(0, capacity - 1) * SelectedRecycleTime + SelectedReloadTime;
            return cycleDuration > 0f ? capacity / cycleDuration : null;
        }
    }

    protected override float _cycleLength => SelectedReloadTime;

    protected override void Start()
    {
        base.Start();
        int effectSlots = Math.Max(
            _temporaryFiringEffectsLight?.Length ?? 0,
            Math.Max(
                _temporaryFiringEffectsHeavy?.Length ?? 0,
                _temporaryFiringEffectsSuperHeavy?.Length ?? 0
            )
        );
        _modeEffectCoroutines = new Coroutine[effectSlots];
        _activeEffectStatNames = new string[effectSlots];
    }

    protected override void OnDestroy()
    {
        ClearTemporaryFiringEffects();
        base.OnDestroy();
    }

    public override void GetFormattedStats(
        List<(string, string)> rows,
        bool full,
        int groupSize = 1
    )
    {
        base.GetFormattedStats(rows, full, groupSize);
        rows.RemoveAll(
            row =>
                row.Equals(_statReloadTime.FullTextWithLinkRow)
                || row.Equals(_statRecycleTime.FullTextWithLinkRow)
                || row.Item1 == "$SHIPSTAT_AUTOLOADERCAPACITY"
                || row.Item1 == "$SHIPSTAT_SINGLESHOT"
        );
        AddProfileStats(
            rows,
            "Light",
            _statMagazineSizeLight,
            _statReloadTimeLight,
            _statRecycleTimeLight,
            _temporaryFiringEffectsLight
        );
        AddProfileStats(
            rows,
            "Heavy",
            _statMagazineSizeHeavy,
            _statReloadTimeHeavy,
            _statRecycleTimeHeavy,
            _temporaryFiringEffectsHeavy
        );
        AddProfileStats(
            rows,
            "Super Heavy",
            _statMagazineSizeSuperHeavy,
            _statReloadTimeSuperHeavy,
            _statRecycleTimeSuperHeavy,
            _temporaryFiringEffectsSuperHeavy
        );
    }

    protected override void OnTarget(Vector3 aimPoint, bool changed)
    {
        if (!CheckFire())
            return;

        Muzzle muzzle = GetNextMuzzle(out int muzzleIndex);
        if (muzzle == null)
            return;

        if (muzzle is RezzingMuzzle rezzingMuzzle)
        {
            Vector3 shotDirection = _fudgeShotVector
                ? rezzingMuzzle.transform.position.To(aimPoint).normalized
                : rezzingMuzzle.transform.forward;
            rezzingMuzzle.Fire(shotDirection);
        }
        else
        {
            muzzle.Fire();
        }

        _weaponRpcProvider.RpcFireMuzzleEffect(RpcKey, muzzleIndex);
        ApplyTemporaryFiringEffects();
        _magazineFired++;
        if (_magazineFired >= SelectedMagazineSize)
            StartReload();
        if (!_reloading)
        {
            _waitingForMuzzle = true;
            _muzzleAccum = 0f;
        }
    }

    protected override void RunTimers(float deltaTime)
    {
        if (_reloading)
        {
            _reloadAccum += deltaTime;
            if (_reloadAccum >= SelectedReloadTime)
            {
                _reloading = false;
                _magazineFired = 0;
            }
        }

        if (!CycleActive)
        {
            _chargingEffectsPlayed = false;
        }
        else if (!_chargingEffectsPlayed && _targetMode != TargetingMode.None)
        {
            float reloadTime = SelectedReloadTime;
            float chargeThreshold =
                reloadTime <= 0f
                    ? 0f
                    : Mathf.Clamp01(
                        (reloadTime - Mathf.Max(0f, SelectedChargingTime)) / reloadTime
                    );
            if (CyclePercent >= chargeThreshold)
            {
                foreach (Muzzle muzzle in _muzzles)
                {
                    if (muzzle is MultiModeChargingRezzingMuzzle chargingMuzzle)
                        chargingMuzzle.PlayChargingEffect();
                }
                _chargingEffectsPlayed = true;
            }
        }

        if (_waitingForMuzzle)
        {
            _muzzleAccum += deltaTime;
            if (_muzzleAccum >= SelectedRecycleTime)
                _waitingForMuzzle = false;
        }
    }

    protected override void AmmoSourceChanged(IMagazine source)
    {
        base.AmmoSourceChanged(source);
        _chargingEffectsPlayed = false;
    }

    private int SelectedMagazineSize =>
        CurrentMode switch
        {
            AmmoMode.Light => Mathf.Max(
                1,
                Mathf.RoundToInt(_statMagazineSizeLight?.Value ?? _magazineSizeLight)
            ),
            AmmoMode.Heavy => Mathf.Max(
                1,
                Mathf.RoundToInt(_statMagazineSizeHeavy?.Value ?? _magazineSizeHeavy)
            ),
            _ => Mathf.Max(
                1,
                Mathf.RoundToInt(
                    _statMagazineSizeSuperHeavy?.Value ?? _magazineSizeSuperHeavy
                )
            ),
        };

    private float SelectedReloadTime =>
        CurrentMode switch
        {
            AmmoMode.Light => _statReloadTimeLight?.Value ?? _reloadTimeLight,
            AmmoMode.Heavy => _statReloadTimeHeavy?.Value ?? _reloadTimeHeavy,
            _ => _statReloadTimeSuperHeavy?.Value ?? _reloadTimeSuperHeavy,
        };

    private float SelectedRecycleTime =>
        CurrentMode switch
        {
            AmmoMode.Light => _statRecycleTimeLight?.Value ?? _recycleTimeLight,
            AmmoMode.Heavy => _statRecycleTimeHeavy?.Value ?? _recycleTimeHeavy,
            _ => _statRecycleTimeSuperHeavy?.Value ?? _recycleTimeSuperHeavy,
        };

    private float SelectedChargingTime =>
        CurrentMode switch
        {
            AmmoMode.Light => _chargingTimeLight,
            AmmoMode.Heavy => _chargingTimeHeavy,
            _ => _chargingTimeSuperHeavy,
        };

    private TemporaryFiringModifier[] SelectedTemporaryFiringEffects =>
        CurrentMode switch
        {
            AmmoMode.Light => _temporaryFiringEffectsLight,
            AmmoMode.Heavy => _temporaryFiringEffectsHeavy,
            _ => _temporaryFiringEffectsSuperHeavy,
        };

    private void ApplyTemporaryFiringEffects()
    {
        TemporaryFiringModifier[] effects = SelectedTemporaryFiringEffects;
        if (effects == null || effects.Length == 0 || Socket == null)
            return;

        for (int index = 0; index < effects.Length; index++)
        {
            ClearTemporaryFiringEffect(index);
            TemporaryFiringModifier effect = effects[index];
            _activeEffectStatNames[index] = effect.Modifier.StatName;
            _modeEffectCoroutines[index] = StartCoroutine(
                CoroutineTemporaryFiringEffect(index, effect)
            );
        }
    }

    private System.Collections.IEnumerator CoroutineTemporaryFiringEffect(
        int index,
        TemporaryFiringModifier effect
    )
    {
        Socket.Stats.AddStatModifier(this, effect.Modifier);
        yield return new WaitForSeconds(effect.Duration);
        ClearTemporaryFiringEffect(index, stopCoroutine: false);
    }

    private void ClearTemporaryFiringEffects()
    {
        for (int index = 0; index < _modeEffectCoroutines.Length; index++)
            ClearTemporaryFiringEffect(index);
    }

    private void ClearTemporaryFiringEffect(int index, bool stopCoroutine = true)
    {
        if (index < 0 || index >= _modeEffectCoroutines.Length)
            return;

        Coroutine active = _modeEffectCoroutines[index];
        if (stopCoroutine && active != null)
            StopCoroutine(active);
        _modeEffectCoroutines[index] = null;

        string statName = _activeEffectStatNames[index];
        if (!string.IsNullOrEmpty(statName) && Socket != null)
            Socket.Stats.RemoveStatModifier(this, statName);
        _activeEffectStatNames[index] = null;
    }

    private static void AddProfileStats(
        List<(string, string)> rows,
        string profileName,
        StatValue magazine,
        StatValue reload,
        StatValue recycle,
        TemporaryFiringModifier[] effects
    )
    {
        int capacity = Mathf.Max(1, Mathf.RoundToInt(magazine?.Value ?? 1f));
        rows.Add(($"{profileName} autoloader capacity", capacity.ToString()));
        rows.Add(($"{profileName} reload", $"{reload?.Value ?? 0f:N1} $UNIT_SECONDS"));
        if (capacity > 1)
        {
            rows.Add(
                ($"{profileName} recycle", $"{recycle?.Value ?? 0f:N2} $UNIT_SECONDS")
            );
        }

        if (effects == null)
            return;
        foreach (TemporaryFiringModifier effect in effects)
        {
            rows.Add(
                (
                    $"{profileName} temporary effect",
                    $"{effect.Modifier} for {effect.Duration:N1} $UNIT_SECONDS"
                )
            );
        }
    }

    private static bool MatchesAny(IEnumerable<MunitionTags> choices, MunitionTags selected)
    {
        return choices?.Any(candidate => candidate.Equals(selected)) ?? false;
    }
}

public class MultiModeChargingRezzingMuzzle : RezzingMuzzle
{
    [SerializeField]
    [FormerlySerializedAs("_weaponComponent")]
    private MultiModeFixedWeapon _modeWeapon;

    [Header("Light")]
    [SerializeField]
    [FormerlySerializedAs("_flashLight")]
    private VisualEffect _fireFlashLight;

    [SerializeField]
    private Animation _fireAnimationLight;

    [SerializeField]
    private VariedSoundEffect _fireSoundLight;

    [SerializeField]
    private VisualEffect _chargingFlashLight;

    [SerializeField]
    private Animation _chargingAnimationLight;

    [SerializeField]
    private VariedSoundEffect _chargingSoundLight;

    [Header("Heavy")]
    [SerializeField]
    [FormerlySerializedAs("_flashHeavy")]
    private VisualEffect _fireFlashHeavy;

    [SerializeField]
    private Animation _fireAnimationHeavy;

    [SerializeField]
    private VariedSoundEffect _fireSoundHeavy;

    [SerializeField]
    private VisualEffect _chargingFlashHeavy;

    [SerializeField]
    private Animation _chargingAnimationHeavy;

    [SerializeField]
    private VariedSoundEffect _chargingSoundHeavy;

    [Header("Super-heavy charge")]
    [SerializeField]
    private VisualEffect _chargingFlashSuperHeavy;

    [SerializeField]
    private Animation _chargingAnimationSuperHeavy;

    [SerializeField]
    private VariedSoundEffect _chargingSoundSuperHeavy;

    public override void FireEffect()
    {
        switch (Mode)
        {
            case MultiModeFixedWeapon.AmmoMode.Light:
                Play(_fireFlashLight, _fireAnimationLight, _fireSoundLight);
                break;
            case MultiModeFixedWeapon.AmmoMode.Heavy:
                Play(_fireFlashHeavy, _fireAnimationHeavy, _fireSoundHeavy);
                break;
            default:
                base.FireEffect();
                break;
        }
    }

    public void PlayChargingEffect()
    {
        switch (Mode)
        {
            case MultiModeFixedWeapon.AmmoMode.Light:
                Play(_chargingFlashLight, _chargingAnimationLight, _chargingSoundLight);
                break;
            case MultiModeFixedWeapon.AmmoMode.Heavy:
                Play(_chargingFlashHeavy, _chargingAnimationHeavy, _chargingSoundHeavy);
                break;
            default:
                Play(
                    _chargingFlashSuperHeavy,
                    _chargingAnimationSuperHeavy,
                    _chargingSoundSuperHeavy
                );
                break;
        }
    }

    private MultiModeFixedWeapon.AmmoMode Mode
    {
        get
        {
            if (_modeWeapon == null)
                _modeWeapon = GetComponentInParent<MultiModeFixedWeapon>();
            return _modeWeapon?.CurrentMode ?? MultiModeFixedWeapon.AmmoMode.SuperHeavy;
        }
    }

    private void Play(VisualEffect effect, Animation animation, VariedSoundEffect sound)
    {
        effect?.Play();
        animation?.Play();
        if (sound != null)
            GlobalSFX.PlayOneShotSpatial(sound, transform);
    }
}
