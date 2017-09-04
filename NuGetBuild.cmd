@echo off
setlocal

tools\NuGet.exe update -Self 
tools\NuGet.exe pack ILRepack.MSBuild.Task.nuspec -OutputDirectory Build