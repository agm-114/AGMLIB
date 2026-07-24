# Evidence ledger

This ledger pins claims in `knowledge/` to the local evidence used on
2026-07-24. Regenerate the ignored workspace after a game update:

```powershell
.\scripts\NativeCode\Export-NebulousManagedCode.ps1 -Force -PackWithRepomix
```

## Native assembly snapshot

| Assembly | SHA-256 | Decompiled C# files | Last write UTC |
|---|---|---:|---|
| `Nebulous.dll` | `26DE96E2E384DA2F50AFF01CC1D2FDEE42DD2E46FE540083F96E249D5CF58365` | 2,533 | 2026-06-24 02:01:05 |
| `Assembly-CSharp.dll` | `D9BA1F1F4E28BA6E81A449062DBF98AFE5F33F524BE9838A024932D6E53B11BF` | 4 | 2026-05-30 05:29:41 |
| `ProceduralPlanets.dll` | `9FC6D8EB530FF96534B701B3DF4D41C7BEE065D5F35DF0ECC25074452E080A02` | 42 | 2026-06-24 02:01:06 |
| `UINC4.dll` | `0904DF073F195D8B2EFA7970AD9852BF23BF7BF5EDDD1135F00B3CFB8091E610` | 72 | 2026-06-24 02:01:06 |

Tooling: ILSpy CLI `10.1.0.8386`; Repomix `1.17.0`. The native pack contains
2,659 files (including project and completion metadata) and approximately
1,234,202 tokens; 2,651 files are decompiled C#.

## Evidence labels

- **Verified-source**: directly visible in the pinned decompile or AGMLIB source.
- **Verified-prefab**: confirmed in dumped prefab YAML from the same installation.
- **Observed-runtime**: reproduced in-game with a dated log/scenario.
- **Inferred**: a reasoned connection not yet closed by prefab/runtime evidence.
- **Historical**: commit or old note that may not match current runtime.

Current knowledge documents primarily contain verified-source claims and clearly
label runtime conclusions that still need observation. Prefab and runtime
evidence should record the exact dump/log paths in the local ignored testing
notes rather than committing machine-specific paths here.

## Refresh rules

1. Record the old manifest before replacing the dump.
2. Rebuild with `-Force`; never mix files from two assembly hashes.
3. Diff type/member indexes and the feature-specific native files.
4. Re-dump relevant prefabs.
5. Verify Harmony and typed-internals targets.
6. Build AGMLIB and run feature smoke tests.
7. Update this ledger only after the supported snapshot is chosen.

Decompiled sources are a local research artifact and remain git-ignored.
