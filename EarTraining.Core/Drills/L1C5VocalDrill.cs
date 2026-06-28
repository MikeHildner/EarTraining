namespace EarTraining.Core.Drills;

/// <summary>
/// An L1C5 vocal drill (pp. 83-90): a 6-note sung pattern that drills a 4th or 5th — identify the
/// sung pattern. Ported from L1C5Controller.GetMelodicDrillEx (types 0-13). <see cref="Offsets"/>
/// are semitones from DO; <see cref="Rhythm"/> is each note's duration in seconds (at 60 bpm).
/// </summary>
public sealed record L1C5VocalDrill(string Label, string Group, IReadOnlyList<int> Offsets, IReadOnlyList<double> Rhythm)
{
    // Two rhythm shapes: an eighth-note pair at notes 2-3 (A) or notes 4-5 (B), then a whole note.
    private static readonly double[] A = [1, 0.5, 0.5, 1, 1, 4];
    private static readonly double[] B = [1, 1, 1, 0.5, 0.5, 4];

    public static readonly IReadOnlyList<L1C5VocalDrill> All =
    [
        new("DO DO DO FA FA MI", "4th", [0, 0, 0, 5, 5, 4], A),
        new("RE RE SO SO SO SO", "4th", [2, 2, 7, 7, 7, 7], B),
        new("MI MI MI LA LA SO", "4th", [4, 4, 4, 9, 9, 7], A),
        new("FA FA TI TI TI DO", "4th", [5, 5, 11, 11, 11, 12], B),
        new("SO SO SO DO DO DO", "4th", [7, 7, 7, 12, 12, 12], A),
        new("LA LA RE RE RE DO", "4th", [9, 9, 14, 14, 14, 12], B),
        new("TI TI TI MI MI MI", "4th", [11, 11, 11, 16, 16, 16], A),
        new("DO DO DO SO SO SO", "5th", [0, 0, 0, 7, 7, 7], A),
        new("RE RE LA LA LA SO", "5th", [2, 2, 9, 9, 9, 7], B),
        new("MI MI MI TI TI DO", "5th", [4, 4, 4, 11, 11, 12], A),
        new("FA FA DO DO DO DO", "5th", [5, 5, 12, 12, 12, 12], B),
        new("SO SO SO RE RE DO", "5th", [7, 7, 7, 14, 14, 12], A),
        new("LA LA MI MI MI MI", "5th", [9, 9, 16, 16, 16, 16], B),
        new("FA FA FA TI TI DO", "5th", [5, 5, 5, -1, -1, 0], A),
    ];
}
