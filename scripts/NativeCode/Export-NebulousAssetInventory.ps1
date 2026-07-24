[CmdletBinding()]
param(
    [string]$GameRoot = (Join-Path ${env:ProgramFiles(x86)} 'Steam\steamapps\common\Nebulous'),
    [string]$PrefabRoot,
    [string]$OutputPath,
    [string]$SummaryPath,
    [switch]$SkipUnity
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$pythonScript = Join-Path $PSScriptRoot 'export_nebulous_asset_inventory.py'
$unityPyPath = Join-Path $repositoryRoot '.agents\cache\unitypy'

if ([string]::IsNullOrWhiteSpace($PrefabRoot))
{
    $PrefabRoot = Join-Path $repositoryRoot '.agents\cache\neb-prefabs'
}
if ([string]::IsNullOrWhiteSpace($OutputPath))
{
    $OutputPath = Join-Path $repositoryRoot '.native-code\current\assets'
}
if ([string]::IsNullOrWhiteSpace($SummaryPath))
{
    $SummaryPath = Join-Path $repositoryRoot 'knowledge\generated\vanilla-asset-resource-inventory.md'
}

$GameRoot = [IO.Path]::GetFullPath($GameRoot)
$PrefabRoot = [IO.Path]::GetFullPath($PrefabRoot)
$OutputPath = [IO.Path]::GetFullPath($OutputPath)
$SummaryPath = [IO.Path]::GetFullPath($SummaryPath)

if (-not (Test-Path -LiteralPath (Join-Path $GameRoot 'Nebulous.exe') -PathType Leaf))
{
    throw "Nebulous game root was not found at $GameRoot"
}
if (-not (Test-Path -LiteralPath $pythonScript -PathType Leaf))
{
    throw "Inventory script was not found at $pythonScript"
}
if (-not $SkipUnity -and -not (Test-Path -LiteralPath (Join-Path $unityPyPath 'UnityPy') -PathType Container))
{
    throw "The local UnityPy runtime was not found at $unityPyPath"
}

$arguments = @(
    $pythonScript,
    '--game-root', $GameRoot,
    '--prefab-root', $PrefabRoot,
    '--prefab-source', 'Vanilla',
    '--prefab-source', 'AGMLIB',
    '--output', $OutputPath,
    '--summary', $SummaryPath
)
if (-not $SkipUnity)
{
    $arguments += @('--unitypy-path', $unityPyPath)
}
else
{
    $arguments += '--skip-unity'
}

& python @arguments
if ($LASTEXITCODE -ne 0)
{
    throw "Asset inventory failed with exit code $LASTEXITCODE."
}
