namespace EarTraining.Core.Drills;

/// <summary>
/// 7-3 melodic line recognition over a II-V-I (book Ch. 8 + workbook pp. 79-82): a single
/// four-part II-V-I (the L2C7 root-3-7 voicings) with one of the two concurrent 7-3 lines
/// doubled an octave up as the melody. Identify which line was on top:
/// "7-3-7" = DO–TI–TI (half-step then commontone) or "3-7-3" = FA–FA–MI (commontone then
/// half-step). Reveals the line's solfeg, as the book teaches it.
/// </summary>
public sealed record L2C8Drill(string Line, IReadOnlyList<int> MelodyOffsets, string Solfeg)
{
    /// <summary>The two 7-3 lines; melody offsets are from DO, an octave above the chord uppers.</summary>
    public static readonly IReadOnlyList<L2C8Drill> Lines =
    [
        new("7–3–7", [24, 23, 23], "DO – TI – TI"),
        new("3–7–3", [17, 17, 16], "FA – FA – MI"),
    ];

    public static L2C8Drill Next(Random rng) => Lines[rng.Next(Lines.Count)];

    /// <summary>
    /// The three chords as (bass root offset, tone offsets incl. the melody note on top),
    /// relative to DO — the L2C7 II-V-I voicings plus this drill's melody line.
    /// </summary>
    public IReadOnlyList<(int Root, IReadOnlyList<int> Tones)> Chords()
    {
        var list = new List<(int, IReadOnlyList<int>)>(3);
        for (int c = 0; c < 3; c++)
        {
            var tones = new List<int>(L2C7Drill.Uppers[c]) { MelodyOffsets[c] };
            list.Add((L2C7Drill.Roots[c], tones));
        }
        return list;
    }
}
