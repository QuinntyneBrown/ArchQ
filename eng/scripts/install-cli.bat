@echo off
setlocal

set "REPO_ROOT=%~dp0..\.."
set "CLI_PROJECT=%REPO_ROOT%\src\ArchQ.Cli\ArchQ.Cli.csproj"
set "NUPKG_DIR=%REPO_ROOT%\artifacts\nupkgs"
set "TOOL_ID=ArchQ.Cli"

echo ========================================
echo  ArchQ CLI Tool Installer
echo ========================================
echo.

:: Build and pack
echo [1/3] Building and packing %TOOL_ID%...
dotnet pack "%CLI_PROJECT%" -o "%NUPKG_DIR%" --nologo -v quiet
if %ERRORLEVEL% neq 0 (
    echo ERROR: Build failed.
    exit /b 1
)
echo       Build succeeded.
echo.

:: Uninstall if already installed
dotnet tool list --global 2>nul | findstr /i "%TOOL_ID%" >nul 2>&1
if %ERRORLEVEL% equ 0 (
    echo [2/3] Uninstalling existing %TOOL_ID%...
    dotnet tool uninstall --global %TOOL_ID%
    if %ERRORLEVEL% neq 0 (
        echo ERROR: Uninstall failed.
        exit /b 1
    )
    echo       Uninstalled.
) else (
    echo [2/3] No existing installation found. Skipping uninstall.
)
echo.

:: Install
echo [3/3] Installing %TOOL_ID%...
dotnet tool install --global --add-source "%NUPKG_DIR%" %TOOL_ID%
if %ERRORLEVEL% neq 0 (
    echo ERROR: Install failed.
    exit /b 1
)
echo.
echo ========================================
echo  Done. Run 'archq --help' to get started.
echo ========================================

endlocal
