namespace EarTraining.Core.Theory;

/// <summary>A musical interval: its size in semitones plus labels for display.</summary>
public sealed record Interval(int Semitones, string Name, string ShortName)
{
    /// <summary>The twelve intervals from a minor 2nd up to the octave.</summary>
    public static readonly IReadOnlyList<Interval> Common =
    [
        new(1,  "Minor 2nd",   "m2"),
        new(2,  "Major 2nd",   "M2"),
        new(3,  "Minor 3rd",   "m3"),
        new(4,  "Major 3rd",   "M3"),
        new(5,  "Perfect 4th", "P4"),
        new(6,  "Tritone",     "TT"),
        new(7,  "Perfect 5th", "P5"),
        new(8,  "Minor 6th",   "m6"),
        new(9,  "Major 6th",   "M6"),
        new(10, "Minor 7th",   "m7"),
        new(11, "Major 7th",   "M7"),
        new(12, "Octave",      "P8"),
    ];
}
