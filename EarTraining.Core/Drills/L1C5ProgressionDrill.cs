namespace EarTraining.Core.Drills;

/// <summary>
/// L1C5 Diatonic Triad Progressions (pp. 101-102): I / IV / V / vi progressions of 2 or 3
/// voice-led chords (each a triad inversion over a bass root), played in sequence — identify the
/// progression. Ported from L1C5Controller.Get2/3ChordProgressionEx. Reuses <see cref="ProgressionChord"/>.
/// </summary>
public sealed record L1C5ProgressionDrill(string Label, IReadOnlyList<ProgressionChord> Chords)
{
    // Voice-led chord voicings used by these progressions (tone offsets from DO + bass root offset).
    private static readonly ProgressionChord IRoot  = new([0, 4, 7], 0);
    private static readonly ProgressionChord I1st   = new([4, 7, 12], 0);
    private static readonly ProgressionChord I2nd   = new([-5, 0, 4], 0);
    private static readonly ProgressionChord IVRoot = new([5, 9, 12], 5);
    private static readonly ProgressionChord IV1st  = new([-3, 0, 5], 5);
    private static readonly ProgressionChord IV2nd  = new([0, 5, 9], 5);
    private static readonly ProgressionChord VRoot  = new([7, 11, 14], 7);
    private static readonly ProgressionChord V1st   = new([-1, 2, 7], 7);
    private static readonly ProgressionChord V2nd   = new([2, 7, 11], 7);
    private static readonly ProgressionChord viRoot = new([9, 12, 16], 9);
    private static readonly ProgressionChord vi1st  = new([0, 4, 9], 9);
    private static readonly ProgressionChord vi2nd  = new([4, 9, 12], 9);

    public static readonly IReadOnlyList<L1C5ProgressionDrill> TwoChord =
    [
        new("IV (2nd) - I (root)", [IV2nd, IRoot]),
        new("V (1st) - I (root)", [V1st, IRoot]),
        new("I (root) - IV (2nd)", [IRoot, IV2nd]),
        new("vi (2nd) - IV (root)", [vi2nd, IVRoot]),
        new("V (root) - vi (root)", [VRoot, viRoot]),
        new("vi (1st) - I (root)", [vi1st, IRoot]),
        new("IV (root) - V (root)", [IVRoot, VRoot]),
        new("vi (root) - V (root)", [viRoot, VRoot]),
        new("IV (root) - vi (2nd)", [IVRoot, vi2nd]),
    ];

    public static readonly IReadOnlyList<L1C5ProgressionDrill> ThreeChord =
    [
        new("IV (2nd) - V (1st) - I (root)", [IV2nd, V1st, IRoot]),
        new("I (root) - IV (2nd) - V (1st)", [IRoot, IV2nd, V1st]),
        new("IV (2nd) - I (root) - V (1st)", [IV2nd, IRoot, V1st]),
        new("I (root) - V (1st) - vi (1st)", [IRoot, V1st, vi1st]),
        new("vi (2nd) - IV (root) - I (1st)", [vi2nd, IVRoot, I1st]),
        new("I (root) - vi (1st) - V (1st)", [IRoot, vi1st, V1st]),
        new("V (root) - IV (root) - vi (2nd)", [VRoot, IVRoot, vi2nd]),
        new("IV (root) - V (root) - vi (root)", [IVRoot, VRoot, viRoot]),
        new("vi (1st) - I (2nd) - IV (1st)", [vi1st, I2nd, IV1st]),
        new("V (2nd) - vi (2nd) - I (root)", [V2nd, vi2nd, IRoot]),
    ];
}
