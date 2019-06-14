using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarTrainingLibrary.Enums
{
    public static class Interval
    {
        public static int Down1Octave { get { return -12; } }
        public static int DownPerfect4th { get { return -5; } }
        public static int DownMinor3rd { get { return -3; } }
        public static int DownHalfStep { get { return -1; } }
        public static int Up2nd { get { return 2; } }
        public static int UpMinor3rd { get { return 3; } }
        public static int UpMajor3rd{get{ return 4; }}
        public static int UpPerfect4th { get { return 5; } }
        public static int UpPerfect5th { get { return 7; } }
        public static int UpMajor6th { get { return 9; } }
        public static int UpMajor7th { get { return 11; } }
        public static int Up1Octave { get { return 12; } }
        public static int UpMajor9th { get { return 14; } }
        public static int UpMajor10th { get { return 16; } }
    }
}
