@echo off

cd PreLaunchTaskr.Configurator.NET8
dotnet publish -r win-x64 -c release
cd ..

cd PreLaunchTaskr.Launcher.NET8
dotnet publish -r win-x64 -c release
cd ..

cd PreLaunchTaskr.GUI.WinUI3
dotnet publish -r win-x64 -c release
cd ..

pause