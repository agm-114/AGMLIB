[CmdletBinding()]
param(
    [switch]$Check
)

$ErrorActionPreference = 'Stop'
$repositoryRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$script = Join-Path $PSScriptRoot 'export_agmlib_data_touchpoints.py'
$nativeIndex = Join-Path $repositoryRoot 'knowledge\generated\native-research-index.json'
$assetRoot = Join-Path $repositoryRoot '.native-code\current\assets'
$assetIndex = Join-Path $assetRoot 'index.json'
$behaviours = Join-Path $assetRoot 'mono-behaviours.csv'
$output = Join-Path $repositoryRoot 'planning\generated\native-data-touchpoints.md'

foreach ($required in @($script, $nativeIndex, $assetIndex, $behaviours))
{
    if (-not (Test-Path -LiteralPath $required -PathType Leaf))
    {
        throw "Required input was not found: $required"
    }
}

if ($Check)
{
    $temporary = Join-Path ([IO.Path]::GetTempPath()) ('agmlib-native-data-' + [Guid]::NewGuid().ToString('N') + '.md')
    try
    {
        & python $script `
            --repository-root $repositoryRoot `
            --native-index $nativeIndex `
            --asset-index $assetIndex `
            --mono-behaviours $behaviours `
            --output $temporary
        if ($LASTEXITCODE -ne 0)
        {
            throw "Touchpoint generation failed with exit code $LASTEXITCODE."
        }
        if (-not (Test-Path -LiteralPath $output -PathType Leaf))
        {
            throw "Generated output is missing: $output"
        }
        $expected = [IO.File]::ReadAllText($output)
        $actual = [IO.File]::ReadAllText($temporary)
        if ($expected -cne $actual)
        {
            throw "Generated native/data touchpoint matrix is stale."
        }
    }
    finally
    {
        Remove-Item -LiteralPath $temporary -Force -ErrorAction SilentlyContinue
    }
    return
}

& python $script `
    --repository-root $repositoryRoot `
    --native-index $nativeIndex `
    --asset-index $assetIndex `
    --mono-behaviours $behaviours `
    --output $output
if ($LASTEXITCODE -ne 0)
{
    throw "Touchpoint generation failed with exit code $LASTEXITCODE."
}
