#!/bin/bash

dotnet clean -c Release -f netcoreapp2.2 deps/ilrepack/ilrepack 
dotnet build -c Release -f netcoreapp2.2 deps/ilrepack/ilrepack/ilrepack.csproj --output ../../../build/ilrepack/unix /p:ILRepackExe=true
dotnet build/ilrepack/unix/ilrepack.dll /target:library /internalize build/ilrepack/unix/ilrepack.dll build/ilrepack/unix/bamlparser.dll build/ilrepack/unix/fasterflect.netstandard.dll build/ilrepack/unix/mono.cecil.dll /out:./build/ilrepack/unix/ilrepacked.dll
cp build/ilrepack/unix/ilrepacked.dll build/ilrepack/unix/ilrepack.dll
cp build/ilrepack/unix/ilrepacked.dll.config build/ilrepack/unix/ilrepack.dll.config
cp build/ilrepack/unix/ilrepacked.pdb build/ilrepack/unix/ilrepack.pdb
