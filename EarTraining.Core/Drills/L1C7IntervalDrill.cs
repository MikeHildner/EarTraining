namespace EarTraining.Core.Drills;

/// <summary>
/// An L1C7 interval-identification prompt — a Minor 2nd or Major 7th. Melodic prompts are two
/// notes in sequence with a direction (Asc/Desc); harmonic prompts are two notes together (no
/// direction). The quiz identifies <see cref="Category"/>. Ported from L1C7Controller.GetMelodicInterval
/// + GetHarmonicDrill. <see cref="Offsets"/> are semitones from DO. (Same shape as L1C5/L1C6IntervalDrill.)
/// </summary>
public sealed record L1C7IntervalDrill(string Label, string Group, string Category, IReadOnlyList<int> Offsets)
{
    /// <summary>8 melodic Minor 2nds + Major 7ths (asc + desc); identify quality + direction.</summary>
    public static readonly IReadOnlyList<L1C7IntervalDrill> Melodic =
    [
        // Minor 2nds
        new("MI FA", "Min 2nd", "Minor 2nd Asc", [4, 5]),
        new("TI DO", "Min 2nd", "Minor 2nd Asc", [11, 12]),
        new("FA MI", "Min 2nd", "Minor 2nd Desc", [5, 4]),
        new("DO TI", "Min 2nd", "Minor 2nd Desc", [12, 11]),
        // Major 7ths
        new("DO TI", "Maj 7th", "Major 7th Asc", [0, 11]),
        new("FA MI", "Maj 7th", "Major 7th Asc", [5, 16]),
        new("TI DO", "Maj 7th", "Major 7th Desc", [11, 0]),
        new("MI FA", "Maj 7th", "Major 7th Desc", [16, 5]),
    ];

    /// <summary>4 harmonic Minor 2nds + Major 7ths ({low, high}); identify quality (no direction).</summary>
    public static readonly IReadOnlyList<L1C7IntervalDrill> Harmonic =
    [
        new("MI FA", "Min 2nd", "Minor 2nd", [4, 5]),
        new("TI DO", "Min 2nd", "Minor 2nd", [11, 12]),
        new("DO TI", "Maj 7th", "Major 7th", [0, 11]),
        new("FA MI", "Maj 7th", "Major 7th", [5, 16]),
    ];
}
