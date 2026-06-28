namespace EarTraining.Core.Theory;

/// <summary>
/// Solfège syllables for Level 1 Chapter 1. Pitch ID plays a note a fixed number of
/// semitones from DO and the listener names the syllable. DO and TI each appear twice
/// (an octave / a step apart), so the quiz dedupes by syllable.
/// </summary>
public static class Solfege
{
    /// <summary>The seven quiz syllables, in scale order.</summary>
    public static readonly IReadOnlyList<string> Syllables = ["DO", "RE", "MI", "FA", "SO", "LA", "TI"];

    /// <summary>Pitch-ID practice notes: semitone offset from DO → syllable
    /// (ported from L1C1 PitchIdentification, offsets -1..12).</summary>
    public static readonly IReadOnlyList<(int Offset, string Syllable)> PitchNotes =
    [
        (-1, "TI"), (0, "DO"), (2, "RE"), (4, "MI"), (5, "FA"),
        (7, "SO"), (9, "LA"), (11, "TI"), (12, "DO"),
    ];
}
