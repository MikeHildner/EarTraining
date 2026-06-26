namespace EarTraining.Core.Drills;

/// <summary>
/// L1C4 Melody Harmonization (p.59): a melody note harmonized by its I, IV, or V triad, voiced
/// with the melody note on top and the root in the bass — identify the harmonizing triad. Ported
/// from L1C4Controller.GetMelodyHarmonizationEx (the 9 note+triad reference prompts).
/// <see cref="ToneOffsets"/> = the 3 triad tones as semitones from DO (ascending; melody note is
/// the top one); <see cref="BassRootOffset"/> = the chord root's offset from DO, folded into the
/// bass register (<see cref="Theory.Voicing.BassNoteNumber"/>) at play time.
/// </summary>
public sealed record MelodyHarmonizationDrill(
    string NoteName, int TriadType, IReadOnlyList<int> ToneOffsets, int BassRootOffset)
{
    public static readonly string[] TriadNames = ["I", "IV", "V"];

    public string TriadName => TriadNames[TriadType];
    public string Label => $"{NoteName} — {TriadName}";
    public string Key => $"{NoteName}-{TriadType}";   // composite include key (note + triad)

    /// <summary>The 9 reference prompts (melody note, harmonizing triad), in the web's order.</summary>
    public static readonly IReadOnlyList<MelodyHarmonizationDrill> All =
    [
        new("DO", 0, [4, 7, 12], 0),    // DO — I   (I, melody DO on top)
        new("DO", 1, [5, 9, 12], 5),    // DO — IV
        new("RE", 2, [7, 11, 14], 7),   // RE — V
        new("MI", 0, [-5, 0, 4], 0),    // MI — I
        new("FA", 1, [9, 12, 17], 5),   // FA — IV
        new("SO", 0, [0, 4, 7], 0),     // SO — I
        new("SO", 2, [11, 14, 19], 7),  // SO — V
        new("LA", 1, [0, 5, 9], 5),     // LA — IV
        new("TI", 2, [2, 7, 11], 7),    // TI — V
    ];
}
