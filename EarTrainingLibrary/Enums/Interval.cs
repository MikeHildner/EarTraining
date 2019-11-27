using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarTrainingLibrary.Enums
{
    public static class Interval
    {
        public static int DownPerfectOctave { get { return -12; } }
        public static int DownPerfect4th { get { return -5; } }
        public static int DownMinor3rd { get { return -3; } }
        public static int DownMinor2nd { get { return -1; } }
        public static int UpMinor2nd { get { return 1; } }
        public static int UpMajor2nd { get { return 2; } }
        public static int UpMinor3rd { get { return 3; } }
        public static int UpMajor3rd{get{ return 4; }}
        public static int UpPerfect4th { get { return 5; } }
        public static int UpDiminished5th { get { return 6; } }
        public static int UpPerfect5th { get { return 7; } }
        public static int UpAugmented5th { get { return 8; } }
        public static int UpMajor6th { get { return 9; } }
        public static int UpMinor7th { get { return 10; } }
        public static int UpMajor7th { get { return 11; } }
        public static int UpPerfectOctave { get { return 12; } }
        public static int UpMinor9th { get { return 13; } }
        public static int UpMajor9th { get { return 14; } }
        public static int UpMajor10th { get { return 16; } }
        public static int UpPerfect11th { get { return 17; } }
        public static int UpPerfect12th { get { return 19; } }
        public static int UpMajor13th { get { return 21; } }
    }
}
