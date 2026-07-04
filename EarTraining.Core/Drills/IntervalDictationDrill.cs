using EarTraining.Core.Theory;

namespace EarTraining.Core.Drills;

/// <summary>Which interval class the L1C3 dictation emphasizes (alongside C1 resolutions and, optionally, C2 intervals).</summary>
public enum L1C3DictationInterval { Minor3rd, Major6th, Both }

/// <summary>Which interval class the L1C2 dictation emphasizes (alongside C1 resolutions).</summary>
public enum L1C2DictationInterval { Major3rd, Minor6th, Both }

/// <summary>
/// Cumulative rhythm difficulty, following the book's progression: quarters/halves (C1),
/// eighth-note pairs (C2+), the dotted quarter-eighth figure (C6), and eighth-note
/// anticipations (C7). Anticipations are written as within-measure syncopation — an "8"
/// followed by a held note — because each measure renders as its own stave (no cross-barline
/// ties); the pushed-attack sound is the same. A push across the half-bar is the "8~4" token:
/// one sounding note displayed as eighth tied to quarter, so beat 3 is always visible
/// (standard engraving — burying the half-bar makes the figure hard to sight-read).
/// </summary>
public enum DictationRhythmStyle { Basic = 0, Eighths = 1, Dotted = 2, Anticipations = 3 }

/// <summary>Which Level 1 chapter's interval pool the general dictation draws from (pools are cumulative).</summary>
public enum L1DictationChapter { C4 = 4, C5 = 5, C6 = 6, C7 = 7 }

/// <summary>
/// A Level 1 melodic dictation woven from C1 resolutions plus an interval class, optionally with a
/// second interval class, and possibly a pair of eighth notes per phrase. Faithful port of the
/// L1C2 / L1C3 AudioAndDictation generators (GetIntervalIntQueue + GetNoteRhythms + the step ≤ 12 /
/// span ≤ 12 constraints):
/// <list type="bullet">
/// <item><b>L1C3</b> (<see cref="Next"/>): C1 + C3 (min 3rd / maj 6th), optionally + C2 (maj 3rd / min 6th).</item>
/// <item><b>L1C2</b> (<see cref="NextC2"/>): C1 + C2 (maj 3rd / min 6th); no second class.</item>
/// </list>
/// Reuses <see cref="DictationMeasure"/>.
/// </summary>
public sealed record IntervalDictationDrill(
    IReadOnlyList<DictationMeasure> Measures,
    string Key,
    double Bpm,
    int DoNoteNumber)
{
    // C major, low TI (index 0) to high SO (index 13). Index 1 is DO.
    private static readonly int[] CMajorScale = [38, 39, 41, 43, 44, 46, 48, 50, 51, 53, 55, 56, 58, 60];

    // Interval pairs as (from, to) scale indices into CMajorScale.
    private static readonly (int, int)[] C1Resolutions = [(2, 1), (4, 3), (6, 5), (7, 8), (0, 1)];
    private static readonly (int, int)[] Min3rdC3 = [(2, 4), (3, 5), (6, 8), (7, 9)];
    private static readonly (int, int)[] Maj6thC3 = [(1, 6), (2, 7), (4, 9), (5, 10)];
    private static readonly (int, int)[] Maj3rdC2 = [(1, 3), (4, 6), (5, 7)];
    private static readonly (int, int)[] Min6thC2 = [(3, 8), (6, 11), (7, 12)];

    // Chapter 5-7 diatonic pairs (book pp. 71-73, 111-113, 145-147), same index scheme.
    private static readonly (int, int)[] Per4thC5 = [(1, 4), (2, 5), (3, 6), (5, 8), (6, 9), (7, 10)];   // DO-FA .. TI-MI'
    private static readonly (int, int)[] Aug4thC5 = [(4, 7)];                                            // FA-TI
    private static readonly (int, int)[] Per5thC5 = [(1, 5), (2, 6), (3, 7), (4, 8), (5, 9), (6, 10)];   // DO-SO .. LA-MI'
    private static readonly (int, int)[] Dim5thC5 = [(7, 11)];                                           // TI-FA'
    private static readonly (int, int)[] Maj2ndC6 = [(1, 2), (2, 3), (4, 5), (5, 6), (6, 7)];            // DO-RE .. LA-TI
    private static readonly (int, int)[] Min7thC6 = [(2, 8), (3, 9), (5, 11), (6, 12), (7, 13)];         // RE-DO' .. TI-LA'
    private static readonly (int, int)[] Min2ndC7 = [(3, 4), (7, 8)];                                    // MI-FA, TI-DO'
    private static readonly (int, int)[] Maj7thC7 = [(1, 7), (4, 10)];                                   // DO-TI, FA-MI'

    /// <summary>L1C3 dictation: C1 + C3 (min 3rd / maj 6th), optionally + C2 (maj 3rd / min 6th).</summary>
    public static IntervalDictationDrill Next(
        L1C3DictationInterval interval, string key, double bpm,
        int numberOfMeasures, bool includeEighths, bool includeC2, Random rng)
    {
        (int, int)[] primary = interval switch
        {
            L1C3DictationInterval.Minor3rd => Min3rdC3,
            L1C3DictationInterval.Major6th => Maj6thC3,
            _ => Min3rdC3.Concat(Maj6thC3).ToArray(),
        };
        (int, int)[]? secondary = includeC2
            ? interval switch
            {
                L1C3DictationInterval.Minor3rd => Maj3rdC2,
                L1C3DictationInterval.Major6th => Min6thC2,
                _ => Maj3rdC2.Concat(Min6thC2).ToArray(),
            }
            : null;
        return Build(key, bpm, numberOfMeasures,
            includeEighths ? DictationRhythmStyle.Eighths : DictationRhythmStyle.Basic,
            primary, secondary, rng);
    }

    /// <summary>L1C2 dictation: C1 + C2 (maj 3rd / min 6th); no second interval class.</summary>
    public static IntervalDictationDrill NextC2(
        L1C2DictationInterval interval, string key, double bpm,
        int numberOfMeasures, bool includeEighths, Random rng)
    {
        (int, int)[] primary = interval switch
        {
            L1C2DictationInterval.Major3rd => Maj3rdC2,
            L1C2DictationInterval.Minor6th => Min6thC2,
            _ => Maj3rdC2.Concat(Min6thC2).ToArray(),
        };
        return Build(key, bpm, numberOfMeasures,
            includeEighths ? DictationRhythmStyle.Eighths : DictationRhythmStyle.Basic,
            primary, secondary: null, rng);
    }

    /// <summary>
    /// L1C4-C7 dictation: C1 resolutions + a cumulative interval pool per the book —
    /// C4 = all four 3rd/6th classes; C5 adds 4ths/5ths (incl. FA-TI / TI-FA tritones);
    /// C6 adds maj 2nds / min 7ths; C7 adds min 2nds / maj 7ths. The rhythm style is
    /// chosen independently (the book pairs C6 with dotted figures and C7 with anticipations,
    /// but every page offers the simpler styles too).
    /// </summary>
    public static IntervalDictationDrill NextChapter(
        L1DictationChapter chapter, string key, double bpm,
        int numberOfMeasures, DictationRhythmStyle style, Random rng)
    {
        IEnumerable<(int, int)> pool = Maj3rdC2.Concat(Min6thC2).Concat(Min3rdC3).Concat(Maj6thC3);
        if (chapter >= L1DictationChapter.C5)
            pool = pool.Concat(Per4thC5).Concat(Aug4thC5).Concat(Per5thC5).Concat(Dim5thC5);
        if (chapter >= L1DictationChapter.C6)
            pool = pool.Concat(Maj2ndC6).Concat(Min7thC6);
        if (chapter >= L1DictationChapter.C7)
            pool = pool.Concat(Min2ndC7).Concat(Maj7thC7);
        return Build(key, bpm, numberOfMeasures, style, pool.ToArray(), secondary: null, rng);
    }

    // Shared engine: pick two 2-measure phrases, generate a note queue satisfying the criteria
    // (each half has a C1 resolution + the primary interval [+ the secondary, if any]) within the
    // step/span limits, then lay the notes onto the chosen rhythms.
    private static IntervalDictationDrill Build(
        string key, double bpm, int numberOfMeasures, DictationRhythmStyle style,
        (int, int)[] primary, (int, int)[]? secondary, Random rng)
    {
        int[] scale = Keys.Transpose(CMajorScale, key);
        bool hasSecondary = secondary is not null;

        var (m1, m2) = PickPhrase(style, hasSecondary, rng);
        var (m3, m4) = PickPhrase(style, hasSecondary, rng);

        string[] patterns = numberOfMeasures == 4 ? [m1, m2, m3, m4] : [m1, m2];

        int first2 = m1.Split(',').Length + m2.Split(',').Length;
        int second2 = m3.Split(',').Length + m4.Split(',').Length;

        var queue = BuildNotes(scale, first2, second2, primary, secondary, rng);

        var measures = new List<DictationMeasure>(patterns.Length);
        foreach (string pattern in patterns)
        {
            string[] rhythms = pattern.Split(',');
            var numbers = new int[rhythms.Length];
            for (int i = 0; i < numbers.Length; i++) numbers[i] = queue.Dequeue();
            measures.Add(new DictationMeasure(numbers, Keys.NamesForKey(key, numbers), rhythms));
        }

        return new IntervalDictationDrill(measures, key, bpm, scale[1]);
    }

    // Per-measure rhythm pools, all summing to 4/4. Each style adds its patterns on top of the
    // simpler ones (mirroring the book: eighth pairs from C2, the dotted quarter-eighth figure
    // in C6, and eighth-note anticipations — written as within-measure syncopation — in C7).
    private static readonly string[] BasicPatterns =
        ["1", "2,2", "4,2.", "2.,4", "4,4,4,4", "4,4,2", "2,4,4", "4,2,4"];
    private static readonly string[] EighthPatterns =
        ["8,8,4,4,4", "8,8,4,2", "4,4,8,8,4", "2,8,8,4"];
    private static readonly string[] DottedPatterns =           // "4.,8" figure starting on a beat
        ["4.,8,2", "2,4.,8", "4.,8,4,4", "4,4,4.,8"];
    private static readonly string[] AnticipationPatterns =     // "8,4." push: attack off the beat, held over it
        ["8,4.,2", "8,4.,4,4", "4,8,8~4,4", "2,8,4.", "4,4,8,4."]; // the beat-3 push is tied ("8~4") so the half-bar shows

    private static List<string> NoteRhythms(DictationRhythmStyle style)
    {
        var r = new List<string>(BasicPatterns);
        if (style >= DictationRhythmStyle.Eighths) r.AddRange(EighthPatterns);
        if (style >= DictationRhythmStyle.Dotted) r.AddRange(DottedPatterns);
        if (style >= DictationRhythmStyle.Anticipations) r.AddRange(AnticipationPatterns);
        return r;
    }

    // One 2-measure phrase: enough notes (>= 4, or >= 6 with a secondary class), an even total (so
    // each interval/resolution completes), and the style's signature figure present — exactly one
    // eighth-pair for Eighths (none for Basic), at least one dotted measure for Dotted, at least
    // one anticipation measure for Anticipations. Mirrors the controller's rhythm-selection loops.
    private static (string, string) PickPhrase(DictationRhythmStyle style, bool hasSecondary, Random rng)
    {
        var rhythms = NoteRhythms(style);
        int minNotes = hasSecondary ? 6 : 4;
        while (true)
        {
            string a = rhythms[rng.Next(rhythms.Count)];
            string b = rhythms[rng.Next(rhythms.Count)];
            int total = a.Split(',').Length + b.Split(',').Length;
            if (total < minNotes) continue;
            if (total % 2 != 0) continue;

            int eighths = a.Split(',').Count(x => x == "8") + b.Split(',').Count(x => x == "8");
            bool ok = style switch
            {
                DictationRhythmStyle.Basic => eighths == 0,
                DictationRhythmStyle.Eighths => eighths == 2,
                DictationRhythmStyle.Dotted =>
                    DottedPatterns.Contains(a) || DottedPatterns.Contains(b),
                _ =>
                    AnticipationPatterns.Contains(a) || AnticipationPatterns.Contains(b),
            };
            if (ok) return (a, b);
        }
    }

    private static Queue<int> BuildNotes(int[] scale, int first2, int second2, (int, int)[] primary, (int, int)[]? secondary, Random rng)
    {
        for (int tries = 0; tries < 2000; tries++)
        {
            var q = Generate(scale, first2, second2, primary, secondary, rng, out bool criteria);
            if (criteria && StepsWithin(q, 12) && SpanWithin(q, 12)) return new Queue<int>(q);
        }
        return new Queue<int>(Generate(scale, first2, second2, primary, secondary, rng, out _)); // fallback
    }

    private static List<int> Generate(int[] scale, int first2, int second2, (int, int)[] primary, (int, int)[]? secondary, Random rng, out bool criteria)
    {
        int total = first2 + second2;
        bool f1 = false, fp = false, fs = false, s1 = false, sp = false, ss = false;

        var q = new List<int>();
        while (q.Count < total)
        {
            bool first = q.Count < first2;
            int kind = rng.Next(1, secondary is null ? 3 : 4);   // 1 = C1 resolution, 2 = primary interval, 3 = secondary interval
            if (kind == 1)
            {
                var (a, b) = C1Resolutions[rng.Next(C1Resolutions.Length)];
                if (first) f1 = true; else s1 = true;
                q.Add(scale[a]); q.Add(scale[b]);
            }
            else if (kind == 2)
            {
                var (a, b) = primary[rng.Next(primary.Length)];
                if (first) fp = true; else sp = true;
                AddPair(q, scale, a, b, rng);
            }
            else
            {
                var (a, b) = secondary![rng.Next(secondary.Length)];
                if (first) fs = true; else ss = true;
                AddPair(q, scale, a, b, rng);
            }
        }

        criteria = secondary is null
            ? (f1 && fp && s1 && sp)
            : (f1 && fp && fs && s1 && sp && ss);
        return q;
    }

    // Intervals can sound ascending or descending; C1 resolutions keep their order.
    private static void AddPair(List<int> q, int[] scale, int a, int b, Random rng)
    {
        if (rng.Next(2) == 0) { q.Add(scale[a]); q.Add(scale[b]); }
        else { q.Add(scale[b]); q.Add(scale[a]); }
    }

    private static bool StepsWithin(List<int> notes, int range)
    {
        for (int i = 0; i + 1 < notes.Count; i++)
            if (Math.Abs(notes[i] - notes[i + 1]) > range) return false;
        return true;
    }

    private static bool SpanWithin(List<int> notes, int range) => notes.Max() - notes.Min() <= range;
}
