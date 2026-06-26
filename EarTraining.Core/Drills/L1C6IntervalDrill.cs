namespace EarTraining.Core.Drills;

/// <summary>
/// An L1C6 interval-identification prompt — a Major 2nd or Minor 7th. Melodic prompts are two
/// notes in sequence with a direction (Asc/Desc); harmonic prompts are two notes together (no
/// direction). The quiz identifies <see cref="Category"/>. Ported from L1C6Controller.GetMelodicInterval
/// + GetHarmonicDrillEx. <see cref="Offsets"/> are semitones from DO. (Same shape as L1C5IntervalDrill.)
/// </summary>
public sealed record L1C6IntervalDrill(string Label, string Group, string Category, IReadOnlyList<int> Offsets)
{
    /// <summary>20 melodic Major 2nds + Minor 7ths (asc + desc); identify quality + direction.</summary>
    public static readonly IReadOnlyList<L1C6IntervalDrill> Melodic =
    [
        // Major 2nds — ascending
        new("DO RE", "Maj 2nd", "Major 2nd Asc", [0, 2]),
        new("RE MI", "Maj 2nd", "Major 2nd Asc", [2, 4]),
        new("FA SO", "Maj 2nd", "Major 2nd Asc", [5, 7]),
        new("SO LA", "Maj 2nd", "Major 2nd Asc", [7, 9]),
        new("LA TI", "Maj 2nd", "Major 2nd Asc", [9, 11]),
        // Major 2nds — descending
        new("RE DO", "Maj 2nd", "Major 2nd Desc", [2, 0]),
        new("MI RE", "Maj 2nd", "Major 2nd Desc", [4, 2]),
        new("SO FA", "Maj 2nd", "Major 2nd Desc", [7, 5]),
        new("LA SO", "Maj 2nd", "Major 2nd Desc", [9, 7]),
        new("TI LA", "Maj 2nd", "Major 2nd Desc", [11, 9]),
        // Minor 7ths — ascending
        new("RE DO", "Min 7th", "Minor 7th Asc", [2, 12]),
        new("MI RE", "Min 7th", "Minor 7th Asc", [4, 14]),
        new("SO FA", "Min 7th", "Minor 7th Asc", [7, 17]),
        new("LA SO", "Min 7th", "Minor 7th Asc", [9, 19]),
        new("TI LA", "Min 7th", "Minor 7th Asc", [11, 21]),
        // Minor 7ths — descending
        new("DO RE", "Min 7th", "Minor 7th Desc", [12, 2]),
        new("RE MI", "Min 7th", "Minor 7th Desc", [14, 4]),
        new("FA SO", "Min 7th", "Minor 7th Desc", [17, 7]),
        new("SO LA", "Min 7th", "Minor 7th Desc", [19, 9]),
        new("LA TI", "Min 7th", "Minor 7th Desc", [21, 11]),
    ];

    /// <summary>10 harmonic Major 2nds + Minor 7ths ({low, high}); identify quality (no direction).</summary>
    public static readonly IReadOnlyList<L1C6IntervalDrill> Harmonic =
    [
        new("DO RE", "Maj 2nd", "Major 2nd", [0, 2]),
        new("RE MI", "Maj 2nd", "Major 2nd", [2, 4]),
        new("FA SO", "Maj 2nd", "Major 2nd", [5, 7]),
        new("SO LA", "Maj 2nd", "Major 2nd", [7, 9]),
        new("LA TI", "Maj 2nd", "Major 2nd", [9, 11]),
        new("RE DO", "Min 7th", "Minor 7th", [2, 12]),
        new("MI RE", "Min 7th", "Minor 7th", [4, 14]),
        new("SO FA", "Min 7th", "Minor 7th", [7, 17]),
        new("LA SO", "Min 7th", "Minor 7th", [9, 19]),
        new("TI LA", "Min 7th", "Minor 7th", [11, 21]),
    ];
}
