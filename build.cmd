@echo off
setlocal

call buildilrepackexe.cmd

dotnet clean -c Release src/ilrepack.msbuild.task /p:PackageForRelease=true 
dotnet build -c Release src/ilrepack.msbuild.task/ilrepack.msbuild.task.csproj /p:PackageForRelease=true 

set /p version="Please enter nuget release version: "
dotnet pack -c Release --no-build src/ilrepack.msbuild.task/ilrepack.msbuild.task.csproj /p:PackageForRelease=true /p:Version=%version%