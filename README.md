ILRepack.MSBuild.Task
=====================

MSBuild task for [ILRepack](https://github.com/gluck/il-repack) which is an open-source alternative to ILMerge.

## Install via NuGet

`Install-Package ILRepack.MSBuild.Task`

## Supported frameworks

`netcoreapp2.1`
`netstandard2.0`
`net46`

NB! `OutputType` EXE on .NET Core assemblies is not supported.

## Building (Windows)

`build.cmd` (Visual Studio 15.9 required)

## Building (Unix)

`build.sh` (.net core 2.2.101 required)

### ILRepack a library with using an explicit list of input assemblies

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>netcoreapp2.2</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="ILRepack.MSBuild.Task" Version="2.0.1" />
    </ItemGroup>

    <Target Name="ILRepack" AfterTargets="Build">

        <PropertyGroup>
            <WorkingDirectory>$(MSBuildThisFileDirectory)bin\$(Configuration)\$(TargetFramework)</WorkingDirectory>
        </PropertyGroup>

        <ItemGroup>
            <InputAssemblies Include="dependency1.dll" />
            <InputAssemblies Include="..\Mono.Cecil.dll" />
            <InputAssemblies Include="c:\a\rooted\path\Mono.Cecil.Mdb.dll" />
        </ItemGroup>
		
        <ItemGroup>
	    <!-- Dot not internalize any types inside this assembly -->
            <InternalizeExcludeAssemblies Include="do.not.internalize.this.assembly.dll" />
	    <!-- Internalize all types inside Some.Assembly.Namespace (Regular expression) -->
            <InternalizeExcludeAssemblies Include="^Some.Assembly.Namespace" />
	    <!-- Internalize all types except Some.Assembly.Namespace.SpecificType (Regular expression) -->
            <InternalizeExcludeAssemblies Include="^Some.Assembly.Namespace.SpecificType" />
        </ItemGroup>

        <ILRepack 
            OutputType="$(OutputType)" 
            MainAssembly="$(AssemblyName).dll" 
            OutputAssembly="$(AssemblyName).dll" 
            InputAssemblies="@(InputAssemblies)" 
            InternalizeExcludeAssemblies="@(InternalizeExcludeAssemblies)" 
            WorkingDirectory="$(WorkingDirectory)" />

    </Target>

</Project>
```

### ILRepack a library and all dependencies

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <TargetFramework>netcoreapp2.2</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="ILRepack.MSBuild.Task" Version="2.0.1" />
    </ItemGroup>
    
    <Target Name="ILRepack" AfterTargets="Build">

        <PropertyGroup>
            <WorkingDirectory>$(MSBuildThisFileDirectory)bin\$(Configuration)\$(TargetFramework)</WorkingDirectory>
        </PropertyGroup>

        <ILRepack 
            OutputType="$(OutputType)" 
            MainAssembly="$(AssemblyName).dll" 
            OutputAssembly="$(AssemblyName).dll" 
            InputAssemblies="*.dll" 
	    WilcardInputAssemblies="true"
            WorkingDirectory="$(WorkingDirectory)" />

    </Target>

</Project>
```

### ILRepack a executable and all dependencies

```xml
<Target Name="ILRepack" AfterTargets="Build">

        <PropertyGroup>
            <WorkingDirectory>$(MSBuildThisFileDirectory)bin\$(Configuration)\$(TargetFramework)</WorkingDirectory>
        </PropertyGroup>

        <ILRepack
            OutputType="$(OutputType)"
            MainAssembly="$(AssemblyName).exe"
            OutputAssembly="$(AssemblyName).exe"
            InputAssemblies="*.dll"
	    WilcardInputAssemblies="true"
            WorkingDirectory="$(WorkingDirectory)" />

</Target>
```

License
=======
Checkout the [License](https://github.com/peters/ILRepack.MSBuild.Task/blob/master/LICENSE.md)
