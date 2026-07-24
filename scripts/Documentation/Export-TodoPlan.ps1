[CmdletBinding()]
param(
    [string]$TodoPath,

    [string]$OutputPath,

    [switch]$Check
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
if ([string]::IsNullOrWhiteSpace($TodoPath))
{
    $TodoPath = Join-Path $repositoryRoot 'TODO.md'
}
if ([string]::IsNullOrWhiteSpace($OutputPath))
{
    $OutputPath = Join-Path $repositoryRoot 'planning\generated\todo-plan-matrix.md'
}

$TodoPath = [IO.Path]::GetFullPath($TodoPath)
$OutputPath = [IO.Path]::GetFullPath($OutputPath)
$planningPrefix = [IO.Path]::GetFullPath((Join-Path $repositoryRoot 'planning')).TrimEnd('\', '/') +
    [IO.Path]::DirectorySeparatorChar
if (-not $OutputPath.StartsWith($planningPrefix, [StringComparison]::OrdinalIgnoreCase))
{
    throw "OutputPath must stay inside the planning workspace: $planningPrefix"
}
if (-not (Test-Path -LiteralPath $TodoPath -PathType Leaf))
{
    throw "TODO file was not found: $TodoPath"
}

function Get-WorkPackage
{
    param([string]$Section, [string]$Item)

    switch -Regex ($Section)
    {
        '^Working Principles$' { return 'G0 governance gate' }
        'Reproducible' { return 'B1 build isolation and versioning' }
        'CI and Release' { return 'B2 CI and GitHub release' }
        'Agent Guidance' { return 'D1 documentation routing' }
        'Workshop' { return 'R2 Workshop publishing skill' }
        'Public API' { return 'C1 compatibility baseline' }
        'Component Documentation' { return 'D2 component catalog and audit' }
        'Native Runtime' { return 'N1 native boundary verification' }
        'Gameplay Correctness' { return 'F1 native/data gameplay parity' }
        'Namespace and Harmony' { return 'N2 namespace and patch governance' }
        'Tests and CI-Safe' { return 'T1 test and fixture architecture' }
        'Runtime Lifecycle' { return 'T2 lifecycle and performance audit' }
        'Packaging and Release' { return 'R1 package validation' }
        'Dependency and Binary' { return 'B3 dependency provenance' }
        'Source and Project' { return 'A1 source consolidation' }
        'Line Endings' { return 'B4 repository hygiene' }
        'Optional Documentation' { return 'D3 generated navigation review' }
        'Completion Definition' { return 'G1 final release-readiness gate' }
        default { return 'Unclassified - assign before implementation' }
    }
}

function Get-Evidence
{
    param([string]$Section, [string]$Item)

    switch -Regex ($Item)
    {
        'version|Version|semantic' { return 'One explicit version is shown to match assembly, manifest, package, and release metadata.' }
        'build|restore|Debug|Release' { return 'Documented clean command succeeds and `git status --porcelain` is unchanged.' }
        'deploy|game installation|launch|NEBULOUS is running|locked' { return 'Opt-in local scenario records paths, process state, hashes, result, and recovery.' }
        'workflow|GitHub Actions|permissions|concurrency|release job|artifact' { return 'Workflow dry run or Actions run proves permissions, trigger, artifact, failure, and concurrency behavior.' }
        'Workshop|Steam|publish|upload|item ID|rollback|clean client' { return 'Non-publishing test covers validation; authorized publish evidence records sanitized manifest and post-publish verification.' }
        'public|serialized|enum|save key|namespace|compatibility|migration|deprecation' { return 'Known-good assembly/fixture baseline diff is clean or an explicit migration record and fixture proves the change.' }
        'component|lifecycle|pool|clone|instantiate|scene transition|teardown' { return 'Component record and applicable lifecycle fixture include result, authority, logs, and follow-up disposition.' }
        'Harmony|native|reflection|Internals|member|game versions|assembly hashes' { return 'Pinned assembly verifier identifies owner and exact target; focused runtime evidence confirms the call path.' }
        'test|fixture|assertion|smoke' { return 'Deterministic test is repeatable, opt-in where runtime is required, and fails for the intended regression.' }
        'package|allowlist|manifest|SHA-256|last known-good' { return 'Disposable package validation records allowlist, layout, hashes, and rollback artifact.' }
        'assemblies|binary|dependency|license|provenance|UnityEditor|reference' { return 'Dependency manifest records source, version, license, hash, necessity, and clean-build result.' }
        'line ending|CRLF|mixed' { return 'Changed-file checker passes and attributes/editor settings agree.' }
        'AGENTS|documentation|repository map|commands|instructions|llms' { return 'Task-routed document is linked, checked for freshness, and does not duplicate canonical detail.' }
        'largest source|EntryPoint|Legacy|experimental|boundaries|helpers' { return 'Compatibility review and focused tests precede extraction; public identities remain or receive shims.' }
        default { return "The owning work-package checklist records an artifact, command/result, reviewer, and rollback or follow-up." }
    }
}

$section = ''
$counter = 0
$rows = New-Object System.Collections.Generic.List[object]
foreach ($line in Get-Content -LiteralPath $TodoPath)
{
    if ($line -match '^##\s+(.+)$')
    {
        $section = $Matches[1]
        continue
    }
    if ($line -notmatch '^(?<indent>\s*)-\s+\[\s\]\s+(?<item>.+)$')
    {
        continue
    }

    $counter++
    $item = $Matches['item'].Trim()
    $depth = [math]::Floor($Matches['indent'].Length / 2)
    $rows.Add([pscustomobject][ordered]@{
        id = 'T{0:D3}' -f $counter
        section = $section
        depth = $depth
        item = $item
        workPackage = Get-WorkPackage -Section $section -Item $item
        evidence = Get-Evidence -Section $section -Item $item
    })
}

$builder = New-Object Text.StringBuilder
[void]$builder.AppendLine('# Exhaustive TODO execution matrix')
[void]$builder.AppendLine()
[void]$builder.AppendLine("This generated matrix assigns all $($rows.Count) unchecked items in `TODO.md` to a work package and completion-evidence rule. IDs are positional and should be regenerated when the source TODO changes.")
[void]$builder.AppendLine()
[void]$builder.AppendLine('Run `.\scripts\Documentation\Export-TodoPlan.ps1 -Check` to detect drift.')
[void]$builder.AppendLine()
foreach ($group in $rows | Group-Object section)
{
    [void]$builder.AppendLine("## $($group.Name)")
    [void]$builder.AppendLine()
    [void]$builder.AppendLine('| ID | Depth | TODO | Work package | Required completion evidence |')
    [void]$builder.AppendLine('|---|---:|---|---|---|')
    foreach ($row in $group.Group)
    {
        [void]$builder.AppendLine((
            '| {0} | {1} | {2} | {3} | {4} |' -f
                $row.id,
                $row.depth,
                ($row.item -replace '\|', '/'),
                $row.workPackage,
                $row.evidence
        ))
    }
    [void]$builder.AppendLine()
}

$normalized = $builder.ToString().TrimEnd() + [Environment]::NewLine
if ($Check)
{
    if (-not (Test-Path -LiteralPath $OutputPath -PathType Leaf))
    {
        throw "Missing generated TODO plan: $OutputPath"
    }
    if ((Get-Content -Raw -LiteralPath $OutputPath) -ne $normalized)
    {
        throw "Generated TODO plan is stale: $OutputPath"
    }
}
else
{
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $OutputPath) | Out-Null
    $normalized | Set-Content -LiteralPath $OutputPath -Encoding UTF8 -NoNewline
}

[pscustomobject][ordered]@{
    valid = $true
    check = [bool]$Check
    todoCount = $rows.Count
    outputPath = $OutputPath
} | ConvertTo-Json
