# Workshop compatibility

This document records support and dependency boundaries for public Workshop
mods in the manual compatibility catalog. It is not a promise that AGMLIB owns
or can repair third-party code.

## CI support policy

The default manual Workshop run selects catalog entries whose
`default_enabled` value is true or omitted. Known deterministic third-party
failures remain in `scripts/CI/WorkshopCompatibilityCatalog.json` with:

- `default_enabled: false`
- `ci_priority: low`
- `support_status: out-of-support`
- a narrow `known_failure` signature

An out-of-support entry can still be selected explicitly with the workflow's
`mod_ids` input. This path is for occasional diagnosis after a game or mod
update, not routine full-matrix coverage.

For public mods containing `Ares.dll`, apply the legacy Ares classification
only when the log also contains the exact `VAEAmmo.PostLoad` failure chain:

```text
NullReferenceException
VAEAmmo.GetPrivateField
VAEAmmo.GetIlluminator
VAEAmmo.PostLoad
```

The DLL filename alone is not enough to suppress a test.

## Solarian/Ares status

Workshop item `2869734654`, catalogued as **Solarian Federation Navy - Ares
Drive Yard** and loaded internally as **Ares Fleet Yard**, is out of AGMLIB
support and omitted from the default matrix.

Verified on 2026-07-30 against the public `Ares.dll` that matched the CI
artifact:

1. `VAEAmmo.PostLoad()` updates two munition prefabs and installs two Harmony
   faction-availability patches.
2. It then finds the stock E57 Floodlight's `Body/Panel` object and requests a
   `Ships.RezFollowingMuzzle`.
3. Current NEBULOUS places `Ships.EWarFollowingMuzzle` on that object. Both
   types derive separately from `Ships.FollowingInstanceMuzzle`; neither is the
   other.
4. `GetComponent<RezFollowingMuzzle>()` therefore returns null.
5. `VAEAmmo.GetPrivateField()` immediately calls `instance.GetType()`, which
   produces the observed `NullReferenceException`.

This is stale third-party reflection against a changed native prefab, not an
AGMLIB regression.

## AGMLIB scripts used by Ares

None were found.

The exact public `Ares.dll` references `Nebulous`, `0Harmony`, `mscorlib`,
`UnityEngine.CoreModule`, and `UnityEngine.VFXModule`. It does not reference
the `AGMLIB` assembly. The post-load prefab snapshot also contains no
AGMLIB-defined component type for this mod.

The relevant third-party script locations are all inside `Ares.dll`:

| Script or method | Use |
| --- | --- |
| `VAEAmmo.PostLoad` | Mod post-load entry point and failing call site |
| `VAEAmmo.updateAllMunitions` | Copies native shell effect and tracer fields |
| `VAEAmmo.GetIlluminator` | Reads the obsolete Floodlight muzzle layout |
| `VAEAmmo.SetEffectPrefab` | Writes `_effectPrefab` on the Artemis missile component |
| `Patch_HullComponent_UseableByFaction` | Harmony faction-availability override |
| `Patch_LookaheadMunition_UseableByFaction` | Harmony munition-availability override |

AGMLIB's `Debug/net481/AGMLIB.dll` is staged in the separate AGMLIB Workshop
item by the compatibility harness. Its presence in the test does not mean
`Ares.dll` consumes an AGMLIB script or API.

## Modernization guidance for the mod author

The old lookup should not traverse the E57 prefab or reflect
`RezFollowingMuzzle._followingPrefab`. Current NEBULOUS exposes the shared
sensor-illumination prefab through `Game.EWar.EWarPrefabCollection`:

```csharp
using Game.EWar;
using Utility;

SensorIlluminator GetIlluminator()
{
    NetworkPoolable prefab =
        SingletonMonobehaviour<EWarPrefabCollection>.Instance
            .SensorIlluminatorPrefab;

    return prefab.GetComponent<SensorIlluminator>()
        ?? throw new InvalidOperationException(
            "The native sensor-illumination prefab has no SensorIlluminator.");
}
```

The remaining write to the custom component's private `_effectPrefab` field is
still a third-party reflection boundary. The mod should replace it with a
typed serialized field or public setter on its own component. If legacy
reflection must remain temporarily, validate the destination object, field,
and assigned type and throw a descriptive compatibility exception rather than
dereferencing null.
