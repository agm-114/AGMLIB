[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$SourceConfigPath,

    [Parameter(Mandatory)]
    [string]$DestinationConfigPath,

    [string]$ServerName = 'AGMLIB CI Smoke',

    [int]$GamePort = 17777,

    [int]$QueryPort = 27026,

    [int]$MaxPlayers = 2,

    [UInt64[]]$ModIds = @(2960504230),

    [switch]$ConfigureHeadlessMatch
)

$ErrorActionPreference = 'Stop'
$SourceConfigPath = [IO.Path]::GetFullPath($SourceConfigPath)
$DestinationConfigPath = [IO.Path]::GetFullPath($DestinationConfigPath)
if (-not (Test-Path -LiteralPath $SourceConfigPath -PathType Leaf))
{
    throw "Source dedicated-server config was not found at '$SourceConfigPath'."
}
if ($GamePort -lt 1 -or $GamePort -gt 65535 -or $QueryPort -lt 1 -or $QueryPort -gt 65535)
{
    throw 'GamePort and QueryPort must be valid TCP/UDP port numbers.'
}
if ($GamePort -eq $QueryPort)
{
    throw 'GamePort and QueryPort must be different.'
}
if ($MaxPlayers -lt 1)
{
    throw 'MaxPlayers must be at least 1.'
}

[xml]$config = Get-Content -LiteralPath $SourceConfigPath -Raw

function Set-RequiredElementValue
{
    param(
        [Parameter(Mandatory)]
        [xml]$Document,

        [Parameter(Mandatory)]
        [string]$ElementName,

        [Parameter(Mandatory)]
        [string]$Value
    )

    $element = $Document.SelectSingleNode("//$ElementName")
    if ($null -eq $element)
    {
        throw "Dedicated-server config does not contain <$ElementName>."
    }
    $element.InnerText = $Value
}

Set-RequiredElementValue -Document $config -ElementName 'ServerName' -Value $ServerName
Set-RequiredElementValue -Document $config -ElementName 'GamePort' -Value $GamePort.ToString([Globalization.CultureInfo]::InvariantCulture)
Set-RequiredElementValue -Document $config -ElementName 'QueryPort' -Value $QueryPort.ToString([Globalization.CultureInfo]::InvariantCulture)
Set-RequiredElementValue -Document $config -ElementName 'MaxPlayers' -Value $MaxPlayers.ToString([Globalization.CultureInfo]::InvariantCulture)

if ($ConfigureHeadlessMatch)
{
    Set-RequiredElementValue -Document $config -ElementName 'TeamSizeToStart' -Value '1'

    $botsElement = $config.SelectSingleNode('//Bots')
    if ($null -eq $botsElement)
    {
        $configRoot = $config.DocumentElement
        if ($null -eq $configRoot)
        {
            throw 'Dedicated-server config does not have a root element.'
        }

        $botsElement = $config.CreateElement('Bots')
        $followingElement = $configRoot.SelectSingleNode(
            'RankRestriction | AutoBalance | AutoBalanceTriggerThreshold | Competitive | AllowModdedFleets | Mods')
        if ($null -eq $followingElement)
        {
            [void]$configRoot.AppendChild($botsElement)
        }
        else
        {
            [void]$configRoot.InsertBefore($botsElement, $followingElement)
        }
    }
    $botsElement.RemoveAll()

    $botDefinitions = @(
        [ordered]@{
            Team = 'TeamA'
            Difficulty = 'Hard'
            Badge = 'Alliance Roundel'
            Fleet = 'Starter Fleets - Alliance/TF Oak.fleet'
        }
        [ordered]@{
            Team = 'TeamB'
            Difficulty = 'Hard'
            Badge = 'OSP_Roundel'
            Fleet = 'Starter Fleets - Protectorate/Tantalum Squadron.fleet'
        }
    )
    foreach ($botDefinition in $botDefinitions)
    {
        $botElement = $config.CreateElement('Bot')
        foreach ($property in $botDefinition.GetEnumerator())
        {
            $propertyElement = $config.CreateElement($property.Key)
            $propertyElement.InnerText = $property.Value
            [void]$botElement.AppendChild($propertyElement)
        }
        [void]$botsElement.AppendChild($botElement)
    }
}

$modsElement = $config.SelectSingleNode('//Mods')
if ($null -eq $modsElement)
{
    $configRoot = $config.DocumentElement
    if ($null -eq $configRoot)
    {
        throw 'Dedicated-server config does not have a root element.'
    }

    $modsElement = $config.CreateElement('Mods')
    [void]$configRoot.AppendChild($modsElement)
}
$modsElement.RemoveAll()
foreach ($modId in $ModIds)
{
    $modElement = $config.CreateElement('unsignedLong')
    $modElement.InnerText = $modId.ToString([Globalization.CultureInfo]::InvariantCulture)
    [void]$modsElement.AppendChild($modElement)
}

$destinationDirectory = Split-Path -Parent $DestinationConfigPath
New-Item -ItemType Directory -Path $destinationDirectory -Force | Out-Null
$settings = [Xml.XmlWriterSettings]::new()
$settings.Encoding = [Text.UTF8Encoding]::new($false)
$settings.Indent = $true
$settings.NewLineChars = "`r`n"
$settings.NewLineHandling = [Xml.NewLineHandling]::Replace
try
{
    $writer = [Xml.XmlWriter]::Create($DestinationConfigPath, $settings)
    $config.Save($writer)
}
finally
{
    if ($null -ne $writer)
    {
        $writer.Dispose()
    }
}

Write-Host "Created isolated integration config at '$DestinationConfigPath'."
Get-Item -LiteralPath $DestinationConfigPath
