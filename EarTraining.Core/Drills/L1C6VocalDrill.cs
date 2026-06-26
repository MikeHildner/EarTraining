namespace EarTraining.Core.Drills;

/// <summary>
/// An L1C6 vocal drill — a five-note Major 2nd / Minor 7th sung pattern (a pair of repeated
/// tones, another pair, then a closing whole note). Ported from L1C6Controller.GetMelodicDrillEx.
/// <see cref="Offsets"/> are semitones from DO; <see cref="Rhythm"/> is each note's length in
/// seconds. The same ten syllable patterns appear under both intervals, so the quiz dedupes by
/// <see cref="Label"/> (like L1C3 Vocal Drills) — you name the pattern, not the octave.
/// </summary>
public sealed record L1C6VocalDrill(string Label, string Group, IReadOnlyList<int> Offsets, IReadOnlyList<double> Rhythm)
{
    // Two rhythm shapes (seconds): the dotted-quarter + eighth syncopation lands on beat 1 (A) or beat 3 (B).
    private static readonly IReadOnlyList<double> A = [1.5, 0.5, 1, 1, 4];
    private static readonly IReadOnlyList<double> B = [1, 1, 1.5, 0.5, 4];

    /// <summary>20 drills — 10 Major 2nd (types 0-9) + 10 Minor 7th (types 10-19), order matches the controller.</summary>
    public static readonly IReadOnlyList<L1C6VocalDrill> All =
    [
        // Major 2nds
        new("DO DO RE RE DO", "Maj 2nd", [0, 0, 2, 2, 0], A),
        new("RE RE MI MI MI", "Maj 2nd", [2, 2, 4, 4, 4], B),
        new("FA FA SO SO SO", "Maj 2nd", [5, 5, 7, 7, 7], A),
        new("SO SO LA LA SO", "Maj 2nd", [7, 7, 9, 9, 7], B),
        new("LA LA TI TI DO", "Maj 2nd", [9, 9, 11, 11, 12], A),
        new("RE RE DO DO DO", "Maj 2nd", [2, 2, 0, 0, 0], B),
        new("MI MI RE RE DO", "Maj 2nd", [4, 4, 2, 2, 0], A),
        new("SO SO FA FA MI", "Maj 2nd", [7, 7, 5, 5, 4], B),
        new("LA LA SO SO SO", "Maj 2nd", [9, 9, 7, 7, 7], A),
        new("TI TI LA LA SO", "Maj 2nd", [11, 11, 9, 9, 7], B),
        // Minor 7ths (same syllable patterns, octave-displaced)
        new("RE RE DO DO DO", "Min 7th", [2, 2, 12, 12, 12], A),
        new("MI MI RE RE DO", "Min 7th", [4, 4, 14, 14, 12], B),
        new("SO SO FA FA MI", "Min 7th", [7, 7, 17, 17, 16], A),
        new("LA LA SO SO SO", "Min 7th", [9, 9, 19, 19, 19], B),
        new("TI TI LA LA SO", "Min 7th", [11, 11, 21, 21, 19], A),
        new("DO DO RE RE DO", "Min 7th", [12, 12, 2, 2, 0], B),
        new("RE RE MI MI MI", "Min 7th", [14, 14, 4, 4, 0], A),
        new("FA FA SO SO SO", "Min 7th", [17, 17, 7, 7, 7], B),
        new("SO SO LA LA SO", "Min 7th", [19, 19, 9, 9, 7], A),
        new("LA LA TI TI DO", "Min 7th", [21, 21, 11, 11, 12], B),
    ];
}
