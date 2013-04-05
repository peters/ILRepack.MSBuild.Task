ILRepack.MSBuild.Task
=====================

MSBuild task for [ILRepack](https://github.com/gluck/il-repack) which is an open-source alternative to ILMerge.

Install via NuGet
=================
	Install-Package ILRepack.MSBuild.Task

Usage
============
```
<!-- ILRepack -->
<Import Project="$(MSBuildProjectDirectory)\packages\ILRepack.MSBuild.Task.1.0.5\tools\ILRepack.MSBuild.Task.Targets" />	
<Target Name="ILRepack" DependsOnTargets="Build" Condition="'$(Configuration)' == 'Release'">
	
   <ItemGroup>
	<InputAssemblies Include="$(OutputPath)\ExampleAssemblyToMerge1.dll" />
	<InputAssemblies Include="$(OutputPath)\ExampleAssemblyToMerge2.dll" />
	<InputAssemblies Include="$(OutputPath)\ExampleAssemblyToMerge3.dll" />
   </ItemGroup>

   <ILRepack 
	Parallel="true" 
	InputAssemblies="@(InputAssemblies)"
	TargetKind="Dll"
	OutputFile="$(OutputPath)\$(AssemblyName).dll"
   />

</Target>
<!-- /ILRepack -->
```

Additional task options
=======================

* KeyFile - Specifies a keyfile to sign the output assembly
* LogFile - Specifies a logfile to output log information
* Union -  Merges types with identical names into one
* DebugInfo - Enable/disable symbol file generation
* AttributeFile - Take assembly attributes from the given assembly file
* CopyAttributes - Copy assembly attributes
* AllowMultiple - Allows multiple attributes (if type allows)
* TargetKind - Target assembly kind (Exe|Dll|WinExe|SameAsPrimaryAssembly)
* TargetPlatformVersion - Target platform (v1, v1.1, v2, v4 supported)
* XmlDocumentation - Merge assembly xml documentation
* LibraryPath - List of paths to use as "include directories" when attempting to merge assemblies
* Internalize - Set all types but the ones from the first assembly 'internal'
* OutputFile - Output name for merged assembly
* InputAssemblies - List of assemblies that will be merged
* DelaySign - Set the keyfile, but don't sign the assembly
* AllowDuplicateResources - Allows to duplicate resources in output assembly 
* ZeroPeKind - Allows assemblies with Zero PeKind (but obviously only IL will get merged)
* Parallel - Use as many CPUs as possible to merge the assemblies
* PauseBeforeExit - Pause execution once completed (good for debugging)
* Verbose - Additional debug information during merge that will be outputted to LogFile
* PrimaryAssemblyFile - Used in conjunction with Interalize to specify the main assembly filename
* Wildcards - Allows (and resolves) file wildcards (e.g. `*`.dll) in input assemblies
 
License
=======
Checkout the [License](https://github.com/peters/ILRepack.MSBuild.Task/blob/master/LICENSE.md)
