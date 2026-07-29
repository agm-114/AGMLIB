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
$fakeSupportAssembly = Join-Path $scratchRoot 'AGMLIB.CI.TestSupport.dll'

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
    Copy-Item `
        -LiteralPath (Join-Path $PackageRoot "$Configuration/net481/AGMLIB.dll") `
        -Destination $fakeSupportAssembly
    & (Join-Path $PSScriptRoot 'Stage-AgmlibIntegrationMod.ps1') `
        -Configuration $Configuration `
        -PackageRoot $PackageRoot `
        -WorkshopItemDirectory $workshopRoot `
        -CiTestSupportAssemblyPath $fakeSupportAssembly | Out-Null
    if (-not (Test-Path -LiteralPath (Join-Path $workshopRoot 'workshop-baseline.txt') -PathType Leaf))
    {
        throw 'Workshop overlay removed an existing workshop file.'
    }
    $stagedSupportAssembly = Join-Path $workshopRoot "$Configuration/net481/AGMLIB.CI.TestSupport.dll"
    [xml]$stagedManifest = Get-Content -LiteralPath (Join-Path $workshopRoot 'ModInfo.xml') -Raw
    if (-not (Test-Path -LiteralPath $stagedSupportAssembly -PathType Leaf) -or
        $stagedManifest.ModInfo.Assemblies.string -notcontains "$Configuration/net481/AGMLIB.CI.TestSupport.dll")
    {
        throw 'Workshop overlay did not stage and register the CI test-support assembly.'
    }

    $configFixture = @(
        '<?xml version="1.0" encoding="utf-8"?>'
        '<SkirmishDedicatedServerConfig>'
        '  <ServerName>Default</ServerName>'
        '  <GamePort>7777</GamePort>'
        '  <QueryPort>27016</QueryPort>'
        '  <MaxPlayers>10</MaxPlayers>'
        '  <TeamSizeToStart>1</TeamSizeToStart>'
        '</SkirmishDedicatedServerConfig>'
    ) -join "`r`n"
    [IO.File]::WriteAllText($sourceConfig, $configFixture, [Text.UTF8Encoding]::new($false))
    & (Join-Path $PSScriptRoot 'New-NebulousIntegrationConfig.ps1') `
        -SourceConfigPath $sourceConfig `
        -DestinationConfigPath $generatedConfig `
        -ConfigureHeadlessMatch | Out-Null

    [xml]$generated = Get-Content -LiteralPath $generatedConfig -Raw
    if ([string]$generated.SkirmishDedicatedServerConfig.ServerName -ne 'AGMLIB CI Smoke' -or
        [string]$generated.SkirmishDedicatedServerConfig.Mods.unsignedLong -ne '2960504230')
    {
        throw 'Generated integration config did not contain the expected server name and AGMLIB mod ID.'
    }
    $generatedBots = @($generated.SkirmishDedicatedServerConfig.Bots.Bot)
    if ($generatedBots.Count -ne 2 -or
        @($generatedBots.Team) -notcontains 'TeamA' -or
        @($generatedBots.Team) -notcontains 'TeamB' -or
        @($generatedBots.Fleet) -notcontains 'Starter Fleets - Alliance/TF Oak.fleet' -or
        @($generatedBots.Fleet) -notcontains 'Starter Fleets - Protectorate/Tantalum Squadron.fleet')
    {
        throw 'Generated integration config did not contain the deterministic two-bot match fixture.'
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
if (Environment.GetEnvironmentVariable("AGMLIB_CI_AUTOSTART_MATCH") != "1")
{
    throw new InvalidOperationException("AGMLIB_CI_AUTOSTART_MATCH was not enabled.");
}
if (Environment.GetEnvironmentVariable("AGMLIB_PREFAB_DUMP_IMMEDIATE") != "1")
{
    throw new InvalidOperationException("AGMLIB_PREFAB_DUMP_IMMEDIATE was not enabled.");
}

Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(logPath))!);
Directory.CreateDirectory(dumpPath);
File.WriteAllText(
    logPath,
    """
Downloading mods
Finished downloading mod 'AGMLIB'
>>>>> Beginning Load of Mod 'AGMLIB' >>>>>
Loaded assembly AGMLIB, Version=6.2.2.940, Culture=neutral, PublicKeyToken=null
Loaded assembly AGMLIB.CI.TestSupport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
Finished Loading Mod 'AGMLIB'. Result: Loaded
All assets loaded. Starting dedicated server.
[TestingComponents] Discovery complete: discovered=0, created=0, skipped=0, failed=0.
[PrefabYamlDump] Completed path='fixture' prefabs=1 enabledMods=1 errors=0.
Server: listening port=17777
Dedicated server startup completed
[AGMLIB CI] headless-match support enabled
[AGMLIB CI] launching headless match players=2 bots=2
Scene change start. New scene: 'SkirmishMapContainer'
SkirmishGameManager - Host Started
Finished loading scene in server-only mode.
Changing server game state to WaitingForClients
Changing server game state to LoadingMap
[AGMLIB CI] waiting for dedicated-server map instantiation
All clients finished loading map.
Changing server game state to TransferringFleets
All fleets uploaded to host
Changing server game state to SpawningFleet
[AGMLIB CI] waiting for bot fleet initialization
[AGMLIB CI] suppressing bot-only return to lobby
Finished spawning fleets
Changing server game state to ChooseSpawn
Changing server game state to Arriving
Changing server game state to Running
GO!
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
        -TimeoutSeconds 45 `
        -RequireGameplayReady

    $smokeSummary = Get-Content -LiteralPath (Join-Path $smokeOutput 'summary.json') -Raw | ConvertFrom-Json
    if ($smokeSummary.succeeded -ne $true)
    {
        throw 'The fake dedicated-server smoke test did not report success.'
    }
    $expectedLifecycleEvents = @(
        'server-process-started',
        'mods-download-started',
        'mod-download-completed',
        'mod-load-started',
        'agmlib-assembly-loaded',
        'ci-support-loaded',
        'mod-load-completed',
        'all-mod-assets-loaded',
        'server-listening',
        'lobby-ready',
        'match-launch-requested',
        'match-scene-loading',
        'match-host-started',
        'match-scene-loaded',
        'waiting-for-clients',
        'map-loading',
        'map-loaded',
        'fleets-transferring',
        'fleets-uploaded',
        'fleets-spawning',
        'bot-fleets-initializing',
        'fleets-spawned',
        'spawn-selection-started',
        'ships-arriving',
        'gameplay-state-running',
        'gameplay-started'
    )
    $missingLifecycleEvents = $expectedLifecycleEvents |
        Where-Object { $_ -notin $smokeSummary.lifecycle_events }
    if ($missingLifecycleEvents.Count -gt 0)
    {
        throw "Smoke test did not emit lifecycle events: $($missingLifecycleEvents -join ', ')."
    }

    $report = [ordered]@{
        configuration = $Configuration
        package_validation = 'passed'
        workshop_overlay = 'passed'
        config_generation = 'passed'
        smoke_process_contract = 'passed'
        gameplay_ready_contract = 'passed'
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
