[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$DownloadRoot,

    [Parameter(Mandatory)]
    [UInt64[]]$WorkshopItemIds,

    [Parameter(Mandatory)]
    [UInt64]$TargetWorkshopItemId,

    [Parameter(Mandatory)]
    [string]$OutputDirectory,

    [UInt64]$GameAppId = 887570
)

$ErrorActionPreference = 'Stop'
$DownloadRoot = [IO.Path]::GetFullPath($DownloadRoot)
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
if (-not (Test-Path -LiteralPath $DownloadRoot -PathType Container))
{
    throw "Workshop download root was not found at '$DownloadRoot'."
}
if ($WorkshopItemIds.Count -eq 0)
{
    throw 'At least one Workshop item ID is required.'
}
if ($TargetWorkshopItemId -notin $WorkshopItemIds)
{
    throw "Target Workshop item $TargetWorkshopItemId is not in the inspected item set."
}

$workshopRoot = Join-Path $DownloadRoot "steamapps/workshop/content/$GameAppId"
if (-not (Test-Path -LiteralPath $workshopRoot -PathType Container))
{
    throw "Workshop content root was not found at '$workshopRoot'."
}

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$manifestOutputRoot = Join-Path $OutputDirectory 'manifests'
New-Item -ItemType Directory -Path $manifestOutputRoot -Force | Out-Null
$treeLines = [Collections.Generic.List[string]]::new()
$items = @()

function Get-RelativeItemPath
{
    param(
        [Parameter(Mandatory)]
        [string]$ItemRoot,

        [Parameter(Mandatory)]
        [string]$Path
    )

    $rootPrefix = $ItemRoot.TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    if (-not $Path.StartsWith($rootPrefix, [StringComparison]::OrdinalIgnoreCase))
    {
        throw "Path '$Path' is not contained by Workshop item root '$ItemRoot'."
    }
    return $Path.Substring($rootPrefix.Length).Replace(
        [IO.Path]::DirectorySeparatorChar,
        '/')
}

foreach ($workshopItemId in $WorkshopItemIds | Select-Object -Unique)
{
    $itemRoot = [IO.Path]::GetFullPath((Join-Path $workshopRoot ([string]$workshopItemId)))
    if (-not (Test-Path -LiteralPath $itemRoot -PathType Container))
    {
        throw "Workshop item $workshopItemId was not found at '$itemRoot'."
    }

    $treeLines.Add("[$workshopItemId]")
    $files = @()
    $extensionSummary = @{}
    $totalBytes = [UInt64]0
    foreach ($file in Get-ChildItem -LiteralPath $itemRoot -File -Recurse | Sort-Object FullName)
    {
        $relativePath = Get-RelativeItemPath -ItemRoot $itemRoot -Path $file.FullName
        $extension = if ([string]::IsNullOrWhiteSpace($file.Extension))
        {
            '(none)'
        }
        else
        {
            $file.Extension.ToLowerInvariant()
        }
        if (-not $extensionSummary.ContainsKey($extension))
        {
            $extensionSummary[$extension] = 0
        }
        $extensionSummary[$extension]++
        $totalBytes += [UInt64]$file.Length
        $hash = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
        $files += [ordered]@{
            relative_path = $relativePath
            size_bytes = [UInt64]$file.Length
            sha256 = $hash
        }
        $treeLines.Add("$relativePath | $($file.Length) bytes | sha256=$hash")
    }

    $manifestSummaries = @()
    foreach ($manifestFile in Get-ChildItem -LiteralPath $itemRoot -Filter 'ModInfo.xml' -File -Recurse |
        Sort-Object FullName)
    {
        $relativeManifestPath = Get-RelativeItemPath `
            -ItemRoot $itemRoot `
            -Path $manifestFile.FullName
        [xml]$manifest = Get-Content -LiteralPath $manifestFile.FullName -Raw
        $modNameNode = $manifest.SelectSingleNode('/ModInfo/ModName')
        if ($null -eq $modNameNode -or [string]::IsNullOrWhiteSpace($modNameNode.InnerText))
        {
            throw (
                "Workshop item $workshopItemId manifest '$relativeManifestPath' " +
                'does not contain <ModName>.')
        }

        $manifestDestination = Join-Path `
            (Join-Path $manifestOutputRoot ([string]$workshopItemId)) `
            $relativeManifestPath
        New-Item -ItemType Directory -Path (Split-Path -Parent $manifestDestination) -Force |
            Out-Null
        Copy-Item -LiteralPath $manifestFile.FullName -Destination $manifestDestination -Force

        $manifestSummaries += [ordered]@{
            relative_path = $relativeManifestPath
            mod_name = $modNameNode.InnerText.Trim()
            mod_version = [string]$manifest.SelectSingleNode('/ModInfo/ModVer').InnerText
            game_version = [string]$manifest.SelectSingleNode('/ModInfo/GameVer').InnerText
            assemblies = @(
                $manifest.SelectNodes('/ModInfo/Assemblies/string') |
                    ForEach-Object { $_.InnerText }
            )
            dependencies = @(
                $manifest.SelectNodes('/ModInfo/Dependencies/string') |
                    ForEach-Object { $_.InnerText }
            )
        }
    }
    if ($manifestSummaries.Count -eq 0)
    {
        throw "Workshop item $workshopItemId does not contain a ModInfo.xml manifest."
    }

    $assemblySummaries = @()
    foreach ($assemblyFile in Get-ChildItem -LiteralPath $itemRoot -Filter '*.dll' -File -Recurse |
        Sort-Object FullName)
    {
        $relativeAssemblyPath = Get-RelativeItemPath `
            -ItemRoot $itemRoot `
            -Path $assemblyFile.FullName
        $assemblySummary = [ordered]@{
            relative_path = $relativeAssemblyPath
            assembly_name = $null
            version = $null
            inspection_error = $null
        }
        try
        {
            $assemblyName = [Reflection.AssemblyName]::GetAssemblyName($assemblyFile.FullName)
            $assemblySummary.assembly_name = $assemblyName.Name
            $assemblySummary.version = $assemblyName.Version.ToString()
        }
        catch
        {
            $assemblySummary.inspection_error = $_.Exception.Message
        }
        $assemblySummaries += $assemblySummary
    }

    $extensionRows = @(
        $extensionSummary.GetEnumerator() |
            Sort-Object Name |
            ForEach-Object {
                [ordered]@{
                    extension = [string]$_.Name
                    count = [int]$_.Value
                }
            }
    )
    $items += [ordered]@{
        workshop_item_id = [UInt64]$workshopItemId
        is_target = $workshopItemId -eq $TargetWorkshopItemId
        content_directory = $itemRoot
        file_count = $files.Count
        total_bytes = $totalBytes
        extensions = $extensionRows
        manifests = @($manifestSummaries)
        assemblies = @($assemblySummaries)
        files = @($files)
    }
    $treeLines.Add('')
}

$report = [ordered]@{
    schema_version = 1
    generated_utc = [DateTime]::UtcNow.ToString('o')
    game_app_id = $GameAppId
    download_root = $DownloadRoot
    target_workshop_item_id = $TargetWorkshopItemId
    item_count = $items.Count
    items = @($items)
}
$reportPath = Join-Path $OutputDirectory 'structure.json'
$treePath = Join-Path $OutputDirectory 'tree.txt'
$report | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $reportPath -Encoding utf8
$treeLines | Set-Content -LiteralPath $treePath -Encoding utf8

Write-Host (
    "Exported structures for $($items.Count) Workshop item(s) to " +
    "'$OutputDirectory'.")
$report
