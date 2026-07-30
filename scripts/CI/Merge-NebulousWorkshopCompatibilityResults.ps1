[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$ResultsRoot,

    [Parameter(Mandatory)]
    [string]$OutputDirectory
)

$ErrorActionPreference = 'Stop'
$ResultsRoot = [IO.Path]::GetFullPath($ResultsRoot)
$OutputDirectory = [IO.Path]::GetFullPath($OutputDirectory)
if (-not (Test-Path -LiteralPath $ResultsRoot -PathType Container))
{
    throw "Workshop compatibility results root was not found at '$ResultsRoot'."
}

$resultFiles = @(
    Get-ChildItem -LiteralPath $ResultsRoot -Filter 'compatibility-result.json' -File -Recurse
)
if ($resultFiles.Count -eq 0)
{
    throw "No compatibility-result.json files were found under '$ResultsRoot'."
}

$results = @(
    $resultFiles |
        ForEach-Object { Get-Content -LiteralPath $_.FullName -Raw | ConvertFrom-Json } |
        Sort-Object target_name
)
$passed = @($results | Where-Object { $_.succeeded -eq $true })
$failed = @($results | Where-Object { $_.succeeded -ne $true })
$summary = [ordered]@{
    schema_version = 1
    generated_utc = [DateTime]::UtcNow.ToString('o')
    result_count = $results.Count
    passed = $passed.Count
    failed = $failed.Count
    results = $results
}

New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$jsonPath = Join-Path $OutputDirectory 'summary.json'
$markdownPath = Join-Path $OutputDirectory 'summary.md'
$summary | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $jsonPath -Encoding utf8

$markdown = [Collections.Generic.List[string]]::new()
$markdown.Add('# NEBULOUS Workshop compatibility')
$markdown.Add('')
$markdown.Add(
    "**$($passed.Count) passed, $($failed.Count) failed, $($results.Count) total.**")
$markdown.Add('')
$markdown.Add('| Workshop mod | ID | Download | Structure | Match | Result |')
$markdown.Add('| --- | ---: | --- | --- | --- | --- |')
foreach ($result in $results)
{
    $resultText = if ($result.succeeded -eq $true) { 'PASS' } else { 'FAIL' }
    $markdown.Add(
        "| $($result.target_name) | $($result.target_workshop_item_id) | " +
        "$($result.download) | $($result.structure) | $($result.match) | $resultText |")
}
$markdown | Set-Content -LiteralPath $markdownPath -Encoding utf8

Write-Host (
    "Merged $($results.Count) Workshop compatibility result(s): " +
    "$($passed.Count) passed, $($failed.Count) failed.")
$summary
