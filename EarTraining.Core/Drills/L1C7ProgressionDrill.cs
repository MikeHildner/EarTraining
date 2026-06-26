namespace EarTraining.Core.Drills;

/// <summary>
/// L1C7 Diatonic Triad Progressions (pp. 164-165): progressions of 2 or 3 voice-led chords drawn
/// from all six diatonic triads (I / IV / V / vi / iii / ii), played in sequence — identify the
/// progression. Ported from L1C7Controller.Get2/3ChordProgressionEx (the TriadType+InversionType
/// overload of Inversion.CreateTriadInversionEx). Reuses <see cref="ProgressionChord"/>; adds the
/// supertonic (ii) over the L1C6 set. The same label may be voiced high or low per progression.
/// </summary>
public sealed record L1C7ProgressionDrill(string Label, IReadOnlyList<ProgressionChord> Chords)
{
    // Inversion voicing, mirroring Inversion.CreateTriadInversionEx: High/Low × First/Second
    // shift chord tones by an octave; bass = the chord root (folded into the bass octave by the page).
    private enum Inv { Root, HighFirst, HighSecond, LowFirst, LowSecond }

    private static ProgressionChord Chord(int n1, int n2, int n3, int bass, Inv inv)
    {
        switch (inv)
        {
            case Inv.HighFirst: n1 += 12; break;
            case Inv.HighSecond: n1 += 12; n2 += 12; break;
            case Inv.LowSecond: n3 -= 12; break;
            case Inv.LowFirst: n2 -= 12; n3 -= 12; break;
        }
        var tones = new[] { n1, n2, n3 };
        Array.Sort(tones);
        return new ProgressionChord(tones, bass);
    }

    // Root-position chord tones (offsets from DO); bass = the chord root. Match Inversion.TriadType.
    private static ProgressionChord I(Inv inv) => Chord(0, 4, 7, 0, inv);
    private static ProgressionChord IV(Inv inv) => Chord(5, 9, 12, 5, inv);
    private static ProgressionChord V(Inv inv) => Chord(7, 11, 14, 7, inv);
    private static ProgressionChord vi(Inv inv) => Chord(9, 12, 16, 9, inv);
    private static ProgressionChord iii(Inv inv) => Chord(4, 7, 11, 4, inv);
    private static ProgressionChord ii(Inv inv) => Chord(2, 5, 9, 2, inv);

    /// <summary>16 two-chord progressions (controller order = quiz2 types 0-5, 17-26).</summary>
    public static readonly IReadOnlyList<L1C7ProgressionDrill> TwoChord =
    [
        new("IV (2nd) - I (root)", [IV(Inv.LowSecond), I(Inv.Root)]),
        new("ii (root) - V (1st)", [ii(Inv.Root), V(Inv.LowFirst)]),
        new("I (2nd) - ii (1st)", [I(Inv.HighSecond), ii(Inv.HighFirst)]),
        new("vi (root) - IV (1st)", [vi(Inv.Root), IV(Inv.HighFirst)]),
        new("I (1st) - V (2nd)", [I(Inv.HighFirst), V(Inv.LowSecond)]),
        new("I (root) - vi (1st)", [I(Inv.Root), vi(Inv.LowFirst)]),
        new("ii (1st) - iii (1st)", [ii(Inv.HighFirst), iii(Inv.HighFirst)]),
        new("V (2nd) - vi (2nd)", [V(Inv.LowSecond), vi(Inv.LowSecond)]),
        new("IV (2nd) - V (1st)", [IV(Inv.LowSecond), V(Inv.LowFirst)]),
        new("iii (1st) - I (2nd)", [iii(Inv.HighFirst), I(Inv.HighSecond)]),
        new("V (2nd) - iii (root)", [V(Inv.LowSecond), iii(Inv.Root)]),
        new("ii (2nd) - IV (1st)", [ii(Inv.HighSecond), IV(Inv.HighFirst)]),
        new("I (1st) - iii (root)", [I(Inv.HighFirst), iii(Inv.Root)]),
        new("iii (root) - ii (1st)", [iii(Inv.Root), ii(Inv.HighFirst)]),
        new("V (2nd) - I (root)", [V(Inv.LowSecond), I(Inv.Root)]),
        new("V (2nd) - IV (root)", [V(Inv.LowSecond), IV(Inv.Root)]),
    ];

    /// <summary>11 three-chord progressions (controller order = quiz3 types 6-16).</summary>
    public static readonly IReadOnlyList<L1C7ProgressionDrill> ThreeChord =
    [
        new("I (2nd) - ii (1st) - iii (1st)", [I(Inv.HighSecond), ii(Inv.HighFirst), iii(Inv.HighFirst)]),
        new("I (1st) - V (2nd) - vi (2nd)", [I(Inv.HighFirst), V(Inv.LowSecond), vi(Inv.LowSecond)]),
        new("IV (2nd) - V (1st) - vi (1st)", [IV(Inv.LowSecond), V(Inv.LowFirst), vi(Inv.LowFirst)]),
        new("iii (1st) - I (2nd) - ii (1st)", [iii(Inv.HighFirst), I(Inv.HighSecond), ii(Inv.HighFirst)]),
        new("IV (root) - V (2nd) - iii (root)", [IV(Inv.Root), V(Inv.LowSecond), iii(Inv.Root)]),
        new("I (root) - ii (2nd) - IV (1st)", [I(Inv.Root), ii(Inv.LowSecond), IV(Inv.LowFirst)]),
        new("I (1st) - iii (root) - ii (1st)", [I(Inv.HighFirst), iii(Inv.Root), ii(Inv.HighFirst)]),
        new("IV (2nd) - I (root) - V (1st)", [IV(Inv.LowSecond), I(Inv.Root), V(Inv.LowFirst)]),
        new("vi (root) - IV (1st) - I (2nd)", [vi(Inv.Root), IV(Inv.HighFirst), I(Inv.HighSecond)]),
        new("V (2nd) - I (root) - ii (root)", [V(Inv.LowSecond), I(Inv.Root), ii(Inv.Root)]),
        new("I (1st) - V (2nd) - IV (root)", [I(Inv.HighFirst), V(Inv.LowSecond), IV(Inv.Root)]),
    ];
}
