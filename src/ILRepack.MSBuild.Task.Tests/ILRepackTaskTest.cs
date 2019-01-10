using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Moq;
using Xunit;

namespace ILRepack.MSBuild.Task.Tests
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ILRepackTaskTests
    {
        [Fact]
        public void Test()
        {
            var mock = new Mock<IBuildEngine>();

            var task = new ILRepack {BuildEngine = mock.Object};

            var packageDir = @"\bin\Snap.Core\Debug\netcoreapp2.2";
            var inputAssemblies = Directory.GetFiles(packageDir).Where(x => x.EndsWith(".dll")).ToList();
            var mainAssemblyName = "Snap.Core.dll";
            var mainAssembly = inputAssemblies.Single(x => x.EndsWith(mainAssemblyName));
            inputAssemblies.Remove(mainAssembly);

            task.InputAssemblies = inputAssemblies.Select(x => new TaskItem((x))).ToArray();
            task.PrimaryAssemblyFile = Path.GetFileName(mainAssembly);
            task.Internalize = true;
            task.DebugInfo = true;
            task.TargetKind = "Dll";
            task.OutputFile = Path.Combine(packageDir, "Snap.Core.Repacked.dll");
            task.SearchDirectories = new ITaskItem[] {new TaskItem(packageDir)};

            task.Execute();
        }
    }
}
