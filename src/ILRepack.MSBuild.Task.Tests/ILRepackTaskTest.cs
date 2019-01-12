using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Xunit;

namespace ILRepack.MSBuild.Task.Tests
{
    public class FakeBuildEngine : IBuildEngine
    {
        public void LogErrorEvent(BuildErrorEventArgs e)
        {
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
        }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
        }

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            return true;
        }

        public bool ContinueOnError { get; set; }
        public int LineNumberOfTaskNode { get; set; }
        public int ColumnNumberOfTaskNode { get; set; }
        public string ProjectFileOfTaskNode { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ILRepackTaskTests
    {
        [Fact]
        public void TestExecute()
        {
            var task = new ILRepacking.ILRepack();
            task.FakeBuildEngine = new FakeBuildEngine();
            task.Execute();
        }
    }
}
