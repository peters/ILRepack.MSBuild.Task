using System;
using System.IO;

namespace ILRepack.MSBuild.Task.Tests.Extensions
{
    static class IoExtensions
    {
        public static void DeleteFileSafe(this string filename)
        {
            if (filename == null) throw new ArgumentNullException(nameof(filename));
            try
            {
                if(File.Exists(filename))
                {
                    File.Delete(filename);
                }
            }
            catch (Exception e)
            {
                // ignore
            }
        }

        public static void DeleteResidueMyLibraries(this string workingDirectory)
        {
            if (workingDirectory == null) throw new ArgumentNullException(nameof(workingDirectory));
            foreach (var file in Directory.GetFiles(workingDirectory, "*.dll"))
            {
                if (file.StartsWith("My") && file.EndsWith(".dll"))
                {
                    file.DeleteFileSafe();
                }
            }
        }
    }
}
