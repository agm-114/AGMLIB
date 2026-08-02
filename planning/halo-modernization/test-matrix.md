# Modernization test matrix

## Runtime POC gates

| Test | Current result |
|---|---|
| Exact legacy identity | Pass: `halo, Version=0.0.0.0` reaches `PostLoad` with 52 façade types. |
| Hull selector icons | Pass for all six UNSC and all three Covenant rows; 18 silhouette fields repaired across nine hulls. |
| VLS-0-43 installation | Pass on Paris Mount 3; no NRE and no magenta material after mapping. |
| VLS-2 installation | Pass on Paris Mount 3; no NRE and no magenta material after mapping. |
| Gladius creation | Pass; no selection crash, `Fusion Drive` module visible, propulsion reports `1x Fusion Drive`. |
| Covenant defaults | Pass on a fresh Mutan: four Emri Ka lances, Mutan Et bridge/bay, and Tamran-pattern drive remain installed. |
| Plasma torpedo launcher | Pass: one palette entry on the authored `TM1` torpedo mount, absent from general `MT2`, repeated palette opens do not duplicate it, and installation succeeds. |
| Covenant shield palette | Pass: Corvex, Morelia, and Tantalus DFGs appear in the Mutan 3x3x3 module; Corvex previews and installs. |
| Saved-fleet editor load | Pass after filtering Unity-destroyed cached sockets; the Ester/Mutan fleet renders with zero NREs or socket-outline errors. |
| Testing Range spawn | Pass on the saved Ester/Mutan fleet against the 266-point stock enemy: both ships spawn and remain responsive with zero NREs, engine-audio stacks, or socket-render errors. |
| Legacy engine audio | Pass: nine invalid `_simpleSource`/`_simpleSoundEffect` combinations normalize at `PostLoad`; the prior `GroupedAudioSource.CoroutineFadeIn` NRE does not recur. |
| Eight missing materials | Startup normalization reports eight repairs; six untested component install paths remain. |
| Broad gameplay | Partial: editor save/reopen and Testing Range spawn pass; firing, damage/repair, missile designer, skirmish, pooling, and multiplayer remain pending. |

## Static and build gates

| Test | Pass condition |
|---|---|
| AGMLIB build | `dotnet build AGMLIB/AGMLIB.csproj --no-restore` succeeds with no errors. |
| Coverage audit | Three bundles scanned; exactly 52 serialized identities and 170 decompiled files match their inventories with no missing/extra entry. |
| Script identity scan | No rebuilt bundle `MonoScript` references `halo.dll`; every custom reference resolves to centralized AGMLIB. |
| Manifest | Current game version, centralized AGMLIB dependency, no bundled Harmony/legacy DLL. |
| Save-key diff | Every removed/changed save key is explained; semantic equivalents retain keys. |
| Prefab dump | Expected 92 assets, zero dump errors, zero missing primary interfaces/components. |
| Cold load | Same results after full game restart with caches cold. |

## Hull and armor

Run for all nine hulls.

| Area | Cases |
|---|---|
| Editor | create hull, fill/remove every socket, copy/paste component, duplicate ship, switch faction, save/reopen fleet |
| Armor | bow/side/stern/top/bottom/seam hits; shallow ricochet; penetration; overpenetration; heat/brush damage |
| Collider sampler | entry UV, exit point, random exterior point, longest interior path, smallest thin feature |
| Damage control | disable, destroy, repair, restore each critical component and every hull segment |
| Movement | six translation directions, pitch/yaw/roll, flank/afterburner, damaged thruster, destroyed drive |
| Presentation | LOD changes, paint, nameplate, outage VFX, status board, DC board, selection volume, shoulder camera |
| Persistence | damaged armor, destroyed components, fuel, cooldowns, and component states survive save/load |

## Submunition warheads

| Case | Expected |
|---|---|
| no submunition selected | clear editor warning; cannot produce an invalid silent launch |
| valid smaller missile | native capacity/cost/volume and registration match descriptor values |
| equal/greater size policy | invalid choices absent or rejected |
| self/nested submunition | rejected by native validation |
| end-of-path | release at path end and finish release state |
| target acquisition | release at configured range/dot threshold |
| spread options | cone/omni directions stay within authored option |
| release interval | immediate and delayed salvos complete, including parent death during delay |
| targeting | track, bearing-only, no track, lead, and programmable submunition paths |
| multiplayer | host spawns; client sees identical count, salvo, targets, and effects |
| save migration | single old entry maps exactly; mixed/invalid old entry follows documented warning policy |

## Missile and launcher

| Case | Expected |
|---|---|
| path launch | complete `_launchesPerLoad` burst, alternating ejectors, configured withdrawal per shot, correct RPC tube index |
| track launch | complete burst; target and optional dogleg preserved |
| programming channels | queue, cancel, launch, and channel exhaustion use native behavior |
| reload | cycle/reload timing, optional index reset, ammo change, empty magazine |
| obstruction | native launch-area check blocks correctly |
| failure | disabled/destroyed launcher and null ejector fail closed without withdrawing ammo |
| command seeker | tracked, locked, visual, superseded, jammed comms, position target, TRP/dogleg |
| evasive torpedo | none/weave/corkscrew, start/end-distance boundaries, host/client path agreement, target loss, repool/reset |
| pooling/save | unpool/reset and bulk-save restore do not retain previous target/dogleg |

## MAC and other weapons

| Case | Expected |
|---|---|
| tag selection | light, heavy, and super-heavy tags choose the intended profile |
| unknown/null ammo | deterministic super-heavy fallback and no null exception |
| magazine | each mode reloads at its capacity and uses its recycle delay |
| stat modifiers | current subtype modifiers affect the selected mode's capacity, reload, and recycle values |
| charge timing | replicated cycle state starts the selected effect once at `reload - charge` and resets after reload/ammo switch |
| muzzle effects | host and client select the same mode-specific fire/charge effect |
| temporary modifiers | exactly the selected mode's firing modifiers apply once; inherited/base effect array is empty |
| reporting | rounds fired, hits, damage, destroyed targets, carried ammo |
| save/load | waiting, reload accumulation, fired count, selected ammo restore |
| AI | fixed-facing target selection, firing clearance, return fire, cease fire |
| plasma | native discrete/continuous targeting, resource starvation, glow, beam stop, pooling |
| EWar | fixed component facing, on/off, jammed target, AI use |
| debuff MAC | never/structure-broken/always modes, nearest component selection, debuff persistence, dedicated structure damage |
| shield-depleting MAC | marked active shield drops in one hit and stops that hit; unshielded target receives the native kinetic/debuff chain |

## Shields

| Case | Expected |
|---|---|
| no shield installed | damage path is byte-for-byte native in observed result/state |
| ordinary hull collider | even with a shield installed, an unmarked collider follows the native damage path |
| toggle off/on | off cancels presentation and passes hits; turning on waits the authored cooldown before the surface becomes active |
| marked shield collider | host absorbs once only when `ShieldHitSurface` is the actual hit collider, reports stopped, reduces shield integrity, hull untouched |
| armor scaling | low penetration reduces effective shield damage by configured curve |
| armor-only | marked shield surface returns ricochet without reducing shield integrity; unmarked collider is native |
| missile multiplier | missile damage applies the authored legacy-compatible 0.2 multiplier before shield damage |
| depletion marker | an explicit shield-disrupting damage wrapper consumes current shield health without leaking ordinary hit damage to the hull |
| penetration capacity | an absorbed ordinary hit consumes the attacker's armor-penetration capacity |
| zero component damage | shield does not swallow a meaningless damage dealer |
| depletion | last hit stops, shield enters cooldown, subsequent hits pass through |
| cooldown | completion restores capacity only when toggled and functional |
| component damage | disabled/destroyed shield unregisters or fails closed; repair restores behavior |
| multiple shields | deterministic primary order; removal leaves remaining shield registered |
| save/load | health, toggle, and cooldown state restore |
| host/client | host alone decides absorption; toggle, integrity, collapse, expand, and hit VFX replicate |
| damage families | spawned shell, lightweight shell, missile, beam/raycast, armor-only splash, debuff |
| cleanup | ship destruction/unload removes registry entries and subscriptions |

## Socket rules and editor

| Case | Expected |
|---|---|
| unrestricted socket | vanilla palette and installation unchanged |
| whitelist | only listed stable save keys appear/install |
| blacklist | forbidden stable save keys absent/rejected |
| requires whitelist | rejected on sockets without whitelist |
| both rules | component must pass both |
| direct/save install | runtime `SetComponent` rejects invalid assets before destroying current component |
| copy/paste | invalid clipboard content fails clearly and leaves existing component |
| faction | rule behavior is independent of localized component name and load order |
| requires-whitelist palette | a requires-whitelist component is absent from unrestricted sockets as well as unlisted restricted sockets |

## Resource and visual adapters

| Case | Expected |
|---|---|
| emissive | reads current native pool, clamps 0-1, updates at configured interval |
| no resource/material | no exception and no gameplay change |
| paint | indexed and emissive material colors survive LOD and editor reopen |
| cap | native cap removal plus named exception has correct empty/install/remove state |
| glow | functional change is event-driven; fire curve starts immediately; destroyed object unsubscribes |
| VFX follower | play/stop transitions only when state changes |
| rotation | follows in `LateUpdate` without physics jitter |
| crew labels | only a marked hull rebuilds crew rows; ordinary ships retain native formatting and total complement |
| socket visual scale | each of the eight marked surface-socket roots applies only the installed component's authored visual scale |
| turret base | the four marked sockets hide/show their configured base hierarchy on install/remove without affecting other turrets |

## Fleet/system regression

Build four fixed fleets:

1. smallest UNSC hull plus basic MAC/missile;
2. mixed six-hull UNSC fleet including Thanatos;
3. mixed three-hull Covenant fleet with shields/plasma/torpedoes;
4. mirror host/client stress fleet with the largest hulls.

For each:

- fleet editor reports and total points;
- AI versus AI for at least one full match;
- player host and one remote player;
- disconnect/reconnect where supported;
- save during missile flight, weapon reload, shield cooldown, and damaged armor;
- reload and finish the match;
- inspect logs for loader, Mirror, pooling, serialization, missing reference, and Harmony errors.

## Final soak

Run Ket and Thanatos in the same long battle for at least 30 minutes with repeated shield hits,
multi-mode firing, torpedo salvos, component destruction/repair, and LOD transitions. Capture frame
time, allocations, network warnings, and registry/pool counts before and after. Pass requires no
unbounded growth and no retained ship/component after scene unload.
