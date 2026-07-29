[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$DestinationDirectory,

    [switch]$Force
)

$ErrorActionPreference = 'Stop'
$runningOnWindows = [Environment]::OSVersion.Platform -eq [PlatformID]::Win32NT
$DestinationDirectory = [IO.Path]::GetFullPath($DestinationDirectory)
$executableName = if ($runningOnWindows) { 'steamcmd.exe' } else { 'steamcmd.sh' }
$steamCmdPath = Join-Path $DestinationDirectory $executableName

if ((Test-Path -LiteralPath $steamCmdPath -PathType Leaf) -and -not $Force)
{
    Write-Output $steamCmdPath
    return
}

New-Item -ItemType Directory -Path $DestinationDirectory -Force | Out-Null
$archiveName = if ($runningOnWindows) { 'steamcmd.zip' } else { 'steamcmd_linux.tar.gz' }
$downloadUri = if ($runningOnWindows)
{
    'https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip'
}
else
{
    'https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz'
}
$archivePath = Join-Path $DestinationDirectory $archiveName

Write-Host "Downloading SteamCMD from $downloadUri"
Invoke-WebRequest -Uri $downloadUri -OutFile $archivePath

if ($runningOnWindows)
{
    Expand-Archive -LiteralPath $archivePath -DestinationPath $DestinationDirectory -Force
}
else
{
    & tar -xzf $archivePath -C $DestinationDirectory
    if ($LASTEXITCODE -ne 0)
    {
        throw "Could not extract SteamCMD; tar exited with code $LASTEXITCODE."
    }
    & chmod +x $steamCmdPath
    if ($LASTEXITCODE -ne 0)
    {
        throw "Could not mark '$steamCmdPath' executable."
    }
}

if (-not (Test-Path -LiteralPath $steamCmdPath -PathType Leaf))
{
    throw "SteamCMD was not created at '$steamCmdPath'."
}

Write-Output $steamCmdPath
