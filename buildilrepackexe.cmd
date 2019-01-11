@echo off
setlocal

dotnet clean -c Release deps\ilrepack\ilrepack
dotnet build -c Debug -f net40 deps\ilrepack\ilrepack\ilrepack.csproj --output ..\..\..\build\ilrepack /p:ILRepackExe=true
build\ilrepack\ilrepack.exe /target:exe /internalize build\ilrepack\ilrepack.exe build\ilrepack\bamlparser.dll build\ilrepack\fasterflect.netstandard.dll build\ilrepack\mono.cecil.dll build\ilrepack\mono.posix.dll /out:./build/ilrepack/ilrepacked.exe
copy build\ilrepack\ilrepacked.exe build\ilrepack\ilrepack.exe /b /Y 1>nul
del /s /f /q build\ilrepack\ilrepacked.exe 1>nul