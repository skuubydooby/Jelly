@echo off
REM Post-build obfuscation script using ConfuserEx
REM This encrypts all strings in the compiled assembly

setlocal enabledelayedexpansion

REM Get the project directory
set "SCRIPT_DIR=%~dp0"
set "CONFIG_DIR=%SCRIPT_DIR%"
set "BUILD_DIR=%SCRIPT_DIR%bin\Release\net451"
set "OUTPUT_DIR=%SCRIPT_DIR%bin\obfuscated"
set "DLL_NAME=Jelly.exe"

REM Check if ConfuserEx is installed
if not exist "%ProgramFiles%\ConfuserEx\ConfuserEx.CLI.exe" (
    echo ConfuserEx not found. Installing via chocolatey...
    choco install confuserex -y
)

REM Create output directory
if not exist "!OUTPUT_DIR!" mkdir "!OUTPUT_DIR!"

REM Run ConfuserEx
echo Obfuscating %DLL_NAME%...
"%ProgramFiles%\ConfuserEx\ConfuserEx.CLI.exe" "!CONFIG_DIR!ConfuserEx.crproj"

REM Check if obfuscation was successful
if exist "!OUTPUT_DIR!\%DLL_NAME%" (
    echo Obfuscation successful!
    REM Copy obfuscated DLL back to release folder
    copy "!OUTPUT_DIR!\%DLL_NAME%" "!BUILD_DIR!\%DLL_NAME%" /Y
    echo Obfuscated DLL replaced in build output
) else (
    echo Obfuscation failed or ConfuserEx not available
    echo Continuing with unobfuscated assembly...
)

endlocal
