@echo off

echo Attempting to kill GHelper
taskkill /F /FI "IMAGENAME eq GHelper.exe" >nul 2>nul && (
	echo Killed GHelper - Proceeding to update
) || (
	echo Unable to kill GHelper - Exiting
	exit /B 1
)

echo Starting download
powershell -Command "Invoke-WebRequest https://github.com/seerge/g-helper/releases/latest/download/GHelper.zip -OutFile package.zip" >nul 2>nul && (
    echo Downloaded latest release
) || (
    echo Failed to download - exiting
    call :CLEAN
    exit /B 1
)

echo Extracting package
powershell -Command "Get-ChildItem package.zip | Expand-Archive" >nul 2>nul && (
    echo Extraction complete
) || (
    echo Failed to extract - exiting
    call :CLEAN
    exit /B 1
)

echo Replacing application
move /Y "%CD%\package\GHelper.exe" "%CD%\" >nul 2>nul && (
  echo Replaced successfully
) || (
  echo Failed to replace - exiting
  call :CLEAN
  exit /B 1
)

call :CLEAN

start GHelper.exe

exit /B 0

:CLEAN
rmdir /S /Q "%CD%\package" >nul 2>nul
del /Q "%CD%\package.zip" >nul 2>nul
exit /B 0