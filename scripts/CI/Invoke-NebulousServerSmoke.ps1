[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$ServerRoot,

    [Parameter(Mandatory)]
    [string]$ConfigPath,

    [Parameter(Mandatory)]
    [string]$OutputDirectory,

    [int]$TimeoutSeconds = 720,

    [int]$HeartbeatSeconds = 15,

    [int]$StallTimeoutSeconds = 90,

    [int]$PhaseTimeoutSeconds = 120,

    [string[]]$RequiredLogPatterns = @(
        "Finished Loading Mod 'AGMLIB'\.\s+Result:\s+Loaded",
        '\[TestingComponents\] Discovery complete: .*failed=0\.',
        '\[PrefabYamlDump\] Completed .*errors=0\.',
        'Server: listening port='
    ),

    [string[]]$ForbiddenLogPatterns = @(
        "Finished Loading Mod 'AGMLIB'\.\s+Result:\s+Failed",
        '\[TestingComponents\].*failed=[1-9][0-9]*',
        '\[PrefabYamlDump\] Failed:',
        '\bHarmonyException\b',
        '\b(TypeLoadException|MissingMethodException|MissingFieldException|NullReferenceException)\b'
    ),

    [string[]]$AdditionalRequiredLogPatterns = @(),

    [string[]]$AdditionalForbiddenLogPatterns = @(),

    [switch]$RequireGameplayReady,

    [switch]$ValidateOnly
)

$ErrorActionPreference = 'Stop'
$runningOnWindows = [Environment]::OSVersion.Platform -eq [PlatformID]::Win32NT
$ServerRoot = [IO.Path]::GetFullPath($ServerRoot)
$ConfigPath = [IO.Path]::GetFullPath($ConfigPath)
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
if (-not (Test-Path -LiteralPath $ServerRoot -PathType Container))
{
    throw "Server root was not found at '$ServerRoot'."
}
if (-not (Test-Path -LiteralPath $ConfigPath -PathType Leaf))
{
    throw "Server config was not found at '$ConfigPath'."
}
if ($TimeoutSeconds -lt 30)
{
    throw 'TimeoutSeconds must be at least 30.'
}
if ($HeartbeatSeconds -lt 5)
{
    throw 'HeartbeatSeconds must be at least 5.'
}
if ($StallTimeoutSeconds -lt 30)
{
    throw 'StallTimeoutSeconds must be at least 30.'
}
if ($PhaseTimeoutSeconds -lt 30)
{
    throw 'PhaseTimeoutSeconds must be at least 30.'
}
if ($RequireGameplayReady)
{
    $RequiredLogPatterns = @($RequiredLogPatterns) + @(
        '\[AGMLIB CI\] headless-match support enabled',
        '\[AGMLIB CI\] launching headless match players=[2-9][0-9]* bots=[2-9][0-9]*',
        '\[AGMLIB CI\] waiting for dedicated-server map instantiation',
        '\[AGMLIB CI\] waiting for bot fleet initialization',
        '\[AGMLIB CI\] suppressing bot-only return to lobby',
        'Finished spawning fleets',
        '(?m)(?:^| - Log - )GO!\r?$'
    )
    $ForbiddenLogPatterns = @($ForbiddenLogPatterns) + @(
        '\[AGMLIB CI\] refusing headless match launch',
        '\[AGMLIB CI\] headless match launch timed out',
        'Could not find fleet .* for bot',
        'Failed to load fleet for bot'
    )
}
$RequiredLogPatterns = @($RequiredLogPatterns) + @($AdditionalRequiredLogPatterns)
$ForbiddenLogPatterns = @($ForbiddenLogPatterns) + @($AdditionalForbiddenLogPatterns)

$serverCandidates = @(
    (Join-Path $ServerRoot 'NebulousDedicatedServer')
    (Join-Path $ServerRoot 'NebulousDedicatedServer.x86_64')
    (Join-Path $ServerRoot 'NebulousDedicatedServer.exe')
)
$serverExecutable = $serverCandidates |
    Where-Object { Test-Path -LiteralPath $_ -PathType Leaf } |
    Select-Object -First 1
if ([string]::IsNullOrWhiteSpace($serverExecutable))
{
    throw "NEBULOUS dedicated-server executable was not found under '$ServerRoot'."
}

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$logPath = Join-Path $OutputDirectory 'server.log'
$stdoutPath = Join-Path $OutputDirectory 'server.stdout.log'
$stderrPath = Join-Path $OutputDirectory 'server.stderr.log'
$prefabDumpPath = Join-Path $OutputDirectory 'prefabs'
$prefabManifestPath = Join-Path $prefabDumpPath 'manifest.yaml'
$summaryPath = Join-Path $OutputDirectory 'summary.json'

if ($ValidateOnly)
{
    [ordered]@{
        validated_only = $true
        server_executable = $serverExecutable
        config_path = $ConfigPath
        output_directory = $OutputDirectory
        heartbeat_seconds = $HeartbeatSeconds
        stall_timeout_seconds = $StallTimeoutSeconds
        phase_timeout_seconds = $PhaseTimeoutSeconds
        required_log_patterns = $RequiredLogPatterns
        forbidden_log_patterns = $ForbiddenLogPatterns
    } | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $summaryPath -Encoding utf8
    Write-Host 'Smoke-test inputs validated; server was not launched.'
    return
}

$startedUtc = [DateTime]::UtcNow
$process = $null
$matchedPatterns = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
$emittedLifecycleEvents = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
$failureMessage = $null
$lastActivityUtc = $startedUtc
$lastHeartbeatUtc = [DateTime]::MinValue
$lastObservedLogLength = 0
$lastObservedDumpBytes = 0L
$lastLogLine = '(no server output yet)'
$lastLifecycleEventUtc = $startedUtc
$lastLifecycleEventName = 'server-process-started'
$oldDumpEnvironment = [Environment]::GetEnvironmentVariable('AGMLIB_PREFAB_DUMP_DIR', 'Process')
$oldImmediateDumpEnvironment = [Environment]::GetEnvironmentVariable('AGMLIB_PREFAB_DUMP_IMMEDIATE', 'Process')
$oldHeadlessMatchEnvironment = [Environment]::GetEnvironmentVariable('AGMLIB_CI_AUTOSTART_MATCH', 'Process')
$oldLibraryPath = [Environment]::GetEnvironmentVariable('LD_LIBRARY_PATH', 'Process')
$lifecycleEvents = @(
    [pscustomobject]@{ Name = 'mods-download-started'; Pattern = '\bDownloading mods\b' }
    [pscustomobject]@{ Name = 'mod-download-completed'; Pattern = "Finished downloading mod '[^']+'" }
    [pscustomobject]@{ Name = 'mod-load-started'; Pattern = "Beginning Load of Mod '[^']+'" }
    [pscustomobject]@{ Name = 'agmlib-assembly-loaded'; Pattern = 'Loaded assembly AGMLIB, Version=[^,\r\n]+' }
    [pscustomobject]@{ Name = 'ci-support-loaded'; Pattern = 'Loaded assembly AGMLIB\.CI\.TestSupport, Version=[^,\r\n]+' }
    [pscustomobject]@{ Name = 'mod-load-completed'; Pattern = "Finished Loading Mod '[^']+'\.\s+Result:\s+\w+" }
    [pscustomobject]@{ Name = 'all-mod-assets-loaded'; Pattern = 'All assets loaded\.\s+Starting dedicated server\.' }
    [pscustomobject]@{ Name = 'server-listening'; Pattern = 'Server: listening port=\d+' }
    [pscustomobject]@{ Name = 'lobby-ready'; Pattern = 'Dedicated server startup completed' }
    [pscustomobject]@{ Name = 'match-launch-requested'; Pattern = '\[AGMLIB CI\] launching headless match[^\r\n]*' }
    [pscustomobject]@{ Name = 'match-scene-loading'; Pattern = "Scene change start\.\s+New scene: 'SkirmishMapContainer'" }
    [pscustomobject]@{ Name = 'match-host-started'; Pattern = 'SkirmishGameManager - Host Started' }
    [pscustomobject]@{ Name = 'match-scene-loaded'; Pattern = 'Finished loading scene in server-only mode\.' }
    [pscustomobject]@{ Name = 'waiting-for-clients'; Pattern = 'Changing server game state to WaitingForClients' }
    [pscustomobject]@{ Name = 'map-loading'; Pattern = 'Changing server game state to LoadingMap' }
    [pscustomobject]@{ Name = 'map-loaded'; Pattern = 'All clients finished loading map\.' }
    [pscustomobject]@{ Name = 'fleets-transferring'; Pattern = 'Changing server game state to TransferringFleets' }
    [pscustomobject]@{ Name = 'fleets-uploaded'; Pattern = 'All fleets uploaded to host' }
    [pscustomobject]@{ Name = 'fleets-spawning'; Pattern = 'Changing server game state to SpawningFleet' }
    [pscustomobject]@{ Name = 'bot-fleets-initializing'; Pattern = '\[AGMLIB CI\] waiting for bot fleet initialization' }
    [pscustomobject]@{ Name = 'fleets-spawned'; Pattern = 'Finished spawning fleets' }
    [pscustomobject]@{ Name = 'spawn-selection-started'; Pattern = 'Changing server game state to ChooseSpawn' }
    [pscustomobject]@{ Name = 'ships-arriving'; Pattern = 'Changing server game state to Arriving' }
    [pscustomobject]@{ Name = 'gameplay-state-running'; Pattern = 'Changing server game state to Running' }
    [pscustomobject]@{ Name = 'gameplay-started'; Pattern = '(?m)(?:^| - Log - )GO!\r?$' }
)

try
{
    [Environment]::SetEnvironmentVariable('AGMLIB_PREFAB_DUMP_DIR', $prefabDumpPath, 'Process')
    [Environment]::SetEnvironmentVariable('AGMLIB_PREFAB_DUMP_IMMEDIATE', '1', 'Process')
    if ($RequireGameplayReady)
    {
        [Environment]::SetEnvironmentVariable('AGMLIB_CI_AUTOSTART_MATCH', '1', 'Process')
    }
    else
    {
        [Environment]::SetEnvironmentVariable('AGMLIB_CI_AUTOSTART_MATCH', $null, 'Process')
    }
    if (-not $runningOnWindows)
    {
        $libraryPaths = @($ServerRoot, (Join-Path $ServerRoot 'linux64'))
        if (-not [string]::IsNullOrWhiteSpace($oldLibraryPath))
        {
            $libraryPaths += $oldLibraryPath
        }
        [Environment]::SetEnvironmentVariable(
            'LD_LIBRARY_PATH',
            ($libraryPaths -join [IO.Path]::PathSeparator),
            'Process')
    }
    $process = Start-Process `
        -FilePath $serverExecutable `
        -ArgumentList @(
            '-nographics',
            '-batchmode',
            '-logFile', $logPath,
            '-serverConfig', $ConfigPath
        ) `
        -WorkingDirectory $ServerRoot `
        -RedirectStandardOutput $stdoutPath `
        -RedirectStandardError $stderrPath `
        -PassThru

    Write-Host (
        "[NEBULOUS event] server-process-started: pid=$($process.Id) " +
        "executable='$serverExecutable'")
    [void]$emittedLifecycleEvents.Add('server-process-started')
    $deadline = [DateTime]::UtcNow.AddSeconds($TimeoutSeconds)
    while ([DateTime]::UtcNow -lt $deadline)
    {
        Start-Sleep -Seconds 2
        $logText = @(
            if (Test-Path -LiteralPath $logPath -PathType Leaf) { Get-Content -LiteralPath $logPath -Raw }
            if (Test-Path -LiteralPath $stdoutPath -PathType Leaf) { Get-Content -LiteralPath $stdoutPath -Raw }
            if (Test-Path -LiteralPath $stderrPath -PathType Leaf) { Get-Content -LiteralPath $stderrPath -Raw }
        ) -join [Environment]::NewLine
        if ($logText.Length -ne $lastObservedLogLength)
        {
            $lastObservedLogLength = $logText.Length
            $lastActivityUtc = [DateTime]::UtcNow
            $lastLogLine = $logText -split '\r?\n' |
                Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
                Select-Object -Last 1
        }

        foreach ($event in $lifecycleEvents)
        {
            $eventMatch = [regex]::Match($logText, $event.Pattern)
            if ($eventMatch.Success -and $emittedLifecycleEvents.Add($event.Name))
            {
                $eventDetail = ($eventMatch.Value -replace '\s+', ' ').Trim()
                $eventMessage = "$($event.Name): $eventDetail"
                Write-Host "[NEBULOUS event] $eventMessage"
                $lastLifecycleEventUtc = [DateTime]::UtcNow
                $lastLifecycleEventName = $event.Name
                if ([Environment]::GetEnvironmentVariable('GITHUB_ACTIONS', 'Process') -eq 'true')
                {
                    Write-Host "::notice title=NEBULOUS integration event::$eventMessage"
                }
            }
        }

        foreach ($pattern in $ForbiddenLogPatterns)
        {
            if ($logText -match $pattern)
            {
                throw "Forbidden log pattern matched: $pattern"
            }
        }
        foreach ($pattern in $RequiredLogPatterns)
        {
            $match = [regex]::Match($logText, $pattern)
            if ($match.Success -and $matchedPatterns.Add($pattern))
            {
                $milestone = ($match.Value -replace '\s+', ' ').Trim()
                if ($milestone.Length -gt 240)
                {
                    $milestone = $milestone.Substring(0, 240) + '...'
                }
                $milestoneMessage = "matched $($matchedPatterns.Count)/$($RequiredLogPatterns.Count): $milestone"
                Write-Host "[NEBULOUS assertion] $milestoneMessage"
            }
        }

        $dumpIsValid = $false
        if (Test-Path -LiteralPath $prefabManifestPath -PathType Leaf)
        {
            $manifestText = Get-Content -LiteralPath $prefabManifestPath -Raw
            $dumpIsValid = $manifestText -match '(?m)^(?:errors|error_count):\s+0\s*$'
            if (-not $dumpIsValid -and
                $manifestText -match '(?m)^(?:errors|error_count):\s+[1-9][0-9]*\s*$')
            {
                throw 'Prefab dump manifest reports one or more serialization errors.'
            }
        }

        $nowUtc = [DateTime]::UtcNow
        if (($nowUtc - $lastHeartbeatUtc).TotalSeconds -ge $HeartbeatSeconds)
        {
            $dumpBytes = 0L
            if (Test-Path -LiteralPath $prefabDumpPath -PathType Container)
            {
                $dumpBytes = [long](Get-ChildItem -LiteralPath $prefabDumpPath -File -Recurse |
                    Measure-Object -Property Length -Sum).Sum
            }
            if ($dumpBytes -ne $lastObservedDumpBytes)
            {
                $lastObservedDumpBytes = $dumpBytes
                $lastActivityUtc = $nowUtc
            }

            $latest = ($lastLogLine -replace '\s+', ' ').Trim()
            if ($latest.Length -gt 240)
            {
                $latest = $latest.Substring(0, 240) + '...'
            }
            $elapsedSeconds = [int]($nowUtc - $startedUtc).TotalSeconds
            $idleSeconds = [int]($nowUtc - $lastActivityUtc).TotalSeconds
            $phaseSeconds = [int]($nowUtc - $lastLifecycleEventUtc).TotalSeconds
            Write-Host (
                "[NEBULOUS heartbeat] elapsed=${elapsedSeconds}s idle=${idleSeconds}s " +
                "phase=${phaseSeconds}s phaseName=$lastLifecycleEventName " +
                "milestones=$($matchedPatterns.Count)/$($RequiredLogPatterns.Count) " +
                "events=$($emittedLifecycleEvents.Count) " +
                "logChars=$lastObservedLogLength dumpBytes=$lastObservedDumpBytes latest='$latest'")
            $lastHeartbeatUtc = $nowUtc
        }

        if ($matchedPatterns.Count -eq $RequiredLogPatterns.Count -and $dumpIsValid)
        {
            break
        }
        if ($process.HasExited)
        {
            throw "Dedicated server exited with code $($process.ExitCode) before smoke-test milestones completed."
        }
        if (([DateTime]::UtcNow - $lastActivityUtc).TotalSeconds -ge $StallTimeoutSeconds)
        {
            $missingPatterns = $RequiredLogPatterns | Where-Object { -not $matchedPatterns.Contains($_) }
            throw (
                "Dedicated server produced no new log or prefab-dump output for $StallTimeoutSeconds seconds. " +
                "Last output: '$lastLogLine'. Missing log patterns: $($missingPatterns -join ', ')")
        }
        if (([DateTime]::UtcNow - $lastLifecycleEventUtc).TotalSeconds -ge $PhaseTimeoutSeconds)
        {
            $missingPatterns = $RequiredLogPatterns | Where-Object { -not $matchedPatterns.Contains($_) }
            throw (
                "Dedicated server did not advance beyond lifecycle event '$lastLifecycleEventName' " +
                "for $PhaseTimeoutSeconds seconds. Last output: '$lastLogLine'. " +
                "Missing log patterns: $($missingPatterns -join ', ')")
        }
    }

    if ($matchedPatterns.Count -ne $RequiredLogPatterns.Count)
    {
        $missingPatterns = $RequiredLogPatterns | Where-Object { -not $matchedPatterns.Contains($_) }
        throw "Smoke test timed out after $TimeoutSeconds seconds. Missing log patterns: $($missingPatterns -join ', ')"
    }
    if (-not (Test-Path -LiteralPath $prefabManifestPath -PathType Leaf))
    {
        throw "Smoke test did not create '$prefabManifestPath'."
    }
}
catch
{
    $failureMessage = $_.Exception.Message
}
finally
{
    [Environment]::SetEnvironmentVariable('AGMLIB_PREFAB_DUMP_DIR', $oldDumpEnvironment, 'Process')
    [Environment]::SetEnvironmentVariable('AGMLIB_PREFAB_DUMP_IMMEDIATE', $oldImmediateDumpEnvironment, 'Process')
    [Environment]::SetEnvironmentVariable('AGMLIB_CI_AUTOSTART_MATCH', $oldHeadlessMatchEnvironment, 'Process')
    if (-not $runningOnWindows)
    {
        [Environment]::SetEnvironmentVariable('LD_LIBRARY_PATH', $oldLibraryPath, 'Process')
    }
    if ($null -ne $process -and -not $process.HasExited)
    {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        [void]$process.WaitForExit(10000)
    }

    $summary = [ordered]@{
        succeeded = [string]::IsNullOrWhiteSpace($failureMessage)
        failure = $failureMessage
        started_utc = $startedUtc.ToString('o')
        finished_utc = [DateTime]::UtcNow.ToString('o')
        timeout_seconds = $TimeoutSeconds
        heartbeat_seconds = $HeartbeatSeconds
        stall_timeout_seconds = $StallTimeoutSeconds
        phase_timeout_seconds = $PhaseTimeoutSeconds
        last_activity_utc = $lastActivityUtc.ToString('o')
        last_log_line = $lastLogLine
        last_lifecycle_event_utc = $lastLifecycleEventUtc.ToString('o')
        last_lifecycle_event = $lastLifecycleEventName
        gameplay_ready_required = [bool]$RequireGameplayReady
        server_executable = $serverExecutable
        config_path = $ConfigPath
        log_path = $logPath
        stdout_path = $stdoutPath
        stderr_path = $stderrPath
        prefab_manifest = $prefabManifestPath
        matched_log_patterns = @($matchedPatterns)
        lifecycle_events = @($emittedLifecycleEvents)
        required_log_patterns = $RequiredLogPatterns
        forbidden_log_patterns = $ForbiddenLogPatterns
        process_exit_code = if ($null -ne $process -and $process.HasExited) { $process.ExitCode } else { $null }
    }
    $summary | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath $summaryPath -Encoding utf8
}

if (-not [string]::IsNullOrWhiteSpace($failureMessage))
{
    throw $failureMessage
}

Write-Host "NEBULOUS dedicated-server smoke test passed. Evidence: '$OutputDirectory'."
