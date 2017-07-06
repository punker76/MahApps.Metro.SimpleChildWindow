@echo off

IF NOT "%VS140COMNTOOLS%" == "" (call "%VS140COMNTOOLS%vsvars32.bat")

@echo on

.paket\paket.bootstrapper.exe
.paket\paket.exe restore

msbuild.exe /ToolsVersion:14.0 "..\MahApps.Metro.SimpleChildWindow\MahApps.Metro.SimpleChildWindow.sln" /p:configuration=Debug /p:platform="Any CPU" /m /t:Clean,Rebuild
msbuild.exe /ToolsVersion:14.0 "..\MahApps.Metro.SimpleChildWindow\MahApps.Metro.SimpleChildWindow.sln" /p:configuration=Release /p:platform="Any CPU" /m /t:Clean,Rebuild

..\BuildTools\NuGet.exe pack MahApps.Metro.SimpleChildWindow.ALPHA.nuspec -OutputDirectory %~dp0