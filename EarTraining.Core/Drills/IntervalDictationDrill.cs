using EarTraining.Core.Theory;

namespace EarTraining.Core.Drills;

/// <summary>Which interval class the L1C3 dictation emphasizes (alongside C1 resolutions and, optionally, C2 intervals).</summary>
public enum L1C3DictationInterval { Minor3rd, Major6th, Both }

/// <summary>
/// A Level 1 Chapter 3 melodic dictation: like <see cref="DictationDrill"/> but the phrase is
/// woven from C1 resolutions, C3 intervals (min 3rd / maj 6th) and — optionally — C2 intervals
/// (maj 3rd / min 6th), and it may contain a pair of eighth notes per phrase. Faithful port of
/// L1C3Controller.AudioAndDictation generation (GetIntervalIntQueue + GetNoteRhythms + the
/// step ≤ 12 / span ≤ 12 constraints). Reuses <see cref="DictationMeasure"/>.
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

    public static IntervalDictationDrill Next(
        L1C3DictationInterval interval, string key, double bpm,
        int numberOfMeasures, bool includeEighths, bool includeC2, Random rng)
    {
        int[] scale = Keys.Transpose(CMajorScale, key);

        var (m1, m2) = PickPhrase(includeEighths, includeC2, rng);
        var (m3, m4) = PickPhrase(includeEighths, includeC2, rng);

        string[] patterns = numberOfMeasures == 4 ? [m1, m2, m3, m4] : [m1, m2];

        int first2 = m1.Split(',').Length + m2.Split(',').Length;
        int second2 = m3.Split(',').Length + m4.Split(',').Length;

        var queue = BuildNotes(scale, first2, second2, interval, includeC2, rng);

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

    private static List<string> NoteRhythms(bool includeEighths)
    {
        var r = new List<string> { "1", "2,2", "4,2.", "2.,4", "4,4,4,4", "4,4,2", "2,4,4", "4,2,4" };
        if (includeEighths)
        {
            r.Add("8,8,4,4,4");
            r.Add("8,8,4,2");
            r.Add("4,4,8,8,4");
            r.Add("2,8,8,4");
        }
        return r;
    }

    // One 2-measure phrase: enough notes (>= 4, or >= 6 with C2), an even total (so each
    // interval/resolution completes), and exactly one eighth-pair when eighths are on (none
    // otherwise) — mirrors the controller's rhythm-selection while-loops.
    private static (string, string) PickPhrase(bool includeEighths, bool includeC2, Random rng)
    {
        var rhythms = NoteRhythms(includeEighths);
        int minNotes = includeC2 ? 6 : 4;
        while (true)
        {
            string a = rhythms[rng.Next(rhythms.Count)];
            string b = rhythms[rng.Next(rhythms.Count)];
            int total = a.Split(',').Length + b.Split(',').Length;
            int eighths = a.Split(',').Count(x => x == "8") + b.Split(',').Count(x => x == "8");
            if (total < minNotes) continue;
            if (total % 2 != 0) continue;
            if (!includeEighths && eighths > 0) continue;
            if (includeEighths && eighths != 2) continue;
            return (a, b);
        }
    }

    private static Queue<int> BuildNotes(int[] scale, int first2, int second2, L1C3DictationInterval interval, bool includeC2, Random rng)
    {
        for (int tries = 0; tries < 2000; tries++)
        {
            var q = Generate(scale, first2, second2, interval, includeC2, rng, out bool criteria);
            if (criteria && StepsWithin(q, 12) && SpanWithin(q, 12)) return new Queue<int>(q);
        }
        return new Queue<int>(Generate(scale, first2, second2, interval, includeC2, rng, out _)); // fallback
    }

    private static List<int> Generate(int[] scale, int first2, int second2, L1C3DictationInterval interval, bool includeC2, Random rng, out bool criteria)
    {
        int total = first2 + second2;
        bool f1 = false, f2 = false, f3 = false, s1 = false, s2 = false, s3 = false;

        (int, int)[] c3 = interval switch
        {
            L1C3DictationInterval.Minor3rd => Min3rdC3,
            L1C3DictationInterval.Major6th => Maj6thC3,
            _ => Min3rdC3.Concat(Maj6thC3).ToArray(),
        };
        (int, int)[] c2 = interval switch
        {
            L1C3DictationInterval.Minor3rd => Maj3rdC2,
            L1C3DictationInterval.Major6th => Min6thC2,
            _ => Maj3rdC2.Concat(Min6thC2).ToArray(),
        };

        var q = new List<int>();
        while (q.Count < total)
        {
            bool first = q.Count < first2;
            int kind = rng.Next(1, includeC2 ? 4 : 3);   // 1 = C1 resolution, 2 = C3 interval, 3 = C2 interval
            if (kind == 1)
            {
                var (a, b) = C1Resolutions[rng.Next(C1Resolutions.Length)];
                if (first) f1 = true; else s1 = true;
                q.Add(scale[a]); q.Add(scale[b]);
            }
            else if (kind == 2)
            {
                var (a, b) = c3[rng.Next(c3.Length)];
                if (first) f3 = true; else s3 = true;
                AddPair(q, scale, a, b, rng);
            }
            else
            {
                var (a, b) = c2[rng.Next(c2.Length)];
                if (first) f2 = true; else s2 = true;
                AddPair(q, scale, a, b, rng);
            }
        }

        criteria = includeC2
            ? (f1 && f2 && f3 && s1 && s2 && s3)
            : (f1 && f3 && s1 && s3);
        return q;
    }

    // C3/C2 intervals can sound ascending or descending; C1 resolutions keep their order.
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
