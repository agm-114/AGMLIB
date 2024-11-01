
rem Set the path to your .csproj file
set "projectFile=AGMLIB.csproj"
setlocal enabledelayedexpansion
echo grab
rem Use PowerShell to increase the last digit of the version prefix
::powershell -command "(Get-Content '%projectFile%') -replace '<VersionPrefix>(\d+\.\d+\.\d+)\.(\d+)</VersionPrefix>', '<VersionPrefix>$1.($2 + 1)</VersionPrefix>' | Set-Content '%projectFile%'"
:: Use PowerShell to read the current version and increment the build number

:: Extract the version prefix and build number
for /f "tokens=1,2,3,4 delims=." %%a in (
    'powershell -command "(Get-Content '%projectFile%' | Select-String -Pattern '<VersionPrefix>(\d+\.\d+\.\d+\.\d+)</VersionPrefix>' -AllMatches).Matches.Value"'
) do (
    set "fullVersion=%%a.%%b.%%c.%%d"
)

rem Remove XML tags from fullVersion
set "fullVersion=!fullVersion:<VersionPrefix>=!"
set "fullVersion=!fullVersion:</VersionPrefix>=!"

for /f "tokens=1,2,3,4 delims=." %%a in ("!fullVersion!") do (
    set "major=%%a"
    set "minor=%%b"
    set "patch=%%c"
    set "revision=%%d"
)

echo Current major prefix: !major!
echo Current minor prefix: !minor!
echo Current patch prefix: !patch!
echo Current revision prefix: !revision!
:: Increment the build number
set /a revision=!revision! + 1
echo Incremented build number: !revision!

rem Construct the new version string
set "newVersion=!major!.!minor!.!patch!.!revision!"
echo NewVersion number: !newVersion!

rem Write the new version back to the project file
::powershell -command "(Get-Content '%projectFile%') -replace '<VersionPrefix>\d+\.\d+\.\d+\.\d+</VersionPrefix>', '<VersionPrefix>!newVersion!</VersionPrefix>' | Set-Content '%projectFile%'"
powershell -command "(Get-Content '%projectFile%') -replace '<VersionPrefix>\d+\.\d+\.\d+\.\d+</VersionPrefix>', '<VersionPrefix>!newVersion!</VersionPrefix>' | Out-File '%projectFile%' -Encoding UTF8 -Force"


:: Update the project file with the new version
::powershell -command "(Get-Content '%projectFile%') -replace '<VersionPrefix>(\d+\.\d+\.\d+)\.\d+</VersionPrefix>', '<VersionPrefix>!newVersion!</VersionPrefix>' | Set-Content '%projectFile%'"

