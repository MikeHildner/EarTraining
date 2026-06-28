namespace EarTraining.Core.Theory;

/// <summary>Chord-voicing helpers shared by the L1C4 harmonization + progression drills.</summary>
public static class Voicing
{
    /// <summary>
    /// Fold a note into the bass register C2–C3 (note numbers 15–27), matching the web app's
    /// <c>BassNoteNumber()</c> extension — used to add a root in the bass under a triad.
    /// </summary>
    public static int BassNoteNumber(int noteNumber)
    {
        const int lowest = 15;  // C2
        const int highest = 27; // C3
        int n = noteNumber;
        while (n > highest) n -= 12;
        while (n < lowest) n += 12;
        return n;
    }
}
