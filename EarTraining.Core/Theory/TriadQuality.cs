namespace EarTraining.Core.Theory;

/// <summary>
/// A triad quality: display labels plus the semitone offsets of its three notes
/// from the root (root, third, fifth).
/// </summary>
public sealed record TriadQuality(string Name, string ShortName, IReadOnlyList<int> Semitones)
{
    public static readonly IReadOnlyList<TriadQuality> Common =
    [
        new("Major",      "Maj", [0, 4, 7]),
        new("Minor",      "min", [0, 3, 7]),
        new("Diminished", "dim", [0, 3, 6]),
        new("Augmented",  "aug", [0, 4, 8]),
    ];
}
