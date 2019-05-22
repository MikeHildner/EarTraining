using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIOWAAiffToWav
{
    class Program
    {
        static void Main(string[] args)
        {
            //To get the location the assembly normally resides on disk or the install directory
            string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

            //once you have the path you get the directory with:
            var directory = System.IO.Path.GetDirectoryName(path);

            string anotherAttempt = AppDomain.CurrentDomain.BaseDirectory;

            //string[] aiffFiles = Directory.GetFiles(directory);

            //string[] aiffFiles = Directory.GetFiles(anotherAttempt);

            var urghHardCoded = @"C:\Users\Mike\source\repos\EarTraining\EarTraining\UIOWA_AIFFs";

            string[] aiffFiles = Directory.GetFiles(urghHardCoded);

            ConvertFiles(aiffFiles);
        }

        private static void ConvertFiles(string[] aiffFiles)
        {
            foreach (var aiffFile in aiffFiles)
            {

            }
        }
    }
}
