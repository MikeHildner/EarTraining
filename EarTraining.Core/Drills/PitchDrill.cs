using EarTraining.Core.Theory;

namespace EarTraining.Core.Drills;

/// <summary>
/// One Pitch-ID prompt: a single note a fixed distance from DO; the listener names the
/// syllable. Ported from L1C1 GetNoteEx (the note sounds ~3 s).
/// </summary>
public sealed record PitchDrill(int DoNote, int Offset, int NoteNumber, string Syllable)
{
    public const double Seconds = 3.0;

    public static PitchDrill Next(int doNote, IReadOnlyList<int> includedOffsets, Random rng)
    {
        int offset = includedOffsets[rng.Next(includedOffsets.Count)];
        string syllable = Solfege.PitchNotes.First(p => p.Offset == offset).Syllable;
        return new PitchDrill(doNote, offset, doNote + offset, syllable);
    }
}
