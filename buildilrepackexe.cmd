@echo off
setlocal

dotnet clean -c Release deps\ilrepack\ilrepack
dotnet build -c Release -f net40 deps\ilrepack\ilrepack\ilrepack.csproj --output ..\..\..\build\ilrepack\windows /p:ILRepackExe=true
build\ilrepack\windows\ilrepack.exe /target:exe /internalize build\ilrepack\windows\ilrepack.exe build\ilrepack\windows\bamlparser.dll build\ilrepack\windows\fasterflect.netstandard.dll build\ilrepack\windows\mono.cecil.dll /out:./build\ilrepack\windows\ilrepacked.exe
copy build\ilrepack\windows\ilrepacked.exe build\ilrepack\windows\ilrepack.exe /b /Y 1>nul
del /s /f /q build\ilrepack\windows\ilrepacked.exe 1>nul