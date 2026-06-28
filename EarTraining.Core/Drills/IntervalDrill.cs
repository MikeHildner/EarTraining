using EarTraining.Core.Theory;

namespace EarTraining.Core.Drills;

/// <summary>
/// One interval-identification prompt: a low note, the note an interval above it,
/// and the correct answer. Play the two notes melodic (in sequence) or harmonic
/// (together) and have the listener pick the interval.
/// </summary>
public sealed record IntervalDrill(int LowNote, int HighNote, Interval Answer)
{
    public string LowName => Note.Name(LowNote);
    public string HighName => Note.Name(HighNote);

    /// <summary>The answer choices to present.</summary>
    public static IReadOnlyList<Interval> Options => Interval.Common;

    /// <summary>A random interval with its root in a comfortable mid-range (C3..C5).</summary>
    public static IntervalDrill Next(Random rng)
    {
        var answer = Interval.Common[rng.Next(Interval.Common.Count)];
        int low = rng.Next(27, 52); // 27 = C3 .. 51 = C5
        return new IntervalDrill(low, low + answer.Semitones, answer);
    }
}
