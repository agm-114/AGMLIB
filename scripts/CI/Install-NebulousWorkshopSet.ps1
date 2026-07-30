[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$SteamCmdPath,

    [Parameter(Mandatory)]
    [string]$DownloadRoot,

    [Parameter(Mandatory)]
    [UInt64[]]$WorkshopItemIds,

    [UInt64]$GameAppId = 887570,

    [ValidateRange(1, 5)]
    [int]$MaxInstallAttempts = 3,

    [string]$ReportPath
)

$ErrorActionPreference = 'Stop'
$SteamCmdPath = [IO.Path]::GetFullPath($SteamCmdPath)
$DownloadRoot = [IO.Path]::GetFullPath($DownloadRoot)
if (-not (Test-Path -LiteralPath $SteamCmdPath -PathType Leaf))
{
    throw "SteamCMD was not found at '$SteamCmdPath'."
}
if ($WorkshopItemIds.Count -eq 0)
{
    throw 'At least one Workshop item ID is required.'
}

$orderedIds = [Collections.Generic.List[UInt64]]::new()
$seenIds = [Collections.Generic.HashSet[UInt64]]::new()
foreach ($workshopItemId in $WorkshopItemIds)
{
    if ($workshopItemId -eq 0)
    {
        throw 'Workshop item IDs must be greater than zero.'
    }
    if ($seenIds.Add($workshopItemId))
    {
        $orderedIds.Add($workshopItemId)
    }
}

$startedUtc = [DateTime]::UtcNow
$installedItems = @()
foreach ($workshopItemId in $orderedIds)
{
    $attempt = 0
    $lastFailure = $null
    while ($attempt -lt $MaxInstallAttempts)
    {
        $attempt++
        try
        {
            Write-Host (
                "[NEBULOUS event] workshop-download-started: " +
                "item=$workshopItemId attempt=$attempt/$MaxInstallAttempts")
            $itemReport = & (Join-Path $PSScriptRoot 'Install-NebulousWorkshopItem.ps1') `
                -SteamCmdPath $SteamCmdPath `
                -DownloadRoot $DownloadRoot `
                -WorkshopItemId $workshopItemId `
                -GameAppId $GameAppId
            $installedItems += [ordered]@{
                workshop_item_id = [UInt64]$workshopItemId
                workshop_item_directory = [string]$itemReport.workshop_item_directory
                attempts = $attempt
            }
            Write-Host (
                "[NEBULOUS event] workshop-download-completed: " +
                "item=$workshopItemId attempt=$attempt")
            $lastFailure = $null
            break
        }
        catch
        {
            $lastFailure = $_.Exception.Message
            Write-Warning (
                "Workshop item $workshopItemId download attempt $attempt failed: " +
                $lastFailure)
        }
    }

    if (-not [string]::IsNullOrWhiteSpace($lastFailure))
    {
        throw (
            "Workshop item $workshopItemId download failed after " +
            "$MaxInstallAttempts attempt(s): $lastFailure")
    }
}

$report = [ordered]@{
    game_app_id = $GameAppId
    download_root = $DownloadRoot
    started_utc = $startedUtc.ToString('o')
    finished_utc = [DateTime]::UtcNow.ToString('o')
    item_count = $installedItems.Count
    items = @($installedItems)
}

if (-not [string]::IsNullOrWhiteSpace($ReportPath))
{
    $ReportPath = [IO.Path]::GetFullPath($ReportPath)
    New-Item -ItemType Directory -Path (Split-Path -Parent $ReportPath) -Force | Out-Null
    $report | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $ReportPath -Encoding utf8
}

$report
