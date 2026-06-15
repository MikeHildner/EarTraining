namespace EarTraining.Core.Theory;

/// <summary>
/// Piano note numbering used by the bundled samples: 0 = A0 (MIDI 21) up to 87 = C8.
/// Sample assets are named "{number}.{Name}{octave}.wav", e.g. "39.C4.wav".
/// </summary>
public static class Note
{
    public const int Lowest = 0;
    public const int Highest = 87;

    private static readonly string[] Flats =
        ["C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B"];

    /// <summary>e.g. 39 -> "C4".</summary>
    public static string Name(int noteNumber)
    {
        int midi = noteNumber + 21;
        return $"{Flats[midi % 12]}{midi / 12 - 1}";
    }

    /// <summary>Bundled MauiAsset filename, e.g. 39 -> "39.C4.wav".</summary>
    public static string SampleFile(int noteNumber) => $"{noteNumber}.{Name(noteNumber)}.wav";
}
