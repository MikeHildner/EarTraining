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
        // I IV V vi iii 2 chord progressions.
        OneRootToSixMin1st = 0,
        OneRootToFour2nd = 1,
        FourRootToFiveRoot = 2,
        One2ndToThreeMin1st = 3,
        ThreeMin1stToSixMinRoot = 4,
        Five2ndToOne1st = 5,
        One1stToFour1st = 6,
        One2ndToSixMinRoot = 7,
        ThreeMinRootToFive2nd = 8,
        Five2ndToSixMin2nd = 9,
        Four2ndToThreeMin2nd = 10,

        // I IV V vi iii 3 chord progressions.
        OneRootFive1stSixMin1st = 11,
        One1stThreeMinRootFourRoot = 12,
        SixMin2ndFive2ndFour2nd = 13,
        ThreeMin2ndOneRootSixMin1st = 14,
        FourRootOne1stThreeMinRoot = 15,
        SixMin2ndThreeMinRootOne1st = 16,
        One2ndFour1stFiveRoot = 17,
        SixMinRootOne2ndFour1st = 18,
        ThreeMin1stSixMinRootFour1st = 19,
        OneRootSixMin1stFive1st = 20,
        Five2ndOne1stSixMinSixMin2nd = 21,
        FourRootFive2ndSixMin2nd = 22
    }
}
