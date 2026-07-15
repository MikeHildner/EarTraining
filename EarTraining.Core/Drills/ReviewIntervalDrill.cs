namespace EarTraining.Core.Drills;

/// <summary>
/// A mixed / review interval-identification prompt, unifying the per-chapter tables:
/// the C4 sets combine chapters 2+3 (the book's "all possible Ma3/Mi6/Mi3/Ma6" questions,
/// pp. 63-64) and the C8 sets add chapters 5-7 for the all-interval review (pp. 180-181).
/// The quiz identifies <see cref="Category"/> (the book's interval description — quality
/// only, no direction). Categories are derived from the interval's span so naming stays
/// uniform across the source tables; the 6-semitone tritone is spelled by scale position
/// like the book (FA-TI = Aug 4th, TI-FA' = Dim 5th).
/// </summary>
public sealed record ReviewIntervalDrill(string Label, string Category, IReadOnlyList<int> Offsets)
{
    /// <summary>Include-row label; distinguishes same-named prompts of different quality.</summary>
    public string IncludeLabel => $"{Label} ({Category})";

    /// <summary>C4 mixed melodic prompts: chapters 2+3 (25 = the book's question count).</summary>
    public static readonly IReadOnlyList<ReviewIntervalDrill> MelodicC4 = BuildC4Melodic();

    /// <summary>C4 mixed harmonic prompts: chapters 2+3.</summary>
    public static readonly IReadOnlyList<ReviewIntervalDrill> HarmonicC4 = BuildC4Harmonic();

    /// <summary>C8 review melodic prompts: chapters 2-7 (all diatonic intervals).</summary>
    public static readonly IReadOnlyList<ReviewIntervalDrill> MelodicC8 = BuildC8Melodic();

    /// <summary>C8 review harmonic prompts: chapters 2-7.</summary>
    public static readonly IReadOnlyList<ReviewIntervalDrill> HarmonicC8 = BuildC8Harmonic();

    private static List<ReviewIntervalDrill> BuildC4Melodic()
    {
        var list = new List<ReviewIntervalDrill>();
        foreach (var d in L1C2Drill.Melodic) list.Add(From(d.Label, d.Offsets));
        foreach (var d in L1C3Drill.Melodic) list.Add(From(d.Label, d.Offsets));
        return list;
    }

    private static List<ReviewIntervalDrill> BuildC4Harmonic()
    {
        var list = new List<ReviewIntervalDrill>();
        foreach (var d in L1C2Drill.Harmonic) list.Add(From(d.Label, d.Offsets));
        foreach (var d in L1C3Drill.Harmonic) list.Add(From(d.Label, d.Offsets));
        return list;
    }

    private static List<ReviewIntervalDrill> BuildC8Melodic()
    {
        var list = new List<ReviewIntervalDrill>(MelodicC4);
        foreach (var d in L1C5IntervalDrill.Melodic) list.Add(From(d.Label, d.Offsets));
        foreach (var d in L1C6IntervalDrill.Melodic) list.Add(From(d.Label, d.Offsets));
        foreach (var d in L1C7IntervalDrill.Melodic) list.Add(From(d.Label, d.Offsets));
        return list;
    }

    private static List<ReviewIntervalDrill> BuildC8Harmonic()
    {
        var list = new List<ReviewIntervalDrill>(HarmonicC4);
        foreach (var d in L1C5IntervalDrill.Harmonic) list.Add(From(d.Label, d.Offsets));
        foreach (var d in L1C6IntervalDrill.Harmonic) list.Add(From(d.Label, d.Offsets));
        foreach (var d in L1C7IntervalDrill.Harmonic) list.Add(From(d.Label, d.Offsets));
        return list;
    }

    private static ReviewIntervalDrill From(string label, IReadOnlyList<int> offsets) =>
        new(label, CategoryOf(offsets), offsets);

    private static string CategoryOf(IReadOnlyList<int> offsets)
    {
        int span = Math.Abs(offsets[1] - offsets[0]);
        return span switch
        {
            1 => "Min 2nd",
            2 => "Maj 2nd",
            3 => "Min 3rd",
            4 => "Maj 3rd",
            5 => "Perfect 4th",
            // Tritone: FA-TI (lower tone FA, pc 5) is the augmented 4th; TI-FA' (lower tone TI, pc 11) the diminished 5th.
            6 => ((Math.Min(offsets[0], offsets[1]) % 12) + 12) % 12 == 5 ? "Aug 4th" : "Dim 5th",
            7 => "Perfect 5th",
            8 => "Min 6th",
            9 => "Maj 6th",
            10 => "Min 7th",
            11 => "Maj 7th",
            _ => throw new NotSupportedException($"Unexpected interval span {span}."),
        };
    }
}
