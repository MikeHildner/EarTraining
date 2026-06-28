using EarTraining.Core.Theory;

namespace EarTraining.Core.Drills;

/// <summary>
/// One triad-recognition prompt: a root note and the quality built on it. Play the
/// three notes together (harmonic) or as an arpeggio (melodic) and identify the quality.
/// </summary>
public sealed record TriadDrill(int RootNote, TriadQuality Answer)
{
    /// <summary>The three sounding note numbers (root + each semitone offset).</summary>
    public IReadOnlyList<int> NoteNumbers => Answer.Semitones.Select(s => RootNote + s).ToList();

    public string RootName => Note.Name(RootNote);

    public static IReadOnlyList<TriadQuality> Options => TriadQuality.Common;

    public static TriadDrill Next(Random rng)
    {
        var answer = TriadQuality.Common[rng.Next(TriadQuality.Common.Count)];
        int root = rng.Next(27, 52); // C3..C5 root keeps the whole triad in range
        return new TriadDrill(root, answer);
    }
}
