[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$CatalogPath,

    [string]$ModIds,

    [string]$ReportPath
)

$ErrorActionPreference = 'Stop'
$CatalogPath = [IO.Path]::GetFullPath($CatalogPath)
if (-not (Test-Path -LiteralPath $CatalogPath -PathType Leaf))
{
    throw "Workshop compatibility catalog was not found at '$CatalogPath'."
}

$catalog = Get-Content -LiteralPath $CatalogPath -Raw | ConvertFrom-Json
if ([int]$catalog.schema_version -ne 1)
{
    throw "Unsupported Workshop compatibility catalog schema '$($catalog.schema_version)'."
}
if ([string]$catalog.game_app_id -notmatch '^[0-9]+$' -or
    [string]$catalog.agmlib_workshop_id -notmatch '^[0-9]+$')
{
    throw 'The catalog game and AGMLIB Workshop IDs must be unsigned integers.'
}

$catalogMods = @($catalog.mods)
if ($catalogMods.Count -eq 0)
{
    throw 'The Workshop compatibility catalog does not contain any mods.'
}

$knownIds = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
$knownSlugs = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
foreach ($mod in $catalogMods)
{
    $id = [string]$mod.id
    $name = [string]$mod.name
    $slug = [string]$mod.slug
    if ($id -notmatch '^[0-9]+$')
    {
        throw "Catalog mod ID '$id' is not an unsigned integer."
    }
    if ([string]::IsNullOrWhiteSpace($name))
    {
        throw "Catalog mod '$id' does not have a name."
    }
    if ($slug -notmatch '^[a-z0-9]+(?:-[a-z0-9]+)*$')
    {
        throw "Catalog mod '$id' has invalid artifact slug '$slug'."
    }
    if (-not $knownIds.Add($id))
    {
        throw "Catalog mod ID '$id' is duplicated."
    }
    if (-not $knownSlugs.Add($slug))
    {
        throw "Catalog artifact slug '$slug' is duplicated."
    }

    $ciPriority = if ($null -eq $mod.ci_priority)
    {
        'normal'
    }
    else
    {
        [string]$mod.ci_priority
    }
    if ($ciPriority -notin @('normal', 'low'))
    {
        throw "Catalog mod '$id' has unsupported CI priority '$ciPriority'."
    }

    $supportStatus = if ($null -eq $mod.support_status)
    {
        'compatibility-tested'
    }
    else
    {
        [string]$mod.support_status
    }
    if ($supportStatus -notin @('compatibility-tested', 'out-of-support'))
    {
        throw "Catalog mod '$id' has unsupported support status '$supportStatus'."
    }

    $defaultEnabled = $null -eq $mod.default_enabled -or [bool]$mod.default_enabled
    if ($supportStatus -eq 'out-of-support' -and $defaultEnabled)
    {
        throw "Out-of-support catalog mod '$id' must be disabled in the default matrix."
    }

    foreach ($dependency in @($mod.dependencies))
    {
        $dependencyId = [string]$dependency
        if ($dependencyId -notmatch '^[0-9]+$')
        {
            throw "Catalog mod '$id' has invalid dependency ID '$dependencyId'."
        }
        if ($dependencyId -eq $id)
        {
            throw "Catalog mod '$id' cannot depend on itself."
        }
    }
}

$selectedIds = @()
if (-not [string]::IsNullOrWhiteSpace($ModIds))
{
    $selectedIds = @(
        $ModIds -split '[,;\s]+' |
            Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
            Select-Object -Unique
    )
    foreach ($selectedId in $selectedIds)
    {
        if ($selectedId -notmatch '^[0-9]+$')
        {
            throw "Requested mod ID '$selectedId' is not an unsigned integer."
        }
        if (-not $knownIds.Contains($selectedId))
        {
            throw "Requested mod ID '$selectedId' is not in the compatibility catalog."
        }
    }
}

$selectedMods = @(
    $catalogMods |
        Where-Object {
            if ($selectedIds.Count -gt 0)
            {
                return [string]$_.id -in $selectedIds
            }
            return $null -eq $_.default_enabled -or [bool]$_.default_enabled
        }
)
if ($selectedMods.Count -eq 0)
{
    throw 'The Workshop compatibility selection is empty.'
}

$matrixEntries = foreach ($mod in $selectedMods)
{
    $installIds = [Collections.Generic.List[string]]::new()
    $seenInstallIds = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
    foreach ($installId in @(
        [string]$catalog.agmlib_workshop_id
        @($mod.dependencies | ForEach-Object { [string]$_ })
        [string]$mod.id
    ))
    {
        if ($seenInstallIds.Add($installId))
        {
            $installIds.Add($installId)
        }
    }

    [ordered]@{
        id = [string]$mod.id
        name = [string]$mod.name
        slug = [string]$mod.slug
        install_ids = $installIds -join ','
        ci_priority = if ($null -eq $mod.ci_priority) { 'normal' } else { [string]$mod.ci_priority }
        support_status = if ($null -eq $mod.support_status)
        {
            'compatibility-tested'
        }
        else
        {
            [string]$mod.support_status
        }
        known_failure_code = if ($null -eq $mod.known_failure)
        {
            ''
        }
        else
        {
            [string]$mod.known_failure.code
        }
    }
}

$matrix = [ordered]@{
    include = @($matrixEntries)
}
$report = [ordered]@{
    schema_version = 1
    generated_utc = [DateTime]::UtcNow.ToString('o')
    catalog_path = $CatalogPath
    game_app_id = [string]$catalog.game_app_id
    agmlib_workshop_id = [string]$catalog.agmlib_workshop_id
    requested_mod_ids = @($selectedIds)
    selected_mod_count = $selectedMods.Count
    matrix = $matrix
}

if (-not [string]::IsNullOrWhiteSpace($ReportPath))
{
    $ReportPath = [IO.Path]::GetFullPath($ReportPath)
    New-Item -ItemType Directory -Path (Split-Path -Parent $ReportPath) -Force | Out-Null
    $report | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $ReportPath -Encoding utf8
}

Write-Output ($matrix | ConvertTo-Json -Depth 6 -Compress)
