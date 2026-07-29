[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$PackageRoot,

    [Parameter(Mandatory)]
    [string]$WorkshopItemDirectory,

    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',

    [string]$ReportPath
)

$ErrorActionPreference = 'Stop'
$PackageRoot = [IO.Path]::GetFullPath($PackageRoot)
$WorkshopItemDirectory = [IO.Path]::GetFullPath($WorkshopItemDirectory)
if (-not (Test-Path -LiteralPath $PackageRoot -PathType Container))
{
    throw "AGMLIB package root was not found at '$PackageRoot'."
}
if (-not (Test-Path -LiteralPath $WorkshopItemDirectory -PathType Container))
{
    throw "Workshop item directory was not found at '$WorkshopItemDirectory'."
}

& (Join-Path $PSScriptRoot 'Test-AgmlibPackage.ps1') `
    -Configuration $Configuration `
    -PackageRoot $PackageRoot | Out-Null

Get-ChildItem -LiteralPath $PackageRoot -Force | ForEach-Object {
    Copy-Item -LiteralPath $_.FullName -Destination $WorkshopItemDirectory -Recurse -Force
}

$stagedManifest = Join-Path $WorkshopItemDirectory 'ModInfo.xml'
$stagedDll = Join-Path $WorkshopItemDirectory "$Configuration/net481/AGMLIB.dll"
if (-not (Test-Path -LiteralPath $stagedManifest -PathType Leaf) -or
    -not (Test-Path -LiteralPath $stagedDll -PathType Leaf))
{
    throw 'AGMLIB integration overlay did not produce the expected manifest and DLL.'
}

$sourceHash = (Get-FileHash -LiteralPath (Join-Path $PackageRoot "$Configuration/net481/AGMLIB.dll") -Algorithm SHA256).Hash
$stagedHash = (Get-FileHash -LiteralPath $stagedDll -Algorithm SHA256).Hash
if ($sourceHash -ne $stagedHash)
{
    throw 'Staged AGMLIB DLL does not match the CI package.'
}

$report = [ordered]@{
    configuration = $Configuration
    package_root = $PackageRoot
    workshop_item_directory = $WorkshopItemDirectory
    staged_manifest = $stagedManifest
    staged_dll = $stagedDll
    staged_dll_sha256 = $stagedHash
}

if (-not [string]::IsNullOrWhiteSpace($ReportPath))
{
    $ReportPath = [IO.Path]::GetFullPath($ReportPath)
    New-Item -ItemType Directory -Path (Split-Path -Parent $ReportPath) -Force | Out-Null
    $report | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $ReportPath -Encoding utf8
}

Write-Host "Staged the $Configuration AGMLIB package over workshop item 2960504230."
$report
