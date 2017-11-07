@echo off

IF NOT "%VS140COMNTOOLS%" == "" (call "%VS140COMNTOOLS%vsvars32.bat")

@echo on

.paket\paket.bootstrapper.exe
.paket\paket.exe restore

msbuild.exe /ToolsVersion:14.0 MahApps.Metro.SimpleChildWindow.sln /p:configuration=Debug /p:platform="Any CPU" /m /t:Clean,Rebuild
