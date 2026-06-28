namespace EarTraining.Core.Drills;

/// <summary>
/// One Level 1 Chapter 2 drill prompt (Major 3rd / Minor 6th), built relative to DO.
/// Ported note-for-note from <c>L1C2Controller</c>: <see cref="Vocal"/> = the 5-note vocal
/// patterns (GetMelodicDrillDO), <see cref="Melodic"/> = the 2-note melodic intervals
/// (GetMelodicDrillNoDO), <see cref="Harmonic"/> = the 2-note harmonic intervals played
/// together (GetHarmonicDrillEx). <see cref="Offsets"/> are semitones from DO.
/// </summary>
public sealed record L1C2Drill(string Label, IntervalQuality Quality, IReadOnlyList<int> Offsets)
{
    /// <summary>Answer label that disambiguates same-named patterns by quality, e.g. "DO MI (Maj 3rd)".</summary>
    public string QuizLabel => $"{Label} ({Quality.Display()})";

    /// <summary>5-note vocal drills (quarter, quarter, quarter, quarter, whole). Maj 3rds (0-5) then Min 6ths (6-11).</summary>
    public static readonly IReadOnlyList<L1C2Drill> Vocal =
    [
        new("DO DO MI MI MI", IntervalQuality.Major3rd, [0, 0, 4, 4, 4]),
        new("FA FA LA LA SO", IntervalQuality.Major3rd, [5, 5, 9, 9, 7]),
        new("SO SO TI TI DO", IntervalQuality.Major3rd, [7, 7, 11, 11, 12]),
        new("TI TI SO SO SO", IntervalQuality.Major3rd, [11, 11, 7, 7, 7]),
        new("LA LA FA FA MI", IntervalQuality.Major3rd, [9, 9, 5, 5, 4]),
        new("MI MI DO DO DO", IntervalQuality.Major3rd, [4, 4, 0, 0, 0]),
        new("MI MI DO DO DO", IntervalQuality.Minor6th, [4, 4, 12, 12, 12]),
        new("LA LA FA FA MI", IntervalQuality.Minor6th, [-3, -3, 5, 5, 4]),
        new("TI TI SO SO SO", IntervalQuality.Minor6th, [-1, -1, 7, 7, 7]),
        new("SO SO TI TI DO", IntervalQuality.Minor6th, [7, 7, -1, -1, 0]),
        new("FA FA LA LA SO", IntervalQuality.Minor6th, [5, 5, -3, -3, -5]),
        new("DO DO MI MI MI", IntervalQuality.Minor6th, [12, 12, 4, 4, 4]),
    ];

    /// <summary>2-note melodic intervals (no DO lead-in), each a half note. Maj 3rds (0-5) then Min 6ths (6-11).</summary>
    public static readonly IReadOnlyList<L1C2Drill> Melodic =
    [
        new("DO MI", IntervalQuality.Major3rd, [0, 4]),
        new("FA LA", IntervalQuality.Major3rd, [5, 9]),
        new("SO TI", IntervalQuality.Major3rd, [7, 11]),
        new("TI SO", IntervalQuality.Major3rd, [11, 7]),
        new("LA FA", IntervalQuality.Major3rd, [9, 5]),
        new("MI DO", IntervalQuality.Major3rd, [4, 0]),
        new("MI DO", IntervalQuality.Minor6th, [4, 12]),
        new("LA FA", IntervalQuality.Minor6th, [-3, 5]),
        new("TI SO", IntervalQuality.Minor6th, [-1, 7]),
        new("SO TI", IntervalQuality.Minor6th, [7, -1]),
        new("FA LA", IntervalQuality.Minor6th, [5, -3]),
        new("DO MI", IntervalQuality.Minor6th, [12, 4]),
    ];

    /// <summary>2-note harmonic intervals (both notes together), {low, high} offsets. Maj 3rds (0-2) then Min 6ths (3-5).</summary>
    public static readonly IReadOnlyList<L1C2Drill> Harmonic =
    [
        new("DO MI", IntervalQuality.Major3rd, [0, 4]),
        new("FA LA", IntervalQuality.Major3rd, [5, 9]),
        new("SO TI", IntervalQuality.Major3rd, [7, 11]),
        new("MI DO", IntervalQuality.Minor6th, [4, 12]),
        new("LA FA", IntervalQuality.Minor6th, [9, 17]),
        new("TI SO", IntervalQuality.Minor6th, [11, 19]),
    ];

    /// <summary>Drills of the given quality, or all when <paramref name="quality"/> is null ("Both").</summary>
    public static IReadOnlyList<L1C2Drill> Filter(IReadOnlyList<L1C2Drill> all, IntervalQuality? quality) =>
        quality is null ? all : all.Where(d => d.Quality == quality).ToList();
}
