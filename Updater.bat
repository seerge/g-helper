@echo off
setlocal enabledelayedexpansion

:: Check if running with administrator privileges
NET SESSION >nul 2>&1
if %errorlevel% neq 0 (
    echo This script requires administrator privileges.
    echo Please run the script as an administrator.
    pause
    exit /b 1
)

:: Your script code goes here

echo Running with administrator privileges.

REM Replace 'owner' and 'repo' with the GitHub owner and repository name
set owner=seerge
set repo=g-helper

REM Fetch the latest release information
for /f "tokens=*" %%i in ('curl -s "https://api.github.com/repos/%owner%/%repo%/releases/latest" ^| jq -r ".assets[0].browser_download_url"') do set download_url=%%i

echo Download URL: !download_url!

REM Specify the process name
set "process_name=GHelper.exe"

REM Kill the process
taskkill /F /IM "%process_name%"

powershell -Command "winget install -e --id jqlang.jq"

echo Process %process_name% killed.

:: Get the full path of the batch file's directory
set "destination_folder=%~dp0"

:: Loop through files in the batch file's directory
for %%F in ("%destination_folder%\*") do (
    set "file=%%~nxF"
    if /I "%%~xF" neq ".bat" (
        del "%%F"
        echo Deleted: !file!
    )
)

echo Files deleted, excluding batch files.

REM Download the file
curl -L -o "%destination_folder%\GHelper.zip" %download_url%

REM Change the current directory to the destination folder
cd /d "%destination_folder%"

REM Unzip the file
powershell -command "Expand-Archive -Path '.\GHelper.zip' -DestinationPath '.\'"

REM Delete the zip file
del "GHelper.zip"
echo Zip file deleted.

echo File downloaded and unzipped successfully.

pause