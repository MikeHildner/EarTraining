namespace EarTraining.Core.Drills;

/// <summary>
/// The Level 2 Chapter 5 progression walk: four random distinct diatonic triads (optionally
/// starting on the tonic), each with a random inversion, returned as tone offsets from DO
/// (post-inversion, sorted) plus the answer degrees/roots — the page supplies a random key.
/// Ported from the L2C5 view generator + GetTriad. (The class also once held the play-only
/// "Major Triad Movements" explorer, removed with its page — the L2C4 quiz covers that ground.)
/// </summary>
public static class L2Explorer
{
    private enum Inv { Root, HighFirst, HighSecond, LowFirst, LowSecond }
    private static readonly Inv[] AllInv = [Inv.Root, Inv.HighFirst, Inv.HighSecond, Inv.LowFirst, Inv.LowSecond];

    private static IReadOnlyList<int> Invert(int n1, int n2, int n3, Inv inv)
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

    private static readonly int[] MajorScale = [0, 2, 4, 5, 7, 9, 11];

    // Diatonic triad (stacked thirds in the major scale) for scale degree 1-7, as offsets from DO.
    private static (int, int, int) DiatonicTones(int degree)
    {
        int d = degree - 1;
        int root = MajorScale[d];
        int third = MajorScale[(d + 2) % 7] + (d + 2 >= 7 ? 12 : 0);
        int fifth = MajorScale[(d + 4) % 7] + (d + 4 >= 7 ? 12 : 0);
        return (root, third, fifth);
    }

    /// <summary>Roman-numeral labels for scale degrees 1-7 (case follows triad quality).</summary>
    public static readonly IReadOnlyList<string> RomanLabels = ["I", "ii", "iii", "IV", "V", "vi", "vii°"];

    /// <summary>
    /// The scored walk (book Ch. 5 workbook, pp. 69-71): returns the answer — the four scale
    /// degrees — and each chord's root offset from DO so the page can supply the bass voice
    /// ("hearing the root in the bass voice, and voiceled upper triads").
    /// </summary>
    public static (IReadOnlyList<int> Degrees, IReadOnlyList<int> Roots, IReadOnlyList<IReadOnlyList<int>> Chords)
        DiatonicQuizWalk(bool tonicFirst, Random rng)
    {
        var degrees = Enumerable.Range(1, 7).OrderBy(_ => rng.Next()).Take(7).ToList();
        if (tonicFirst) { degrees.Remove(1); degrees.Insert(0, 1); }
        degrees = degrees.Take(4).ToList();
        var roots = new List<int>(4);
        var chords = new List<IReadOnlyList<int>>(4);
        foreach (int degree in degrees)
        {
            var (r, t, f) = DiatonicTones(degree);
            roots.Add(r);
            chords.Add(Invert(r, t, f, AllInv[rng.Next(AllInv.Length)]));
        }
        return (degrees, roots, chords);
    }
}
