# Halo compatibility façade

This project is a local proof-of-concept façade for the public Workshop package documented under
`planning/halo-modernization`.

- The output assembly name, version, and public type identities intentionally match the legacy
  `halo.dll` contract.
- Gameplay deltas should inherit or compose the centralized AGMLIB modernization types. Do not
  copy AGMLIB into this mod.
- Keep obsolete editor and presentation identities inert when the current game no longer needs
  them.
- Preserve legacy serialized field names through inherited fields or
  `FormerlySerializedAsAttribute`; do not mutate the original asset bundles.
- Treat fleet/save migration as out of scope for this POC.
- The project is disabled by default and must not be packaged. An intentional local-only build
  requires `-p:BuildHaloShim=true`.
- Deploy only to the local `Mods/Halo` copy, after making a recoverable backup. Never overwrite
  the Workshop source.
- Do not commit this project or its derived investigation output unless the user later changes the
  no-commit instruction.
