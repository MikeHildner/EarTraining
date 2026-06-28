namespace EarTraining.Core.Drills;

/// <summary>
/// Level 2 progression "explorers" — these build a RANDOM progression each time, so there's no
/// finite answer set (no quiz/scoring): you just listen. Two generators:
/// <list type="bullet">
/// <item><b>MajorWalk</b> (L2/MajorTriadProgressions) — a chain of major triads whose root steps by
/// a chosen movement each chord (Circle of 5ths / 4ths / half-step up / down), random inversions.</item>
/// <item><b>DiatonicWalk</b> (L2C5/DiatonicTriadProgressions4) — four random distinct diatonic triads
/// (optionally starting on the tonic), random inversions.</item>
/// </list>
/// Chords are returned as tone offsets from DO (post-inversion, sorted); the page supplies a random
/// key. Ported from the L2 / L2C5 view generators + L2Controller.CreateMajorTriad / L2C5 GetTriad.
/// </summary>
public static class L2Explorer
{
    public enum Movement { CircleOf5ths, CircleOf4ths, HalfStepUp, HalfStepDown }

    // Root pitch-class step per movement (semitones). Circle of 5ths = down a 5th (≡ up a 4th),
    // Circle of 4ths = down a 4th (≡ up a 5th); roots are folded back near DO so they don't drift.
    private static int Delta(Movement m) => m switch
    {
        Movement.CircleOf5ths => -7,
        Movement.CircleOf4ths => -5,
        Movement.HalfStepUp => 1,
        Movement.HalfStepDown => -1,
        _ => 0,
    };

    public static string Label(Movement m) => m switch
    {
        Movement.CircleOf5ths => "Circle of 5ths",
        Movement.CircleOf4ths => "Circle of 4ths",
        Movement.HalfStepUp => "Half-step up",
        Movement.HalfStepDown => "Half-step down",
        _ => "",
    };

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

    // Fold a root pitch-class into the octave nearest DO (offset 0), i.e. into [-6, 5].
    private static int FoldNearDo(int offset)
    {
        while (offset > 5) offset -= 12;
        while (offset < -6) offset += 12;
        return offset;
    }

    /// <summary>
    /// A chain of <paramref name="count"/> major triads. The first root is DO; each subsequent root
    /// steps by a randomly chosen included <paramref name="movements"/>. Every chord gets a random
    /// inversion. Returns each chord's tone offsets from DO.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<int>> MajorWalk(int count, IReadOnlyList<Movement> movements, Random rng)
    {
        var chords = new List<IReadOnlyList<int>>(count);
        int root = 0;
        for (int i = 0; i < count; i++)
        {
            if (i > 0) root = FoldNearDo(root + Delta(movements[rng.Next(movements.Count)]));
            var inv = AllInv[rng.Next(3)]; // root / 1st / 2nd, matching the web's three inversions
            chords.Add(Invert(root, root + 4, root + 7, inv)); // major triad on the moving root
        }
        return chords;
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

    /// <summary>
    /// Four random, distinct diatonic triads (scale degrees 1-7), each with a random inversion. When
    /// <paramref name="tonicFirst"/>, the first chord is the tonic (I). Returns tone offsets from DO.
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<int>> DiatonicWalk(bool tonicFirst, Random rng)
    {
        var degrees = Enumerable.Range(1, 7).OrderBy(_ => rng.Next()).ToList();
        if (tonicFirst) { degrees.Remove(1); degrees.Insert(0, 1); }
        var chords = new List<IReadOnlyList<int>>(4);
        for (int i = 0; i < 4; i++)
        {
            var (r, t, f) = DiatonicTones(degrees[i]);
            chords.Add(Invert(r, t, f, AllInv[rng.Next(AllInv.Length)]));
        }
        return chords;
    }
}
