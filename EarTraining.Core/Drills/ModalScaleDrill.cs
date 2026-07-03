namespace EarTraining.Core.Drills;

/// <summary>
/// A modal scale recognition prompt (book Ch. 6 + workbook pp. 72-73): an eight-note
/// ascending modal scale starting on C or C#, identified by mode name; the book also asks
/// for the relative (implied) major, which the reveal shows. Middle C is always played as
/// a DO reference beforehand. The C-start pool covers all seven modes; the C#-start pool
/// matches the book's five (no Ionian/Lydian — their implied majors are impractical keys).
/// </summary>
public sealed record ModalScaleDrill(string Mode, string StartName, int StartNote, string RelativeMajor, IReadOnlyList<int> Offsets)
{
    /// <summary>Middle C — the reference tone the book plays before every question.</summary>
    public const int ReferenceNote = 39;

    /// <summary>Answer/reveal text, e.g. "C# Dorian — B major scale".</summary>
    public string RevealText => $"{StartName} {Mode} — {RelativeMajor} major scale";

    private static ModalScaleDrill C(string mode, string relative, params int[] offsets) =>
        new(mode, "C", 39, relative, offsets);
    private static ModalScaleDrill Cs(string mode, string relative, params int[] offsets) =>
        new(mode, "C#", 40, relative, offsets);

    /// <summary>Modes starting on C (workbook Q1-25).</summary>
    public static readonly IReadOnlyList<ModalScaleDrill> StartOnC =
    [
        C("Ionian",     "C",  0, 2, 4, 5, 7, 9, 11, 12),
        C("Dorian",     "Bb", 0, 2, 3, 5, 7, 9, 10, 12),
        C("Phrygian",   "Ab", 0, 1, 3, 5, 7, 8, 10, 12),
        C("Lydian",     "G",  0, 2, 4, 6, 7, 9, 11, 12),
        C("Mixolydian", "F",  0, 2, 4, 5, 7, 9, 10, 12),
        C("Aeolian",    "Eb", 0, 2, 3, 5, 7, 8, 10, 12),
        C("Locrian",    "Db", 0, 1, 3, 5, 6, 8, 10, 12),
    ];

    /// <summary>Modes starting on C# (workbook Q26-50 — the book's five-mode pool).</summary>
    public static readonly IReadOnlyList<ModalScaleDrill> StartOnCs =
    [
        Cs("Dorian",     "B",  0, 2, 3, 5, 7, 9, 10, 12),
        Cs("Phrygian",   "A",  0, 1, 3, 5, 7, 8, 10, 12),
        Cs("Mixolydian", "F#", 0, 2, 4, 5, 7, 9, 10, 12),
        Cs("Aeolian",    "E",  0, 2, 3, 5, 7, 8, 10, 12),
        Cs("Locrian",    "D",  0, 1, 3, 5, 6, 8, 10, 12),
    ];

    /// <summary>The seven mode names in scale-degree order (for the answer buttons).</summary>
    public static readonly IReadOnlyList<string> ModeNames =
        ["Ionian", "Dorian", "Phrygian", "Lydian", "Mixolydian", "Aeolian", "Locrian"];
}
