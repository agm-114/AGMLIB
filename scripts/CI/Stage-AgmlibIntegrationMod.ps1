[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$PackageRoot,

    [Parameter(Mandatory)]
    [string]$WorkshopItemDirectory,

    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',

    [string]$CiTestSupportAssemblyPath,

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

$supportAssemblyTarget = $null
$supportAssemblyHash = $null
if (-not [string]::IsNullOrWhiteSpace($CiTestSupportAssemblyPath))
{
    $CiTestSupportAssemblyPath = [IO.Path]::GetFullPath($CiTestSupportAssemblyPath)
    if (-not (Test-Path -LiteralPath $CiTestSupportAssemblyPath -PathType Leaf))
    {
        throw "CI test-support assembly was not found at '$CiTestSupportAssemblyPath'."
    }

    $supportAssemblyName = 'AGMLIB.CI.TestSupport.dll'
    if ((Split-Path -Leaf $CiTestSupportAssemblyPath) -ne $supportAssemblyName)
    {
        throw "CI test-support assembly must be named '$supportAssemblyName'."
    }

    $supportRelativePath = "$Configuration/net481/$supportAssemblyName"
    $supportAssemblyTarget = Join-Path $WorkshopItemDirectory $supportRelativePath
    New-Item -ItemType Directory -Path (Split-Path -Parent $supportAssemblyTarget) -Force | Out-Null
    Copy-Item -LiteralPath $CiTestSupportAssemblyPath -Destination $supportAssemblyTarget -Force

    [xml]$manifest = Get-Content -LiteralPath $stagedManifest -Raw
    $assembliesElement = $manifest.SelectSingleNode('/ModInfo/Assemblies')
    if ($null -eq $assembliesElement)
    {
        throw "Staged AGMLIB manifest does not contain <Assemblies>."
    }
    $existingSupportAssembly = $assembliesElement.SelectNodes('string') |
        Where-Object { $_.InnerText -eq $supportRelativePath }
    if ($null -eq $existingSupportAssembly)
    {
        $supportElement = $manifest.CreateElement('string')
        $supportElement.InnerText = $supportRelativePath
        [void]$assembliesElement.AppendChild($supportElement)
    }

    $manifestSettings = [Xml.XmlWriterSettings]::new()
    $manifestSettings.Encoding = [Text.UTF8Encoding]::new($false)
    $manifestSettings.Indent = $true
    $manifestSettings.NewLineChars = "`r`n"
    $manifestSettings.NewLineHandling = [Xml.NewLineHandling]::Replace
    try
    {
        $manifestWriter = [Xml.XmlWriter]::Create($stagedManifest, $manifestSettings)
        $manifest.Save($manifestWriter)
    }
    finally
    {
        if ($null -ne $manifestWriter)
        {
            $manifestWriter.Dispose()
        }
    }

    $supportAssemblyHash = (Get-FileHash -LiteralPath $supportAssemblyTarget -Algorithm SHA256).Hash
}

$report = [ordered]@{
    configuration = $Configuration
    package_root = $PackageRoot
    workshop_item_directory = $WorkshopItemDirectory
    staged_manifest = $stagedManifest
    staged_dll = $stagedDll
    staged_dll_sha256 = $stagedHash
    ci_test_support_assembly = $supportAssemblyTarget
    ci_test_support_sha256 = $supportAssemblyHash
}

if (-not [string]::IsNullOrWhiteSpace($ReportPath))
{
    $ReportPath = [IO.Path]::GetFullPath($ReportPath)
    New-Item -ItemType Directory -Path (Split-Path -Parent $ReportPath) -Force | Out-Null
    $report | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $ReportPath -Encoding utf8
}

Write-Host "Staged the $Configuration AGMLIB package over workshop item 2960504230."
$report
