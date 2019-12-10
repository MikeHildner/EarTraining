using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarTrainingLibrary.Enums
{
    public enum L2C4ProgressionType
    {
        // 2 chords.
        Five2ndToOne1st = 0,
        Five1stToOneRoot = 1,
        FiveRootToOne2nd = 2,
        Four1stToOne2nd = 3,
        FourRootToOne1st = 4,
        Four2ndToOneRoot = 5,
        OneRootToFlatTwoRoot = 6,
        OneRootToFlatTwo2nd = 7,
        OneRootToSevenRoot = 8,
        OneRootToSeven1st = 9,

        // 3 chords.
        OneThen5ThenHalfUp = 100,
        OneThen4ThenHalfDown = 101,
    }
}
