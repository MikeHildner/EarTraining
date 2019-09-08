using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarTrainingLibrary.Enums
{
    // ProgressionType enum was getting a little too large to easily look at.
    public enum ProgressionType2
    {
        // I IV V vi ii 2 chord progressions.
        OneRootToSixMin1st = 0,
        OneRootToFourMaj2nd = 1,
        FourRootToFiveRoot = 2,
        One2ndToThreeMin1st = 3,
        ThreeMin1stToSixMinRoot = 4,
        Five2ndToOne1st = 5,
        One1stToFour1st = 6,
        One2ndToSixMinRoot = 7,
        ThreeMinRootToFive2nd = 8,
        Five2ndToSixMin2nd = 9,
        Four2ndToThreeMin2nd = 10
    }
}
