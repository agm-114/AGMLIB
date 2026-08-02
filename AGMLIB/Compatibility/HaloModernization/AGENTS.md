# Workshop modernization scripts

These scripts are clean AGMLIB-side replacements for runtime behavior found in the public Workshop
package documented under `planning/halo-modernization`.

They are not binary-compatible shims for the package's old `halo.dll`. Unity serializes the
assembly-qualified script identity into each bundle, so the source prefabs must be rebuilt and
rebound to these AGMLIB types (or to the native replacements listed in the plan). The rebuilt mod
must depend on the centralized AGMLIB Workshop item and must not ship its own Harmony or AGMLIB
assembly.

Keep this folder limited to behavior that remains genuinely custom:

- prefer a current native component or descriptor whenever it supplies the required behavior;
- keep serialized authoring fields flat because nested AGMLIB-defined custom values are not
  supported by the current bundle serializer;
- serialize runtime references that must survive finalized-pattern cloning or pooling;
- make authoritative gameplay decisions on the host and use native RPC/state machinery;
- keep version-sensitive native access behind typed `Internals()` accessors.

The deep pass established several non-negotiable boundaries:

- shield damage interception requires the exact hit collider to carry `ShieldHitSurface`; never
  fall back to "any shield registered on this ship";
- the multi-ejector launcher owns a complete authored burst, not one shot per programming request;
- multi-mode weapons retain per-profile stat values and temporary firing modifiers while leaving
  native targeting, reports, feed, and RPC ownership intact;
- the debuff MAC shell wraps the current lightweight shell and exposes shield depletion through an
  explicit interface, never by reflecting into the native chained-damager queue;
- plasma-torpedo evasion runs as a host-side delta after native cruise state processing;
- crew labels, socket visual scaling, and turret-base visibility are opt-in adapters activated only
  by their explicit authoring components.

The inventory and migration disposition for every old serialized type and patch family lives in
`planning/halo-modernization/script-inventory.md`.
