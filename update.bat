@echo off
setlocal enabledelayedexpansion

SET gitlink=https://github.com/seerge/g-helper/releases/latest/download/GHelper.zip
IF "%1"=="--ver" (
    ECHO Set to download v%2% instead of latest
    SET gitlink=https://github.com/seerge/g-helper/releases/download/v%2%/GHelper.zip
)

ECHO Attempting to kill GHelper
TASKKILL /F /FI "IMAGENAME eq GHelper.exe" >nul 2>nul && (
	ECHO Killed GHelper - Proceeding to update
) || (
	ECHO Unable to kill GHelper - Exiting
	EXIT /B 1
)

ECHO Starting download
powershell -Command "Invoke-WebRequest %gitlink% -OutFile package.zip" >nul 2>nul && (
    ECHO Downloaded latest release
) || (
    ECHO Failed to download - exiting
    CALL :CLEAN
    EXIT /B 1
)

ECHO Extracting package
powershell -Command "Get-ChildItem package.zip | Expand-Archive" >nul 2>nul && (
    ECHO Extraction complete
) || (
    ECHO Failed to extract - exiting
    CALL :CLEAN
    EXIT /B 1
)

ECHO Replacing application
move /Y "%CD%\package\GHelper.exe" "%CD%\" >nul 2>nul && (
  ECHO Replaced successfully
) || (
  ECHO Failed to replace - exiting
  CALL :CLEAN
  EXIT /B 1
)

CALL :CLEAN

powershell -Command "Start-Process '%CD%\GHelper.exe' -Verb runAs"

EXIT /B 0

:CLEAN
rmdir /S /Q "%CD%\package" >nul 2>nul
del /Q "%CD%\package.zip" >nul 2>nul
exit /B 0