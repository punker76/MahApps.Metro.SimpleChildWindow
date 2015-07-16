@echo on
call "%VS120COMNTOOLS%vsvars32.bat"

msbuild.exe /ToolsVersion:4.0 "MahApps.Metro.SimpleChildWindow.sln" /p:configuration=Release /t:Clean,Rebuild
.nuget\NuGet.exe pack %~dp0MahApps.Metro.SimpleChildWindow.nuspec -OutputDirectory %~dp0

msbuild.exe /ToolsVersion:4.0 "MahApps.Metro.SimpleChildWindow.sln" /p:configuration=Debug /t:Clean,Rebuild
.nuget\NuGet.exe pack %~dp0MahApps.Metro.SimpleChildWindow.ALPHA.nuspec -OutputDirectory %~dp0

pause