using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarTrainingLibrary.Enums
{
    public static class Interval
    {
        public static int DownHalfStep { get { return -1; } }
        public static int Up2nd { get { return 2; } }
        public static int UpMajor3rd{get{ return 4; }}
        public static int UpPerfect4th { get { return 5; } }
        public static int UpPerfect5th { get { return 7; } }
        public static int UpMajor6th { get { return 9; } }
        public static int UpMajor7th { get { return 11; } }
    }
}
