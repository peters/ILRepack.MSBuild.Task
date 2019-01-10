@echo off
setlocal

call buildilrepackexe.bat

dotnet clean -c Release src
dotnet build -c Release src
tools\nuget.exe pack ILRepack.MSBuild.Task.nuspec -OutputDirectory c:\nupkgs  -Version 2.0.0