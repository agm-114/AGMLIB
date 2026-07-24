[CmdletBinding()]
param(
    [string]$OutputRoot,

    [switch]$Check
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
if ([string]::IsNullOrWhiteSpace($OutputRoot))
{
    $OutputRoot = Join-Path $repositoryRoot 'planning\generated'
}

$OutputRoot = [IO.Path]::GetFullPath($OutputRoot)
$planningRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'planning'))
$planningPrefix = $planningRoot.TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
if (-not $OutputRoot.StartsWith($planningPrefix, [StringComparison]::OrdinalIgnoreCase))
{
    throw "OutputRoot must stay inside the planning workspace: $planningRoot"
}

function Get-RelativePath
{
    param([string]$Path)

    return [IO.Path]::GetFullPath($Path).Substring($repositoryRoot.Length + 1)
}

function Get-TextOrEmpty
{
    param([string]$Path)

    $content = Get-Content -Raw -LiteralPath $Path -ErrorAction SilentlyContinue
    if ($null -eq $content)
    {
        return ''
    }

    return $content
}

function Get-FilePurpose
{
    param(
        [string]$RelativePath,
        [string[]]$TypeNames
    )

    if ($RelativePath -like '*.cs')
    {
        if ($TypeNames.Count -gt 0)
        {
            return "Defines $($TypeNames -join ', ')."
        }

        return 'C# source without a top-level type detected by the inventory parser.'
    }

    switch -Regex ($RelativePath)
    {
        '\.csproj$' { return 'MSBuild project configuration and dependency surface.' }
        '\.sln$' { return 'Visual Studio solution definition.' }
        '\.ya?ml$' { return 'YAML configuration or generated evidence.' }
        '\.json$' { return 'Structured configuration or metadata.' }
        '\.xml$' { return 'XML configuration, manifest, or serialized content.' }
        '\.ps1$' { return 'PowerShell automation or diagnostic workflow.' }
        '\.md$' { return 'Documentation, instructions, or planning material.' }
        '\.dll$' { return 'Tracked binary dependency; inspect provenance and version manifest.' }
        default { return 'Repository asset or configuration file.' }
    }
}

$paths = @(
    git -C $repositoryRoot ls-files --cached --others --exclude-standard |
        Where-Object {
            $_ -notlike '.native-code/*' -and
            $_ -notlike 'planning/generated/*' -and
            $_ -notmatch '(^|/)(bin|obj|\.vs)/'
        } |
        Sort-Object -Unique
)

$records = New-Object System.Collections.Generic.List[object]
$sourceRecords = New-Object System.Collections.Generic.List[object]
foreach ($relativePath in $paths)
{
    $fullPath = Join-Path $repositoryRoot ($relativePath -replace '/', '\')
    if (-not (Test-Path -LiteralPath $fullPath -PathType Leaf))
    {
        continue
    }

    $file = Get-Item -LiteralPath $fullPath
    $typeRecords = @()
    $namespaces = @()
    $harmonyTargets = @()
    $lifecycleMethods = @()
    $serializedFieldCount = 0
    $reflectionCount = 0
    $logCount = 0
    $todoCount = 0
    $lineCount = 0

    if ($file.Extension -eq '.cs')
    {
        $content = Get-TextOrEmpty $file.FullName
        $lineCount = if ($content.Length -eq 0) { 0 } else { ($content -split "`n").Count }
        $namespaces = @(
            [regex]::Matches($content, '(?m)^\s*namespace\s+([A-Za-z_][A-Za-z0-9_.]*)') |
                ForEach-Object { $_.Groups[1].Value } |
                Sort-Object -Unique
        )

        $typeMatches = [regex]::Matches(
            $content,
            '(?m)^\s*(?<access>public|internal|private|protected)?\s*(?:(?:sealed|abstract|static|partial|readonly|ref)\s+)*(?<kind>class|struct|interface|enum|record)\s+(?<name>[A-Za-z_][A-Za-z0-9_]*)(?:\s*:\s*(?<bases>[^{\r\n]+))?'
        )
        $typeRecords = @(
            $typeMatches | ForEach-Object {
                [pscustomobject][ordered]@{
                    access = $_.Groups['access'].Value
                    kind = $_.Groups['kind'].Value
                    name = $_.Groups['name'].Value
                    bases = $_.Groups['bases'].Value.Trim()
                }
            }
        )

        $harmonyTargets = @(
            [regex]::Matches($content, '(?m)^\s*\[HarmonyPatch(?<arguments>[^\]]*)\]') |
                ForEach-Object { $_.Groups['arguments'].Value.Trim('(', ')', ' ') }
        )
        $lifecycleMethods = @(
            [regex]::Matches(
                $content,
                '(?m)^\s*(?:(?:public|internal|private|protected)\s+)?(?:static\s+)?(?:void|IEnumerator|Task(?:<[^>]+>)?)\s+(Awake|Start|OnEnable|OnDisable|OnDestroy|Update|FixedUpdate|LateUpdate|OnAdded|OnUnpooled|OnPooled|OnLaunched|OnSpawnServer|OnStartServer|OnStartClient|OnStopServer|OnStopClient)\s*\('
            ) |
                ForEach-Object { $_.Groups[1].Value } |
                Sort-Object -Unique
        )
        $serializedFieldCount = ([regex]::Matches($content, '\[SerializeField\]')).Count
        $reflectionCount = ([regex]::Matches(
            $content,
            'Common\.(?:GetVal|SetVal|RunFunc)|AccessTools\.(?:Field|Property|Method)|Traverse\.|(?:FieldInfo|PropertyInfo|MethodInfo)'
        )).Count
        $logCount = ([regex]::Matches(
            $content,
            '(?:Debug\.(?:Log|LogWarning|LogError)|Common\.(?:Trace|LogPatch))\s*\('
        )).Count
        $todoCount = ([regex]::Matches($content, '(?i)\b(?:TODO|FIXME|HACK|WIP)\b')).Count
    }

    $record = [pscustomobject][ordered]@{
        path = $relativePath
        extension = $file.Extension
        bytes = $file.Length
        lines = $lineCount
        purpose = Get-FilePurpose $relativePath @($typeRecords.name)
        namespaces = $namespaces
        types = $typeRecords
        harmonyTargets = $harmonyTargets
        lifecycleMethods = $lifecycleMethods
        serializedFieldCount = $serializedFieldCount
        reflectionCount = $reflectionCount
        logCount = $logCount
        todoCount = $todoCount
    }
    $records.Add($record)
    if ($file.Extension -eq '.cs')
    {
        $sourceRecords.Add($record)
    }
}

$components = @(
    foreach ($source in $sourceRecords)
    {
        foreach ($type in $source.types)
        {
            if ($type.bases -match '\b(MonoBehaviour|ScriptableObject|NetworkBehaviour|HullComponent|WeaponComponent|Munition|Missile|EffectModule|ShipState|Descriptor)\b')
            {
                [pscustomobject][ordered]@{
                    type = $type.name
                    bases = $type.bases
                    path = $source.path
                    namespace = ($source.namespaces -join ', ')
                    serializedFieldCount = $source.serializedFieldCount
                    lifecycleMethods = $source.lifecycleMethods
                }
            }
        }
    }
)

$patches = @(
    foreach ($source in $sourceRecords)
    {
        foreach ($target in $source.harmonyTargets)
        {
            [pscustomobject][ordered]@{
                path = $source.path
                target = $target
                reflectionCount = $source.reflectionCount
            }
        }
    }
)

$reflectionFiles = @(
    $sourceRecords |
        Where-Object { $_.reflectionCount -gt 0 } |
        Sort-Object reflectionCount -Descending
)

$binaryDependencies = @(
    foreach ($record in $records | Where-Object { $_.extension -eq '.dll' } | Sort-Object path)
    {
        $binaryPath = Join-Path $repositoryRoot ($record.path -replace '/', '\')
        $assemblyVersion = ''
        try
        {
            $assemblyVersion = [Reflection.AssemblyName]::GetAssemblyName($binaryPath).Version.ToString()
        }
        catch
        {
            $assemblyVersion = '<not a managed assembly>'
        }
        $versionInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($binaryPath)
        [pscustomobject][ordered]@{
            path = $record.path
            bytes = $record.bytes
            sha256 = (Get-FileHash -LiteralPath $binaryPath -Algorithm SHA256).Hash
            assemblyVersion = $assemblyVersion
            fileVersion = $versionInfo.FileVersion
            productVersion = $versionInfo.ProductVersion
        }
    }
)

$json = [pscustomobject][ordered]@{
    schemaVersion = 1
    sourceRevision = (git -C $repositoryRoot rev-parse HEAD)
    repositoryFileCount = $records.Count
    sourceFileCount = $sourceRecords.Count
    componentCount = $components.Count
    harmonyPatchAttributeCount = $patches.Count
    binaryDependencyCount = $binaryDependencies.Count
    binaryDependencies = $binaryDependencies
    files = @($records | ForEach-Object { $_ })
} | ConvertTo-Json -Depth 10

$fileInventory = New-Object Text.StringBuilder
[void]$fileInventory.AppendLine('# Repository file inventory')
[void]$fileInventory.AppendLine()
[void]$fileInventory.AppendLine("Generated from $($records.Count) repository files. Generated output and ignored native-code dumps are excluded.")
[void]$fileInventory.AppendLine()
foreach ($group in $records | Group-Object { ($_.path -split '/')[0] })
{
    [void]$fileInventory.AppendLine("## $($group.Name)")
    [void]$fileInventory.AppendLine()
    foreach ($record in $group.Group | Sort-Object path)
    {
        $typeSummary = if ($record.types.Count -gt 0)
        {
            " Types: $($record.types.name -join ', ')."
        }
        else
        {
            ''
        }
        [void]$fileInventory.AppendLine(('- `{0}` - {1}{2}' -f $record.path, $record.purpose, $typeSummary))
    }
    [void]$fileInventory.AppendLine()
}

$sourceAtlas = New-Object Text.StringBuilder
[void]$sourceAtlas.AppendLine('# Source file atlas')
[void]$sourceAtlas.AppendLine()
[void]$sourceAtlas.AppendLine('Every C# source file is listed with the structural signals used to prioritize review. Counts are navigation aids, not proof of runtime behavior.')
[void]$sourceAtlas.AppendLine()
foreach ($group in $sourceRecords | Group-Object {
    $segments = $_.path -split '/'
    if ($segments[0] -eq 'AGMLIB' -and $segments.Count -gt 2)
    {
        return $segments[1]
    }

    return $segments[0]
})
{
    [void]$sourceAtlas.AppendLine("## $($group.Name)")
    [void]$sourceAtlas.AppendLine()
    [void]$sourceAtlas.AppendLine('| Source | Lines | Types | Namespace | Serialized | Lifecycle | Patches | Reflection | Logs | TODO markers |')
    [void]$sourceAtlas.AppendLine('|---|---:|---|---|---:|---|---:|---:|---:|---:|')
    foreach ($source in $group.Group | Sort-Object path)
    {
        $typeSummary = @(
            $source.types | ForEach-Object {
                if ([string]::IsNullOrWhiteSpace($_.bases))
                {
                    return $_.name
                }

                return "$($_.name) : $($_.bases)"
            }
        ) -join '<br>'
        [void]$sourceAtlas.AppendLine((
            '| `{0}` | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} | {9} |' -f
                $source.path,
                $source.lines,
                ($typeSummary -replace '\|', '/'),
                (($source.namespaces -join '<br>') -replace '\|', '/'),
                $source.serializedFieldCount,
                ($source.lifecycleMethods -join ', '),
                $source.harmonyTargets.Count,
                $source.reflectionCount,
                $source.logCount,
                $source.todoCount
        ))
    }
    [void]$sourceAtlas.AppendLine()
}

$namespaceInventory = New-Object Text.StringBuilder
[void]$namespaceInventory.AppendLine('# Namespace inventory')
[void]$namespaceInventory.AppendLine()
[void]$namespaceInventory.AppendLine('This inventory exposes global-namespace and mixed-namespace compatibility risks. A missing namespace is not permission to move a released type.')
[void]$namespaceInventory.AppendLine()
[void]$namespaceInventory.AppendLine('| Namespace | Files | Declared types | Public types |')
[void]$namespaceInventory.AppendLine('|---|---:|---:|---:|')
$namespaceRows = @(
    foreach ($source in $sourceRecords)
    {
        $declaredNamespaces = if ($source.namespaces.Count -eq 0) { @('<global>') } else { $source.namespaces }
        foreach ($namespace in $declaredNamespaces)
        {
            [pscustomobject]@{
                namespace = $namespace
                path = $source.path
                typeCount = $source.types.Count
                publicTypeCount = @($source.types | Where-Object { $_.access -eq 'public' }).Count
            }
        }
    }
)
foreach ($group in $namespaceRows | Group-Object namespace | Sort-Object Name)
{
    [void]$namespaceInventory.AppendLine((
        '| `{0}` | {1} | {2} | {3} |' -f
            $group.Name,
            @($group.Group.path | Sort-Object -Unique).Count,
            ($group.Group | Measure-Object typeCount -Sum).Sum,
            ($group.Group | Measure-Object publicTypeCount -Sum).Sum
    ))
}

$componentInventory = New-Object Text.StringBuilder
[void]$componentInventory.AppendLine('# Component inventory')
[void]$componentInventory.AppendLine()
[void]$componentInventory.AppendLine("Detected $($components.Count) Unity/game component-like public and internal types.")
[void]$componentInventory.AppendLine()
[void]$componentInventory.AppendLine('| Type | Base/interface surface | Source | Serialized fields | Lifecycle methods |')
[void]$componentInventory.AppendLine('|---|---|---|---:|---|')
foreach ($component in $components | Sort-Object type, path)
{
    [void]$componentInventory.AppendLine((
        '| `{0}` | `{1}` | `{2}` | {3} | {4} |' -f
            $component.type,
            ($component.bases -replace '\|', '/'),
            $component.path,
            $component.serializedFieldCount,
            ($component.lifecycleMethods -join ', ')
    ))
}

$patchInventory = New-Object Text.StringBuilder
[void]$patchInventory.AppendLine('# Harmony patch inventory')
[void]$patchInventory.AppendLine()
[void]$patchInventory.AppendLine(('Detected {0} active-looking `HarmonyPatch` attribute declarations. Verify overloads against the current native assembly.' -f $patches.Count))
[void]$patchInventory.AppendLine()
[void]$patchInventory.AppendLine('| Source | Declared target arguments | Reflection references in file |')
[void]$patchInventory.AppendLine('|---|---|---:|')
foreach ($patch in $patches | Sort-Object path, target)
{
    [void]$patchInventory.AppendLine((
        '| `{0}` | `{1}` | {2} |' -f
            $patch.path,
            ($patch.target -replace '\|', '/'),
            $patch.reflectionCount
    ))
}

$reflectionInventory = New-Object Text.StringBuilder
[void]$reflectionInventory.AppendLine('# Native reflection migration inventory')
[void]$reflectionInventory.AppendLine()
[void]$reflectionInventory.AppendLine('Files are ordered by the number of known reflection-boundary tokens. Each known native member should move incrementally to a typed `Internals()` accessor when its owning function is changed.')
[void]$reflectionInventory.AppendLine()
[void]$reflectionInventory.AppendLine('| Source | Reflection-boundary tokens | Harmony attributes |')
[void]$reflectionInventory.AppendLine('|---|---:|---:|')
foreach ($source in $reflectionFiles)
{
    [void]$reflectionInventory.AppendLine((
        '| `{0}` | {1} | {2} |' -f
            $source.path,
            $source.reflectionCount,
            $source.harmonyTargets.Count
    ))
}

$binaryInventory = New-Object Text.StringBuilder
[void]$binaryInventory.AppendLine('# Tracked binary inventory')
[void]$binaryInventory.AppendLine()
[void]$binaryInventory.AppendLine("Detected $($binaryDependencies.Count) tracked DLLs. Hashes identify the current repository content; source, license, and redistribution decisions still require maintainer review.")
[void]$binaryInventory.AppendLine()
[void]$binaryInventory.AppendLine('| Path | Bytes | Assembly version | File version | SHA-256 |')
[void]$binaryInventory.AppendLine('|---|---:|---|---|---|')
foreach ($binary in $binaryDependencies)
{
    [void]$binaryInventory.AppendLine((
        '| `{0}` | {1} | `{2}` | `{3}` | `{4}` |' -f
            $binary.path,
            $binary.bytes,
            $binary.assemblyVersion,
            $binary.fileVersion,
            $binary.sha256
    ))
}

$outputs = [ordered]@{
    'repository-inventory.json' = $json
    'file-inventory.md' = $fileInventory.ToString()
    'source-file-atlas.md' = $sourceAtlas.ToString()
    'component-inventory.md' = $componentInventory.ToString()
    'harmony-patch-inventory.md' = $patchInventory.ToString()
    'reflection-migration-inventory.md' = $reflectionInventory.ToString()
    'namespace-inventory.md' = $namespaceInventory.ToString()
    'binary-inventory.md' = $binaryInventory.ToString()
}

if (-not $Check)
{
    New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null
}

$failures = New-Object System.Collections.Generic.List[string]
foreach ($entry in $outputs.GetEnumerator())
{
    $path = Join-Path $OutputRoot $entry.Key
    $normalized = $entry.Value.TrimEnd() + [Environment]::NewLine
    if ($Check)
    {
        if (-not (Test-Path -LiteralPath $path -PathType Leaf))
        {
            $failures.Add("Missing generated file: $path")
            continue
        }

        $existing = Get-Content -Raw -LiteralPath $path
        if ($existing -ne $normalized)
        {
            $failures.Add("Generated file is stale: $path")
        }
    }
    else
    {
        $normalized | Set-Content -LiteralPath $path -Encoding UTF8 -NoNewline
    }
}

if ($failures.Count -gt 0)
{
    $failures | ForEach-Object { Write-Error $_ }
    exit 1
}

[pscustomobject][ordered]@{
    valid = $true
    check = [bool]$Check
    outputRoot = $OutputRoot
    repositoryFileCount = $records.Count
    sourceFileCount = $sourceRecords.Count
    componentCount = $components.Count
    harmonyPatchAttributeCount = $patches.Count
    reflectionFileCount = $reflectionFiles.Count
    binaryDependencyCount = $binaryDependencies.Count
} | ConvertTo-Json
