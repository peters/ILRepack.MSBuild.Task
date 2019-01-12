using System;
using System.Collections.Generic;
using System.IO;

namespace ILRepack.MSBuild.Task.Tests.Misc
{
    class DisposableFiles : IDisposable
    {
        readonly IReadOnlyCollection<string> _filenames;

        public DisposableFiles(IReadOnlyCollection<string> filenames)
        {
            _filenames = filenames ?? throw new ArgumentNullException(nameof(filenames));
        }

        public void Dispose()
        {
            foreach (var filename in _filenames)
            {
                try
                {
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                }
                catch (Exception)
                {
                    // ignore
                }
            }
        }
    }

}
