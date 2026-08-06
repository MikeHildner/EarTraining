namespace EarTraining.Core.Drills;

/// <summary>
/// The four Level 2 vocal drills (book Ch. 9, pp. 51-58): singing around the complete
/// circle of fifths / fourths, with either Moveable-DO syllables (the same four syllables
/// re-applied in every key) or Fixed-DO syllables (chromatic solfeg from the home C —
/// transcribed verbatim from the book's tables). Thirteen 4-note stations each; the
/// pitches are identical between the Moveable and Fixed variants of a circle — only the
/// syllables differ. Station DOs fold into the singable G3..Gb4 window.
/// As printed, every arrival DO carries its tonic chord, and the two circle-of-5ths
/// drills also interleave the dominant 7th of the next key between stations
/// (<see cref="HasDominants"/>); the 4ths drills print no sevenths.
/// </summary>
public sealed record L2C9VocalDrill(
    string Name,
    IReadOnlyList<string> StationKeys,
    IReadOnlyList<IReadOnlyList<string>> StationSyllables,
    IReadOnlyList<IReadOnlyList<int>> StationNotes,
    bool HasDominants)
{
    private const int HomeDo = 39;   // C4

    // Circle of 5ths: sing SO LA TI DO up into each new key (offsets from the station's DO).
    private static readonly int[] Circle5Pattern = [-5, -3, -1, 0];
    private static readonly string[] Circle5Keys = ["C", "F", "Bb", "Eb", "Ab", "Db", "Gb", "B", "E", "A", "D", "G", "C"];
    private static readonly string[] Circle5Moveable = ["SO", "LA", "TI", "DO"];
    private static readonly string[][] Circle5Fixed =
    [
        ["SO", "LA", "TI", "DO"], ["DO", "RE", "MI", "FA"], ["FA", "SO", "LA", "TE"],
        ["TE", "DO", "RE", "ME"], ["ME", "FA", "SO", "LE"], ["LE", "TE", "DO", "RA"],
        ["RA", "ME", "FA", "SE"], ["FI", "SI", "LI", "TI"], ["TI", "DI", "RI", "MI"],
        ["MI", "FI", "SI", "LA"], ["LA", "TI", "DI", "RE"], ["RE", "MI", "FI", "SO"],
        ["SO", "LA", "TI", "DO"],
    ];

    // Circle of 4ths: sing FA MI RE DO down into each new key.
    private static readonly int[] Circle4Pattern = [5, 4, 2, 0];
    private static readonly string[] Circle4Keys = ["C", "G", "D", "A", "E", "B", "F#", "Db", "Ab", "Eb", "Bb", "F", "C"];
    private static readonly string[] Circle4Moveable = ["FA", "MI", "RE", "DO"];
    private static readonly string[][] Circle4Fixed =
    [
        ["FA", "MI", "RE", "DO"], ["DO", "TI", "LA", "SO"], ["SO", "FI", "MI", "RE"],
        ["RE", "DI", "TI", "LA"], ["LA", "SI", "FI", "MI"], ["MI", "RI", "DI", "TI"],
        ["TI", "LI", "SI", "FI"], ["SE", "FA", "ME", "RA"], ["RA", "DO", "TE", "LE"],
        ["LE", "SO", "FA", "ME"], ["ME", "RE", "DO", "TE"], ["TE", "LA", "SO", "FA"],
        ["FA", "MI", "RE", "DO"],
    ];

    public static readonly IReadOnlyList<L2C9VocalDrill> All =
    [
        Build("Circle of 5ths — Moveable DO", Circle5Keys, -7, Circle5Pattern, i => Circle5Moveable, hasDominants: true),
        Build("Circle of 5ths — Fixed DO", Circle5Keys, -7, Circle5Pattern, i => Circle5Fixed[i], hasDominants: true),
        Build("Circle of 4ths — Moveable DO", Circle4Keys, 7, Circle4Pattern, i => Circle4Moveable, hasDominants: false),
        Build("Circle of 4ths — Fixed DO", Circle4Keys, 7, Circle4Pattern, i => Circle4Fixed[i], hasDominants: false),
    ];

    private static L2C9VocalDrill Build(
        string name, string[] keys, int delta, int[] pattern, Func<int, string[]> syllables, bool hasDominants)
    {
        var stationSyllables = new List<IReadOnlyList<string>>(keys.Length);
        var stationNotes = new List<IReadOnlyList<int>>(keys.Length);
        int stationDo = HomeDo;
        for (int i = 0; i < keys.Length; i++)
        {
            if (i > 0) stationDo = Fold(stationDo + delta);
            stationSyllables.Add(syllables(i));
            stationNotes.Add(pattern.Select(offset => stationDo + offset).ToArray());
        }
        return new L2C9VocalDrill(name, keys, stationSyllables, stationNotes, hasDominants);
    }

    // Keep each station's DO in the singable G3..Gb4 window (34..45), like Tonic.RandomDo's range.
    private static int Fold(int note)
    {
        while (note < 34) note += 12;
        while (note > 45) note -= 12;
        return note;
    }
}
