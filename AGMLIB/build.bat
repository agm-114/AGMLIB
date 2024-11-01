rem START  C:\Steam\steamapps\common\Nebulous\NFC.url
rem del C:\Steam\steamapps\workshop\content\887570\2960504230\Debug\net481\AGMLIB.dll
rem robocopy C:\Steam\steamapps\common\Nebulous\Mods\AGMLIB\Debug\net481 C:\Steam\steamapps\workshop\content\887570\2960504230\Debug\net481	
rem robocopy C:\Steam\steamapps\common\Nebulous\Mods\AGMLIB\Debug\net481\AGMLIB.dll "D:\AGMLIB - Remote\Assets\AGMLIB\AGMLIB.dll"
::"C:\Program Files (x86)\Steam\steamapps\common\Nebulous\NFC.url"
::xcopy /y /f "C:\Program Files (x86)\Steam\steamapps\common\Nebulous\Mods\AGMLIB" "C:\Program Files (x86)\Steam\steamapps\workshop\content\887570\2960504230\Debug\net481\AGMLIB.dll"
:: xcopy /y /f C:\Steam\steamapps\common\Nebulous\Mods\AGMLIB\Debug\net481\AGMLIB.dll "D:\AGMLIB - Remote\Assets\AGMLIB\AGMLIB.dll"
::xcopy /y /f C:\Steam\steamapps\common\Nebulous\Mods\AGMLIB\Debug\net481\AGMLIB.dll C:\Users\dearv\TestProject\Assets\AGMLIB.dll
@echo on
echo test
setlocal enabledelayedexpansion

@echo off






:: Check if the user provided the ModVer as a parameter
if "%~1"=="" (
    echo Please provide a ModVer value.
    exit /b 1
)

echo set
set "ModVer=%~1"
echo setdone

:: Create the XML content
(
echo ^<?xml version="1.0" encoding="utf-8" ?^>
echo.
echo ^<ModInfo^>
echo.
echo     ^<ModName^>AGMLIB^</ModName^>
echo.
echo     ^<ModDescription^>By: AGM^</ModDescription^>
echo.
echo     ^<ModVer^>%ModVer%^</ModVer^>
echo.
echo     ^<Assemblies^>
echo.
echo         ^<string^>Debug/net481/AGMLIB.dll^</string^>
echo         ^<string^>Debug/net481/0Harmony.dll^</string^>
echo.
echo     ^</Assemblies^>
echo.
echo     ^<GameVer^>0.3.2^</GameVer^>
echo.
echo ^</ModInfo^>
) > ModInfo.xml
echo wrote

exit /b 0