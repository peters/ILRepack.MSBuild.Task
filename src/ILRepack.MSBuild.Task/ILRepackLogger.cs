#if !NETFULLFRAMEWORK
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Framework;

namespace ILRepack.MSBuild.Task
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    sealed class ILRepackLogger :  ILRepacking.ILogger
    {
        readonly Microsoft.Build.Utilities.Task _task;

        public ILRepackLogger(Microsoft.Build.Utilities.Task task)
        {
            _task = task ?? throw new ArgumentNullException(nameof(task));
        }

        public void Log(object str)
        {
            _task.Log.LogMessage(MessageImportance.Normal, str as string, str);
        }

        public void Error(string msg)
        {
            _task.Log.LogError(msg);
        }

        public void Warn(string msg)
        {
            _task.Log.LogWarning(msg);
        }

        public void Info(string msg)
        {
            _task.Log.LogMessage(MessageImportance.Normal, msg);
        }

        public void Verbose(string msg)
        {
            _task.Log.LogMessage(MessageImportance.Normal, msg);
        }

        public void DuplicateIgnored(string ignoredType, object ignoredObject)
        {
           
        }

        public bool ShouldLogVerbose { get; set; }
    }
}
#endif