@echo off
setlocal

if not "%1"=="" SET CONFIG=%1
if "%CONFIG%"=="" SET CONFIG=Release

echo CONFIG: %CONFIG%
dotnet clean -c %CONFIG% deps\ilrepack\ilrepack
dotnet build -c %CONFIG% -f net40 deps\ilrepack\ilrepack\ilrepack.csproj --output ..\..\..\build\ilrepack\windows /p:ILRepackExe=true
build\ilrepack\windows\ilrepack.exe /target:exe /internalize build\ilrepack\windows\ilrepack.exe build\ilrepack\windows\bamlparser.dll build\ilrepack\windows\fasterflect.netstandard.dll build\ilrepack\windows\mono.cecil.dll /out:./build\ilrepack\windows\ilrepacked.exe
copy build\ilrepack\windows\ilrepacked.exe build\ilrepack\windows\ilrepack.exe /b /Y 1>nul
del /s /f /q build\ilrepack\windows\ilrepacked.exe 1>nul