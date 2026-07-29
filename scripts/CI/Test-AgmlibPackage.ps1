[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [string]$PackageRoot,

    [string]$ReportPath
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
if ([string]::IsNullOrWhiteSpace($PackageRoot))
{
    $PackageRoot = Join-Path $repositoryRoot 'artifacts\AGMLIB'
}

$PackageRoot = [IO.Path]::GetFullPath($PackageRoot)
$manifestPath = Join-Path $PackageRoot 'ModInfo.xml'
if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf))
{
    throw "AGMLIB manifest was not found at '$manifestPath'."
}

[xml]$manifest = Get-Content -LiteralPath $manifestPath -Raw
$modInfo = $manifest.ModInfo
if ($null -eq $modInfo)
{
    throw "Manifest '$manifestPath' does not contain a ModInfo root element."
}

if ([string]$modInfo.ModName -ne 'AGMLIB')
{
    throw "Expected ModName 'AGMLIB', found '$($modInfo.ModName)'."
}

$modVersion = $null
if (-not [version]::TryParse([string]$modInfo.ModVer, [ref]$modVersion))
{
    throw "ModVer '$($modInfo.ModVer)' is not a valid version."
}

$gameVersion = $null
if (-not [version]::TryParse([string]$modInfo.GameVer, [ref]$gameVersion))
{
    throw "GameVer '$($modInfo.GameVer)' is not a valid version."
}

$assemblyEntries = @($modInfo.Assemblies.string | ForEach-Object { [string]$_ })
$expectedEntries = @(
    "$Configuration/net481/AGMLIB.dll"
    "$Configuration/net481/0Harmony.dll"
)

if ($assemblyEntries.Count -ne $expectedEntries.Count)
{
    throw "Expected $($expectedEntries.Count) assembly entries, found $($assemblyEntries.Count)."
}

foreach ($expectedEntry in $expectedEntries)
{
    if ($assemblyEntries -cnotcontains $expectedEntry)
    {
        throw "Manifest is missing assembly entry '$expectedEntry'. Found: $($assemblyEntries -join ', ')"
    }
}

$packagePrefix = $PackageRoot.TrimEnd(
    [IO.Path]::DirectorySeparatorChar,
    [IO.Path]::AltDirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
$assemblyReports = @()
foreach ($entry in $assemblyEntries)
{
    if ([IO.Path]::IsPathRooted($entry))
    {
        throw "Assembly entry must be package-relative: '$entry'."
    }

    $normalizedEntry = $entry.Replace('/', [IO.Path]::DirectorySeparatorChar)
    $assemblyPath = [IO.Path]::GetFullPath((Join-Path $PackageRoot $normalizedEntry))
    if (-not $assemblyPath.StartsWith($packagePrefix, [StringComparison]::OrdinalIgnoreCase))
    {
        throw "Assembly entry escapes the package root: '$entry'."
    }
    if (-not (Test-Path -LiteralPath $assemblyPath -PathType Leaf))
    {
        throw "Manifest assembly was not found: '$assemblyPath'."
    }

    $assemblyName = [Reflection.AssemblyName]::GetAssemblyName($assemblyPath)
    $assemblyReports += [ordered]@{
        entry = $entry
        name = $assemblyName.Name
        version = $assemblyName.Version.ToString()
        sha256 = (Get-FileHash -LiteralPath $assemblyPath -Algorithm SHA256).Hash
        bytes = (Get-Item -LiteralPath $assemblyPath).Length
    }
}

$agmlibAssembly = $assemblyReports | Where-Object { $_.name -eq 'AGMLIB' } | Select-Object -First 1
if ($null -eq $agmlibAssembly)
{
    throw 'The package does not contain an assembly named AGMLIB.'
}
if ([version]$agmlibAssembly.version -ne $modVersion)
{
    throw "AGMLIB assembly version '$($agmlibAssembly.version)' does not match ModVer '$modVersion'."
}

$report = [ordered]@{
    package_root = $PackageRoot
    configuration = $Configuration
    mod_name = [string]$modInfo.ModName
    mod_version = $modVersion.ToString()
    game_version = $gameVersion.ToString()
    manifest_sha256 = (Get-FileHash -LiteralPath $manifestPath -Algorithm SHA256).Hash
    assemblies = $assemblyReports
}

if (-not [string]::IsNullOrWhiteSpace($ReportPath))
{
    $ReportPath = [IO.Path]::GetFullPath($ReportPath)
    $reportDirectory = Split-Path -Parent $ReportPath
    if (-not [string]::IsNullOrWhiteSpace($reportDirectory))
    {
        New-Item -ItemType Directory -Path $reportDirectory -Force | Out-Null
    }
    $report | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $ReportPath -Encoding utf8
}

Write-Host "Validated AGMLIB $Configuration package version $modVersion for NEBULOUS $gameVersion."
$report
