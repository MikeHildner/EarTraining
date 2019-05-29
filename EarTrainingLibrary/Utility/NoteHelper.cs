using EarTrainingLibrary.NAudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarTrainingLibrary.Utility
{
    public static class NoteHelper
    {
        public static int NoteNumberFromNoteName(string noteName)
        {
            string thisFileName = NAudioHelper.GetFileNameFromNoteName(noteName);
            thisFileName = Path.GetFileName(thisFileName);
            int noteNumber = int.Parse(thisFileName.Split('.')[0]);
            return noteNumber;
        }
    }
}
