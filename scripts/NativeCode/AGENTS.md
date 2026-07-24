# Native-code workspace automation

`Export-NebulousManagedCode.ps1` creates a local searchable reconstruction of
configured NEBULOUS managed assemblies under the git-ignored `.native-code/`
folder.

## Common commands

```powershell
# Validate installation paths and required tools only
.\scripts\NativeCode\Export-NebulousManagedCode.ps1 -ValidateOnly

# Rebuild the current hash-pinned dump and Repomix pack
.\scripts\NativeCode\Export-NebulousManagedCode.ps1 -Force -PackWithRepomix

# Explicit assembly set or alternate game install
.\scripts\NativeCode\Export-NebulousManagedCode.ps1 `
  -GameRoot 'D:\Steam\steamapps\common\Nebulous' `
  -AssemblyNames Nebulous.dll,Assembly-CSharp.dll `
  -Force
```

The script refuses output outside `.native-code`, writes hash-named assembly
directories, and records tool/assembly identity in `manifest.json`. Repomix is
run with `--no-gitignore` because the generated input is intentionally ignored.

Do not commit the decompile or cite it as current after a game update. Preserve
the old manifest long enough to compare hashes and target signatures.
