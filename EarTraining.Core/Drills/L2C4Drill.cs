namespace EarTraining.Core.Drills;

/// <summary>
/// L2C4 Major Triad Progressions (2- and 3-chord, pp. 63 & 65): all-major-triad progressions that
/// demonstrate a root-MOVEMENT type — Circle of 5th (V→I), Circle of 4th (IV→I), Half-step up
/// (I→♭II), Half-step down (I→♭VII). The key is re-randomized every drill, so the exact inversions
/// aren't guessable by ear — the quiz identifies the <see cref="Category"/> (movement type). Ported
/// from L2C4Controller.Get2/3ChordProgression. Each chord is 3 tone offsets from DO (post-inversion,
/// sorted); no bass voice. (3-chord category labels follow the book's wording, even where the middle
/// chord doesn't literally match the numeral.)
/// </summary>
public sealed record L2C4Drill(string Category, IReadOnlyList<IReadOnlyList<int>> Chords)
{
    private enum Inv { Root, HighFirst, HighSecond, LowFirst, LowSecond }

    private static IReadOnlyList<int> Tri(int n1, int n2, int n3, Inv inv)
    {
        switch (inv)
        {
            case Inv.HighFirst: n1 += 12; break;
            case Inv.HighSecond: n1 += 12; n2 += 12; break;
            case Inv.LowSecond: n3 -= 12; break;
            case Inv.LowFirst: n2 -= 12; n3 -= 12; break;
        }
        var t = new[] { n1, n2, n3 };
        Array.Sort(t);
        return t;
    }

    // Major triads as root-position tone offsets from DO, then inverted.
    private static IReadOnlyList<int> One(Inv i) => Tri(0, 4, 7, i);        // I    (DO MI SO)
    private static IReadOnlyList<int> Five(Inv i) => Tri(7, 11, 14, i);     // V    (SO TI RE)
    private static IReadOnlyList<int> Four(Inv i) => Tri(5, 9, 12, i);      // IV   (FA LA DO)
    private static IReadOnlyList<int> FlatTwo(Inv i) => Tri(1, 5, 8, i);    // ♭II  (half-step above)
    private static IReadOnlyList<int> SevenBelow(Inv i) => Tri(-1, 3, 6, i);// major triad a half-step below
    private static IReadOnlyList<int> SharpFour(Inv i) => Tri(6, 10, 13, i);// the 3-chord tail (♭V major)
    private static IReadOnlyList<int> FlatSix(Inv i) => Tri(8, 12, 15, i);  // ♭VI major (½-up → 4th tail)
    private static IReadOnlyList<int> Three(Inv i) => Tri(4, 8, 11, i);     // III major (½-down → 5th tail)

    public static readonly string[] TwoChordCategories = ["Circle of 5th", "Circle of 4th", "Half-step up", "Half-step down"];
    public static readonly string[] ThreeChordCategories =
        ["5th → ½ up", "4th → ½ down", "½ up → 5th", "½ up → 4th", "½ down → 5th", "½ down → 4th"];

    /// <summary>10 two-chord voicings (controller types 0-9), tagged by movement category.</summary>
    public static readonly IReadOnlyList<L2C4Drill> TwoChord =
    [
        new("Circle of 5th", [Five(Inv.LowSecond), One(Inv.HighFirst)]),
        new("Circle of 5th", [Five(Inv.LowFirst), One(Inv.Root)]),
        new("Circle of 5th", [Five(Inv.Root), One(Inv.HighSecond)]),
        new("Circle of 4th", [Four(Inv.LowFirst), One(Inv.LowSecond)]),
        new("Circle of 4th", [Four(Inv.Root), One(Inv.HighFirst)]),
        new("Circle of 4th", [Four(Inv.LowSecond), One(Inv.Root)]),
        new("Half-step up", [One(Inv.Root), FlatTwo(Inv.Root)]),
        new("Half-step up", [One(Inv.Root), FlatTwo(Inv.LowSecond)]),
        new("Half-step down", [One(Inv.Root), SevenBelow(Inv.Root)]),
        new("Half-step down", [One(Inv.Root), SevenBelow(Inv.HighFirst)]),
    ];

    /// <summary>3-chord voicings: the two web-era movements (controller types 100 + 101) plus the
    /// four reversed, ½-step-first movements from the book's 3-chord answer key (p. 93) — GitHub #11.
    /// Roots mod 12: ½U→5th lands on ♯IV, ½U→4th on ♭VI, ½D→5th on III, ½D→4th on ♯IV.</summary>
    public static readonly IReadOnlyList<L2C4Drill> ThreeChord =
    [
        new("5th → ½ up", [One(Inv.LowSecond), Four(Inv.LowFirst), SharpFour(Inv.LowFirst)]),
        new("4th → ½ down", [One(Inv.Root), Five(Inv.LowFirst), SharpFour(Inv.LowFirst)]),
        new("½ up → 5th", [One(Inv.Root), FlatTwo(Inv.Root), SharpFour(Inv.LowSecond)]),
        new("½ up → 4th", [One(Inv.Root), FlatTwo(Inv.Root), FlatSix(Inv.LowFirst)]),
        new("½ down → 5th", [One(Inv.Root), SevenBelow(Inv.Root), Three(Inv.LowSecond)]),
        new("½ down → 4th", [One(Inv.Root), SevenBelow(Inv.Root), SharpFour(Inv.LowFirst)]),
    ];
}
