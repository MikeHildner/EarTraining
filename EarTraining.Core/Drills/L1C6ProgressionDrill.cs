namespace EarTraining.Core.Drills;

/// <summary>
/// L1C6 Diatonic Triad Progressions (p. 134): I / IV / V / vi / iii progressions of 2 or 3
/// voice-led chords (each a triad inversion over a bass root), played in sequence — identify the
/// progression. Ported from L1C6Controller.Get2/3ChordProgressionEx + Inversion.CreateTriadInversionEx.
/// Reuses <see cref="ProgressionChord"/>; adds the mediant (iii) over the L1C5 set. The same label
/// (e.g. "I (2nd)") may be voiced high or low depending on the progression's voice-leading, so the
/// voicings are computed per chord rather than shared.
/// </summary>
public sealed record L1C6ProgressionDrill(string Label, IReadOnlyList<ProgressionChord> Chords)
{
    // Inversion voicing, mirroring Inversion.CreateTriadInversionEx: High/Low × First/Second
    // shift chord tones by an octave to keep the voice-leading smooth.
    private enum Inv { Root, HighFirst, HighSecond, LowFirst, LowSecond }

    private static ProgressionChord Chord(int n1, int n2, int n3, int bass, Inv inv)
    {
        switch (inv)
        {
            case Inv.HighFirst: n1 += 12; break;                  // bottom note up an octave
            case Inv.HighSecond: n1 += 12; n2 += 12; break;       // bottom two up an octave
            case Inv.LowSecond: n3 -= 12; break;                  // top note down an octave
            case Inv.LowFirst: n2 -= 12; n3 -= 12; break;         // top two down an octave
        }
        var tones = new[] { n1, n2, n3 };
        Array.Sort(tones);
        return new ProgressionChord(tones, bass);
    }

    // Root-position chord tones (offsets from DO); bass = the chord root, folded into the bass octave by the page.
    private static ProgressionChord I(Inv inv) => Chord(0, 4, 7, 0, inv);
    private static ProgressionChord IV(Inv inv) => Chord(5, 9, 12, 5, inv);
    private static ProgressionChord V(Inv inv) => Chord(7, 11, 14, 7, inv);
    private static ProgressionChord vi(Inv inv) => Chord(9, 12, 16, 9, inv);
    private static ProgressionChord iii(Inv inv) => Chord(4, 7, 11, 4, inv);

    /// <summary>11 two-chord progressions (controller types 0-10).</summary>
    public static readonly IReadOnlyList<L1C6ProgressionDrill> TwoChord =
    [
        new("I (root) - vi (1st)", [I(Inv.Root), vi(Inv.LowFirst)]),
        new("I (root) - IV (2nd)", [I(Inv.Root), IV(Inv.LowSecond)]),
        new("IV (root) - V (root)", [IV(Inv.Root), V(Inv.Root)]),
        new("I (2nd) - iii (1st)", [I(Inv.LowSecond), iii(Inv.LowFirst)]),
        new("iii (1st) - vi (root)", [iii(Inv.HighFirst), vi(Inv.Root)]),
        new("V (2nd) - I (1st)", [V(Inv.HighSecond), I(Inv.HighFirst)]),
        new("I (1st) - IV (1st)", [I(Inv.LowFirst), IV(Inv.LowFirst)]),
        new("I (2nd) - vi (root)", [I(Inv.HighSecond), vi(Inv.Root)]),
        new("iii (root) - V (2nd)", [iii(Inv.Root), V(Inv.LowSecond)]),
        new("V (2nd) - vi (2nd)", [V(Inv.LowSecond), vi(Inv.LowSecond)]),
        new("IV (2nd) - iii (2nd)", [IV(Inv.LowSecond), iii(Inv.LowSecond)]),
    ];

    /// <summary>12 three-chord progressions (controller types 11-22).</summary>
    public static readonly IReadOnlyList<L1C6ProgressionDrill> ThreeChord =
    [
        new("I (root) - V (1st) - vi (1st)", [I(Inv.Root), V(Inv.LowFirst), vi(Inv.LowFirst)]),
        new("I (1st) - iii (root) - IV (root)", [I(Inv.HighFirst), iii(Inv.Root), IV(Inv.Root)]),
        new("vi (2nd) - V (2nd) - IV (2nd)", [vi(Inv.HighSecond), V(Inv.HighSecond), IV(Inv.HighSecond)]),
        new("iii (2nd) - I (root) - vi (1st)", [iii(Inv.LowSecond), I(Inv.Root), vi(Inv.LowFirst)]),
        new("IV (root) - I (1st) - iii (root)", [IV(Inv.Root), I(Inv.HighFirst), iii(Inv.Root)]),
        new("vi (2nd) - iii (root) - I (1st)", [vi(Inv.LowSecond), iii(Inv.Root), I(Inv.HighFirst)]),
        new("I (2nd) - IV (1st) - V (1st)", [I(Inv.LowSecond), IV(Inv.LowFirst), V(Inv.LowFirst)]),
        new("vi (root) - I (2nd) - IV (1st)", [vi(Inv.Root), I(Inv.HighSecond), IV(Inv.HighFirst)]),
        new("iii (1st) - vi (root) - IV (1st)", [iii(Inv.HighFirst), vi(Inv.Root), IV(Inv.HighFirst)]),
        new("I (root) - vi (1st) - V (1st)", [I(Inv.Root), vi(Inv.LowFirst), V(Inv.LowFirst)]),
        new("V (2nd) - I (1st) - vi (2nd)", [V(Inv.LowSecond), I(Inv.HighFirst), vi(Inv.LowSecond)]),
        new("IV (root) - V (2nd) - vi (2nd)", [IV(Inv.Root), V(Inv.LowSecond), vi(Inv.LowSecond)]),
    ];
}
