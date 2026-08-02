extern alias agmlib;

using System.Collections.Generic;
using Munitions;
using Ships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class ComplexCrewJobNames
    : agmlib::AGMLIB.Compatibility.HaloModernization.CrewJobLabels
{
}

public class CustomBehaviorThrusterPartConfig : agmlib::CustomBehaviorThrusterPartConfig
{
}

public class HullSegmentPatchIndicator : MonoBehaviour
{
}

public class HullSocketBuiltIn : MonoBehaviour
{
}

public class RotationFollower
    : agmlib::AGMLIB.Compatibility.HaloModernization.RotationFollower
{
}

public class SocketComponentScaler
    : agmlib::AGMLIB.Compatibility.HaloModernization.SocketComponentScaler
{
}

public class SyncVisualEffect
    : agmlib::AGMLIB.Compatibility.HaloModernization.VisualEffectStateFollower
{
}

public class TurretHideBase
    : agmlib::AGMLIB.Compatibility.HaloModernization.TurretBaseVisibilityMarker
{
}

namespace Factions
{
    public class FactionDescriptionWithoutDefaults : FactionDescription
    {
        [Header("Legacy façade presentation")]
        public Sprite[] StorageIcons;

        public Sprite ShieldProjectedIcon;

        public Sprite ShieldNotProjectedIcon;

        public GameObject ShieldNetworkBehaviorPrefab;
    }
}

namespace FleetEditor
{
    public class ClusterMagazineAmmoItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _ammoNameText;

        [SerializeField]
        private TextMeshProUGUI _ammoDetailsText;

        [SerializeField]
        private UI.InputSpinner _spinner;

        private SettingsClusterLoadout _parent;
        private IMagazine _magazine;
    }

    public class SettingsClusterLoadout : MonoBehaviour
    {
        [SerializeField]
        private Image _gaugeImage;

        [SerializeField]
        private TextMeshProUGUI _capacityText;

        [SerializeField]
        private TextMeshProUGUI _emptyLoadText;

        [SerializeField]
        private Button _addMagazineButton;

        [SerializeField]
        private GameObject _magazineItemPrefab;

        [SerializeField]
        private Color _normalGaugeColor;

        [SerializeField]
        private Color _maxGaugeColor;

        [SerializeField]
        private UI.SequentialButton _stagingSettings;

        [SerializeField]
        private GameObject _earlyStageSettings;

        [SerializeField]
        private UI.InputSpinner _earlyStageSettingsSpinner;

        [SerializeField]
        private UI.InputSpinner _separationDistanceSettingsSpinner;
    }
}

namespace FleetEditor.MissileEditor
{
    public class ClusterWarheadSettings : MonoBehaviour
    {
    }
}

namespace Munitions
{
    public class CustomCommandGuidedSeeker
        : agmlib::AGMLIB.Compatibility.HaloModernization.ModernCommandGuidedSeeker
    {
    }

    public class GuidedSplashingShellMunition : LightweightKineticShell
    {
        [Header("Legacy inert splash settings")]
        [SerializeField]
        private float _armorSplashRadius;

        [SerializeField]
        private float _radiusJitter;

        [SerializeField]
        private int _splashRayCount;

        [SerializeField]
        private float _backoffDistance;

        [SerializeField]
        private float _splashRayLength;

        [SerializeField]
        private bool _armorDamageFallsOff;

        [SerializeField]
        private bool _componentDamageFallsOff;

        [SerializeField]
        private AnimationCurve _damageFalloff;
    }

    public class LightweightDebuffMACShell
        : agmlib::AGMLIB.Compatibility.HaloModernization.ModernDebuffKineticShell
    {
        [SerializeField]
        private string _shellFlavorText;
    }

    public class LightweightMACShell : LightweightKineticShell
    {
        [SerializeField]
        private string _shellFlavorText;
    }

    public class PlasmaTorpedo
        : agmlib::AGMLIB.Compatibility.HaloModernization.ModernEvasiveCruiseMissile
    {
    }
}

namespace Munitions.ModularMissiles
{
    public class ModularMissileDetailStringReplacement : MonoBehaviour
    {
        public string ReplaceFrom;
        public string ReplaceWith;
    }
}

namespace Munitions.ModularMissiles.Descriptors.Warheads
{
    public class ClusterWarheadDescriptor
        : agmlib::AGMLIB.Compatibility.HaloModernization.ModernSubmunitionWarhead
    {
        public GameObject ClusterLoadout;

        [SerializeField]
        private WeaponEffectSet _effects;

        [SerializeField]
        private bool _scalable;

        [SerializeField]
        private float _maxCapacity;

        [SerializeField]
        private MunitionTags[] _compatibleAmmoTags;

        [SerializeField]
        private float _launchSpeed;

        [SerializeField]
        private List<Magazine> _magazines;

        [SerializeField]
        private int _stagingMode;

        [SerializeField]
        private int _earlyStagingPercentage;

        [SerializeField]
        private int _separationDistance;
    }
}

namespace Ships
{
    public class BerthingComponentFractional : BerthingComponent
    {
        [SerializeField]
        private float _actualCrewProvided;
    }

    public class ChargingRezzingMuzzle
        : agmlib::AGMLIB.Compatibility.HaloModernization.MultiModeChargingRezzingMuzzle
    {
    }

    public class ComponentHullPaintLODSharedIndexed
        : agmlib::AGMLIB.Compatibility.HaloModernization.IndexedComponentHullPaint
    {
    }

    public class CovenantBarrelGlow
        : agmlib::AGMLIB.Compatibility.HaloModernization.FunctionalBarrelGlow
    {
    }

    public class CustomBehaviorThrusterPart : agmlib::Ships.CustomBehaviorThrusterPart
    {
    }

    public class CustomLineBeamMuzzleEffects : LineBeamMuzzleEffects
    {
        [SerializeField]
        protected GameObject _spawnAtSource;

        [SerializeField]
        protected GameObject _spawnAtTarget;
    }

    public class FixedEWarComponent : agmlib::FixedEWarComponent
    {
    }

    public class HullComponentStorage : MonoBehaviour
    {
        public ResourceModifier[] ResourcesStored;
        public bool StartFilled;
    }

    public class HullComponentTileable : HullComponent
    {
    }

    public class HullPartDrive : DriveComponent
    {
        public override string PartKey => _partKey;

        public override string UIName => ShortUIName;

        public override Vector3 PositionInHull => transform.localPosition;

        protected override void Start()
        {
            HullSocket socket =
                GetComponent<HullSocket>() ?? gameObject.AddComponent<HullSocket>();
            agmlib::HullSocketInternals socketInternals =
                agmlib::NativeInternalsExtensions.Internals(socket);
            socketInternals.Key = PartKey;
            socketInternals.Size = Size;
            socketInternals.Component = this;
            socketInternals.Hull = Platform as BaseHull;

            SetSocket(socket);
            socket.UpdateColliderActive();
        }
    }

    public class HullResources : HullComponent
    {
    }

    public class HullSocketFixedWeaponGuidance : MonoBehaviour
    {
        [SerializeField]
        private float _steerAngleFactor;
    }

    public class InfiniteRezzingMuzzle : RezzingMuzzle
    {
        [SerializeField]
        private VisualEffect _flash;

        [SerializeField]
        private Animation _fireAnimation;

        [SerializeField]
        private bool _bulletLook;

        [SerializeField]
        private Sound.VariedSoundEffect _fireSound;

        [SerializeField]
        private Sound.VariedSoundEffect _fireWithReload;

        [SerializeField]
        private CovenantBarrelGlow _glower;

        [SerializeField]
        private string _munitionSaveKey;
    }

    public class MACFixedDiscreteWeaponComponent
        : agmlib::AGMLIB.Compatibility.HaloModernization.MultiModeFixedWeapon
    {
    }

    public class MultipleEjectorTubeLauncherComponent
        : agmlib::AGMLIB.Compatibility.HaloModernization.MultiEjectorTubeLauncher
    {
    }

    public class PassiveSensorComponentCustomWake : PassiveSensorComponent
    {
    }

    public class PlasmaCannon : TurretedDiscreteWeaponComponent
    {
        [SerializeField]
        protected string _munitionSaveKey;

        [SerializeField]
        protected bool _hasFlankSpeedMultiplier;

        [SerializeField]
        protected float _flankSpeedMultiplier;
    }

    public class PlasmaLance : FixedContinuousWeaponComponent
    {
        [SerializeField]
        private bool _steerableBeam;

        [SerializeField]
        private float _steerAngle;

        [SerializeField]
        private float _steerRate;
    }

    public class PowerUsageEmissive
        : agmlib::AGMLIB.Compatibility.HaloModernization.ResourceUsageEmissive
    {
    }

    public class RestrictedBulkMagazineComponent : BulkMagazineComponent
    {
        [SerializeField]
        private string[] _validMunitionKeys;
    }

    public class ShieldComponentHullPaint
        : agmlib::AGMLIB.Compatibility.HaloModernization.EmissiveComponentHullPaint
    {
    }

    public class SocketCapRemover
        : agmlib::AGMLIB.Compatibility.HaloModernization.SocketCapController
    {
    }

    public class TripleChargingRezzingMuzzle
        : agmlib::AGMLIB.Compatibility.HaloModernization.MultiModeChargingRezzingMuzzle
    {
    }

    public class TripleMACFixedDiscreteWeaponComponent
        : agmlib::AGMLIB.Compatibility.HaloModernization.MultiModeFixedWeapon
    {
    }

    public class TurretedContinuousWeaponComponentExtra
        : TurretedContinuousWeaponComponent
    {
        [SerializeField]
        protected GameObject[] _baseToRemove;
    }

    public class TurretedDiscreteWeaponComponentExtra : TurretedDiscreteWeaponComponent
    {
        [SerializeField]
        protected GameObject[] _baseToRemove;
    }
}

namespace Ships.Shield
{
    public class ShieldComponent
        : agmlib::AGMLIB.Compatibility.HaloModernization.ModernShieldComponent
    {
    }

    public class ShieldComponentEffects
        : agmlib::AGMLIB.Compatibility.HaloModernization.ShieldHitVisuals
    {
        [SerializeField]
        protected MeshRenderer meshRenderer;

        [SerializeField]
        protected AnimationCurve hitProgress;

        [SerializeField]
        protected AnimationCurve deflectionProgress;
    }

    public class ShieldComponentHolder
        : agmlib::AGMLIB.Compatibility.HaloModernization.ShieldHitSurface
    {
    }

    public class ShieldNetworkBehavior : Mirror.NetworkBehaviour
    {
    }
}

namespace Ships.SocketRestrictor
{
    public class ComponentRequiresWhitelist
        : agmlib::AGMLIB.Compatibility.HaloModernization.ComponentRequiresSocketWhitelist
    {
    }

    public class SocketComponentBlacklist
        : agmlib::AGMLIB.Compatibility.HaloModernization.SocketComponentBlacklist
    {
    }

    public class SocketComponentWhitelist
        : agmlib::AGMLIB.Compatibility.HaloModernization.SocketComponentWhitelist
    {
    }
}
