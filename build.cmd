@echo off
setlocal

set /p version="Please enter nuget release version: "

if not "%1"=="" SET CONFIG=%1
if "%CONFIG%"=="" SET CONFIG=Release


call buildilrepackexe.cmd %CONFIG%

dotnet clean -c %CONFIG% deps/ilrepack/ilrepack 
dotnet build -c %CONFIG% deps/ilrepack/ilrepack/ilrepack.csproj /p:ILRepackPackForRelease=true
dotnet pack -c %CONFIG% --no-build deps/ilrepack/ilrepack/ilrepack.csproj /p:Version=%version%

dotnet clean -c %CONFIG% deps/ilrepack/ilrepack
dotnet clean -c %CONFIG% src/ilrepack.msbuild.task 
dotnet build -c %CONFIG% src/ilrepack.msbuild.task/ilrepack.msbuild.task.csproj /p:ILRepackMSBuildTaskPackageForRelease=true
dotnet pack -c %CONFIG% --no-build src/ilrepack.msbuild.task/ilrepack.msbuild.task.csproj /p:ILRepackMSBuildTaskPackageForRelease=true /p:Version=%version%
