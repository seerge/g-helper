:: Check for administrative privileges
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Requesting administrative privileges...
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)

set G_HELPER_PATH="C:\Program Files\GHelper"
set URL=https://github.com/seerge/g-helper/releases/latest/download/GHelper.zip

:: Kill current running instance
taskkill /IM GHelper.exe /F

:: Go to GHelper Directory
cd /D %G_HELPER_PATH%

:: Delete old exe
del GHelper.exe

:: Download and unzip new version
curl -L %URL% --output GHelper.zip
powershell -Command "Expand-Archive .\GHelper.zip"
move .\GHelper\GHelper.exe %G_HELPER_PATH%

:: Delete downloaded setup
rmdir /S /Q GHelper
del GHelper.zip

:: Start updated GHelper
start GHelper.exe
