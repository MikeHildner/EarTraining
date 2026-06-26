namespace EarTraining.Core.Drills;

/// <summary>
/// A diatonic triad recognition drill (L1C3 "Index", p.54): the I, IV and V major triads of
/// the key, each at root / 1st / 2nd inversion, played as a chord. <see cref="Offsets"/> are
/// semitones from DO (sorted ascending for a clean low→high roll). Ported from
/// L1C3Controller.GetTriadEx + Inversion.CreateTriadInversionEx (no bass note).
/// </summary>
public sealed record DiatonicTriadDrill(int TriadIndex, int InversionIndex, IReadOnlyList<int> Offsets)
{
    public static readonly string[] TriadNames = ["I", "IV", "V", "vi", "iii"];
    public static readonly string[] InversionNames = ["root", "1st", "2nd"];

    public string TriadName => TriadNames[TriadIndex];
    public string InversionName => InversionNames[InversionIndex];

    /// <summary>Answer label in "Triad + inversion" mode, e.g. "IV (1st)".</summary>
    public string FullLabel => $"{TriadName} ({InversionName})";

    /// <summary>Composite include key, e.g. "1-0" = IV root.</summary>
    public string Key => $"{TriadIndex}-{InversionIndex}";

    // Root-position chord tones (offsets from DO) for I, IV, V — all major triads.
    private static readonly int[][] Roots =
    [
        [0, 4, 7],    // I  : DO MI SO
        [5, 9, 12],   // IV : FA LA (high DO)
        [7, 11, 14],  // V  : SO TI RE
        [9, 12, 16],  // vi : LA DO MI (minor) — used by L1C5
        [4, 7, 11],   // iii: MI SO TI (minor) — used by L1C6
    ];

    /// <summary>
    /// Build one (triad, inversion) drill. Inversions mirror Inversion.CreateTriadInversionEx:
    /// 1st takes the bottom note up an octave, 2nd takes the bottom two up an octave.
    /// </summary>
    public static DiatonicTriadDrill Build(int triad, int inversion)
    {
        int[] o = (int[])Roots[triad].Clone();
        if (inversion >= 1) o[0] += 12;
        if (inversion >= 2) o[1] += 12;
        Array.Sort(o);
        return new DiatonicTriadDrill(triad, inversion, o);
    }

    /// <summary>
    /// All (triad × inversion) drills in I/IV/V[/vi/iii] × root/1st/2nd order. <paramref name="triadCount"/>
    /// = 3 (L1C3: I/IV/V), 4 (L1C5: adds vi), or 5 (L1C6: adds iii). Triad order matches the
    /// controllers' triadtype numbering, so a longer list never disturbs the smaller chapters.
    /// </summary>
    public static IReadOnlyList<DiatonicTriadDrill> All(int triadCount = 3)
    {
        var list = new List<DiatonicTriadDrill>(triadCount * InversionNames.Length);
        for (int t = 0; t < triadCount; t++)
            for (int inv = 0; inv < InversionNames.Length; inv++)
                list.Add(Build(t, inv));
        return list;
    }
}
