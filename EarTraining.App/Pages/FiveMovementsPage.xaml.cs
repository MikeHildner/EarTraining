using EarTraining.App.Services;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

/// <summary>
/// L2C7 reference — the five II-V-I key-change movements, one card each: what the name
/// means (the I→II root interval, per the book), where the key center actually goes, and
/// the listening fingerprint from the book's ¶7.8 summary. Each card plays a FIXED demo
/// from C (DO = C4) — unlike the quiz's randomized keys — so the reader can connect the
/// name to the sound. Book Ch. 7, pp. 37–45.
/// </summary>
public partial class FiveMovementsPage : ContentPage
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);

    private const int DemoDo = 39; // C4 — every demo starts from Dmi7-G7-Cma7 in C

    // The L2C7 quiz's pacing, so the demos sound like the drill they explain.
    private const double ChordSeconds = 1.6;
    private const double FirstResolveSeconds = 3.2;
    private const double FinalSeconds = 3.0;

    // Per-category teaching copy; the categories and key deltas come from L2C7Drill.Movements.
    private static readonly Dictionary<string, (string RootMotion, string KeyGoes, string Fingerprint)> Info = new()
    {
        ["Circle of 5ths"] = ("Cma7 → Gmi7", "C → F — the next clockwise key",
            "Bass: the strongest, most tonal motion — 4th/5th root intervals the whole way. 7-3 lines: swap by opposite half-steps. Chromaticism: low (adjacent key)."),
        ["Circle of 4ths"] = ("Cma7 → Ami7", "C → G — the next counter-clockwise key",
            "Bass: a 'non-leading' minor 3rd at the seam. 7-3 lines: mostly 3rds. Chromaticism: low — the new II chord is still diatonic to the old key."),
        ["Half-step up"] = ("Cma7 → C#mi7", "C → B — the key drops a half-step",
            "Bass: a leading half-step at the seam. 7-3 lines: DON'T MOVE — they hold as commontones. Chromaticism: the highest of the five (five new sharps)."),
        ["Half-step down"] = ("Cma7 → Bmi7", "C → A — the key drops a minor 3rd",
            "Bass: a leading half-step at the seam. 7-3 lines: slide down a whole step in parallel. Chromaticism: medium-high (three new sharps)."),
        ["Root commontone"] = ("Cma7 → Cmi7", "C → Bb — the key drops a whole step",
            "Bass: DOESN'T MOVE — the same root carries across the seam. 7-3 lines: slide down a half-step in parallel. Chromaticism: low-medium (two new flats)."),
    };

    public FiveMovementsPage()
    {
        InitializeComponent();
        foreach (var (category, delta) in L2C7Drill.Movements)
            Cards.Add(BuildCard(category, delta));
    }

    private Border BuildCard(string category, int delta)
    {
        var (rootMotion, keyGoes, fingerprint) = Info[category];
        var stack = new VerticalStackLayout { Spacing = 6 };
        stack.Add(new Label { Text = category, Style = (Style)Application.Current!.Resources["Heading"] });
        stack.Add(new Label { Text = $"At the seam: {rootMotion}", FontSize = 13.5 });
        stack.Add(new Label { Text = $"Key center: {keyGoes}", FontSize = 13.5, FontAttributes = FontAttributes.Bold });
        stack.Add(new Label { Text = fingerprint, FontSize = 12.5, TextColor = Theme.Muted });
        var play = new Button { Text = "▶ Play it" };
        play.Clicked += async (_, _) => await PlayMovementAsync(category, delta);
        stack.Add(play);
        return new Border { Style = (Style)Application.Current!.Resources["Card"], Content = stack };
    }

    private async Task PlayMovementAsync(string category, int delta)
    {
        try
        {
            StatusLabel.Text = string.Empty;
            var chords = new L2C7Drill(category, delta).Chords();
            var steps = new List<(IReadOnlyList<int> Notes, double Seconds)>();
            for (int i = 0; i < chords.Count; i++)
            {
                var notes = new List<int> { Voicing.BassNoteNumber(DemoDo + chords[i].Root) };
                notes.AddRange(chords[i].Upper.Select(u => DemoDo + u));
                double seconds = i == 2 ? FirstResolveSeconds
                               : i == chords.Count - 1 ? FinalSeconds : ChordSeconds;
                steps.Add((notes, seconds));
            }
            await DemoAudio.PlayAsync(_samples, _audio, steps);
        }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }
}
