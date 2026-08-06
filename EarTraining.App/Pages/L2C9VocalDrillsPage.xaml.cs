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
/// Playback follows the notation: pickups alone, the arrival DO together with its
/// tonic chord, and on the 5ths drills the next key's dominant 7th between stations.
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

    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private bool _playing;

    public L2C9VocalDrillsPage()
    {
        InitializeComponent();
        DrillPicker.ItemsSource = L2C9VocalDrill.All.Select(d => d.Name).ToList();
        DrillPicker.SelectedIndexChanged += (_, _) => { StopPlayback(); ShowStations(); };
        DrillPicker.SelectedIndex = 0;
        TempoPicker.ItemsSource = Tempos.Select(t => t.Name).ToList();
        TempoPicker.SelectedIndex = Math.Clamp(Preferences.Get("l2c9.tempo", 1), 0, Tempos.Length - 1);
        TempoPicker.SelectedIndexChanged += (_, _) => Preferences.Set("l2c9.tempo", TempoPicker.SelectedIndex);
        _audio.PlaybackEnded += (_, _) =>
            MainThread.BeginInvokeOnMainThread(() => ResetPlayButton("Done — sing it again?"));
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
        if (_playing)
        {
            StopPlayback("Stopped — Play starts from the top.");
            return;
        }
        try
        {
            StatusLabel.Text = "Loading…";
            var drill = Drill;
            var (_, noteSeconds, restSeconds) = Tempo;
            double holdSeconds = restSeconds * 1.8;   // the printed whole-note DO + chord

            // As printed (pp. 51-54): the pickup notes sound alone, the arrival DO carries
            // its tonic triad (root position an octave below, sung DO on top), and on the
            // circle-of-5ths drills the dominant 7th of the NEXT key — same root, add the
            // flat-7 — fills the gap to the next station. The 4ths drills print no
            // dominants, so there the held DO chord is the gap.
            var steps = new List<(IReadOnlyList<byte[]> chord, double seconds)>();
            var stations = drill.StationNotes;
            for (int s = 0; s < stations.Count; s++)
            {
                var station = stations[s];
                int stationDo = station[^1];   // both circle patterns end on the station's DO
                for (int n = 0; n < station.Count - 1; n++)
                    steps.Add((new[] { await _samples.LoadAsync(Note.SampleFile(station[n])) }, noteSeconds));

                var arrival = new List<byte[]>();
                foreach (int offset in new[] { -12, -8, -5, 0 })
                    arrival.Add(await _samples.LoadAsync(Note.SampleFile(stationDo + offset)));
                steps.Add((arrival, holdSeconds));

                if (drill.HasDominants && s < stations.Count - 1)
                {
                    var dominant = new List<byte[]>();
                    foreach (int offset in new[] { -12, -8, -5, -2 })
                        dominant.Add(await _samples.LoadAsync(Note.SampleFile(stationDo + offset)));
                    steps.Add((dominant, restSeconds));
                }
            }
            _audio.Play(AudioRenderer.RenderProgression(steps, gain: 0.65));
            _playing = true;
            PlayButton.Text = "◼ Stop";
            StatusLabel.Text = "Playing — sing along!";
        }
        catch (Exception ex)
        {
            ResetPlayButton("Audio error: " + ex.Message);
        }
    }

    private void StopPlayback(string status = "")
    {
        _audio.Stop();
        ResetPlayButton(status);
    }

    private void ResetPlayButton(string status)
    {
        _playing = false;
        PlayButton.Text = "▶ Play (sing along)";
        StatusLabel.Text = status;
    }

    private async void OnReferenceLink(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//l2c2");

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopPlayback();
    }
}
