@echo off
echo Attempting to kill GHelper
taskkill /F /FI "IMAGENAME eq GHelper.exe" >nul 2>nul && (
	echo Killed GHelper - Proceeding to update
) || (
	echo Unable to kill GHelper - Exiting
	exit /B 1
)

echo Starting download
powershell -Command "Invoke-WebRequest %1 -OutFile package.zip" >nul 2>nul
echo Download complete

echo Extracting package
powershell -Command "Get-ChildItem package.zip | Expand-Archive" >nul 2>nul
echo Extraction complete

echo Moving updated GHelper
move /Y "%CD%\package\GHelper.exe" "%CD%\" >nul 2>nul
echo Moved updated GHelper

echo Clearing up
rmdir /S /Q "%CD%\package" >nul 2>nul
del /Q "%CD%\package.zip" >nul 2>nul
echo Cleared up

echo Starting GHelper
start GHelper.exe
echo Started GHelper

exit /B 0
