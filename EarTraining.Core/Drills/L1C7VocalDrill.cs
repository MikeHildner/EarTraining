namespace EarTraining.Core.Drills;

/// <summary>
/// An L1C7 vocal drill — a short Minor 2nd / Major 7th sung pattern (3 or 4 notes, closing on a
/// whole note). Ported from L1C7Controller.GetMelodicDrill. <see cref="Offsets"/> are semitones
/// from DO; <see cref="Rhythm"/> is each note's length in seconds. All eight labels are unique
/// (across both intervals), so the quiz identifies the full pattern (like L1C5 Vocal Drills).
/// </summary>
public sealed record L1C7VocalDrill(string Label, string Group, IReadOnlyList<int> Offsets, IReadOnlyList<double> Rhythm)
{
    // Rhythm shapes (seconds): 4-note = quarter, eighth, half+eighth, whole; 3-note = dotted-quarter, half+eighth, whole.
    private static readonly IReadOnlyList<double> Four = [1, 0.5, 2.5, 4];
    private static readonly IReadOnlyList<double> Three = [1.5, 2.5, 4];

    /// <summary>8 drills — 4 Minor 2nd (types 0-3) + 4 Major 7th (types 4-7), order matches the controller.</summary>
    public static readonly IReadOnlyList<L1C7VocalDrill> All =
    [
        // Minor 2nds
        new("MI MI FA MI", "Min 2nd", [4, 4, 5, 4], Four),
        new("TI DO DO", "Min 2nd", [-1, 0, 0], Three),
        new("FA FA MI MI", "Min 2nd", [5, 5, 4, 4], Four),
        new("DO TI DO", "Min 2nd", [0, -1, 0], Three),
        // Major 7ths
        new("DO DO TI DO", "Maj 7th", [0, 0, 11, 12], Four),
        new("FA MI MI", "Maj 7th", [5, 16, 16], Three),
        new("TI TI DO DO", "Maj 7th", [11, 11, 0, 0], Four),
        new("MI FA MI", "Maj 7th", [16, 5, 4], Three),
    ];
}
