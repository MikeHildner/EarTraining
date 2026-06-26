using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class L2ProgressionsExplorerPage : ContentPage
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private IReadOnlyList<IReadOnlyList<int>> _walk = [];
    private int _do;

    public L2ProgressionsExplorerPage()
    {
        InitializeComponent();
        Includes.Build(Enum.GetValues<L2Explorer.Movement>().Select(m => (m.ToString(), L2Explorer.Label(m))));
        Includes.Changed += (_, _) => NewProgression();
        NewProgression();
    }

    private int ChordCount => ThreeRadio.IsChecked ? 3 : FiveRadio.IsChecked ? 5 : 4;

    private List<L2Explorer.Movement> IncludedMovements() =>
        Includes.Included.Select(Enum.Parse<L2Explorer.Movement>).ToList();

    private void NewProgression()
    {
        var movements = IncludedMovements();
        if (movements.Count == 0)
        {
            _walk = [];
            StatusLabel.Text = "Pick at least one movement type.";
            return;
        }
        _walk = L2Explorer.MajorWalk(ChordCount, movements, _rng);
        _do = _rng.Next(34, 46); // random key, like the web's new-random-key-per-progression
        StatusLabel.Text = $"{_walk.Count} chords — press play.";
    }

    private async void OnPlay(object? sender, EventArgs e)
    {
        try
        {
            if (_walk.Count == 0) return;
            var steps = new List<(IReadOnlyList<byte[]> chord, double seconds)>();
            for (int i = 0; i < _walk.Count; i++)
            {
                var tones = new List<byte[]>();
                foreach (var offset in _walk[i])
                    tones.Add(await _samples.LoadAsync(Note.SampleFile(_do + offset)));
                double seconds = i == _walk.Count - 1 ? 3.0 : 1.8;
                steps.Add((tones, seconds));
            }
            _audio.Play(AudioRenderer.RenderProgression(steps));
        }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private void OnNext(object? sender, EventArgs e) => NewProgression();
}
