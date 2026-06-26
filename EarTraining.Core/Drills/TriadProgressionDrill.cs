namespace EarTraining.Core.Drills;

/// <summary>One chord in a progression: the 3 triad tones (semitones from DO) plus the root's
/// offset from DO, folded into the bass register at play time (<see cref="Theory.Voicing.BassNoteNumber"/>).</summary>
public sealed record ProgressionChord(IReadOnlyList<int> ToneOffsets, int BassRootOffset);

/// <summary>
/// L1C4 Diatonic Triad Progressions (p.66): I / IV / V progressions of 2 or 3 chords, each chord
/// a triad inversion over a bass root, played in sequence — identify the progression. Ported from
/// L1C4Controller.Get2ChordProgressionEx / Get3ChordProgressionEx.
/// </summary>
public sealed record TriadProgressionDrill(string Label, IReadOnlyList<ProgressionChord> Chords)
{
    // The chord voicings used by these progressions (offsets from DO):
    private static readonly ProgressionChord IRoot  = new([0, 4, 7], 0);    // I  root
    private static readonly ProgressionChord IVRoot = new([5, 9, 12], 5);   // IV root
    private static readonly ProgressionChord IV2nd  = new([0, 5, 9], 5);    // IV 2nd inversion
    private static readonly ProgressionChord VRoot  = new([7, 11, 14], 7);  // V  root
    private static readonly ProgressionChord V1st   = new([-1, 2, 7], 7);   // V  1st inversion

    public static readonly IReadOnlyList<TriadProgressionDrill> TwoChord =
    [
        new("IV (2nd) - I (root)", [IV2nd, IRoot]),
        new("V (1st) - I (root)", [V1st, IRoot]),
        new("V (root) - IV (root)", [VRoot, IVRoot]),
        new("I (root) - IV (2nd)", [IRoot, IV2nd]),
        new("I (root) - V (1st)", [IRoot, V1st]),
    ];

    public static readonly IReadOnlyList<TriadProgressionDrill> ThreeChord =
    [
        new("IV (2nd) - V (1st) - I (root)", [IV2nd, V1st, IRoot]),
        new("V (1st) - IV (2nd) - I (root)", [V1st, IV2nd, IRoot]),
        new("I (root) - IV (2nd) - V (1st)", [IRoot, IV2nd, V1st]),
        new("V (1st) - I (root) - IV (2nd)", [V1st, IRoot, IV2nd]),
        new("I (root) - V (1st) - IV (2nd)", [IRoot, V1st, IV2nd]),
        new("IV (2nd) - I (root) - V (1st)", [IV2nd, IRoot, V1st]),
    ];
}
