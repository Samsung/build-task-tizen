@if not defined _echo @echo off
setlocal enabledelayedexpansion

set BatchFile=%0
set Root=%~dp0
set BuildConfiguration=Debug
set MSBuildBuildTarget=Build
set NodeReuse=false
set DeveloperCommandPrompt=%VS150COMNTOOLS%\VsDevCmd.bat
set MSBuildAdditionalArguments=/m
set RunTests=true
set RunIntegrationTests=false
set DeployVsixExtension=false
set FileLoggerVerbosity=detailed
REM Turn on MSBuild async logging to speed up builds
set MSBUILDLOGASYNC=1
set InstallVSIX=false
set UninstallVSIX=false
set OnlySetupBuildEnv=false
:ParseArguments
if "%1" == "" goto :DoneParsing
if /I "%1" == "/?" call :Usage && exit /b 1
if /I "%1" == "/build" set MSBuildBuildTarget=Build&&shift&& goto :ParseArguments
if /I "%1" == "/rebuild" set MSBuildBuildTarget=Rebuild&&shift&& goto :ParseArguments
if /I "%1" == "/uninstall" set UninstallVSIX=true&&shift&& goto :ParseArguments
if /I "%1" == "/install" set UninstallVSIX=true&&set InstallVSIX=true&&shift&& goto :ParseArguments
if /I "%1" == "/copy-artifacts" set CopyOutputArtifacts=true&&shift&& goto :ParseArguments
if /I "%1" == "/debug" set BuildConfiguration=Debug&&shift&& goto :ParseArguments
if /I "%1" == "/release" set BuildConfiguration=Release&&shift&& goto :ParseArguments
if /I "%1" == "/signbuild" set ShouldSignBuild=true&&shift&& goto :ParseArguments
if /I "%1" == "/skiptests" set RunTests=false&&shift&& goto :ParseArguments
if /I "%1" == "/deploy-extension" set DeployVsixExtension=true&&shift&& goto :ParseArguments
if /I "%1" == "/no-node-reuse" set NodeReuse=false&&shift&& goto :ParseArguments
if /I "%1" == "/diagnostic" set FileLoggerVerbosity=diagnostic&&set MSBuildAdditionalArguments=&&shift&& goto :ParseArguments
if /I "%1" == "/integrationtests" set RunIntegrationTests=true&&shift&& goto :ParseArguments
if /I "%1" == "/setup" set OnlySetupBuildEnv=true&&shift&& goto :ParseArguments
call :Usage && exit /b 1
:DoneParsing

REM Install dotnet cli to test
powershell -NoProfile -NoLogo -ExecutionPolicy Bypass -Command "& \"%Root%build.ps1\" %* ; exit $LastExitCode;"
if "%OnlySetupBuildEnv%" == "true" (
	call :PrintColor Green "OnlySetupBuildEnv is true."
	exit /b 0
)

if not exist "%VS150COMNTOOLS%" (
  echo To build this repository, this script needs to be run from a Visual Studio 2017 developer command prompt.
  echo.
  echo If Visual Studio is not installed, visit this page to download:
  echo.
  echo https://www.visualstudio.com/downloads/
  exit /b 1
)

if not exist "%VSSDK150Install%" (
  echo To build this repository, you need to modify your Visual Studio installation to include the "Visual Studio extension development" workload.
  exit /b 1
)

if "%VisualStudioVersion%" == "" (
  REM In Jenkins and MicroBuild, we set VS150COMNTOOLS and VSSDK150Install to point to where VS is installed but don't launch in a developer prompt
  call "%DeveloperCommandPrompt%" || goto :BuildFailed
)

set RestoreDirectory=%Root%packages\
if exist "%RestoreDirectory%" rd /s /q "%RestoreDirectory%" || goto :BuildFailed

set BinariesDirectory=%Root%bin\%BuildConfiguration%\
if exist "%BinariesDirectory%" rd /s /q "%BinariesDirectory%" || goto :BuildFailed

set LogsDirectory=%BinariesDirectory%Logs\
if not exist "%LogsDirectory%" mkdir "%LogsDirectory%" || goto :BuildFailed

set VSIXDirectory=%BinariesDirectory%VSIX\
if not exist "%VSIXDirectory%" mkdir "%VSIXDirectory%" || goto :BuildFailed

set RawDirectory=%BinariesDirectory%Raw\Sdks\Tizen.NET.Sdk\
if not exist "%RawDirectory%" mkdir "%RawDirectory%" || goto :BuildFailed

set TestsDirectory=%BinariesDirectory%Tests\
if not exist "%TestsDirectory%" mkdir "%TestsDirectory%" || goto :BuildFailed

set PackagesDirectory=%BinariesDirectory%Packages\
if not exist "%PackagesDirectory%" mkdir "%PackagesDirectory%" || goto :BuildFailed


REM We build Restore, Build and BuildModernVsixPackages in different MSBuild processes.
REM %MSBuildBuildTarget%NuGetPackages,
for %%T IN (Restore, %MSBuildBuildTarget%,  xUnitTest, TestAssets) do (

  set LogFile=%LogsDirectory%%%T.log

  echo.

  if "%%T" == "Restore" (
    set ConsoleLoggerVerbosity=quiet
    echo   Restoring packages for Tizen.NET.Sdk (this may take some time^)
  ) else (
    set ConsoleLoggerVerbosity=minimal
  )

  set BuildCommand=msbuild /nologo /nodeReuse:%NodeReuse% /consoleloggerparameters:Verbosity=!ConsoleLoggerVerbosity! /fileLogger /fileloggerparameters:LogFile="!LogFile!";verbosity=%FileLoggerVerbosity% /t:"%%T" /p:Configuration="%BuildConfiguration%" /p:RunTests="%RunTests%" /p:RunIntegrationTests="%RunIntegrationTests%" /p:ShouldSignBuild="%ShouldSignBuild%" /p:DeployExtension="%DeployVsixExtension%" "%Root%build\build.proj" %MSBuildAdditionalArguments%
  if "%FileLoggerVerbosity%" == "diagnostic" (
    echo !BuildCommand!
  )

  !BuildCommand!

  if ERRORLEVEL 1 (
    echo.
    call :PrintColor Red "Build failed, for full log see !LogFile!."
    exit /b 1
  )
)

echo.
call :PrintColor Green "Build completed successfully, for full logs see %LogsDirectory%."
exit /b 0

:Usage
echo Usage: %BatchFile% [/setup^|/build^|/rebuild] [/debug^|/release] [/diagnostic] [/skiptests]
echo.

echo   Build targets:
echo     /setup                  setup dotnet cli to test
echo     /build                  Perform a build (default)
echo     /rebuild                Perform a clean, then build
echo.
echo   Configurations:
echo     /debug                  Perform debug build (default)
echo     /release                Perform release build
echo.
echo   Build options:
echo     /diagnostic             Turns on diagnostic logging and turns off multi-proc build, useful for diagnosing build logs
echo     /skiptests              Does not run unit tests
goto :eof

:BuildFailed
call :PrintColor Red "Build failed with ERRORLEVEL %ERRORLEVEL%."
exit /b 1

:PrintColor
"%Windir%\System32\WindowsPowerShell\v1.0\Powershell.exe" write-host -foregroundcolor %1 "'%2'"