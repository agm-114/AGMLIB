[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$SteamCmdPath,

    [Parameter(Mandatory)]
    [string]$InstallDirectory,

    [string]$ReportPath,

    [switch]$SkipValidation
)

$ErrorActionPreference = 'Stop'
$runningOnWindows = [Environment]::OSVersion.Platform -eq [PlatformID]::Win32NT
$SteamCmdPath = [IO.Path]::GetFullPath($SteamCmdPath)
$InstallDirectory = [IO.Path]::GetFullPath($InstallDirectory)
if (-not (Test-Path -LiteralPath $SteamCmdPath -PathType Leaf))
{
    throw "SteamCMD was not found at '$SteamCmdPath'."
}

New-Item -ItemType Directory -Path $InstallDirectory -Force | Out-Null
$arguments = @(
    '+force_install_dir', $InstallDirectory,
    '+login', 'anonymous',
    '+app_update', '2353090'
)
if (-not $SkipValidation)
{
    $arguments += 'validate'
}
$arguments += '+quit'

Write-Host "Installing NEBULOUS dedicated server app 2353090 into '$InstallDirectory'."
& $SteamCmdPath @arguments
if ($LASTEXITCODE -ne 0)
{
    throw "SteamCMD dedicated-server install failed with exit code $LASTEXITCODE."
}

$serverCandidates = @(
    (Join-Path $InstallDirectory 'NebulousDedicatedServer')
    (Join-Path $InstallDirectory 'NebulousDedicatedServer.x86_64')
    (Join-Path $InstallDirectory 'NebulousDedicatedServer.exe')
)
$serverExecutable = $serverCandidates |
    Where-Object { Test-Path -LiteralPath $_ -PathType Leaf } |
    Select-Object -First 1
if ([string]::IsNullOrWhiteSpace($serverExecutable))
{
    throw "NEBULOUS dedicated-server executable was not found under '$InstallDirectory'."
}

if (-not $runningOnWindows)
{
    & chmod +x $serverExecutable
    if ($LASTEXITCODE -ne 0)
    {
        throw "Could not mark '$serverExecutable' executable."
    }
}

$configPath = Join-Path $InstallDirectory 'DedicatedServerConfig.xml'
if (-not (Test-Path -LiteralPath $configPath -PathType Leaf))
{
    throw "Default dedicated-server config was not found at '$configPath'."
}

$manifestPath = Join-Path $InstallDirectory 'steamapps\appmanifest_2353090.acf'
$report = [ordered]@{
    app_id = 2353090
    install_directory = $InstallDirectory
    server_executable = $serverExecutable
    default_config = $configPath
    app_manifest = if (Test-Path -LiteralPath $manifestPath -PathType Leaf) { $manifestPath } else { $null }
}

if (-not [string]::IsNullOrWhiteSpace($ReportPath))
{
    $ReportPath = [IO.Path]::GetFullPath($ReportPath)
    New-Item -ItemType Directory -Path (Split-Path -Parent $ReportPath) -Force | Out-Null
    $report | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $ReportPath -Encoding utf8
}

Write-Host "Installed NEBULOUS dedicated server: $serverExecutable"
$report
