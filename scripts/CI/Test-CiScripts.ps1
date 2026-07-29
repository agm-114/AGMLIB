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

$temporaryRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
$scratchRoot = Join-Path $temporaryRoot "agmlib-ci-contract-$([Guid]::NewGuid().ToString('N'))"
$workshopRoot = Join-Path $scratchRoot 'workshop-item'
$fakeSourceRoot = Join-Path $scratchRoot 'fake-server-source'
$serverRoot = Join-Path $scratchRoot 'fake-server'
$smokeOutput = Join-Path $scratchRoot 'smoke-output'
$sourceConfig = Join-Path $scratchRoot 'DedicatedServerConfig.source.xml'
$generatedConfig = Join-Path $scratchRoot 'DedicatedServerConfig.xml'

try
{
    New-Item -ItemType Directory -Path $workshopRoot -Force | Out-Null
    New-Item -ItemType Directory -Path $fakeSourceRoot -Force | Out-Null
    New-Item -ItemType Directory -Path $serverRoot -Force | Out-Null

    & (Join-Path $PSScriptRoot 'Test-AgmlibPackage.ps1') `
        -Configuration $Configuration `
        -PackageRoot $PackageRoot | Out-Null

    [IO.File]::WriteAllText(
        (Join-Path $workshopRoot 'workshop-baseline.txt'),
        'Preserved workshop content.',
        [Text.UTF8Encoding]::new($false))
    & (Join-Path $PSScriptRoot 'Stage-AgmlibIntegrationMod.ps1') `
        -Configuration $Configuration `
        -PackageRoot $PackageRoot `
        -WorkshopItemDirectory $workshopRoot | Out-Null
    if (-not (Test-Path -LiteralPath (Join-Path $workshopRoot 'workshop-baseline.txt') -PathType Leaf))
    {
        throw 'Workshop overlay removed an existing workshop file.'
    }

    $configFixture = @(
        '<?xml version="1.0" encoding="utf-8"?>'
        '<SkirmishDedicatedServerConfig>'
        '  <ServerName>Default</ServerName>'
        '  <GamePort>7777</GamePort>'
        '  <QueryPort>27016</QueryPort>'
        '  <MaxPlayers>10</MaxPlayers>'
        '  <Mods />'
        '</SkirmishDedicatedServerConfig>'
    ) -join "`r`n"
    [IO.File]::WriteAllText($sourceConfig, $configFixture, [Text.UTF8Encoding]::new($false))
    & (Join-Path $PSScriptRoot 'New-NebulousIntegrationConfig.ps1') `
        -SourceConfigPath $sourceConfig `
        -DestinationConfigPath $generatedConfig | Out-Null

    [xml]$generated = Get-Content -LiteralPath $generatedConfig -Raw
    if ([string]$generated.SkirmishDedicatedServerConfig.ServerName -ne 'AGMLIB CI Smoke' -or
        [string]$generated.SkirmishDedicatedServerConfig.Mods.unsignedLong -ne '2960504230')
    {
        throw 'Generated integration config did not contain the expected server name and AGMLIB mod ID.'
    }
    if ([IO.File]::ReadAllText($generatedConfig) -match '(?<!\r)\n')
    {
        throw 'Generated integration config contains an LF line ending without CR.'
    }

    $fakeProjectPath = Join-Path $fakeSourceRoot 'FakeNebulousDedicatedServer.csproj'
    $fakeProgramPath = Join-Path $fakeSourceRoot 'Program.cs'
    $fakeProject = @'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <AssemblyName>NebulousDedicatedServer</AssemblyName>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
'@
    $fakeProgram = @'
string? GetArgument(string name)
{
    int index = Array.FindIndex(args, value => string.Equals(value, name, StringComparison.Ordinal));
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}

string logPath = GetArgument("-logFile")
    ?? throw new InvalidOperationException("-logFile was not supplied.");
string dumpPath = Environment.GetEnvironmentVariable("AGMLIB_PREFAB_DUMP_DIR")
    ?? throw new InvalidOperationException("AGMLIB_PREFAB_DUMP_DIR was not supplied.");

Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(logPath))!);
Directory.CreateDirectory(dumpPath);
File.WriteAllText(
    logPath,
    """
Finished Loading Mod 'AGMLIB'. Result: Loaded
[TestingComponents] Discovery complete: discovered=0, created=0, skipped=0, failed=0.
[PrefabYamlDump] Completed path='fixture' prefabs=1 enabledMods=1 errors=0.
Server: listening port=17777
""");
File.WriteAllText(Path.Combine(dumpPath, "manifest.yaml"), "errors: 0\n");
Console.WriteLine("Fake NEBULOUS dedicated server is ready.");
Thread.Sleep(TimeSpan.FromSeconds(30));
'@
    [IO.File]::WriteAllText($fakeProjectPath, $fakeProject, [Text.UTF8Encoding]::new($false))
    [IO.File]::WriteAllText($fakeProgramPath, $fakeProgram, [Text.UTF8Encoding]::new($false))

    & dotnet publish $fakeProjectPath `
        --configuration Release `
        --output $serverRoot `
        --nologo `
        --verbosity quiet
    if ($LASTEXITCODE -ne 0)
    {
        throw "Could not build the fake dedicated server; dotnet exited with code $LASTEXITCODE."
    }

    & (Join-Path $PSScriptRoot 'Invoke-NebulousServerSmoke.ps1') `
        -ServerRoot $serverRoot `
        -ConfigPath $generatedConfig `
        -OutputDirectory $smokeOutput `
        -TimeoutSeconds 45

    $smokeSummary = Get-Content -LiteralPath (Join-Path $smokeOutput 'summary.json') -Raw | ConvertFrom-Json
    if ($smokeSummary.succeeded -ne $true)
    {
        throw 'The fake dedicated-server smoke test did not report success.'
    }

    $report = [ordered]@{
        configuration = $Configuration
        package_validation = 'passed'
        workshop_overlay = 'passed'
        config_generation = 'passed'
        smoke_process_contract = 'passed'
    }
    if (-not [string]::IsNullOrWhiteSpace($ReportPath))
    {
        $ReportPath = [IO.Path]::GetFullPath($ReportPath)
        New-Item -ItemType Directory -Path (Split-Path -Parent $ReportPath) -Force | Out-Null
        $report | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $ReportPath -Encoding utf8
    }
    Write-Host "AGMLIB $Configuration CI script contracts passed."
}
finally
{
    $resolvedScratchRoot = [IO.Path]::GetFullPath($scratchRoot)
    $temporaryPrefix = $temporaryRoot.TrimEnd(
        [IO.Path]::DirectorySeparatorChar,
        [IO.Path]::AltDirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    if ($resolvedScratchRoot.StartsWith($temporaryPrefix, [StringComparison]::OrdinalIgnoreCase) -and
        (Split-Path -Leaf $resolvedScratchRoot) -like 'agmlib-ci-contract-*' -and
        (Test-Path -LiteralPath $resolvedScratchRoot -PathType Container))
    {
        Remove-Item -LiteralPath $resolvedScratchRoot -Recurse -Force
    }
}
