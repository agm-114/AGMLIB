[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectRoot,
    [Parameter(Mandatory = $true)]
    [string]$StatePath,
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$BaseVersion,
    [Parameter(Mandatory = $true)]
    [ValidateRange(0, 65534)]
    [int]$BaseRevision
)

$ErrorActionPreference = 'Stop'
$ProjectRoot = [IO.Path]::GetFullPath($ProjectRoot)
$StatePath = [IO.Path]::GetFullPath($StatePath)
$stateDirectory = Split-Path -Parent $StatePath
New-Item -ItemType Directory -Force $stateDirectory | Out-Null

$includedExtensions = @(
    '.cs',
    '.csproj',
    '.dll',
    '.jpg',
    '.props',
    '.resx',
    '.settings'
)

$files = @(
    Get-ChildItem -LiteralPath $ProjectRoot -Recurse -File |
        Where-Object {
            $relativePath = $_.FullName.Substring($ProjectRoot.Length + 1)
            $topLevelDirectory = ($relativePath -split '[\\/]')[0]
            $includedExtensions -contains $_.Extension.ToLowerInvariant() -and
                $topLevelDirectory -notin @('bin', 'obj', 'ProjectFiles')
        }
)

$templatePath = Join-Path $ProjectRoot 'ModInfo.template.xml'
if (Test-Path -LiteralPath $templatePath -PathType Leaf)
{
    $files += Get-Item -LiteralPath $templatePath
}

$fingerprintLines = @(
    foreach ($file in $files | Sort-Object FullName -Unique)
    {
        $relativePath = $file.FullName.Substring($ProjectRoot.Length + 1).Replace('\', '/')
        $contentHash = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
        "$relativePath`0$contentHash"
    }

    $versionScriptHash = (Get-FileHash -LiteralPath $PSCommandPath -Algorithm SHA256).Hash
    "../scripts/Build/Get-AgmlibBuildVersion.ps1`0$versionScriptHash"
)

$fingerprintText = $fingerprintLines -join "`n"
$fingerprintBytes = [Text.Encoding]::UTF8.GetBytes($fingerprintText)
$sha256 = [Security.Cryptography.SHA256]::Create()
try
{
    $fingerprint = [BitConverter]::ToString($sha256.ComputeHash($fingerprintBytes)).Replace('-', '')
}
finally
{
    $sha256.Dispose()
}

$lockPath = "$StatePath.lock"
$lockStream = $null
for ($attempt = 0; $attempt -lt 50 -and $null -eq $lockStream; $attempt++)
{
    try
    {
        $lockStream = [IO.File]::Open($lockPath, 'OpenOrCreate', 'ReadWrite', 'None')
    }
    catch [IO.IOException]
    {
        Start-Sleep -Milliseconds 100
    }
}

if ($null -eq $lockStream)
{
    throw "Timed out waiting for the AGMLIB build-version ledger lock at $lockPath"
}

try
{
    $revision = $BaseRevision
    $existingState = $null
    if (Test-Path -LiteralPath $StatePath -PathType Leaf)
    {
        $existingState = Get-Content -LiteralPath $StatePath -Raw | ConvertFrom-Json
    }

    $sameBaseline = $null -ne $existingState -and
        $existingState.base_version -eq $BaseVersion -and
        [int]$existingState.base_revision -eq $BaseRevision

    if ($sameBaseline)
    {
        $revision = [int]$existingState.revision
        if ($existingState.fingerprint -ne $fingerprint)
        {
            $revision++
        }
    }

    if ($revision -gt 65534)
    {
        throw "AGMLIB build revision $revision exceeds the .NET assembly-version limit of 65534."
    }

    $state = [ordered]@{
        base_version = $BaseVersion
        base_revision = $BaseRevision
        fingerprint = $fingerprint
        revision = $revision
    }

    $stateJson = $state | ConvertTo-Json
    $temporaryStatePath = "$StatePath.$PID.tmp"
    [IO.File]::WriteAllText(
        $temporaryStatePath,
        $stateJson + [Environment]::NewLine,
        (New-Object Text.UTF8Encoding($false)))
    Move-Item -LiteralPath $temporaryStatePath -Destination $StatePath -Force
}
finally
{
    $lockStream.Dispose()
}

Write-Output "$BaseVersion.$revision"
