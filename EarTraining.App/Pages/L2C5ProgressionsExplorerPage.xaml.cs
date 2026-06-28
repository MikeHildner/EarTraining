using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class L2C5ProgressionsExplorerPage : ContentPage
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private IReadOnlyList<IReadOnlyList<int>> _chords = [];
    private int _do;

    public L2C5ProgressionsExplorerPage()
    {
        InitializeComponent();
        NewProgression();
    }

    private void NewProgression()
    {
        _chords = L2Explorer.DiatonicWalk(TonicFirstSwitch.IsToggled, _rng);
        _do = _rng.Next(34, 46); // random key, re-randomized each progression like the web
        StatusLabel.Text = "Four chords — press play.";
    }

    private async void OnPlay(object? sender, EventArgs e)
    {
        try
        {
            if (_chords.Count == 0) return;
            var steps = new List<(IReadOnlyList<byte[]> chord, double seconds)>();
            for (int i = 0; i < _chords.Count; i++)
            {
                var tones = new List<byte[]>();
                foreach (var offset in _chords[i])
                    tones.Add(await _samples.LoadAsync(Note.SampleFile(_do + offset)));
                double seconds = i == _chords.Count - 1 ? 3.0 : 1.8;
                steps.Add((tones, seconds));
            }
            _audio.Play(AudioRenderer.RenderProgression(steps));
        }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private void OnNext(object? sender, EventArgs e) => NewProgression();

    private void OnTonicToggled(object? sender, ToggledEventArgs e) => NewProgression();
}
