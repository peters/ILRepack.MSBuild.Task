using System.Collections;
using Microsoft.Build.Framework;

namespace ILRepack.MSBuild.Task.Tests.MSBuild
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
}