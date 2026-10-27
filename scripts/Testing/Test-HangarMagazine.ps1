[CmdletBinding()]
param(
    [string]$GameRoot = "${env:ProgramFiles(x86)}\Steam\steamapps\common\Nebulous",
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug'
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path (Split-Path $PSScriptRoot)
$managed = Join-Path $GameRoot 'Nebulous_Data\Managed'
$binary = Join-Path $repoRoot "artifacts\AGMLIB\$Configuration\net481\AGMLIB.dll"
$resolver = [ResolveEventHandler] {
    param($sender, $eventArgs)
    $name = ([Reflection.AssemblyName]$eventArgs.Name).Name + '.dll'
    foreach ($directory in @((Split-Path $binary), $managed)) {
        $candidate = Join-Path $directory $name
        if (Test-Path -LiteralPath $candidate) { return [Reflection.Assembly]::UnsafeLoadFrom($candidate) }
    }
    return $null
}
[AppDomain]::CurrentDomain.add_AssemblyResolve($resolver)
try {
    $mod = [Reflection.Assembly]::UnsafeLoadFrom($binary)
    if ($mod.GetType('Lib.Testing.HangarMagazineTestingFactory')) { throw 'Local fixture types leaked into the normal build.' }
    [Reflection.Assembly]::UnsafeLoadFrom((Join-Path $managed 'Nebulous.dll')) | Out-Null
    [Reflection.Assembly]::UnsafeLoadFrom((Join-Path (Split-Path $binary) '0Harmony.dll')) | Out-Null
    $component = $mod.GetType('HangarMagazineComponent', $true)
    if ($component.BaseType -ne [ConfigurableBulkCraftHangarComponent]) { throw 'Expected configurable hangar inheritance.' }
    if (-not ($component.GetInterfaces().FullName -contains 'Ships.Controls.ISyncCraftHangar')) { throw 'Native hangar synchronization is missing.' }
    if ($mod.GetType('HangarMagazineEditorHangar') -or $mod.GetType('HangarMagazineCapacityPatch') -or $mod.GetType('HangarMagazineSyncPatch')) {
        throw 'Obsolete adapters remain in the assembly.'
    }
    Write-Output 'Native hangar inheritance and synchronization verified; obsolete adapters removed'
    foreach ($name in @('RemoveCraftFromStorage', 'RemoveTypeFromStorage', 'UpperStorageBound', 'SpawnStoredCraft', 'GetHangarReference')) {
        if ($component.GetMethod($name).DeclaringType.Assembly -ne [Ships.BulkCraftHangarComponent].Assembly) {
            throw "Hangar operation is no longer inherited from the game: $name"
        }
    }
    $storageMap = $component.GetInterfaceMap([Game.InventoryManagement.IInstanceStorageContainer])
    if (@($storageMap.TargetMethods | Where-Object { $_.DeclaringType -ne [Ships.BaseCraftHangarComponent] }).Count -ne 0) {
        throw 'Craft deposit or withdrawal no longer uses native hangar behavior'
    }
    Write-Output 'Craft storage operations and instance transfers use the native implementations'
    $calculate = $component.GetMethod('CalculateMagazineCapacity', [Reflection.BindingFlags]'Static,NonPublic')
    $cases = @(
        @(1000, 0, 10, 0, 1000), @(1000, 3, 10, 0, 700),
        @(1000, 10, 10, 0, 0), @(1000, 11, 10, 0, 0),
        @(1000, 3, 0, 50, 850), @(1000, 3, 5, 50, 700),
        @(2000, 3, 5, 50, 1550), @(1000, 3, 0, 0, 1000),
        @(1000, 1, 200, 0, 0), @(1000, 2, -10, -50, 1000),
        @(1000, -1, 10, 50, 1000), @(0, 5, 10, 50, 0),
        @(101, 1, 10, 0, 90), @(1000, 1, 0, 0.5, 999)
    )
    foreach ($case in $cases) {
        $actual = $calculate.Invoke($null, [object[]]@([int]$case[0], [int]$case[1], [single]$case[2], [single]$case[3]))
        if ($actual -ne $case[4]) { throw "Capacity mismatch: $case -> $actual" }
    }
    Write-Output "Capacity checks passed: $($cases.Count)"

    $hangarCapacity = $component.GetMethod('CalculateHangarCapacity', [Reflection.BindingFlags]'Static,NonPublic')
    $hangarCases = @(
        @(1000, 999, 8, 5, 50, 0), @(1000, 950, 8, 5, 50, 0),
        @(1000, 900, 8, 5, 50, 1), @(1000, 600, 8, 5, 50, 4),
        @(1000, 500, 8, 5, 50, 5), @(1000, 100, 8, 5, 50, 8),
        @(1000, 0, 8, 5, 50, 8), @(1000, 999, 8, 0, 0, 8),
        @(1000, 500, 8, 10, 0, 5), @(1000, 750, 8, 0, 50, 5),
        @(1000, 900.01, 8, 5, 50, 0), @(1000, 999, 8, 0, 0.5, 2),
        @(1000, 999.2, 8, 0, 0.5, 0), @(1000, 0, 8, 0, 2000, 0),
        @(0, 0, 8, 0, 50, 0), @(1000, 1100, 8, 5, 50, 0)
    )
    foreach ($case in $hangarCases) {
        $actual = $hangarCapacity.Invoke($null, [object[]]@([int]$case[0], [single]$case[1], [int]$case[2], [single]$case[3], [single]$case[4]))
        if ($actual -ne $case[5]) { throw "Hangar capacity mismatch: $case -> $actual" }
    }
    Write-Output "Ammunition-limited hangar checks passed: $($hangarCases.Count)"

    $magazineComponent = $mod.GetType('MagazineComponent', $true)
    if (-not [UnityEngine.MonoBehaviour].IsAssignableFrom($magazineComponent)) { throw 'MagazineComponent must be a MonoBehaviour.' }
    if ($magazineComponent.GetProperty('Collection').PropertyType -ne [Munitions.MagazineCollection]) { throw 'The native magazine collection is not exposed.' }
    $scaling = $magazineComponent.GetMethod('GetLoadScaling', [Reflection.BindingFlags]'Static,NonPublic')
    foreach ($case in @(@(200, 100, 0.5), @(100, 100, $null), @(50, 100, $null), @($null, 100, $null), @(0, 0, $null), @(200, 0, 0))) {
        $volume = if ($null -eq $case[0]) { $null } else { [single]$case[0] }
        $actual = $scaling.Invoke($null, [object[]]@($volume, [int]$case[1]))
        if ($actual -ne $case[2]) { throw "Clipboard scaling mismatch: $case -> $actual" }
    }
    # Allocate inert managed shells to check binding without starting Unity or invoking its constructors.
    Add-Type -WarningAction SilentlyContinue -TypeDefinition 'public class MagazineScaleTestHost : FixedDiscreteMagWeaponComponent { protected override int CalcTileMultiplier() { return 3; } }' -ReferencedAssemblies @(
        $binary, (Join-Path $managed 'Nebulous.dll'), (Join-Path $managed 'UnityEngine.CoreModule.dll'), (Join-Path $managed 'UnityEngine.dll')
    )
    $ownerType = [MagazineScaleTestHost]
    $owner = [Runtime.Serialization.FormatterServices]::GetUninitializedObject($ownerType)
    $support = [Runtime.Serialization.FormatterServices]::GetUninitializedObject($magazineComponent)
    $magazineComponent.GetField('_availableVolume', [Reflection.BindingFlags]'Instance,NonPublic').SetValue($support, 17)
    $initialize = $magazineComponent.GetMethod('Initialize').MakeGenericMethod($ownerType)
    $initialize.Invoke($support, @($owner)) | Out-Null
    $collection = $support.Collection
    $initialize.Invoke($support, @($owner)) | Out-Null
    if (-not [object]::ReferenceEquals($collection, $support.Collection) -or -not [object]::ReferenceEquals($collection.Provider, $owner)) {
        throw 'Initialization changed collection or provider identity'
    }
    if (-not [object]::ReferenceEquals($support.Parent, $owner) -or $support.AvailableVolume -ne 51) {
        throw 'Magazine volume must use its stored parent and virtual tile multiplier exactly once'
    }
    if ($support.GetPointCost() -ne 0 -or -not $support.GetDCTooltipDetails().Contains('$UI_SKIRMISH_DC_PART_EMPTY')) {
        throw 'Empty magazine reporting failed'
    }
    $otherOwner = [Runtime.Serialization.FormatterServices]::GetUninitializedObject($ownerType)
    $rejected = $false
    try { $initialize.Invoke($support, @($otherOwner)) | Out-Null } catch { $rejected = $_.Exception.InnerException -is [InvalidOperationException] }
    if (-not $rejected) { throw 'Rebinding the collection to another provider must fail' }
    Write-Output 'Reusable magazine component, clipboard scaling, provider identity, and empty reporting verified'

    $harmony = New-Object HarmonyLib.Harmony 'agmlib.hangar-magazine.validation'
    foreach ($name in @('HangarMagazineEditorPatch', 'HangarMagazineSerializationPatch')) {
        $harmony.CreateClassProcessor($mod.GetType($name, $true)).Patch() | Out-Null
    }
    Write-Output 'Both Harmony adapters installed against the current game assembly'
    foreach ($name in @('BaseCraftHangarComponentInternals', 'HullComponentInternals', 'SettingsHangarLoadoutInternals')) {
        $refs = $mod.GetType($name, $true).GetNestedType('Refs', [Reflection.BindingFlags]::NonPublic)
        [Runtime.CompilerServices.RuntimeHelpers]::RunClassConstructor($refs.TypeHandle)
    }
    Write-Output 'Native field and method accessors bound successfully'

    $data = New-Object 'HangarMagazineComponent+HangarMagazineData'
    $data.GetType().GetField('ClipboardTotalVolume').SetValue($data, [single]200)
    if ($data -isnot [IMagazineClipboardData] -or ([IMagazineClipboardData]$data).ClipboardTotalVolume -ne 200) {
        throw 'Custom save data does not expose generic clipboard metadata'
    }
    $craft = New-Object SmallCraft.SavedStoredCraft
    $craft.CraftTemplateKey = 'test/craft'
    $data.StoredCraft.Add($craft)
    $ammo = New-Object 'Munitions.Magazine+MagSaveData'
    $ammo.MagazineKey = 'test/magazine'
    $ammo.MunitionKey = 'test/ammo'
    $ammo.Quantity = 42
    $data.Load.Add($ammo)
    $socket = New-Object Ships.Serialization.SerializedHullSocket
    $socket.Key = 'test/socket'
    $socket.ComponentName = 'test/hangar-magazine'
    $socket.ComponentData = $data
    $document = New-Object Xml.XmlDocument
    $document.LoadXml('<Test/>')
    [Utility.XmlDocSerializerExtensions]::AppendToDocument($socket, $document.DocumentElement, 'HullSocket')
    $reader = [Utility.XmlDocSerializerExtensions].GetMethods() | Where-Object { $_.Name -eq 'ReadFromDocumentParent' -and $_.IsGenericMethodDefinition }
    $loaded = $reader.MakeGenericMethod([Ships.Serialization.SerializedHullSocket]).Invoke($null, @($document.DocumentElement, 'HullSocket', $null, $null))
    if ($loaded.ComponentData.StoredCraft[0].CraftTemplateKey -ne 'test/craft' -or $loaded.ComponentData.Load[0].Quantity -ne 42) {
        throw 'Combined save payload did not round-trip'
    }
    Write-Output 'Native socket XML round-trip retained both ammunition and craft'
    $document.LoadXml('<Test xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><HullSocket><Key>old/socket</Key><ComponentName>test/hangar-magazine</ComponentName><ComponentData xsi:type="HangarMagazineData"><Load><MagSaveData><MagazineKey>old/magazine</MagazineKey><MunitionKey>old/ammo</MunitionKey><Quantity>12</Quantity></MagSaveData></Load><StoredCraft><SavedStoredCraft><CraftTemplateKey>old/craft</CraftTemplateKey></SavedStoredCraft></StoredCraft></ComponentData></HullSocket></Test>')
    $loaded = $reader.MakeGenericMethod([Ships.Serialization.SerializedHullSocket]).Invoke($null, @($document.DocumentElement, 'HullSocket', $null, $null))
    if ($loaded.ComponentData -isnot [Ships.BaseCraftHangarComponent+CraftHangarData] -or $loaded.ComponentData.StoredCraft[0].CraftTemplateKey -ne 'old/craft' -or $loaded.ComponentData.Load[0].Quantity -ne 12) {
        throw 'Previous combined fleet XML is incompatible'
    }
    Write-Output 'Previous combined fleet XML loads through the native hangar save type'
    $socket.ComponentData = New-Object 'Ships.BulkMagazineComponent+BulkMagazineData'
    $socket.ComponentData.Load.Add($ammo)
    $document.LoadXml('<Test/>')
    [Utility.XmlDocSerializerExtensions]::AppendToDocument($socket, $document.DocumentElement, 'HullSocket')
    $loaded = $reader.MakeGenericMethod([Ships.Serialization.SerializedHullSocket]).Invoke($null, @($document.DocumentElement, 'HullSocket', $null, $null))
    if ($loaded.ComponentData.GetType() -ne [Ships.BulkMagazineComponent+BulkMagazineData] -or $loaded.ComponentData.Load[0].Quantity -ne 42) {
        throw 'Vanilla magazine payload did not round-trip'
    }
    Write-Output 'Vanilla magazine XML remains compatible'
} finally {
    if ($harmony) { $harmony.UnpatchAll($harmony.Id) }
    [AppDomain]::CurrentDomain.remove_AssemblyResolve($resolver)
}

