[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$TypeName,

    [string]$AssemblyName = 'Nebulous.dll',

    [string]$AssemblyPath,

    [string]$GameRoot = 'C:\Program Files (x86)\Steam\steamapps\common\Nebulous',

    [string]$Pattern,

    [string]$OutputRoot
)

$ErrorActionPreference = 'Stop'

$repoRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..\..'))
if ([string]::IsNullOrWhiteSpace($OutputRoot)) {
    $OutputRoot = Join-Path $repoRoot '.agents\cache\neb-runtime-research'
}

if ([string]::IsNullOrWhiteSpace($AssemblyPath)) {
    $candidates = @(
        (Join-Path $GameRoot "Nebulous_Data\Managed\$AssemblyName"),
        (Join-Path $repoRoot "AGMLIB\libs\$AssemblyName")
    )
    $AssemblyPath = $candidates | Where-Object { Test-Path -LiteralPath $_ -PathType Leaf } | Select-Object -First 1
}

if ([string]::IsNullOrWhiteSpace($AssemblyPath) -or !(Test-Path -LiteralPath $AssemblyPath -PathType Leaf)) {
    throw "Managed assembly not found. Pass -AssemblyPath or verify -GameRoot and -AssemblyName."
}

$ilspyCommand = Get-Command ilspycmd -ErrorAction SilentlyContinue
if ($ilspyCommand) {
    $ilspy = $ilspyCommand.Source
} else {
    $ilspy = Join-Path $env:USERPROFILE '.dotnet\tools\ilspycmd.exe'
    if (!(Test-Path -LiteralPath $ilspy -PathType Leaf)) {
        throw 'ilspycmd was not found on PATH or under %USERPROFILE%\.dotnet\tools. Install it with: dotnet tool install --global ilspycmd'
    }
}

$assembly = Get-Item -LiteralPath $AssemblyPath
$hash = (Get-FileHash -LiteralPath $assembly.FullName -Algorithm SHA256).Hash
$safeTypeName = $TypeName -replace '[^A-Za-z0-9_.-]', '_'
$destination = Join-Path $OutputRoot (Join-Path $hash.Substring(0, 12) $safeTypeName)
New-Item -ItemType Directory -Force -Path $destination | Out-Null

$existing = Get-ChildItem -LiteralPath $destination -Filter '*.cs' -File -ErrorAction SilentlyContinue | Select-Object -First 1
if (!$existing) {
    & $ilspy -t $TypeName -o $destination $assembly.FullName
    $decompileExitCode = $LASTEXITCODE
    if ($decompileExitCode -ne 0) {
        $shortTypeName = ($TypeName -split '[.+]')[-1]
        Write-Warning "Could not decompile '$TypeName'. Likely class-name matches in $($assembly.Name):"
        & $ilspy -l c $assembly.FullName |
            Select-String -Pattern ([regex]::Escape($shortTypeName)) |
            Select-Object -First 20 |
            ForEach-Object { Write-Warning $_.Line.Trim() }
        throw "ilspycmd failed with exit code $decompileExitCode."
    }
}

$source = Get-ChildItem -LiteralPath $destination -Filter '*.cs' -File | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (!$source) {
    throw "ilspycmd did not produce a C# file for '$TypeName'."
}

[pscustomobject]@{
    TypeName = $TypeName
    Assembly = $assembly.FullName
    LastWriteTime = $assembly.LastWriteTime
    SHA256 = $hash
    DecompiledSource = $source.FullName
} | Format-List

if (![string]::IsNullOrWhiteSpace($Pattern)) {
    $rg = Get-Command rg -ErrorAction SilentlyContinue
    if ($rg) {
        & $rg.Source -n -C 3 -- $Pattern $source.FullName
    } else {
        Select-String -LiteralPath $source.FullName -Pattern $Pattern -Context 3, 3
    }
}
