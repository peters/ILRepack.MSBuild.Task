#!/bin/bash
sh buildilrepackdll.sh

dotnet clean -c Release src -f netcoreapp2.2
dotnet build -c Release src -f netcoreapp2.2
dotnet test -c Release src -f netcoreapp2.2