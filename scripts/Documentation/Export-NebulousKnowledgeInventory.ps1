[CmdletBinding()]
param(
    [string]$NativeRoot,

    [string]$OutputRoot,

    [switch]$Check
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$knowledgeRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'knowledge'))

if ([string]::IsNullOrWhiteSpace($NativeRoot))
{
    $NativeRoot = Join-Path $repositoryRoot '.native-code\current'
}
if ([string]::IsNullOrWhiteSpace($OutputRoot))
{
    $OutputRoot = Join-Path $knowledgeRoot 'generated'
}

$NativeRoot = [IO.Path]::GetFullPath($NativeRoot)
$OutputRoot = [IO.Path]::GetFullPath($OutputRoot)
$knowledgePrefix = $knowledgeRoot.TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
if (-not $OutputRoot.StartsWith($knowledgePrefix, [StringComparison]::OrdinalIgnoreCase))
{
    throw "OutputRoot must stay inside the knowledge workspace: $knowledgeRoot"
}

$manifestPath = Join-Path $NativeRoot 'manifest.json'
if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf))
{
    throw "Native-code manifest was not found: $manifestPath"
}

$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
$nebulousAssembly = @($manifest.assemblies | Where-Object name -eq 'Nebulous.dll') | Select-Object -First 1
if ($null -eq $nebulousAssembly)
{
    throw 'Nebulous.dll is not recorded in the native-code manifest.'
}

$sourceRoot = [IO.Path]::GetFullPath($nebulousAssembly.decompiledPath)
if (-not (Test-Path -LiteralPath $sourceRoot -PathType Container))
{
    throw "Pinned Nebulous source directory was not found: $sourceRoot"
}

function Get-RelativeSourcePath
{
    param([string]$Path)

    return [IO.Path]::GetFullPath($Path).Substring($sourceRoot.Length + 1).Replace('\', '/')
}

function Escape-MarkdownCell
{
    param([AllowNull()][object]$Value)

    if ($null -eq $Value)
    {
        return ''
    }

    return ([string]$Value).Replace('|', '\|').Replace("`r", '').Replace("`n", '<br>')
}

function Get-DisplayBases
{
    param([string]$Bases)

    if ([string]::IsNullOrWhiteSpace($Bases))
    {
        return ''
    }

    $value = ($Bases -replace '\s+', ' ').Trim()
    if ($value.Length -gt 140)
    {
        return $value.Substring(0, 137) + '...'
    }
    return $value
}

function Get-SignalNames
{
    param([string]$Content)

    $signals = New-Object System.Collections.Generic.List[string]
    $checks = [ordered]@{
        'NetworkBehaviour' = '\bNetworkBehaviour\b'
        'SyncVar' = '\[SyncVar'
        'Command' = '\[Command'
        'ClientRpc' = '\[ClientRpc'
        'TargetRpc' = '\[TargetRpc'
        'NetworkServer' = '\bNetworkServer\b'
        'NetworkClient' = '\bNetworkClient\b'
        'save-state' = '\b(?:IBulkSaveComponent|SaveFileObject|SavedHullComponentStates|WriteSaveState|LoadSaveState|RestoreFromSaveState)\b'
        'XML' = '\bXml(?:Element|Document|Serializer)\b|AppendToDocument|ReadFromDocument'
        'JSON' = '\bJsonUtility\b|JsonConvert|System\.Text\.Json'
        'AssetBundle' = '\bAssetBundle\b'
        'Addressables' = '\bAddressable'
        'pooling' = '\b(?:OnUnpooled|OnRepooled|RepoolSelf|Poolable|NetworkPool)\b'
        'prefab' = '\b(?:Prefab|GameObject)\b'
    }

    foreach ($entry in $checks.GetEnumerator())
    {
        if ($Content -match $entry.Value)
        {
            $signals.Add($entry.Key)
        }
    }
    return @($signals)
}

$lifecyclePattern = '(?m)^\s*(?:(?:public|internal|private|protected)\s+)?(?:(?:static|virtual|override|abstract|sealed|new|async)\s+)*(?:void|IEnumerator|Task(?:<[^>]+>)?|UniTask(?:<[^>]+>)?)\s+(Awake|Start|OnEnable|OnDisable|OnDestroy|Update|FixedUpdate|LateUpdate|OnAdded|OnCloned|OnUnpooled|OnRepooled|OnLaunched|OnDead|OnSpawnServer|OnStartServer|OnStartClient|OnStopServer|OnStopClient|OnStartAuthority|OnStopAuthority)\s*\('
$typePattern = '(?m)^\s*(?<access>public|internal|private|protected)?\s*(?:(?:sealed|abstract|static|partial|readonly|ref|unsafe|new)\s+)*(?<kind>class|struct|interface|enum|record)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)(?:\s*<[^>{}\r\n]+>)?(?:\s*:\s*(?<bases>[^{\r\n]+))?'

$records = New-Object System.Collections.Generic.List[object]
foreach ($file in Get-ChildItem -LiteralPath $sourceRoot -Recurse -File -Filter '*.cs' | Sort-Object FullName)
{
    $content = Get-Content -Raw -LiteralPath $file.FullName
    if ($null -eq $content)
    {
        $content = ''
    }

    $relativePath = Get-RelativeSourcePath $file.FullName
    $segments = $relativePath -split '/'
    $area = if ($segments.Count -gt 1) { $segments[0] } else { '<root>' }
    $subsystem = if ($segments.Count -gt 2) { "$($segments[0])/$($segments[1])" } else { $area }
    $namespaces = @(
        [regex]::Matches($content, '(?m)^\s*namespace\s+([^;{\r\n]+)') |
            ForEach-Object { $_.Groups[1].Value.Trim() } |
            Sort-Object -Unique
    )
    $types = @(
        [regex]::Matches($content, $typePattern) |
            ForEach-Object {
                [pscustomobject][ordered]@{
                    access = $_.Groups['access'].Value
                    kind = $_.Groups['kind'].Value
                    name = $_.Groups['name'].Value
                    bases = (Get-DisplayBases $_.Groups['bases'].Value)
                }
            }
    )
    $lifecycle = @(
        [regex]::Matches($content, $lifecyclePattern) |
            ForEach-Object { $_.Groups[1].Value } |
            Sort-Object -Unique
    )
    $signals = @(Get-SignalNames $content)
    $lineCount = if ($content.Length -eq 0) { 0 } else { ($content -split "`n").Count }
    $methodCount = ([regex]::Matches(
        $content,
        '(?m)^\s*(?:(?:public|internal|private|protected)\s+)?(?:(?:static|virtual|override|abstract|sealed|new|async|extern|partial)\s+)*(?:[A-Za-z_][A-Za-z0-9_.<>\[\],?]*\s+)+[A-Za-z_][A-Za-z0-9_]*\s*\('
    )).Count

    $records.Add([pscustomobject][ordered]@{
        path = $relativePath
        area = $area
        subsystem = $subsystem
        bytes = $file.Length
        lines = $lineCount
        namespaces = $namespaces
        types = $types
        methods = $methodCount
        serializedFields = ([regex]::Matches($content, '\[SerializeField\]')).Count
        lifecycle = $lifecycle
        signals = $signals
    })
}

$typeIndexPath = Join-Path $NativeRoot 'indexes\Nebulous.types.txt'
$indexedTypes = @()
if (Test-Path -LiteralPath $typeIndexPath -PathType Leaf)
{
    $indexedTypes = @(
        Get-Content -LiteralPath $typeIndexPath |
            ForEach-Object {
                if ($_ -match '^(Class|Interface|Struct|Delegate|Enum)\s+(.+)$')
                {
                    [pscustomobject][ordered]@{
                        kind = $Matches[1]
                        fullName = $Matches[2]
                        compilerGenerated = $Matches[2] -match '[<>]'
                    }
                }
            }
    )
}

$areaRows = @(
    $records |
        Group-Object area |
        ForEach-Object {
            [pscustomobject][ordered]@{
                area = $_.Name
                files = $_.Count
                lines = ($_.Group | Measure-Object lines -Sum).Sum
                methods = ($_.Group | Measure-Object methods -Sum).Sum
                serializedFields = ($_.Group | Measure-Object serializedFields -Sum).Sum
                lifecycleFiles = @($_.Group | Where-Object { $_.lifecycle.Count -gt 0 }).Count
                networkFiles = @($_.Group | Where-Object { $_.signals -contains 'NetworkBehaviour' -or $_.signals -contains 'SyncVar' -or $_.signals -contains 'Command' -or $_.signals -contains 'ClientRpc' -or $_.signals -contains 'TargetRpc' }).Count
                saveFiles = @($_.Group | Where-Object { $_.signals -contains 'save-state' -or $_.signals -contains 'XML' -or $_.signals -contains 'JSON' }).Count
            }
        } |
        Sort-Object @{ Expression = 'files'; Descending = $true }, @{ Expression = 'area'; Descending = $false }
)

$subsystemRows = @(
    $records |
        Group-Object subsystem |
        ForEach-Object {
            [pscustomobject][ordered]@{
                subsystem = $_.Name
                files = $_.Count
                lines = ($_.Group | Measure-Object lines -Sum).Sum
                methods = ($_.Group | Measure-Object methods -Sum).Sum
                serializedFields = ($_.Group | Measure-Object serializedFields -Sum).Sum
                lifecycleFiles = @($_.Group | Where-Object { $_.lifecycle.Count -gt 0 }).Count
                networkFiles = @($_.Group | Where-Object { $_.signals -contains 'NetworkBehaviour' -or $_.signals -contains 'SyncVar' -or $_.signals -contains 'Command' -or $_.signals -contains 'ClientRpc' -or $_.signals -contains 'TargetRpc' }).Count
                saveFiles = @($_.Group | Where-Object { $_.signals -contains 'save-state' -or $_.signals -contains 'XML' -or $_.signals -contains 'JSON' }).Count
            }
        } |
        Sort-Object subsystem
)

$namespaceSource = @(
    foreach ($record in $records)
    {
        foreach ($namespace in $record.namespaces)
        {
            [pscustomobject]@{ namespace = $namespace; lines = $record.lines; path = $record.path }
        }
    }
)
$namespaceRows = @(
    $namespaceSource |
        Group-Object namespace |
        ForEach-Object {
            [pscustomobject][ordered]@{
                namespace = $_.Name
                files = @($_.Group.path | Sort-Object -Unique).Count
                lines = ($_.Group | Measure-Object lines -Sum).Sum
            }
        } |
        Sort-Object @{ Expression = 'files'; Descending = $true }, @{ Expression = 'namespace'; Descending = $false }
)

$inventory = New-Object Text.StringBuilder
[void]$inventory.AppendLine('# Native subsystem inventory')
[void]$inventory.AppendLine()
[void]$inventory.AppendLine('Generated from the pinned local `Nebulous.dll` decompile. Counts are navigation evidence, not runtime proof.')
[void]$inventory.AppendLine()
[void]$inventory.AppendLine("Assembly SHA-256: ``$($nebulousAssembly.sha256)``  ")
[void]$inventory.AppendLine("Decompiled C# files: $($records.Count)  ")
[void]$inventory.AppendLine("Approximate source lines: $(($records | Measure-Object lines -Sum).Sum)  ")
[void]$inventory.AppendLine("ILSpy indexed types: $($indexedTypes.Count) ($(@($indexedTypes | Where-Object { -not $_.compilerGenerated }).Count) excluding compiler-generated names)")
[void]$inventory.AppendLine()
[void]$inventory.AppendLine('## Top-level areas')
[void]$inventory.AppendLine()
[void]$inventory.AppendLine('| Area | Files | Lines | Methods | Serialized fields | Lifecycle files | Network files | Save/serialization files |')
[void]$inventory.AppendLine('|---|---:|---:|---:|---:|---:|---:|---:|')
foreach ($row in $areaRows)
{
    [void]$inventory.AppendLine("| $($row.area) | $($row.files) | $($row.lines) | $($row.methods) | $($row.serializedFields) | $($row.lifecycleFiles) | $($row.networkFiles) | $($row.saveFiles) |")
}

[void]$inventory.AppendLine()
[void]$inventory.AppendLine('## Subsystems')
[void]$inventory.AppendLine()
[void]$inventory.AppendLine('| Subsystem | Files | Lines | Methods | Serialized fields | Lifecycle files | Network files | Save/serialization files |')
[void]$inventory.AppendLine('|---|---:|---:|---:|---:|---:|---:|---:|')
foreach ($row in $subsystemRows)
{
    [void]$inventory.AppendLine("| $($row.subsystem) | $($row.files) | $($row.lines) | $($row.methods) | $($row.serializedFields) | $($row.lifecycleFiles) | $($row.networkFiles) | $($row.saveFiles) |")
}

[void]$inventory.AppendLine()
[void]$inventory.AppendLine('## Largest exact namespaces')
[void]$inventory.AppendLine()
[void]$inventory.AppendLine('| Namespace | Files | Lines |')
[void]$inventory.AppendLine('|---|---:|---:|')
foreach ($row in $namespaceRows | Select-Object -First 100)
{
    [void]$inventory.AppendLine("| $($row.namespace) | $($row.files) | $($row.lines) |")
}

function Add-HotspotTable
{
    param(
        [Text.StringBuilder]$Builder,
        [string]$Heading,
        [object[]]$Rows,
        [string]$Metric
    )

    [void]$Builder.AppendLine()
    [void]$Builder.AppendLine("## $Heading")
    [void]$Builder.AppendLine()
    [void]$Builder.AppendLine("| File | Lines | Types | $Metric |")
    [void]$Builder.AppendLine('|---|---:|---|---:|')
    foreach ($row in $Rows)
    {
        $types = Escape-MarkdownCell (($row.types | ForEach-Object name) -join ', ')
        [void]$Builder.AppendLine("| ``$($row.path)`` | $($row.lines) | $types | $($row.$Metric) |")
    }
}

Add-HotspotTable $inventory 'Largest source files' @($records | Sort-Object lines -Descending | Select-Object -First 50) 'methods'
Add-HotspotTable $inventory 'Serialized-field hotspots' @($records | Where-Object serializedFields -gt 0 | Sort-Object serializedFields -Descending | Select-Object -First 50) 'serializedFields'

$atlas = New-Object Text.StringBuilder
[void]$atlas.AppendLine('# Native source file atlas')
[void]$atlas.AppendLine()
[void]$atlas.AppendLine("Every decompiled C# file from pinned `Nebulous.dll` is listed. Generated from assembly SHA-256 ``$($nebulousAssembly.sha256)``.")
[void]$atlas.AppendLine()
[void]$atlas.AppendLine('| Source | Lines | Namespace | Declared types and bases | Lifecycle | Signals |')
[void]$atlas.AppendLine('|---|---:|---|---|---|---|')
foreach ($record in $records)
{
    $namespaces = Escape-MarkdownCell ($record.namespaces -join ', ')
    $types = Escape-MarkdownCell (($record.types | ForEach-Object {
        if ([string]::IsNullOrWhiteSpace($_.bases))
        {
            "$($_.kind) $($_.name)"
        }
        else
        {
            "$($_.kind) $($_.name) : $($_.bases)"
        }
    }) -join '<br>')
    $lifecycle = Escape-MarkdownCell ($record.lifecycle -join ', ')
    $signals = Escape-MarkdownCell ($record.signals -join ', ')
    [void]$atlas.AppendLine("| ``$($record.path)`` | $($record.lines) | $namespaces | $types | $lifecycle | $signals |")
}

$json = [pscustomobject][ordered]@{
    schemaVersion = 1
    assembly = [pscustomobject][ordered]@{
        name = $nebulousAssembly.name
        sha256 = $nebulousAssembly.sha256
        lastWriteTimeUtc = $nebulousAssembly.lastWriteTimeUtc
        sourceFileCount = $records.Count
        indexedTypeCount = $indexedTypes.Count
    }
    areas = $areaRows
    subsystems = $subsystemRows
    namespaces = $namespaceRows
    files = @($records | ForEach-Object { $_ })
    indexedTypes = @($indexedTypes | ForEach-Object { $_ })
} | ConvertTo-Json -Depth 10 -Compress

$outputs = [ordered]@{
    'native-subsystem-inventory.md' = $inventory.ToString()
    'native-source-file-atlas.md' = $atlas.ToString()
    'native-research-index.json' = $json
}

if ($Check)
{
    $differences = New-Object System.Collections.Generic.List[string]
    foreach ($entry in $outputs.GetEnumerator())
    {
        $path = Join-Path $OutputRoot $entry.Key
        if (-not (Test-Path -LiteralPath $path -PathType Leaf))
        {
            $differences.Add("$($entry.Key) is missing")
            continue
        }

        $existing = (Get-Content -Raw -LiteralPath $path).Replace("`r`n", "`n").TrimEnd("`n")
        $expected = ([string]$entry.Value).Replace("`r`n", "`n").TrimEnd("`n")
        if ($existing -ne $expected)
        {
            $differences.Add("$($entry.Key) is stale")
        }
    }

    if ($differences.Count -gt 0)
    {
        throw "Nebulous knowledge inventory check failed: $($differences -join '; ')"
    }

    Write-Host 'Nebulous knowledge inventory is current.'
    return
}

New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null
foreach ($entry in $outputs.GetEnumerator())
{
    Set-Content -LiteralPath (Join-Path $OutputRoot $entry.Key) -Value $entry.Value -Encoding UTF8
}

[pscustomobject][ordered]@{
    valid = $true
    assemblyHash = $nebulousAssembly.sha256
    sourceFiles = $records.Count
    indexedTypes = $indexedTypes.Count
    outputRoot = $OutputRoot
    outputs = @($outputs.Keys)
} | ConvertTo-Json -Depth 4
