[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',

    [string]$PowerShellExecutable
)

$ErrorActionPreference = 'Stop'
if ([string]::IsNullOrWhiteSpace($PowerShellExecutable))
{
    $PowerShellExecutable = if (Get-Command pwsh -ErrorAction SilentlyContinue)
    {
        'pwsh'
    }
    else
    {
        'powershell'
    }
}

$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$projectPath = Join-Path $repositoryRoot 'AGMLIB\AGMLIB.csproj'
$sourceManifestPath = Join-Path $repositoryRoot 'AGMLIB\ModInfo.xml'

function Get-RepositorySourceState
{
    $state = [ordered]@{}
    $trackedFiles = & git -C $repositoryRoot ls-files
    if ($LASTEXITCODE -ne 0)
    {
        throw 'Unable to enumerate tracked files.'
    }

    foreach ($relativePath in $trackedFiles)
    {
        $fullPath = Join-Path $repositoryRoot $relativePath
        $state["tracked:$relativePath"] = if (Test-Path -LiteralPath $fullPath -PathType Leaf)
        {
            (Get-FileHash -LiteralPath $fullPath -Algorithm SHA256).Hash
        }
        else
        {
            '<missing>'
        }
    }

    $state['generated-source:AGMLIB/ModInfo.xml'] = if (Test-Path -LiteralPath $sourceManifestPath -PathType Leaf)
    {
        (Get-FileHash -LiteralPath $sourceManifestPath -Algorithm SHA256).Hash
    }
    else
    {
        '<missing>'
    }

    return $state
}

$before = Get-RepositorySourceState

& dotnet clean $projectPath -v:minimal `
    "-p:Configuration=$Configuration" `
    -p:DeployToGame=false `
    "-p:PowerShellExecutable=$PowerShellExecutable"
if ($LASTEXITCODE -ne 0)
{
    throw "AGMLIB clean failed with exit code $LASTEXITCODE."
}

& dotnet build $projectPath --no-restore -v:minimal `
    "-p:Configuration=$Configuration" `
    -p:DeployToGame=false `
    "-p:PowerShellExecutable=$PowerShellExecutable"
if ($LASTEXITCODE -ne 0)
{
    throw "AGMLIB build failed with exit code $LASTEXITCODE."
}

$after = Get-RepositorySourceState
$changed = @(
    foreach ($key in $before.Keys)
    {
        if ($before[$key] -ne $after[$key])
        {
            $key
        }
    }
)

if ($changed.Count -gt 0)
{
    throw "Ordinary build changed repository source state:`n$($changed -join [Environment]::NewLine)"
}

$artifactManifest = Join-Path $repositoryRoot 'artifacts\AGMLIB\ModInfo.xml'
if (-not (Test-Path -LiteralPath $artifactManifest -PathType Leaf))
{
    throw "Generated artifact manifest was not found at $artifactManifest"
}

Write-Host "AGMLIB $Configuration repository-only build completed without changing tracked files or AGMLIB\ModInfo.xml."
Write-Host "Generated manifest: $artifactManifest"
