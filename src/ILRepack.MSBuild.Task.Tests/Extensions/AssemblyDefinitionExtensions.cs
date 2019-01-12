using System;
using System.IO;
using Mono.Cecil;

namespace ILRepack.MSBuild.Task.Tests.Extensions
{
    internal static class AssemblyDefinitionExtensions
    {
        public static string GetRelativeFilename(this AssemblyDefinition assemblyDefinition)
        {
            if (assemblyDefinition == null) throw new ArgumentNullException(nameof(assemblyDefinition));
            return assemblyDefinition.MainModule.Kind == ModuleKind.Dll ? $"{assemblyDefinition.Name.Name}.dll" : $"{assemblyDefinition.Name.Name}.exe";
        }

        public static string GetFullPath(this AssemblyDefinition assemblyDefinition, string workingDirectory)
        {
            if (assemblyDefinition == null) throw new ArgumentNullException(nameof(assemblyDefinition));
            return Path.Combine(workingDirectory, assemblyDefinition.GetRelativeFilename());
        }

        public static string GetInternalizeExcludeNamespace(this AssemblyDefinition assemblyDefinition)
        {
            if (assemblyDefinition == null) throw new ArgumentNullException(nameof(assemblyDefinition));
            return assemblyDefinition.Name.Name;
        }

        public static string GetInternalizeRegex(this AssemblyDefinition assemblyDefinition)
        {
            if (assemblyDefinition == null) throw new ArgumentNullException(nameof(assemblyDefinition));
            return $"^{assemblyDefinition.Name.Name}";
        }
    }
}
