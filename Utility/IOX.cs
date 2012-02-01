using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace L24CM.Utility
{
    public static class IOX
    {
        public static void DeleteRecursive(this DirectoryInfo dir)
        {
            if (dir.FullName == "C:\\"
                || dir.FullName.StartsWith("C:\\Windows")
                || dir.FullName.StartsWith("C:\\Program Files"))
                throw new ArgumentException("Cannot recursive delete from system folders");

            FileInfo[] files = dir.GetFiles();
            DirectoryInfo[] dirs = dir.GetDirectories();

            foreach (FileInfo file in files)
            {
                file.Attributes = FileAttributes.Normal;
                file.Delete();
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                subDir.DeleteRecursive();
            }

            dir.Delete(false);
        }
    }
}
