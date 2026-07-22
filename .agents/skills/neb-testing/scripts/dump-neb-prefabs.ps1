[CmdletBinding()]
param(
    [string]$GameRoot = (Join-Path ${env:ProgramFiles(x86)} 'Steam\steamapps\common\Nebulous'),
    [string]$PlayerLog = (Join-Path $env:USERPROFILE 'AppData\LocalLow\Eridanus Industries\Nebulous\Player.log'),
    [string]$OutputPath,
    [int]$TimeoutSeconds = 180,
    [switch]$SkipBuild,
    [switch]$ValidateOnly
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..\..'))
$projectPath = Join-Path $repositoryRoot 'AGMLIB\AGMLIB.csproj'
$executablePath = Join-Path $GameRoot 'Nebulous.exe'
$deploymentRoot = Join-Path $GameRoot 'Mods\AGMLIB'

if ([string]::IsNullOrWhiteSpace($OutputPath))
{
    $OutputPath = Join-Path $repositoryRoot '.agents\cache\neb-prefabs'
}

$GameRoot = [IO.Path]::GetFullPath($GameRoot)
$PlayerLog = [IO.Path]::GetFullPath($PlayerLog)
$OutputPath = [IO.Path]::GetFullPath($OutputPath)
$manifestPath = Join-Path $OutputPath 'manifest.yaml'

if (-not (Test-Path -LiteralPath $projectPath -PathType Leaf))
{
    throw "AGMLIB project was not found at $projectPath"
}

if (-not (Test-Path -LiteralPath $executablePath -PathType Leaf))
{
    throw "Nebulous executable was not found at $executablePath"
}

Write-Host "Repository: $repositoryRoot"
Write-Host "Game root: $GameRoot"
Write-Host "Player log: $PlayerLog"
Write-Host "Prefab snapshot: $OutputPath"

if ($ValidateOnly)
{
    Write-Host 'Prefab dump paths are valid.'
    return
}

if (Get-Process -Name 'Nebulous' -ErrorAction SilentlyContinue)
{
    throw 'Nebulous is already running. Close it so the dump environment is present from process startup.'
}

if (-not $SkipBuild)
{
    $baseOutputPath = $deploymentRoot + [IO.Path]::DirectorySeparatorChar
    & dotnet build $projectPath --no-restore -v:minimal "-p:BaseOutputPath=$baseOutputPath"
    if ($LASTEXITCODE -ne 0)
    {
        throw "AGMLIB build failed with exit code $LASTEXITCODE."
    }

    $modInfoPath = Join-Path $repositoryRoot 'AGMLIB\ModInfo.xml'
    if (Test-Path -LiteralPath $modInfoPath -PathType Leaf)
    {
        New-Item -ItemType Directory -Force $deploymentRoot | Out-Null
        Copy-Item -LiteralPath $modInfoPath -Destination (Join-Path $deploymentRoot 'ModInfo.xml') -Force
    }
}

$startedAtUtc = [DateTime]::UtcNow
$previousDumpEnvironment = [Environment]::GetEnvironmentVariable('AGMLIB_PREFAB_DUMP_DIR', 'Process')

try
{
    [Environment]::SetEnvironmentVariable('AGMLIB_PREFAB_DUMP_DIR', $OutputPath, 'Process')
    $gameProcess = Start-Process $executablePath -ArgumentList '-screen-width 1920 -screen-height 1080 -screen-fullscreen 0' -PassThru
}
finally
{
    [Environment]::SetEnvironmentVariable('AGMLIB_PREFAB_DUMP_DIR', $previousDumpEnvironment, 'Process')
}

$deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
while ([DateTime]::UtcNow -lt $deadline)
{
    if ($gameProcess.HasExited)
    {
        throw "Nebulous exited with code $($gameProcess.ExitCode) before producing the prefab snapshot."
    }

    if (Test-Path -LiteralPath $manifestPath -PathType Leaf)
    {
        $manifest = Get-Item -LiteralPath $manifestPath
        if ($manifest.LastWriteTimeUtc -ge $startedAtUtc)
        {
            Write-Host "Prefab snapshot completed: $manifestPath"
            return
        }
    }

    Start-Sleep -Milliseconds 250
}

$diagnostics = @()
if (Test-Path -LiteralPath $PlayerLog -PathType Leaf)
{
    $diagnostics = Get-Content -LiteralPath $PlayerLog -Tail 1000 |
        Select-String -Pattern '\[PrefabYamlDump\]|Exception|Error' |
        Select-Object -Last 30
}

if ($diagnostics.Count -gt 0)
{
    Write-Host ($diagnostics -join [Environment]::NewLine)
}

throw "Timed out after $TimeoutSeconds seconds waiting for $manifestPath"
