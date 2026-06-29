namespace EarTraining.Core.Theory;

/// <summary>
/// Key-signature support for dictation: transpose the C-major scale note numbers
/// into a key, and spell each sounding pitch with the letter the key signature
/// expects (so VexFlow's key signature renders the right accidentals).
/// Ported verbatim from the web app (NoteHelper.TransposeScaleNoteNumbers /
/// AdjustNoteNamesForKey + the ExtensionMethods flat/sharp helpers).
/// </summary>
public static class Keys
{
    /// <summary>The 12 keys offered, in circle-of-fifths-ish order for the picker.</summary>
    public static readonly IReadOnlyList<string> All =
        ["C", "G", "D", "A", "E", "B", "F#", "Db", "Ab", "Eb", "Bb", "F"];

    /// <summary>Semitone offset from C for each supported key.</summary>
    public static int Offset(string key) => key switch
    {
        "F#" => 6,
        "F" => 5,
        "E" => 4,
        "Eb" => 3,
        "D" => 2,
        "Db" => 1,
        "C" => 0,
        "B" => -1,
        "Bb" => -2,
        "A" => -3,
        "Ab" => -4,
        "G" => -5,
        _ => throw new NotSupportedException($"Key signature '{key}' is not supported."),
    };

    /// <summary>
    /// The DO (tonic) note number for a key — the single note of that pitch class within the Tonic
    /// singing range (34=G3 … 45=Gb4 spans all 12 pitch classes). Used by the "fixed practice key"
    /// setting so every DO-relative drill builds on that key. e.g. "C" → 39 (C4), "G" → 34 (G3).
    /// </summary>
    public static int DoNote(string key)
    {
        int pc = ((Offset(key) % 12) + 12) % 12;   // pitch class 0..11 (C=0)
        return pc <= 6 ? 39 + pc : 34 + (pc - 7);  // C4..Gb4 (39..45) wrapping to G3..B3 (34..38)
    }

    /// <summary>Transpose C-major scale note numbers into <paramref name="key"/>.</summary>
    public static int[] Transpose(int[] scaleNoteNumbers, string key)
    {
        int offset = Offset(key);
        var result = new int[scaleNoteNumbers.Length];
        for (int i = 0; i < scaleNoteNumbers.Length; i++)
            result[i] = scaleNoteNumbers[i] + offset;
        return result;
    }

    /// <summary>
    /// Spell each note number for the key. <see cref="Note.Name"/> gives a flat name
    /// (e.g. "Gb4"); under a sharp key signature that pitch is written as the natural
    /// letter ("F4") and the key signature sharps it, etc.
    /// </summary>
    public static string[] NamesForKey(string key, IReadOnlyList<int> noteNumbers)
    {
        var names = new string[noteNumbers.Count];
        for (int i = 0; i < names.Length; i++)
        {
            string name = Note.Name(noteNumbers[i]);
            if ((key is "G" or "A" or "D" or "E" or "B") && name.Contains('b'))
                name = FlatToNaturalForSharpKeys(name);
            else if ((key is "F" or "Bb" or "Eb" or "Ab" or "Db") && name.Contains('b'))
                name = FlatToNaturalForFlatKeys(name);
            else if (key == "F#")
                name = AdjustForFSharp(name);
            names[i] = name;
        }
        return names;
    }

    private static string FlatToNaturalForSharpKeys(string n) => n switch
    {
        _ when n.StartsWith("Gb") => n.Replace("Gb", "F"),
        _ when n.StartsWith("Db") => n.Replace("Db", "C"),
        _ when n.StartsWith("Ab") => n.Replace("Ab", "G"),
        _ when n.StartsWith("Eb") => n.Replace("Eb", "D"),
        _ when n.StartsWith("Bb") => n.Replace("Bb", "A"),
        _ => throw new NotSupportedException($"Converting '{n}' FlatToNatural (sharp keys) is not supported."),
    };

    private static string FlatToNaturalForFlatKeys(string n) => n switch
    {
        _ when n.StartsWith("Bb") => n.Replace("Bb", "B"),
        _ when n.StartsWith("Eb") => n.Replace("Eb", "E"),
        _ when n.StartsWith("Ab") => n.Replace("Ab", "A"),
        _ when n.StartsWith("Db") => n.Replace("Db", "D"),
        _ when n.StartsWith("Gb") => n.Replace("Gb", "G"),
        _ => throw new NotSupportedException($"Converting '{n}' FlatToNatural (flat keys) is not supported."),
    };

    private static string AdjustForFSharp(string n) => n switch
    {
        _ when n.StartsWith("Bb") => n.Replace("Bb", "A"),
        _ when n.StartsWith("Eb") => n.Replace("Eb", "D"),
        _ when n.StartsWith("Ab") => n.Replace("Ab", "G"),
        _ when n.StartsWith("Db") => n.Replace("Db", "C"),
        _ when n.StartsWith("Gb") => n.Replace("Gb", "F"),
        _ when n.StartsWith("F") => n.Replace("F", "E"),
        _ when n.StartsWith("B") => n,
        _ => throw new NotSupportedException($"Converting '{n}' AdjustForFSharp is not supported."),
    };
}
