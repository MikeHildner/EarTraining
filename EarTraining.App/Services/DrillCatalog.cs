namespace EarTraining.App.Services;

/// <summary>
/// The set of drills that produce a score, mapped from their Shell route to a friendly display name,
/// in Level → chapter order. Used by <see cref="ProgressStore"/> to validate which routes to record
/// (so non-scoring pages — dictation, the play-only explorers, Solfège Syllables — are
/// ignored) and by the Progress page to label per-drill stats. Routes match AppShell.xaml / HomePage.
/// </summary>
public static class DrillCatalog
{
    /// <summary>Scoring drills only, in display order. (Dictation + explorer/reference pages have no quiz.)</summary>
    public static readonly IReadOnlyList<(string Route, string Name)> Ordered = new (string, string)[]
    {
        ("l1c1vocal",       "L1 C1 · Vocal Drills"),
        ("l1c1resolution",  "L1 C1 · Resolution ID"),
        ("l1c1pitch",       "L1 C1 · Pitch ID"),
        ("l1c2vocal",       "L1 C2 · Vocal Drills"),
        ("l1c2melodic",     "L1 C2 · Melodic Intervals"),
        ("l1c2harmonic",    "L1 C2 · Harmonic Intervals"),
        ("l1c3vocal",       "L1 C3 · Vocal Drills"),
        ("l1c3melodic",     "L1 C3 · Melodic Intervals"),
        ("l1c3harmonic",    "L1 C3 · Harmonic Intervals"),
        ("l1c3triad",       "L1 C3 · Triad Recognition"),
        ("l1c4harmonize",   "L1 C4 · Melody Harmonization"),
        ("l1c4progressions","L1 C4 · Triad Progressions"),
        ("l1c4intervals",   "L1 C4 · Mixed Intervals"),
        ("l1c5vocal",       "L1 C5 · Vocal Drills"),
        ("l1c5melodic",     "L1 C5 · Melodic Intervals"),
        ("l1c5harmonic",    "L1 C5 · Harmonic Intervals"),
        ("l1c5triad",       "L1 C5 · Triad Recognition"),
        ("l1c5progressions","L1 C5 · Triad Progressions"),
        ("l1c6vocal",       "L1 C6 · Vocal Drills"),
        ("l1c6melodic",     "L1 C6 · Melodic Intervals"),
        ("l1c6harmonic",    "L1 C6 · Harmonic Intervals"),
        ("l1c6triad",       "L1 C6 · Triad Recognition"),
        ("l1c6progressions","L1 C6 · Triad Progressions"),
        ("l1c7vocal",       "L1 C7 · Vocal Drills"),
        ("l1c7melodic",     "L1 C7 · Melodic Intervals"),
        ("l1c7harmonic",    "L1 C7 · Harmonic Intervals"),
        ("l1c7triad",       "L1 C7 · Triad Recognition"),
        ("l1c7progressions","L1 C7 · Triad Progressions"),
        ("l1c8review",      "L1 C8 · All-Interval Review"),
        ("l2c4",            "L2 C4 · Major Triad Progressions"),
        ("l2c5",            "L2 C5 · Diatonic Triad Progressions"),
        ("interval",        "Interval ID"),
        ("triad",           "Triad ID"),
        ("finddo",          "Find the DO"),
    };

    private static readonly Dictionary<string, string> Names =
        Ordered.ToDictionary(e => e.Route, e => e.Name);

    public static bool IsDrill(string route) => Names.ContainsKey(route);

    public static string NameFor(string route) => Names.TryGetValue(route, out var n) ? n : route;
}
