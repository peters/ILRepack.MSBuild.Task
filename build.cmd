@echo off
setlocal

set /p version="Please enter nuget release version: "

call buildilrepackexe.cmd

dotnet clean -c Release deps/ilrepack/ilrepack 
dotnet build -c Release deps/ilrepack/ilrepack/ilrepack.csproj /p:ILRepackPackForRelease=true
dotnet pack -c Release --no-build deps/ilrepack/ilrepack/ilrepack.csproj /p:Version=%version%

dotnet clean -c Release deps/ilrepack/ilrepack
dotnet clean -c Release src/ilrepack.msbuild.task 
dotnet build -c Release src/ilrepack.msbuild.task/ilrepack.msbuild.task.csproj /p:ILRepackMSBuildTaskPackageForRelease=true
dotnet pack -c Release --no-build src/ilrepack.msbuild.task/ilrepack.msbuild.task.csproj /p:ILRepackMSBuildTaskPackageForRelease=true /p:Version=%version%
