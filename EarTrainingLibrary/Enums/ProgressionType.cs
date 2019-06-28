namespace EarTrainingLibrary.Enums
{
    public enum ProgressionType
    {
        // I IV V two chord progressions.
        Four2ndToOneRoot = 0,
        Five1stToOneRoot = 1,
        FiveRootToFourRoot = 2,
        OneRootToFour2nd = 3,

        // I IV V three chord progressions.
        Four2ndFive1stOneRoot = 4,
        Five1stFour2ndOneRoot = 5,
        OneRootFour2ndFive1st = 6,
        Five1stOneRootFour2nd = 7,
        OneRootFive1stFour2nd = 8,
        Four2ndOneRootFive1st = 9,

        // I IV V iv two chord progressions.
        SixMin2ndToFourRoot = 10,
        FiveRootToSixMinRoot = 11,
        SixMin1stToOneRoot = 12,
        FourRootToFiveRoot = 13, // OK, not a minor, but it came in at chapter 5, so putting here.
        SixMinRootToFiveRoot = 14,
        FourRootToSixMin2nd = 15,

        // I IV V iv three chord progressions.
        OneRootFive1stSixMin1st = 16,
        SixMin2ndFourRootOne1st = 17,
        OneRootSixMin1stFive1st = 18,
        FiveRootFourRootSixMin2nd = 19,
        FourRootFiveRootSixMinRoot = 20,
        SixMin1stOne2ndFour1st = 21,
        Five2ndSixMin2ndOneRoot = 22
    }
}
