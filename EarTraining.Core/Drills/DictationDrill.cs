using EarTraining.Core.Theory;

namespace EarTraining.Core.Drills;

/// <summary>One measure of a dictation: the sounding pitches, their key-spelled
/// names (for notation), and the rhythm code of each note.</summary>
public sealed record DictationMeasure(
    IReadOnlyList<int> NoteNumbers,
    IReadOnlyList<string> NoteNames,
    IReadOnlyList<string> Rhythms);

/// <summary>
/// A melodic dictation: a short tonal melody built from solfège resolutions, in a
/// key, at a tempo. Plays as audio (DO lead-in + metronome + the melody) and reveals
/// as staff notation. Pure port of L1C1Controller.AudioAndDictation's generation
/// logic + EarTrainingLibrary NoteHelper — no NAudio, no server.
/// </summary>
public sealed record DictationDrill(
    IReadOnlyList<DictationMeasure> Measures,
    string Key,
    double Bpm,
    int DoNoteNumber)
{
    // C major scale note numbers with a low TI (index 0). Index 1 is DO.
    private static readonly int[] CMajorScale = [38, 39, 41, 43, 44, 46, 48, 50, 51];

    // Per-measure rhythm possibilities (comma-separated note durations summing to 4/4).
    private static readonly string[] RhythmPatterns =
        ["1", "2,2", "4,2.", "2.,4", "4,4,4,4", "4,4,2", "2,4,4", "4,2,4"];

    // Solfège resolutions as (from, to) scale indices. RE-DO, FA-MI, LA-SO, high TI-DO, low TI-DO.
    private static readonly (int From, int To)[] Resolutions =
        [(2, 1), (4, 3), (6, 5), (7, 8), (0, 1)];

    /// <param name="resolutionType">1 = resolutions, 2 = reverse, 3 = both.</param>
    /// <param name="numberOfMeasures">2 or 4.</param>
    public static DictationDrill Next(int resolutionType, string key, double bpm, int numberOfMeasures, Random rng)
    {
        int[] scale = Keys.Transpose(CMajorScale, key);

        // Pick a rhythm per measure; force an even total note count so a (reverse)
        // resolution always completes.
        string m1 = RhythmPatterns[rng.Next(RhythmPatterns.Length)];
        string m2 = RhythmPatterns[rng.Next(RhythmPatterns.Length)];
        if (numberOfMeasures == 2)
        {
            while ((m1.Split(',').Length + m2.Split(',').Length) % 2 == 1)
                m2 = RhythmPatterns[rng.Next(RhythmPatterns.Length)];
        }
        string m3 = RhythmPatterns[rng.Next(RhythmPatterns.Length)];
        string m4 = RhythmPatterns[rng.Next(RhythmPatterns.Length)];
        if (numberOfMeasures == 4)
        {
            while ((m1.Split(',').Length + m2.Split(',').Length + m3.Split(',').Length + m4.Split(',').Length) % 2 == 1)
                m4 = RhythmPatterns[rng.Next(RhythmPatterns.Length)];
        }

        string[] patterns = numberOfMeasures == 4 ? [m1, m2, m3, m4] : [m1, m2];
        int totalNotes = patterns.Sum(p => p.Split(',').Length);
        var queue = ResolutionQueue(scale, totalNotes, resolutionType, rng);

        var measures = new List<DictationMeasure>(patterns.Length);
        foreach (string pattern in patterns)
        {
            string[] rhythms = pattern.Split(',');
            var numbers = new int[rhythms.Length];
            for (int i = 0; i < numbers.Length; i++)
                numbers[i] = queue.Dequeue();
            measures.Add(new DictationMeasure(numbers, Keys.NamesForKey(key, numbers), rhythms));
        }

        return new DictationDrill(measures, key, bpm, scale[1]);
    }

    private static Queue<int> ResolutionQueue(int[] scale, int count, int resolutionType, Random rng)
    {
        var q = new Queue<int>();
        while (q.Count < count)
        {
            var (from, to) = Resolutions[rng.Next(Resolutions.Length)];
            switch (resolutionType)
            {
                case 1:
                    q.Enqueue(scale[from]); q.Enqueue(scale[to]);
                    break;
                case 2:
                    q.Enqueue(scale[to]); q.Enqueue(scale[from]);
                    break;
                case 3:
                    if (rng.Next(2) == 0) { q.Enqueue(scale[from]); q.Enqueue(scale[to]); }
                    else { q.Enqueue(scale[to]); q.Enqueue(scale[from]); }
                    break;
                default:
                    throw new NotSupportedException($"Resolution type '{resolutionType}' is not supported.");
            }
        }
        return q;
    }
}
