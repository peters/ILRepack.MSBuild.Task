# Migrating from `V1.0.x` To `V2.0`

If you're using `ILRepack.MsBuild.Task V1.x` you may have something like the following in your .csproj:

```xml
  <!-- ILRepack -->
  <Target Name="ILRepack" AfterTargets="Build">
    <ItemGroup>
      <InputAssemblies Include="$(OutputPath)\MyLib1.dll" />
      <InputAssemblies Include="$(OutputPath)\MyLib2.dll" />
      <InputAssemblies Include="$(OutputPath)\MyLib3.dll" />
      <InputAssemblies Include="$(OutputPath)\$(AssemblyName).dll" />
    </ItemGroup>
    <ItemGroup>
      <DoNotInternalizeAssemblies Include="$(AssemblyName)" />
    </ItemGroup>
    
    <ILRepack 
        Parallel="true" 
        Internalize="true" 
        InternalizeExclude="@(DoNotInternalizeAssemblies)" 
        InputAssemblies="@(InputAssemblies)" 
        TargetKind="Dll" 
        OutputFile="$(OutputPath)\$(AssemblyName).dll" 
        LibraryPath="$(OutputPath)" 
        KeyFile="Properties\KeyFile.snk" 
        PrimaryAssemblyFile="$(AssemblyName).dll" />
  </Target>
  <!-- /ILRepack -->
  ```

  After updating to `V2.x` you might get one or more of these compile errors. 

> Error	MSB4064	The "InternalizeExclude" parameter is not supported by the "ILRepack" task. Verify the parameter exists on the task, and it is a settable public instance property.	```

  - Change `InternalizeExclude` to `InternalizeExcludeAssemblies`

> Error MSB4064	The "TargetKind" parameter is not supported by the "ILRepack" task. Verify the parameter exists on the task, and it is a settable public instance property.

  - Change `TargetKind` to `OutputType`

> Error	MSB4064	The "OutputFile" parameter is not supported by the "ILRepack" task. Verify the parameter exists on the task, and it is a settable public instance property.

  - Change `OutputFile` to `OutputAssembly`

> Error	MSB4064	The "LibraryPath" parameter is not supported by the "ILRepack" task. Verify the parameter exists on the task, and it is a settable public instance property.

  - Change `LibraryPath` to `WorkingDirectory`

> Error	MSB4064	The "KeyFile" parameter is not supported by the "ILRepack" task. Verify the parameter exists on the task, and it is a settable public instance property.

  - The `KeyFile` property is missing in V2. I've raised an [issue](https://github.com/peters/ILRepack.MSBuild.Task/issues/26) about it. At the moment, there is no way to sign the output assembly.

> Error	MSB4064	The "PrimaryAssemblyFile" parameter is not supported by the "ILRepack" task. Verify the parameter exists on the task, and it is a settable public instance property.

  - Change `PrimaryAssemblyFile` to `MainAssembly`

Also, take into account that if you define a `WorkingDirectory` then you no longer need to use `$(OutputPath)` anymore. Remove it from all file references.

