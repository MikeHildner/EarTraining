namespace EarTraining.Core.Drills;

/// <summary>
/// One Level 1 Chapter 3 drill prompt (Minor 3rd / Major 6th), built relative to DO.
/// Ported from the web L1C3 views, which reuse <c>L1C2Controller</c>'s GetMelodicDrillDO /
/// GetMelodicDrillNoDO (types 12–24) and GetHarmonicDrillEx (types 6–13). <see cref="Offsets"/>
/// are semitones from DO; shares <see cref="IntervalQuality"/> with L1C2.
/// </summary>
public sealed record L1C3Drill(string Label, IntervalQuality Quality, IReadOnlyList<int> Offsets)
{
    /// <summary>Answer label that disambiguates same-named patterns by quality, e.g. "RE FA (Min 3rd)".</summary>
    public string QuizLabel => $"{Label} ({Quality.Display()})";

    /// <summary>5-note vocal drills (quarter, quarter, quarter, quarter, whole). Min 3rds (0-4) then Maj 6ths (5-12).</summary>
    public static readonly IReadOnlyList<L1C3Drill> Vocal =
    [
        new("RE RE FA FA MI", IntervalQuality.Minor3rd, [2, 2, 5, 5, 4]),
        new("MI MI SO SO SO", IntervalQuality.Minor3rd, [4, 4, 7, 7, 7]),
        new("LA LA DO DO DO", IntervalQuality.Minor3rd, [9, 9, 12, 12, 12]),
        new("TI TI RE RE DO", IntervalQuality.Minor3rd, [11, 11, 14, 14, 12]),   // high
        new("TI TI RE RE DO", IntervalQuality.Minor3rd, [-1, -1, 2, 2, 0]),      // low (same label → dedups in quiz)
        new("DO DO LA LA SO", IntervalQuality.Major6th, [0, 0, 9, 9, 7]),
        new("RE RE TI TI DO", IntervalQuality.Major6th, [2, 2, 11, 11, 12]),
        new("FA FA RE RE DO", IntervalQuality.Major6th, [5, 5, 14, 14, 12]),
        new("SO SO MI MI MI", IntervalQuality.Major6th, [7, 7, 16, 16, 16]),
        new("LA LA DO DO DO", IntervalQuality.Major6th, [9, 9, 0, 0, 0]),
        new("TI TI RE RE DO", IntervalQuality.Major6th, [11, 11, 2, 2, 0]),
        new("RE RE FA FA MI", IntervalQuality.Major6th, [14, 14, 5, 5, 4]),
        new("MI MI SO SO SO", IntervalQuality.Major6th, [4, 4, -5, -5, -5]),
    ];

    /// <summary>2-note melodic intervals (no DO lead-in), each a half note. Min 3rds (0-4) then Maj 6ths (5-12).</summary>
    public static readonly IReadOnlyList<L1C3Drill> Melodic =
    [
        new("RE FA", IntervalQuality.Minor3rd, [2, 5]),
        new("MI SO", IntervalQuality.Minor3rd, [4, 7]),
        new("LA DO", IntervalQuality.Minor3rd, [9, 12]),
        new("TI RE (high)", IntervalQuality.Minor3rd, [11, 14]),
        new("TI RE (low)", IntervalQuality.Minor3rd, [-1, 2]),
        new("DO LA", IntervalQuality.Major6th, [0, 9]),
        new("RE TI", IntervalQuality.Major6th, [2, 11]),
        new("FA RE", IntervalQuality.Major6th, [5, 14]),
        new("SO MI", IntervalQuality.Major6th, [7, 16]),
        new("LA DO", IntervalQuality.Major6th, [9, 0]),
        new("TI RE", IntervalQuality.Major6th, [11, 2]),
        new("RE FA", IntervalQuality.Major6th, [14, 5]),
        new("MI SO", IntervalQuality.Major6th, [4, -5]),
    ];

    /// <summary>2-note harmonic intervals (both notes together), {low, high} offsets. Min 3rds (0-3) then Maj 6ths (4-7).</summary>
    public static readonly IReadOnlyList<L1C3Drill> Harmonic =
    [
        new("RE FA", IntervalQuality.Minor3rd, [2, 5]),
        new("MI SO", IntervalQuality.Minor3rd, [4, 7]),
        new("LA DO", IntervalQuality.Minor3rd, [9, 12]),
        new("TI RE", IntervalQuality.Minor3rd, [11, 14]),
        new("FA RE", IntervalQuality.Major6th, [5, 14]),
        new("SO MI", IntervalQuality.Major6th, [7, 16]),
        new("DO LA", IntervalQuality.Major6th, [0, 9]),
        new("RE TI", IntervalQuality.Major6th, [2, 11]),
    ];
}
