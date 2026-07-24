[CmdletBinding()]
param(
    [string]$GameRoot = (Join-Path ${env:ProgramFiles(x86)} 'Steam\steamapps\common\Nebulous'),

    [string]$OutputRoot,

    [string[]]$AssemblyNames = @(
        'Nebulous.dll',
        'Assembly-CSharp.dll',
        'ProceduralPlanets.dll',
        'UINC4.dll'
    ),

    [switch]$AllManagedAssemblies,

    [switch]$Force,

    [switch]$PackWithRepomix,

    [switch]$ValidateOnly
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$managedRoot = [IO.Path]::GetFullPath((Join-Path $GameRoot 'Nebulous_Data\Managed'))
$nativeRoot = [IO.Path]::GetFullPath((Join-Path $repositoryRoot '.native-code'))

if ([string]::IsNullOrWhiteSpace($OutputRoot))
{
    $OutputRoot = Join-Path $nativeRoot 'current'
}

$OutputRoot = [IO.Path]::GetFullPath($OutputRoot)
$nativePrefix = $nativeRoot.TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
if (-not $OutputRoot.StartsWith($nativePrefix, [StringComparison]::OrdinalIgnoreCase))
{
    throw "OutputRoot must stay inside the ignored native-code workspace: $nativeRoot"
}

if (-not (Test-Path -LiteralPath $managedRoot -PathType Container))
{
    throw "NEBULOUS managed assembly directory was not found: $managedRoot"
}

$ilspyCommand = Get-Command ilspycmd -ErrorAction SilentlyContinue
if (-not $ilspyCommand)
{
    throw 'ilspycmd is required. Install it with: dotnet tool install --global ilspycmd'
}

$assemblies = if ($AllManagedAssemblies)
{
    Get-ChildItem -LiteralPath $managedRoot -Filter '*.dll' -File | Sort-Object Name
}
else
{
    foreach ($assemblyName in $AssemblyNames)
    {
        $assemblyPath = Join-Path $managedRoot $assemblyName
        if (-not (Test-Path -LiteralPath $assemblyPath -PathType Leaf))
        {
            throw "Requested managed assembly was not found: $assemblyPath"
        }

        Get-Item -LiteralPath $assemblyPath
    }
}

Write-Host "Repository: $repositoryRoot"
Write-Host "Managed assemblies: $managedRoot"
Write-Host "Ignored native-code workspace: $OutputRoot"
Write-Host "Assemblies: $($assemblies.Name -join ', ')"

if ($ValidateOnly)
{
    Write-Host 'Native-code dump paths and tools are valid.'
    return
}

New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null
$assemblyOutputRoot = Join-Path $OutputRoot 'assemblies'
$indexRoot = Join-Path $OutputRoot 'indexes'
New-Item -ItemType Directory -Force -Path $assemblyOutputRoot, $indexRoot | Out-Null

$records = New-Object System.Collections.Generic.List[object]
foreach ($assembly in $assemblies)
{
    $hash = (Get-FileHash -LiteralPath $assembly.FullName -Algorithm SHA256).Hash
    $shortHash = $hash.Substring(0, 12)
    $baseName = [IO.Path]::GetFileNameWithoutExtension($assembly.Name)
    $destination = Join-Path $assemblyOutputRoot "$baseName-$shortHash"
    $completionPath = Join-Path $destination '.agmlib-native-dump.json'
    $assemblyPrefix = $assemblyOutputRoot.TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar

    if ($Force -and (Test-Path -LiteralPath $destination))
    {
        $resolvedDestination = [IO.Path]::GetFullPath($destination)
        if (-not $resolvedDestination.StartsWith($assemblyPrefix, [StringComparison]::OrdinalIgnoreCase))
        {
            throw "Refusing to clear an assembly dump outside $assemblyOutputRoot"
        }

        Remove-Item -LiteralPath $resolvedDestination -Recurse -Force
    }

    if ((Test-Path -LiteralPath $destination -PathType Container) -and
        -not (Test-Path -LiteralPath $completionPath -PathType Leaf))
    {
        throw "Incomplete native dump detected at $destination. Re-run with -Force."
    }

    if (-not (Test-Path -LiteralPath $destination -PathType Container))
    {
        $temporaryDestination = "$destination.incomplete-$PID"
        if (Test-Path -LiteralPath $temporaryDestination)
        {
            $resolvedTemporaryDestination = [IO.Path]::GetFullPath($temporaryDestination)
            if (-not $resolvedTemporaryDestination.StartsWith($assemblyPrefix, [StringComparison]::OrdinalIgnoreCase))
            {
                throw "Refusing to clear an incomplete dump outside $assemblyOutputRoot"
            }
            Remove-Item -LiteralPath $resolvedTemporaryDestination -Recurse -Force
        }
        New-Item -ItemType Directory -Force -Path $temporaryDestination | Out-Null
        & $ilspyCommand.Source --disable-updatecheck --nested-directories -p `
            -r $managedRoot -o $temporaryDestination $assembly.FullName
        if ($LASTEXITCODE -ne 0)
        {
            throw "ilspycmd failed while decompiling $($assembly.FullName) with exit code $LASTEXITCODE."
        }
        $sourceFileCount = (Get-ChildItem -LiteralPath $temporaryDestination -Recurse -Filter '*.cs' -File).Count
        [pscustomobject][ordered]@{
            assembly = $assembly.Name
            sha256 = $hash
            sourceFileCount = $sourceFileCount
            ilspy = (& $ilspyCommand.Source --version | Select-Object -First 1)
        } | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $temporaryDestination '.agmlib-native-dump.json') -Encoding UTF8
        Move-Item -LiteralPath $temporaryDestination -Destination $destination
    }

    $typeIndexPath = Join-Path $indexRoot "$baseName.types.txt"
    $typeIndex = foreach ($entityType in @('c', 'i', 's', 'd', 'e'))
    {
        $entityOutput = @(& $ilspyCommand.Source --disable-updatecheck -l $entityType $assembly.FullName)
        if ($LASTEXITCODE -ne 0)
        {
            throw "ilspycmd failed while indexing entity type '$entityType' in $($assembly.FullName)."
        }
        $entityOutput
    }
    Set-Content -LiteralPath $typeIndexPath -Value @($typeIndex) -Encoding UTF8

    $resourceIndexPath = Join-Path $indexRoot "$baseName.resources.txt"
    $resourceIndex = @(& $ilspyCommand.Source --disable-updatecheck --list-resources $assembly.FullName)
    if ($LASTEXITCODE -ne 0)
    {
        throw "ilspycmd failed while indexing resources in $($assembly.FullName)."
    }
    Set-Content -LiteralPath $resourceIndexPath -Value $resourceIndex -Encoding UTF8

    $assemblyIdentity = [Reflection.AssemblyName]::GetAssemblyName($assembly.FullName)
    $records.Add([pscustomobject][ordered]@{
        name = $assembly.Name
        sourcePath = $assembly.FullName
        assemblyVersion = $assemblyIdentity.Version.ToString()
        fileVersion = $assembly.VersionInfo.FileVersion
        lastWriteTimeUtc = $assembly.LastWriteTimeUtc.ToString('O')
        sha256 = $hash
        decompiledPath = $destination
        sourceFileCount = (Get-ChildItem -LiteralPath $destination -Recurse -Filter '*.cs' -File).Count
        typeIndex = $typeIndexPath
        resourceIndex = $resourceIndexPath
    })
}

$ilspyVersion = (& $ilspyCommand.Source --version | Select-Object -First 1)
$manifest = [pscustomobject][ordered]@{
    schemaVersion = 1
    generatedAtUtc = [DateTime]::UtcNow.ToString('O')
    repository = $repositoryRoot
    gameRoot = [IO.Path]::GetFullPath($GameRoot)
    managedRoot = $managedRoot
    ilspyVersion = $ilspyVersion
    assemblies = @($records | ForEach-Object { $_ })
}

$manifestPath = Join-Path $OutputRoot 'manifest.json'
$manifest | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $manifestPath -Encoding UTF8

$workspaceGuide = @"
# Generated native-code workspace

This directory is intentionally git-ignored. It contains reconstructed C# from
the currently installed NEBULOUS managed assemblies and is not original source.

Regenerate:

``````powershell
.\scripts\NativeCode\Export-NebulousManagedCode.ps1 -PackWithRepomix
``````

Search:

``````
rg -n "TypeOrMember" .native-code/current/assemblies
sg run -p 'class `$C' -l csharp .native-code/current/assemblies
``````

Evidence identity is recorded in `manifest.json`. Rebuild after every game
update and do not cite an old dump as proof of current runtime behavior.
"@
$workspaceGuide | Set-Content -LiteralPath (Join-Path $OutputRoot 'AGENTS.md') -Encoding UTF8

if ($PackWithRepomix)
{
    $npxCommand = Get-Command npx -ErrorAction SilentlyContinue
    if (-not $npxCommand)
    {
        throw 'npx is required for -PackWithRepomix.'
    }

    $repomixOutput = Join-Path $OutputRoot 'repomix-native.xml'
    & $npxCommand.Source --yes repomix@latest $assemblyOutputRoot --compress --no-gitignore --output $repomixOutput
    if ($LASTEXITCODE -ne 0)
    {
        throw "Repomix failed with exit code $LASTEXITCODE."
    }
}

[pscustomobject][ordered]@{
    valid = $true
    outputRoot = $OutputRoot
    manifest = $manifestPath
    assemblyCount = $records.Count
    sourceFileCount = ($records | Measure-Object sourceFileCount -Sum).Sum
    repomix = if ($PackWithRepomix) { Join-Path $OutputRoot 'repomix-native.xml' } else { $null }
} | ConvertTo-Json -Depth 4
