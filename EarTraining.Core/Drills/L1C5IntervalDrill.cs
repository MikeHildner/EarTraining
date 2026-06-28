namespace EarTraining.Core.Drills;

/// <summary>
/// An L1C5 interval-identification prompt — a 4th or 5th. Melodic prompts are two notes in
/// sequence and carry a direction (Asc/Desc); harmonic prompts are two notes together (no
/// direction). The quiz identifies <see cref="Category"/> (the quality, plus direction for
/// melodic), not the specific solfège pair. Ported from L1C5Controller.GetMelodicInterval
/// (types 14-41) and GetHarmonicDrillEx (types 0-13). <see cref="Offsets"/> are semitones from DO.
/// </summary>
public sealed record L1C5IntervalDrill(string Label, string Group, string Category, IReadOnlyList<int> Offsets)
{
    /// <summary>28 melodic 4ths (asc + desc) and 5ths (asc + desc); identify quality + direction.</summary>
    public static readonly IReadOnlyList<L1C5IntervalDrill> Melodic =
    [
        // 4ths — ascending
        new("DO FA", "4th", "Perfect 4th Asc", [0, 5]),
        new("RE SO", "4th", "Perfect 4th Asc", [2, 7]),
        new("MI LA", "4th", "Perfect 4th Asc", [4, 9]),
        new("FA TI", "4th", "Augmented 4th Asc", [5, 11]),
        new("SO DO", "4th", "Perfect 4th Asc", [7, 12]),
        new("LA RE", "4th", "Perfect 4th Asc", [9, 14]),
        new("TI MI", "4th", "Perfect 4th Asc", [11, 16]),
        // 4ths — descending
        new("FA DO", "4th", "Perfect 4th Desc", [5, 0]),
        new("SO RE", "4th", "Perfect 4th Desc", [7, 2]),
        new("LA MI", "4th", "Perfect 4th Desc", [9, 4]),
        new("TI FA", "4th", "Augmented 4th Desc", [11, 5]),
        new("DO SO", "4th", "Perfect 4th Desc", [12, 7]),
        new("RE LA", "4th", "Perfect 4th Desc", [14, 9]),
        new("MI TI", "4th", "Perfect 4th Desc", [16, 11]),
        // 5ths — ascending
        new("DO SO", "5th", "Perfect 5th Asc", [0, 7]),
        new("RE LA", "5th", "Perfect 5th Asc", [2, 9]),
        new("MI TI", "5th", "Perfect 5th Asc", [4, 11]),
        new("FA DO", "5th", "Perfect 5th Asc", [5, 12]),
        new("SO RE", "5th", "Perfect 5th Asc", [7, 14]),
        new("LA MI", "5th", "Perfect 5th Asc", [9, 16]),
        new("TI FA", "5th", "Diminished 5th Asc", [11, 17]),
        // 5ths — descending
        new("SO DO", "5th", "Perfect 5th Desc", [7, 0]),
        new("LA RE", "5th", "Perfect 5th Desc", [9, 2]),
        new("TI MI", "5th", "Perfect 5th Desc", [11, 4]),
        new("DO FA", "5th", "Perfect 5th Desc", [12, 5]),
        new("RE SO", "5th", "Perfect 5th Desc", [14, 7]),
        new("MI LA", "5th", "Perfect 5th Desc", [16, 9]),
        new("FA TI", "5th", "Diminished 5th Desc", [17, 11]),
    ];

    /// <summary>14 harmonic 4ths and 5ths ({low, high}); identify quality (no direction).</summary>
    public static readonly IReadOnlyList<L1C5IntervalDrill> Harmonic =
    [
        new("DO FA", "4th", "Perfect 4th", [0, 5]),
        new("RE SO", "4th", "Perfect 4th", [2, 7]),
        new("MI LA", "4th", "Perfect 4th", [4, 9]),
        new("FA TI", "4th", "Augmented 4th", [5, 11]),
        new("SO DO", "4th", "Perfect 4th", [7, 12]),
        new("LA RE", "4th", "Perfect 4th", [9, 14]),
        new("TI MI", "4th", "Perfect 4th", [11, 16]),
        new("DO SO", "5th", "Perfect 5th", [0, 7]),
        new("RE LA", "5th", "Perfect 5th", [2, 9]),
        new("MI TI", "5th", "Perfect 5th", [4, 11]),
        new("FA DO", "5th", "Perfect 5th", [5, 12]),
        new("SO RE", "5th", "Perfect 5th", [7, 14]),
        new("LA MI", "5th", "Perfect 5th", [9, 16]),
        new("TI FA", "5th", "Diminished 5th", [11, 17]),
    ];
}
