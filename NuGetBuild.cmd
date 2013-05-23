@echo off
setlocal

tools\NuGet.exe update -Self 
tools\NuGet.exe pack ILRepack.MSBuild.Task.nuspec -OutputDirectory D:\Bruker_data\PeterLocal.NuGetRepository