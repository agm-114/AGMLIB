[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',
    [string]$GameRoot = (Join-Path ${env:ProgramFiles(x86)} 'Steam\steamapps\common\Nebulous'),
    [switch]$SkipBuild,
    [switch]$ValidateOnly
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$projectPath = Join-Path $repositoryRoot 'AGMLIB\AGMLIB.csproj'
$GameRoot = [IO.Path]::GetFullPath($GameRoot)
$modInstallDir = Join-Path $GameRoot 'Mods\AGMLIB'
$artifactRoot = Join-Path $repositoryRoot 'artifacts\AGMLIB'
$artifactBin = Join-Path $artifactRoot "$Configuration\net481"
$sourceDll = Join-Path $artifactBin 'AGMLIB.dll'
$sourceHarmony = Join-Path $artifactBin '0Harmony.dll'
$sourceManifest = Join-Path $artifactRoot 'ModInfo.xml'

if (-not (Test-Path -LiteralPath $projectPath -PathType Leaf))
{
    throw "AGMLIB project was not found at $projectPath"
}

if (-not (Test-Path -LiteralPath $GameRoot -PathType Container))
{
    throw "Nebulous game root was not found at $GameRoot"
}

Write-Host "Configuration: $Configuration"
Write-Host "Artifact root: $artifactRoot"
Write-Host "Deployment root: $modInstallDir"

if ($ValidateOnly)
{
    Write-Host 'AGMLIB deployment paths are valid. No files were changed.'
    return
}

$gameProcess = Get-Process -Name 'Nebulous' -ErrorAction SilentlyContinue
if ($gameProcess)
{
    $processSummary = ($gameProcess | ForEach-Object { "$($_.ProcessName) (PID $($_.Id))" }) -join ', '
    throw "Cannot deploy while $processSummary is running. Close Nebulous, then rerun this command."
}

if (-not $SkipBuild)
{
    & dotnet build $projectPath --no-restore -v:minimal "-p:Configuration=$Configuration"
    if ($LASTEXITCODE -ne 0)
    {
        throw "AGMLIB build failed with exit code $LASTEXITCODE."
    }
}

& dotnet msbuild $projectPath /t:DeployAgmlibToGame /v:minimal `
    "/p:Configuration=$Configuration" `
    /p:DeployToGame=true `
    "/p:ModInstallDir=$modInstallDir"
if ($LASTEXITCODE -ne 0)
{
    throw "AGMLIB deployment failed with exit code $LASTEXITCODE."
}

$expectedFiles = @(
    @{ Source = $sourceDll; Destination = Join-Path $modInstallDir "$Configuration\net481\AGMLIB.dll" },
    @{ Source = $sourceHarmony; Destination = Join-Path $modInstallDir "$Configuration\net481\0Harmony.dll" },
    @{ Source = $sourceManifest; Destination = Join-Path $modInstallDir 'ModInfo.xml' }
)

foreach ($file in $expectedFiles)
{
    if (-not (Test-Path -LiteralPath $file.Source -PathType Leaf))
    {
        throw "Expected deployment source is missing: $($file.Source)"
    }

    if (-not (Test-Path -LiteralPath $file.Destination -PathType Leaf))
    {
        throw "Expected deployed file is missing: $($file.Destination)"
    }

    $sourceHash = (Get-FileHash -LiteralPath $file.Source -Algorithm SHA256).Hash
    $destinationHash = (Get-FileHash -LiteralPath $file.Destination -Algorithm SHA256).Hash
    if ($sourceHash -ne $destinationHash)
    {
        throw "Deployment hash mismatch for $($file.Destination). Source: $sourceHash Destination: $destinationHash"
    }

    Write-Host "Verified $($file.Destination)"
    Write-Host "  SHA-256 $destinationHash"
}

[xml]$manifest = Get-Content -LiteralPath $sourceManifest -Raw
Write-Host "Deployed AGMLIB $($manifest.ModInfo.ModVer) for Nebulous $($manifest.ModInfo.GameVer)."
