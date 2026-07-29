[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$SteamCmdPath,

    [Parameter(Mandatory)]
    [string]$DownloadRoot,

    [UInt64]$WorkshopItemId = 2960504230,

    [UInt64]$GameAppId = 887570,

    [string]$ReportPath
)

$ErrorActionPreference = 'Stop'
$SteamCmdPath = [IO.Path]::GetFullPath($SteamCmdPath)
$DownloadRoot = [IO.Path]::GetFullPath($DownloadRoot)
if (-not (Test-Path -LiteralPath $SteamCmdPath -PathType Leaf))
{
    throw "SteamCMD was not found at '$SteamCmdPath'."
}

New-Item -ItemType Directory -Path $DownloadRoot -Force | Out-Null
Write-Host "Downloading workshop item $WorkshopItemId for game $GameAppId."
& $SteamCmdPath `
    '+force_install_dir' $DownloadRoot `
    '+login' 'anonymous' `
    '+workshop_download_item' $GameAppId $WorkshopItemId 'validate' `
    '+quit'
if ($LASTEXITCODE -ne 0)
{
    throw "SteamCMD workshop download failed with exit code $LASTEXITCODE."
}

$steamCmdDirectory = Split-Path -Parent $SteamCmdPath
$relativeWorkshopPath = Join-Path 'steamapps\workshop\content' (Join-Path $GameAppId $WorkshopItemId)
$canonicalWorkshopPath = Join-Path $DownloadRoot $relativeWorkshopPath
$candidatePaths = @(
    $canonicalWorkshopPath
    (Join-Path $steamCmdDirectory $relativeWorkshopPath)
) | Select-Object -Unique
$workshopItemDirectory = $candidatePaths |
    Where-Object { Test-Path -LiteralPath $_ -PathType Container } |
    Select-Object -First 1

if ([string]::IsNullOrWhiteSpace($workshopItemDirectory))
{
    throw "Workshop item $WorkshopItemId was not found. Checked: $($candidatePaths -join ', ')"
}

if (-not [string]::Equals(
    [IO.Path]::GetFullPath($workshopItemDirectory),
    [IO.Path]::GetFullPath($canonicalWorkshopPath),
    [StringComparison]::OrdinalIgnoreCase))
{
    New-Item -ItemType Directory -Path (Split-Path -Parent $canonicalWorkshopPath) -Force | Out-Null
    Copy-Item -LiteralPath $workshopItemDirectory -Destination $canonicalWorkshopPath -Recurse -Force
    $workshopItemDirectory = $canonicalWorkshopPath
}

$report = [ordered]@{
    game_app_id = $GameAppId
    workshop_item_id = $WorkshopItemId
    workshop_item_directory = [IO.Path]::GetFullPath($workshopItemDirectory)
}

if (-not [string]::IsNullOrWhiteSpace($ReportPath))
{
    $ReportPath = [IO.Path]::GetFullPath($ReportPath)
    New-Item -ItemType Directory -Path (Split-Path -Parent $ReportPath) -Force | Out-Null
    $report | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $ReportPath -Encoding utf8
}

Write-Host "Downloaded workshop item to '$workshopItemDirectory'."
$report
