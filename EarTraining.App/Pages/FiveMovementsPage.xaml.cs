using EarTraining.App.Services;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

/// <summary>
/// L2C7 reference — the five II-V-I key-change movements. Framed the way Mark asked for:
/// every movement is stated first as a KEY-CENTER change (all five move the key down —
/// by 1, 2, 3, 5 or 7 half-steps, exactly the deltas in L2C7Drill.Movements), with the
/// name's origin (the seam's bass interval, for three of them) second, where it doubles
/// as the listening cue. Circle distance = new accidentals = the book's "degree of
/// chromaticism" row. Each card plays a FIXED demo from C (DO = C4), unlike the quiz's
/// randomized keys. Book Ch. 7, pp. 37–45.
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

    // Per-category teaching copy. Categories and key deltas come from L2C7Drill.Movements —
    // those deltas ARE the key-center moves below (all five are downward: -1, -2, -3, -5, -7).
    private static readonly Dictionary<string, (string KeyMove, string Circle, string Why, string Lines)> Info = new()
    {
        ["Circle of 5ths"] = ("down a perfect 5th — C → F",
            "1 step clockwise · 1 new flat — the least chromatic, with the next key's II chord a short reach from home",
            "Named for the direction on the circle: every clockwise step is a V→I resolution. At the seam the roots move by a 4th (Cma7 → Gmi7) — the strongest, most tonal bass motion of the five.",
            "7-3 lines: swap by opposite half-steps."),
        ["Circle of 4ths"] = ("down a perfect 4th — C → G",
            "1 step counter-clockwise · 1 new sharp — the least chromatic",
            "Named for the direction on the circle: every counter-clockwise step is a IV→I resolution. At the seam the roots move by a minor 3rd (Cma7 → Ami7), which doesn't lead — and the new II chord is still diatonic to the old key, giving the move its 'plural' sound.",
            "7-3 lines: mostly 3rds."),
        ["Half-step up"] = ("down a minor 2nd — C → B",
            "5 steps counter-clockwise · 5 new sharps — the most chromatic of the five",
            "Named for the bass at the seam, which rises a half-step (Cma7 → C#mi7). The key itself lands a half-step LOWER.",
            "7-3 lines: don't move at all — they hold as commontones."),
        ["Half-step down"] = ("down a minor 3rd — C → A",
            "3 steps counter-clockwise · 3 new sharps — medium-high",
            "Named for the bass at the seam, which falls a half-step (Cma7 → Bmi7). The key drops a minor 3rd.",
            "7-3 lines: slide down a whole step in parallel."),
        ["Root commontone"] = ("down a major 2nd — C → Bb",
            "2 steps clockwise · 2 new flats — low-medium",
            "Named for the root shared across the seam: Cma7 → Cmi7, the same bass note twice. The key drops a whole step.",
            "7-3 lines: slide down a half-step in parallel."),
    };

    public FiveMovementsPage()
    {
        InitializeComponent();
        foreach (var (category, delta) in L2C7Drill.Movements)
            Cards.Add(BuildCard(category, delta));
    }

    private Border BuildCard(string category, int delta)
    {
        var (keyMove, circle, why, lines) = Info[category];
        var stack = new VerticalStackLayout { Spacing = 6 };
        stack.Add(new Label { Text = category, Style = (Style)Application.Current!.Resources["Heading"] });
        stack.Add(new Label { Text = $"Key center: {keyMove}", FontSize = 13.5, FontAttributes = FontAttributes.Bold });
        stack.Add(new Label { Text = circle, FontSize = 12.5, TextColor = Theme.Muted });
        stack.Add(new Label { Text = $"Why the name: {why}", FontSize = 13.5 });
        stack.Add(new Label { Text = lines, FontSize = 12.5, TextColor = Theme.Muted });
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
