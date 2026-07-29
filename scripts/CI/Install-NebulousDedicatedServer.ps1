[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$SteamCmdPath,

    [Parameter(Mandatory)]
    [string]$InstallDirectory,

    [string]$ReportPath,

    [switch]$SkipValidation,

    [ValidateRange(1, 3)]
    [int]$MaxInstallAttempts = 2
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

Write-Host 'Refreshing SteamCMD application metadata.'
& $SteamCmdPath `
    '+login' 'anonymous' `
    '+app_info_update' '1' `
    '+quit'
if ($LASTEXITCODE -ne 0)
{
    throw "SteamCMD metadata refresh failed with exit code $LASTEXITCODE."
}

$arguments = @(
    '+force_install_dir', $InstallDirectory,
    '+login', 'anonymous',
    '+app_info_update', '1',
    '+app_update', '2353090'
)
if (-not $SkipValidation)
{
    $arguments += 'validate'
}
$arguments += '+quit'

Write-Host "Installing NEBULOUS dedicated server app 2353090 into '$InstallDirectory'."
$installExitCode = $null
for ($attempt = 1; $attempt -le $MaxInstallAttempts; $attempt++)
{
    & $SteamCmdPath @arguments
    $installExitCode = $LASTEXITCODE
    if ($installExitCode -eq 0)
    {
        break
    }

    if ($attempt -lt $MaxInstallAttempts)
    {
        Write-Warning (
            "SteamCMD dedicated-server install attempt $attempt failed with exit code " +
            "$installExitCode. Refreshing metadata before the bounded retry.")
        & $SteamCmdPath `
            '+login' 'anonymous' `
            '+app_info_update' '1' `
            '+quit'
        if ($LASTEXITCODE -ne 0)
        {
            throw "SteamCMD metadata refresh before retry failed with exit code $LASTEXITCODE."
        }
    }
}
if ($installExitCode -ne 0)
{
    throw "SteamCMD dedicated-server install failed after $MaxInstallAttempts attempt(s) with exit code $installExitCode."
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
