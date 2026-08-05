using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

/// <summary>
/// L2C9 vocal drills (book Ch. 9, pp. 51-58): the four sing-along drills around the
/// complete circle of 5ths / 4ths, in Moveable-DO or Fixed-DO (chromatic) solfège.
/// A practice page like the book's — thirteen 4-note key stations play through while
/// the syllable table is on screen; no quiz/scoring (the pitches are the drill).
/// </summary>
public partial class L2C9VocalDrillsPage : ContentPage
{
    // (Name, note seconds, station-rest seconds) — Standard = the original pacing; Mark
    // flagged it as likely too fast for some users, hence Relaxed (and Brisk for balance).
    private static readonly (string Name, double Note, double Breath)[] Tempos =
    [
        ("Relaxed", 0.80, 1.30),
        ("Standard", 0.55, 0.90),
        ("Brisk", 0.42, 0.70),
    ];
    private const double TriadSeconds = 1.4;   // tonic-triad orientation before each station (Mark)

    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);

    public L2C9VocalDrillsPage()
    {
        InitializeComponent();
        DrillPicker.ItemsSource = L2C9VocalDrill.All.Select(d => d.Name).ToList();
        DrillPicker.SelectedIndexChanged += (_, _) => ShowStations();
        DrillPicker.SelectedIndex = 0;
        TempoPicker.ItemsSource = Tempos.Select(t => t.Name).ToList();
        TempoPicker.SelectedIndex = Math.Clamp(Preferences.Get("l2c9.tempo", 1), 0, Tempos.Length - 1);
        TempoPicker.SelectedIndexChanged += (_, _) => Preferences.Set("l2c9.tempo", TempoPicker.SelectedIndex);
    }

    private L2C9VocalDrill Drill => L2C9VocalDrill.All[Math.Max(0, DrillPicker.SelectedIndex)];
    private (string Name, double Note, double Breath) Tempo => Tempos[Math.Max(0, TempoPicker.SelectedIndex)];

    private void ShowStations()
    {
        StationsLayout.Children.Clear();
        var drill = Drill;
        for (int i = 0; i < drill.StationKeys.Count; i++)
        {
            StationsLayout.Children.Add(new Label
            {
                Text = $"{drill.StationKeys[i],-3}  {string.Join("  ", drill.StationSyllables[i])}",
                FontFamily = "Courier",
                FontSize = 15,
                HorizontalOptions = LayoutOptions.Center,
            });
        }
        StatusLabel.Text = string.Empty;
    }

    private async void OnPlay(object? sender, EventArgs e)
    {
        try
        {
            StatusLabel.Text = "Loading…";
            var drill = Drill;
            var (_, noteSeconds, restSeconds) = Tempo;
            // Each station: its tonic triad (root position, an octave below DO) to set the
            // key in the ear, then the four sung notes. Single notes ride as 1-note "chords".
            var steps = new List<(IReadOnlyList<byte[]> chord, double seconds)>();
            foreach (var station in drill.StationNotes)
            {
                int stationDo = station[^1];   // both circle patterns end on the station's DO
                var triad = new List<byte[]>();
                foreach (int offset in new[] { -12, -8, -5 })
                    triad.Add(await _samples.LoadAsync(Note.SampleFile(stationDo + offset)));
                steps.Add((triad, TriadSeconds));
                for (int n = 0; n < station.Count; n++)
                    steps.Add((new[] { await _samples.LoadAsync(Note.SampleFile(station[n])) },
                               n == station.Count - 1 ? restSeconds : noteSeconds));
            }
            _audio.Play(AudioRenderer.RenderProgression(steps, gain: 0.65));
            StatusLabel.Text = "Playing — sing along!";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "Audio error: " + ex.Message;
        }
    }

    private async void OnReferenceLink(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//l2c2");
}
