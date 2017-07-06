@echo on

..\BuildTools\GitLink.exe . -u https://github.com/punker76/MahApps.Metro.SimpleChildWindow -b master -c Release -f ..\MahApps.Metro.SimpleChildWindow\MahApps.Metro.SimpleChildWindow.sln -ignore MahApps.Metro.SimpleChildWindow.Demo

..\BuildTools\NuGet.exe pack MahApps.Metro.SimpleChildWindow.nuspec -OutputDirectory %~dp0