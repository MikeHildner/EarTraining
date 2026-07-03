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
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);

    private const double NoteSeconds = 0.55;
    private const double StationRestSeconds = 0.9;   // breath after each station's last note

    public L2C9VocalDrillsPage()
    {
        InitializeComponent();
        DrillPicker.ItemsSource = L2C9VocalDrill.All.Select(d => d.Name).ToList();
        DrillPicker.SelectedIndexChanged += (_, _) => ShowStations();
        DrillPicker.SelectedIndex = 0;
    }

    private L2C9VocalDrill Drill => L2C9VocalDrill.All[Math.Max(0, DrillPicker.SelectedIndex)];

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
            var notes = new List<(byte[] sample, double seconds)>();
            foreach (var station in drill.StationNotes)
                for (int n = 0; n < station.Count; n++)
                    notes.Add((await _samples.LoadAsync(Note.SampleFile(station[n])),
                               n == station.Count - 1 ? StationRestSeconds : NoteSeconds));
            _audio.Play(AudioRenderer.RenderSequence(notes));
            StatusLabel.Text = "Playing — sing along!";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "Audio error: " + ex.Message;
        }
    }
}
