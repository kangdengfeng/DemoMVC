using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calc
{
    public class FileUtils
    {
        public static void WriteFile(string filePath,string content)
        {
            File.WriteAllText(filePath,content);
        }
        public static string ReadFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }
        public static bool IsFileExists(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}
