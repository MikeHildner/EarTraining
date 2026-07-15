namespace EarTraining.Core.Drills;

/// <summary>
/// II-V-I "pairs" recognition (book Ch. 7 + workbook pp. 75-76): two four-part II-V-I
/// sequences in different keys; identify how the key centers moved. Chords use the book's
/// definitive root-3-7 voicings — in-key upper offsets II {FA, DO'} → V {FA, TI} → I {MI, TI}
/// (each 7th resolves down a half-step into the next 3rd; the other voice holds as a
/// commontone), with the root in the bass. Movements follow the book's definitions:
/// circle terms relate the KEY CENTERS; the half-step/commontone terms relate the first
/// key's I root to the second key's II root.
/// </summary>
public sealed record L2C7Drill(string Category, int SecondKeyOffset)
{
    /// <summary>Bass root offsets (from the key's DO) for II, V, I.</summary>
    public static readonly int[] Roots = [2, 7, 0];

    /// <summary>Upper-voice offsets (from the key's DO) for II, V, I — the voiceled 3rd+7th pairs.</summary>
    public static readonly int[][] Uppers = [[5, 12], [5, 11], [4, 11]];

    /// <summary>The five movement categories, in the book's order (5 / 4 / ½U / ½D / RC).</summary>
    public static readonly IReadOnlyList<(string Category, int KeyDelta)> Movements =
    [
        ("Circle of 5ths", -7),    // C -> F
        ("Circle of 4ths", -5),    // C -> G
        ("Half-step up", -1),      // II root of key 2 = I root of key 1 + 1  (C -> B: C#mi7...)
        ("Half-step down", -3),    // II root of key 2 = I root of key 1 - 1  (C -> A: Bmi7...)
        ("Root commontone", -2),   // II root of key 2 = I root of key 1      (C -> Bb: Cmi7...)
    ];

    /// <summary>Pick a movement among the included categories; the second key is folded near DO.</summary>
    public static L2C7Drill Next(IReadOnlyList<string> includedCategories, Random rng)
    {
        var options = Movements.Where(m => includedCategories.Contains(m.Category)).ToList();
        var (category, delta) = options[rng.Next(options.Count)];
        return new L2C7Drill(category, FoldNearDo(delta));
    }

    /// <summary>
    /// The six chords of the pair as (bass root offset, upper offsets), all relative to the
    /// FIRST key's DO. Chords 0-2 are the first key's II-V-I; 3-5 the second key's.
    /// </summary>
    public IReadOnlyList<(int Root, IReadOnlyList<int> Upper)> Chords()
    {
        var list = new List<(int, IReadOnlyList<int>)>(6);
        for (int k = 0; k < 2; k++)
        {
            int keyOffset = k == 0 ? 0 : SecondKeyOffset;
            for (int c = 0; c < 3; c++)
                list.Add((keyOffset + Roots[c], Uppers[c].Select(u => keyOffset + u).ToArray()));
        }
        return list;
    }

    private static int FoldNearDo(int offset)
    {
        while (offset > 5) offset -= 12;
        while (offset < -6) offset += 12;
        return offset;
    }
}
