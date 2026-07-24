# Documentation inventory automation

`Export-AgmlibInventory.ps1` analyzes every non-ignored repository file and
generates source, component, patch, reflection, namespace, and tracked-binary
inventories under `planning/generated`.

`Export-TodoPlan.ps1` maps every unchecked root TODO item to a work package and
completion-evidence rule.

`Export-NebulousKnowledgeInventory.ps1` analyzes the ignored, hash-pinned
native decompile and generates the exhaustive subsystem/file research indexes
under `knowledge/generated`.

`Export-AgmlibDataTouchpoints.ps1` maps every AGMLIB C# file to pinned native
type names and the vanilla prefab/raw serialized evidence indexed for those
types. It is a lexical navigation aid, not behavioral proof.

```powershell
.\scripts\Documentation\Export-AgmlibInventory.ps1
.\scripts\Documentation\Export-AgmlibInventory.ps1 -Check
.\scripts\Documentation\Export-TodoPlan.ps1
.\scripts\Documentation\Export-TodoPlan.ps1 -Check
.\scripts\Documentation\Export-NebulousKnowledgeInventory.ps1
.\scripts\Documentation\Export-NebulousKnowledgeInventory.ps1 -Check
.\scripts\Documentation\Export-AgmlibDataTouchpoints.ps1
.\scripts\Documentation\Export-AgmlibDataTouchpoints.ps1 -Check
```

Generated output must be deterministic. Do not add timestamps or local absolute
paths to committed inventory content. Structural counts are navigation aids;
compiled metadata, native assembly inspection, prefab evidence, and runtime
fixtures remain authoritative for compatibility and behavior.
