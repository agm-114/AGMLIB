using Munitions.ModularMissiles;
using Munitions.ModularMissiles.Descriptors.Warheads;

namespace AGMLIB.Compatibility.HaloModernization;

/// <summary>
/// A thin authoring profile over the game's selectable submunition warhead.
/// The native descriptor owns capacity, cost, spawning, salvo registration,
/// target programming, pooling, and multiplayer behavior.
/// </summary>
[CreateAssetMenu(
    fileName = "New Legacy Submunition Profile",
    menuName = "AGMLIB/Compatibility/Selectable Submunition Profile"
)]
public class ModernSubmunitionWarhead : SelectableSubmunitionWarheadDescriptor
{
    [Header("Legacy profile migration")]
    [SerializeField]
    private bool _applyProfileWhenInstalled = true;

    [SerializeField]
    private DetonationMode _profileMode = DetonationMode.TargetAcquisition;

    [SerializeField]
    [Min(0f)]
    private float _profileDetonationRange = 100f;

    [SerializeField]
    [Min(0)]
    private int _profileSpreadOption;

    [SerializeField]
    [Min(0)]
    private int _profileReleaseInterval;

    protected override void OnInstalled(ModularMissile missile, MissileSocket socket)
    {
        base.OnInstalled(missile, socket);
        if (!_applyProfileWhenInstalled)
            return;

        Mode = _profileMode;
        DetonateAcqRange = _profileDetonationRange;
        Spread = _profileSpreadOption;
        Interval = _profileReleaseInterval;
    }
}
