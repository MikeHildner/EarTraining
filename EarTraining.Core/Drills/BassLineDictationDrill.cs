using EarTraining.Core.Theory;

namespace EarTraining.Core.Drills;

/// <summary>
/// A bass-line dictation: a melody in the bass register built the way the book teaches
/// (Ch. 5 §1.15-1.21) — each measure implies one diatonic triad, the strong beats (1 and 3)
/// carry chord tones of that triad, weak beats move by diatonic step (passing/neighbor
/// tones), and the line resolves to DO in the nearer octave. An anticipated note (an "8"
/// pushed into a held "4.") carries the following strong beat's chord tone, per the idiom.
/// Renders on a bass clef; reuses <see cref="DictationMeasure"/> and the dictation audio.
/// </summary>
public sealed record BassLineDictationDrill(
    IReadOnlyList<DictationMeasure> Measures,
    string Key,
    double Bpm,
    int DoNoteNumber)
{
    // Bass register scale around DO = C3 (note 27): low SO up to DO an octave above.
    // index:                                    0   1   2   3   4   5   6   7   8   9   10
    // solfeg:                                   SO, LA, TI, DO, RE, MI, FA, SO, LA, TI, DO'
    private static readonly int[] CBassScale = [22, 24, 26, 27, 29, 31, 32, 34, 36, 38, 39];
    private static readonly int[] Degrees = [4, 5, 6, 0, 1, 2, 3, 4, 5, 6, 0]; // 0=DO .. 6=TI

    private const int DoIndex = 3;
    private const int HighDoIndex = 10;

    // Diatonic triads as scale-degree sets {root, 3rd, 5th}. Pool grows with the chapter:
    // C5 = I, IV, V, VI (the chapter that introduces bass lines); C6 adds III; C7 adds II.
    private static readonly int[][] Triads =
    [
        [0, 2, 4],   // I
        [3, 5, 0],   // IV
        [4, 6, 1],   // V
        [5, 0, 2],   // VI
        [2, 4, 6],   // III
        [1, 3, 5],   // II
    ];

    // Per-measure rhythm pools (the book's C5 bass lines use halves & quarters only;
    // C6 adds eighth pairs + the dotted figure; C7 adds anticipations).
    private static readonly string[] BasicPatterns = ["2,2", "4,4,2", "2,4,4", "4,2,4", "4,4,4,4"];
    private static readonly string[] EighthPatterns = ["4,4,8,8,4", "8,8,4,2", "2,8,8,4", "8,8,4,4,4"];
    private static readonly string[] DottedPatterns = ["4.,8,2", "2,4.,8", "4,4,4.,8"];
    private static readonly string[] AnticipationPatterns = ["8,4.,2", "4,8,4.,4", "2,8,4.", "4,4,8,4."];

    public static BassLineDictationDrill Next(
        L1DictationChapter chapter, string key, double bpm,
        int numberOfMeasures, DictationRhythmStyle style, Random rng)
    {
        // Bass DO sits an octave below the melodic DO (G2..F#3), same key wrap.
        int offset = Keys.DoNote(key) - 12 - CBassScale[DoIndex];
        int[] scale = new int[CBassScale.Length];
        for (int i = 0; i < scale.Length; i++) scale[i] = CBassScale[i] + offset;

        int triadPool = chapter >= L1DictationChapter.C7 ? 6 : chapter >= L1DictationChapter.C6 ? 5 : 4;

        string[] patterns = PickPatterns(numberOfMeasures, style, rng);

        // Generate note indices until the line stays within an octave-ish span (like the
        // melodic engine's constraint), falling back to the last attempt.
        List<int> best = GenerateLine(patterns, triadPool, rng);
        for (int tries = 0; tries < 300; tries++)
        {
            if (SpanWithin(best, scale)) break;
            best = GenerateLine(patterns, triadPool, rng);
        }

        var measures = new List<DictationMeasure>(patterns.Length);
        int cursor = 0;
        foreach (string pattern in patterns)
        {
            string[] rhythms = pattern.Split(',');
            var numbers = new int[rhythms.Length];
            for (int i = 0; i < numbers.Length; i++) numbers[i] = scale[best[cursor++]];
            measures.Add(new DictationMeasure(numbers, Keys.NamesForKey(key, numbers), rhythms));
        }

        return new BassLineDictationDrill(measures, key, bpm, scale[DoIndex]);
    }

    private static string[] PickPatterns(int numberOfMeasures, DictationRhythmStyle style, Random rng)
    {
        var pool = new List<string>(BasicPatterns);
        if (style >= DictationRhythmStyle.Eighths) pool.AddRange(EighthPatterns);
        if (style >= DictationRhythmStyle.Dotted) pool.AddRange(DottedPatterns);
        if (style >= DictationRhythmStyle.Anticipations) pool.AddRange(AnticipationPatterns);

        while (true)
        {
            var patterns = new string[numberOfMeasures];
            for (int i = 0; i < numberOfMeasures; i++) patterns[i] = pool[rng.Next(pool.Count)];

            bool ok = style switch
            {
                DictationRhythmStyle.Basic => true,
                DictationRhythmStyle.Eighths => patterns.Any(EighthPatterns.Contains),
                DictationRhythmStyle.Dotted => patterns.Any(DottedPatterns.Contains),
                _ => patterns.Any(AnticipationPatterns.Contains),
            };
            if (ok) return patterns;
        }
    }

    private static List<int> GenerateLine(string[] patterns, int triadPool, Random rng)
    {
        var line = new List<int>();
        int prev = DoIndex;
        for (int m = 0; m < patterns.Length; m++)
        {
            bool lastMeasure = m == patterns.Length - 1;
            int[] triad = lastMeasure ? Triads[0] : Triads[rng.Next(triadPool)];   // resolve on I

            string[] codes = patterns[m].Split(',');
            double onset = 0;
            for (int s = 0; s < codes.Length; s++)
            {
                bool anticipated = codes[s] == "4." && s > 0 && codes[s - 1] == "8";
                bool strong = onset == 0.0 || onset == 2.0 || anticipated;
                bool finalNote = lastMeasure && s == codes.Length - 1;

                int idx;
                if (finalNote)
                {
                    // Resolve on DO in the nearer octave, so the ending approach stays melodic
                    // (a high line ends on DO' rather than plunging an octave to low DO).
                    idx = Math.Abs(prev - HighDoIndex) < Math.Abs(prev - DoIndex) ? HighDoIndex : DoIndex;
                }
                else if (strong)
                {
                    var candidates = new List<int>();
                    for (int i = 0; i < Degrees.Length; i++)
                        if (triad.Contains(Degrees[i]) && Math.Abs(i - prev) <= 4) candidates.Add(i);
                    idx = candidates.Count > 0
                        ? candidates[rng.Next(candidates.Count)]
                        : prev;
                }
                else
                {
                    // Weak beats move by diatonic step (passing / neighbor tones).
                    int dir = rng.Next(2) == 0 ? -1 : 1;
                    idx = prev + dir;
                    if (idx < 0 || idx >= Degrees.Length) idx = prev - dir;
                }

                line.Add(idx);
                prev = idx;
                onset += Rhythm.Seconds(codes[s], 60.0);   // at 60 bpm, seconds == beats
            }
        }
        return line;
    }

    private static bool SpanWithin(List<int> line, int[] scale)
    {
        int min = int.MaxValue, max = int.MinValue;
        foreach (int idx in line)
        {
            min = Math.Min(min, scale[idx]);
            max = Math.Max(max, scale[idx]);
        }
        return max - min <= 12;
    }
}
