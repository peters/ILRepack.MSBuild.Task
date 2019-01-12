using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ILRepack.MSBuild.Task.Tests.Extensions;
using ILRepack.MSBuild.Task.Tests.MSBuild;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xunit;
using Mono.Cecil;

namespace ILRepack.MSBuild.Task.Tests
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ILRepackTaskTests : IClassFixture<BaseFixture>
    {
        readonly BaseFixture _baseFixture;

        public ILRepackTaskTests(BaseFixture baseFixture)
        {
            _baseFixture = baseFixture ?? throw new ArgumentNullException(nameof(baseFixture));
        }

        [Fact]
        public void Test_WildCardInputAssemblies_All_Assemblies_Are_Public_If_Internalize_False()
        {
            var workingDirectory = _baseFixture.WorkingDirectory;

            // If test cases fail then we have to remove those libraries or this test case may
            // fail because there are multiple "My*.dll" libraries present in working directory causing duplicate type exceptions.
            workingDirectory.DeleteResidueMyLibraries();

            var ticks = DateTime.Now.Ticks;

            var publicLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"MyPublicLibrary{ticks}", "ThisClassShouldBePublic");
            var internalLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"MyInternalLibrary{ticks}", "ThisClassShouldBeInternal");
            var ilRepackedAssemblyDefinition = _baseFixture.BuildLibrary($"MyILRepackedLibrary{ticks}", "ThisClassShouldAlsoBePublic");

            using (_baseFixture.WithDisposableAssemblies(workingDirectory, publicLibraryAssemblyDefinition, internalLibraryAssemblyDefinition, ilRepackedAssemblyDefinition))
            {
                var task = new ILRepack
                {
                    FakeBuildEngine = new FakeBuildEngine(),
                    Internalize = false,
                    OutputType = "Library",
                    MainAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    OutputAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    InputAssemblies = new List<ITaskItem>
                    {
                        new TaskItem("My*.dll")
                    }.ToArray(),
                    WilcardInputAssemblies = true,
                    WorkingDirectory = workingDirectory
                };

                Assert.True(task.Execute());

                Assert.True(File.Exists(task.OutputAssembly));

                using (ilRepackedAssemblyDefinition = AssemblyDefinition.ReadAssembly(task.OutputAssembly))
                {
                    Assert.NotNull(ilRepackedAssemblyDefinition);

                    var references = ilRepackedAssemblyDefinition.MainModule.AssemblyReferences.ToList();
                    Assert.Single(references);
                    Assert.Equal("mscorlib", references[0].Name);

                    var types = ilRepackedAssemblyDefinition.MainModule.Types.ToList();

                    var thisClassShouldBePublic = types.SingleOrDefault(x => x.Namespace == publicLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBePublic");
                    Assert.NotNull(thisClassShouldBePublic);
                    Assert.True(thisClassShouldBePublic.IsPublic);

                    var thisClassShouldBeInternal = types.SingleOrDefault(x => x.Namespace == internalLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBeInternal");
                    Assert.NotNull(thisClassShouldBeInternal);
                    Assert.True(thisClassShouldBeInternal.IsPublic);

                    var thisClassShouldAlsoBePublic = types.SingleOrDefault(x => x.Namespace == ilRepackedAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldAlsoBePublic");
                    Assert.NotNull(thisClassShouldAlsoBePublic);
                    Assert.True(thisClassShouldAlsoBePublic.IsPublic);
                }

            }
        }

        [Fact]
        public void TestWilcardInputAssemblies()
        {
            var workingDirectory = _baseFixture.WorkingDirectory;

            var ticks = DateTime.Now.Ticks;

            var publicLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"MyPublicLibrary{ticks}", "ThisClassShouldBePublic");
            var internalLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"MyInternalLibrary{ticks}", "ThisClassShouldBeInternal");
            var ilRepackedAssemblyDefinition = _baseFixture.BuildLibrary($"MyILRepackedLibrary{ticks}", "ThisClassShouldAlsoBePublic");

            using (_baseFixture.WithDisposableAssemblies(workingDirectory, publicLibraryAssemblyDefinition, internalLibraryAssemblyDefinition, ilRepackedAssemblyDefinition))
            {
                var task = new ILRepack
                {
                    FakeBuildEngine = new FakeBuildEngine(),
                    OutputType = "Library",
                    MainAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    OutputAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    InputAssemblies = new List<ITaskItem>
                    {
                        new TaskItem("My*.dll"),
                    }.ToArray(),
                    WilcardInputAssemblies = true,
                    WorkingDirectory = workingDirectory
                };

                Assert.True(task.Execute());

                Assert.True(File.Exists(task.OutputAssembly));

                using (ilRepackedAssemblyDefinition = AssemblyDefinition.ReadAssembly(task.OutputAssembly))
                {
                    Assert.NotNull(ilRepackedAssemblyDefinition);

                    var references = ilRepackedAssemblyDefinition.MainModule.AssemblyReferences.ToList();
                    Assert.Single(references);
                    Assert.Equal("mscorlib", references[0].Name);

                    var types = ilRepackedAssemblyDefinition.MainModule.Types.ToList();

                    var thisClassShouldBePublic = types.SingleOrDefault(x => x.Namespace == publicLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBePublic");
                    Assert.NotNull(thisClassShouldBePublic);
                    Assert.False(thisClassShouldBePublic.IsPublic);

                    var thisClassShouldBeInternal = types.SingleOrDefault(x => x.Namespace == internalLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBeInternal");
                    Assert.NotNull(thisClassShouldBeInternal);
                    Assert.False(thisClassShouldBeInternal.IsPublic);

                    var thisClassShouldAlsoBePublic = types.SingleOrDefault(x => x.Namespace == ilRepackedAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldAlsoBePublic");
                    Assert.NotNull(thisClassShouldAlsoBePublic);
                    Assert.True(thisClassShouldAlsoBePublic.IsPublic);
                }

            }
        }

        [Fact]
        public void TestWilcardInputAssemblies_Exclude()
        {
            var workingDirectory = _baseFixture.WorkingDirectory;

            var ticks = DateTime.Now.Ticks;

            var publicLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"MyPublicLibrary{ticks}", "ThisClassShouldBePublic");
            var internalLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"MyInternalLibrary{ticks}", "ThisClassShouldBeInternal");
            var ilRepackedAssemblyDefinition = _baseFixture.BuildLibrary($"MyILRepackedLibrary{ticks}", "ThisClassShouldAlsoBePublic");

            using (_baseFixture.WithDisposableAssemblies(workingDirectory, publicLibraryAssemblyDefinition, internalLibraryAssemblyDefinition, ilRepackedAssemblyDefinition))
            {
                var task = new ILRepack
                {
                    FakeBuildEngine = new FakeBuildEngine(),
                    OutputType = "Library",
                    MainAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    OutputAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    InputAssemblies = new List<ITaskItem>
                    {
                        new TaskItem("My*.dll"),
                    }.ToArray(),
                    InternalizeExcludeAssemblies = new ITaskItem[]
                    {
                        new TaskItem(publicLibraryAssemblyDefinition.GetInternalizeExcludeNamespace())
                    },
                    WilcardInputAssemblies = true,
                    WorkingDirectory = workingDirectory
                };

                Assert.True(task.Execute());

                Assert.True(File.Exists(task.OutputAssembly));

                using (ilRepackedAssemblyDefinition = AssemblyDefinition.ReadAssembly(task.OutputAssembly))
                {
                    Assert.NotNull(ilRepackedAssemblyDefinition);

                    var references = ilRepackedAssemblyDefinition.MainModule.AssemblyReferences.ToList();
                    Assert.Single(references);
                    Assert.Equal("mscorlib", references[0].Name);

                    var types = ilRepackedAssemblyDefinition.MainModule.Types.ToList();

                    var thisClassShouldBePublic = types.SingleOrDefault(x => x.Namespace == publicLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBePublic");
                    Assert.NotNull(thisClassShouldBePublic);
                    Assert.True(thisClassShouldBePublic.IsPublic);

                    var thisClassShouldBeInternal = types.SingleOrDefault(x => x.Namespace == internalLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBeInternal");
                    Assert.NotNull(thisClassShouldBeInternal);
                    Assert.False(thisClassShouldBeInternal.IsPublic);

                    var thisClassShouldAlsoBePublic = types.SingleOrDefault(x => x.Namespace == ilRepackedAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldAlsoBePublic");
                    Assert.NotNull(thisClassShouldAlsoBePublic);
                    Assert.True(thisClassShouldAlsoBePublic.IsPublic);
                }

            }
        }

        [Fact]
        public void TestInternalize_MainAssembly_Is_Public_By_Default()
        {
            var workingDirectory = _baseFixture.WorkingDirectory;

            var ticks = DateTime.Now.Ticks;

            var publicLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"PublicLibrary{ticks}", "ThisClassShouldBePublic");
            var internalLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"InternalLibrary{ticks}", "ThisClassShouldBeInternal");
            var ilRepackedAssemblyDefinition = _baseFixture.BuildLibrary($"ILRepackedLibrary{ticks}", "ThisClassShouldAlsoBePublic");

            using (_baseFixture.WithDisposableAssemblies(workingDirectory, publicLibraryAssemblyDefinition, internalLibraryAssemblyDefinition, ilRepackedAssemblyDefinition))
            {
                var task = new ILRepack
                {
                    FakeBuildEngine = new FakeBuildEngine(),
                    OutputType = "Library",
                    MainAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    OutputAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    InputAssemblies = new List<ITaskItem>
                    {
                        new TaskItem(publicLibraryAssemblyDefinition.GetRelativeFilename()),
                        new TaskItem(internalLibraryAssemblyDefinition.GetRelativeFilename()),
                    }.ToArray(),
                    WorkingDirectory = workingDirectory
                };

                Assert.True(task.Execute());

                Assert.True(File.Exists(task.OutputAssembly));

                using (ilRepackedAssemblyDefinition = AssemblyDefinition.ReadAssembly(task.OutputAssembly))
                {
                    Assert.NotNull(ilRepackedAssemblyDefinition);

                    var references = ilRepackedAssemblyDefinition.MainModule.AssemblyReferences.ToList();
                    Assert.Single(references);
                    Assert.Equal("mscorlib", references[0].Name);

                    var types = ilRepackedAssemblyDefinition.MainModule.Types.ToList();

                    var thisClassShouldBePublic = types.SingleOrDefault(x => x.Namespace == publicLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBePublic");
                    Assert.NotNull(thisClassShouldBePublic);
                    Assert.False(thisClassShouldBePublic.IsPublic);

                    var thisClassShouldBeInternal = types.SingleOrDefault(x => x.Namespace == internalLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBeInternal");
                    Assert.NotNull(thisClassShouldBeInternal);
                    Assert.False(thisClassShouldBeInternal.IsPublic);

                    var thisClassShouldAlsoBePublic = types.SingleOrDefault(x => x.Namespace == ilRepackedAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldAlsoBePublic");
                    Assert.NotNull(thisClassShouldAlsoBePublic);
                    Assert.True(thisClassShouldAlsoBePublic.IsPublic);
                }

            }
        }

        [Fact]
        public void TestInternalize_Exclude_Relative_Filename()
        {
            var workingDirectory = _baseFixture.WorkingDirectory;

            var ticks = DateTime.Now.Ticks;

            var publicLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"PublicLibrary{ticks}", "ThisClassShouldBePublic");
            var internalLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"InternalLibrary{ticks}", "ThisClassShouldBeInternal");
            var ilRepackedAssemblyDefinition = _baseFixture.BuildLibrary($"ILRepackedLibrary{ticks}", "ThisClassShouldAlsoBePublic");

            using (_baseFixture.WithDisposableAssemblies(workingDirectory, publicLibraryAssemblyDefinition, internalLibraryAssemblyDefinition, ilRepackedAssemblyDefinition))
            {
                var task = new ILRepack
                {
                    FakeBuildEngine = new FakeBuildEngine(),
                    OutputType = "Library",
                    MainAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    OutputAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    InputAssemblies = new List<ITaskItem>
                    {
                        new TaskItem(publicLibraryAssemblyDefinition.GetRelativeFilename()),
                        new TaskItem(internalLibraryAssemblyDefinition.GetRelativeFilename())
                    }.ToArray(),
                    InternalizeExcludeAssemblies = new ITaskItem[]
                    {
                        new TaskItem(publicLibraryAssemblyDefinition.GetRelativeFilename())
                    },
                    WorkingDirectory = workingDirectory
                };

                Assert.True(task.Execute());

                Assert.True(File.Exists(task.OutputAssembly));

                using (ilRepackedAssemblyDefinition = AssemblyDefinition.ReadAssembly(task.OutputAssembly))
                {
                    Assert.NotNull(ilRepackedAssemblyDefinition);

                    var references = ilRepackedAssemblyDefinition.MainModule.AssemblyReferences.ToList();
                    Assert.Single(references);
                    Assert.Equal("mscorlib", references[0].Name);

                    var types = ilRepackedAssemblyDefinition.MainModule.Types.ToList();

                    var thisClassShouldBePublic = types.SingleOrDefault(x => x.Namespace == publicLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBePublic");
                    Assert.NotNull(thisClassShouldBePublic);
                    Assert.True(thisClassShouldBePublic.IsPublic);

                    var thisClassShouldBeInternal = types.SingleOrDefault(x => x.Namespace == internalLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBeInternal");
                    Assert.NotNull(thisClassShouldBeInternal);
                    Assert.False(thisClassShouldBeInternal.IsPublic);

                    var thisClassShouldAlsoBePublic = types.SingleOrDefault(x => x.Namespace == ilRepackedAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldAlsoBePublic");
                    Assert.NotNull(thisClassShouldAlsoBePublic);
                    Assert.True(thisClassShouldAlsoBePublic.IsPublic);
                }

            }
        }

        [Fact]
        public void TestInternalize_Exclude_Relative_Fullpath()
        {
            var workingDirectory = _baseFixture.WorkingDirectory;

            var ticks = DateTime.Now.Ticks;

            var publicLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"PublicLibrary{ticks}", "ThisClassShouldBePublic");
            var internalLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"InternalLibrary{ticks}", "ThisClassShouldBeInternal");
            var ilRepackedAssemblyDefinition = _baseFixture.BuildLibrary($"ILRepackedLibrary{ticks}", "ThisClassShouldAlsoBePublic");

            using (_baseFixture.WithDisposableAssemblies(workingDirectory, publicLibraryAssemblyDefinition, internalLibraryAssemblyDefinition, ilRepackedAssemblyDefinition))
            {
                var task = new ILRepack
                {
                    FakeBuildEngine = new FakeBuildEngine(),
                    OutputType = "Library",
                    MainAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    OutputAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    InputAssemblies = new List<ITaskItem>
                    {
                        new TaskItem(publicLibraryAssemblyDefinition.GetRelativeFilename()),
                        new TaskItem(internalLibraryAssemblyDefinition.GetRelativeFilename())
                    }.ToArray(),
                    InternalizeExcludeAssemblies = new ITaskItem[]
                    {
                        new TaskItem(publicLibraryAssemblyDefinition.GetFullPath(workingDirectory))
                    },
                    WorkingDirectory = workingDirectory
                };

                Assert.True(task.Execute());

                Assert.True(File.Exists(task.OutputAssembly));

                using (ilRepackedAssemblyDefinition = AssemblyDefinition.ReadAssembly(task.OutputAssembly))
                {
                    Assert.NotNull(ilRepackedAssemblyDefinition);

                    var references = ilRepackedAssemblyDefinition.MainModule.AssemblyReferences.ToList();
                    Assert.Single(references);
                    Assert.Equal("mscorlib", references[0].Name);

                    var types = ilRepackedAssemblyDefinition.MainModule.Types.ToList();

                    var thisClassShouldBePublic = types.SingleOrDefault(x => x.Namespace == publicLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBePublic");
                    Assert.NotNull(thisClassShouldBePublic);
                    Assert.True(thisClassShouldBePublic.IsPublic);

                    var thisClassShouldBeInternal = types.SingleOrDefault(x => x.Namespace == internalLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBeInternal");
                    Assert.NotNull(thisClassShouldBeInternal);
                    Assert.False(thisClassShouldBeInternal.IsPublic);

                    var thisClassShouldAlsoBePublic = types.SingleOrDefault(x => x.Namespace == ilRepackedAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldAlsoBePublic");
                    Assert.NotNull(thisClassShouldAlsoBePublic);
                    Assert.True(thisClassShouldAlsoBePublic.IsPublic);
                }

            }
        }

        [Fact]
        public void TestInternalize_Exclude_Relative_Regex()
        {
            var workingDirectory = _baseFixture.WorkingDirectory;

            var ticks = DateTime.Now.Ticks;

            var publicLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"PublicLibrary{ticks}", "ThisClassShouldBePublic");
            var internalLibraryAssemblyDefinition = _baseFixture.BuildLibrary($"InternalLibrary{ticks}", "ThisClassShouldBeInternal");
            var ilRepackedAssemblyDefinition = _baseFixture.BuildLibrary($"ILRepackedLibrary{ticks}", "ThisClassShouldAlsoBePublic");

            using (_baseFixture.WithDisposableAssemblies(workingDirectory, publicLibraryAssemblyDefinition, internalLibraryAssemblyDefinition, ilRepackedAssemblyDefinition))
            {
                var task = new ILRepack
                {
                    FakeBuildEngine = new FakeBuildEngine(),
                    OutputType = "Library",
                    MainAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    OutputAssembly = ilRepackedAssemblyDefinition.GetRelativeFilename(),
                    InputAssemblies = new List<ITaskItem>
                    {
                        new TaskItem(publicLibraryAssemblyDefinition.GetRelativeFilename()),
                        new TaskItem(internalLibraryAssemblyDefinition.GetRelativeFilename())
                    }.ToArray(),
                    InternalizeExcludeAssemblies = new ITaskItem[]
                    {
                        new TaskItem(publicLibraryAssemblyDefinition.GetInternalizeRegex())
                    },
                    WorkingDirectory = workingDirectory
                };

                Assert.True(task.Execute());

                Assert.True(File.Exists(task.OutputAssembly));

                using (ilRepackedAssemblyDefinition = AssemblyDefinition.ReadAssembly(task.OutputAssembly))
                {
                    Assert.NotNull(ilRepackedAssemblyDefinition);

                    var references = ilRepackedAssemblyDefinition.MainModule.AssemblyReferences.ToList();
                    Assert.Single(references);
                    Assert.Equal("mscorlib", references[0].Name);

                    var types = ilRepackedAssemblyDefinition.MainModule.Types.ToList();

                    var thisClassShouldBePublic = types.SingleOrDefault(x => x.Namespace == publicLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBePublic");
                    Assert.NotNull(thisClassShouldBePublic);
                    Assert.True(thisClassShouldBePublic.IsPublic);

                    var thisClassShouldBeInternal = types.SingleOrDefault(x => x.Namespace == internalLibraryAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldBeInternal");
                    Assert.NotNull(thisClassShouldBeInternal);
                    Assert.False(thisClassShouldBeInternal.IsPublic);

                    var thisClassShouldAlsoBePublic = types.SingleOrDefault(x => x.Namespace == ilRepackedAssemblyDefinition.MainModule.Name && x.Name == "ThisClassShouldAlsoBePublic");
                    Assert.NotNull(thisClassShouldAlsoBePublic);
                    Assert.True(thisClassShouldAlsoBePublic.IsPublic);
                }

            }
        }

    }
}
