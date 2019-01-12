#if !NETFULLFRAMEWORK
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Framework;
using Mono.Cecil;

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
            var message = str as string;
            _task.Log.LogMessage(MessageImportance.High, $"{nameof(ILRepack)}: {message}", str);
        }

        public void Error(string msg)
        {
            _task.Log.LogError($"{nameof(ILRepack)}: {msg}");
        }

        public void Warn(string msg)
        {
            _task.Log.LogWarning($"{nameof(ILRepack)}: {msg}");
        }

        public void Info(string msg)
        {
            _task.Log.LogMessage(MessageImportance.High, $"{nameof(ILRepack)}: {msg}");
        }

        public void Verbose(string msg)
        {
            _task.Log.LogMessage(MessageImportance.High, $"{nameof(ILRepack)}: {msg}");
        }

        public void DuplicateIgnored(string ignoredType, object ignoredObject)
        {
            string type;
  
            switch (ignoredObject)
            {
                case FieldDefinition _:
                    type = nameof(FieldDefinition);
                    break;
                case EventDefinition _:
                    type = nameof(EventDefinition);
                    break;
                case PropertyDefinition _:
                    type = nameof(PropertyDefinition);
                    break;
                case MethodDefinition _:
                    type = nameof(MethodDefinition);
                    break;
                default:
                    type = $"{ignoredObject?.GetType().Name}";
                    break;
            }

           _task.Log.LogWarning($"{nameof(ILRepack)}: Duplicate ignored: {ignoredType}. Mono.Cecil type: {type}.");
        }

        public bool ShouldLogVerbose { get; set; }
    }
}
#endif