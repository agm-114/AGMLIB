param(
    [string]$Pattern = 'AGMLIB|Exception|Error',
    [int]$Tail = 500
)

$logPath = Join-Path $env:USERPROFILE 'AppData\LocalLow\Eridanus Industries\Nebulous\Player.log'

if (-not (Test-Path -LiteralPath $logPath))
{
    throw "Nebulous Player.log was not found at $logPath"
}

Get-Content -LiteralPath $logPath -Tail $Tail |
    Select-String -Pattern $Pattern -Context 1, 2
