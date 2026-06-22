namespace EarTraining.Core.Drills;

public enum ResolutionType { DoDoReDo, DoDoFaMi, DoDoLaSo, DoDoTiHighDo, DoDoLowTiDo }

/// <summary>
/// One solfège resolution built on a DO. Vocal Drills plays the full
/// DO-DO-[interval]-[resolution] (<see cref="WithDo"/>); Resolution ID plays just the
/// 2-note resolution (<see cref="ResolutionOnly"/>). Ported from
/// L1C1Controller.GetResolutionDO / GetResolutionNoDO at 60 bpm
/// (quarter 1 s, half 2 s, whole 4 s).
/// </summary>
public sealed record ResolutionDrill(
    ResolutionType Type,
    int DoNote,
    string Label,
    IReadOnlyList<(int note, double seconds)> WithDo,
    IReadOnlyList<(int note, double seconds)> ResolutionOnly)
{
    private const double Quarter = 1.0, Half = 2.0, Whole = 4.0;

    // (interval offset, resolution offset, label) per type — matches the web controller.
    private static readonly IReadOnlyDictionary<ResolutionType, (int interval, int resolution, string label)> Spec =
        new Dictionary<ResolutionType, (int, int, string)>
        {
            [ResolutionType.DoDoReDo]     = (2, 0, "DO DO RE DO"),
            [ResolutionType.DoDoFaMi]     = (5, 4, "DO DO FA MI"),
            [ResolutionType.DoDoLaSo]     = (9, 7, "DO DO LA SO"),
            [ResolutionType.DoDoTiHighDo] = (11, 12, "DO DO (high)TI DO"),
            [ResolutionType.DoDoLowTiDo]  = (-1, 0, "DO DO (low)TI DO"),
        };

    public static IReadOnlyList<ResolutionType> All { get; } =
    [
        ResolutionType.DoDoReDo, ResolutionType.DoDoFaMi, ResolutionType.DoDoLaSo,
        ResolutionType.DoDoTiHighDo, ResolutionType.DoDoLowTiDo,
    ];

    public static string LabelOf(ResolutionType type) => Spec[type].label;

    public static ResolutionDrill Build(ResolutionType type, int doNote)
    {
        var (interval, resolution, label) = Spec[type];
        var withDo = new (int, double)[]
        {
            (doNote, Quarter), (doNote, Quarter), (doNote + interval, Half), (doNote + resolution, Whole),
        };
        var resolutionOnly = new (int, double)[]
        {
            (doNote + interval, Half), (doNote + resolution, Whole),
        };
        return new ResolutionDrill(type, doNote, label, withDo, resolutionOnly);
    }

    public static ResolutionDrill Next(int doNote, IReadOnlyList<ResolutionType> included, Random rng) =>
        Build(included[rng.Next(included.Count)], doNote);
}
