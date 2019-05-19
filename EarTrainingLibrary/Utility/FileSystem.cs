using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarTrainingLibrary.Utility
{
    public class FileSystem
    {
        public static void CleanFolder(string tempFolder)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(tempFolder);
            DateTime cutoff = DateTime.Now.AddMinutes(-5);
            IEnumerable<FileInfo> files = dirInfo.GetFiles().Where(w => w.LastWriteTime < cutoff);
            foreach (var file in files)
            {
                File.Delete(file.FullName);
            }
        }
    }
}
