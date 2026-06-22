using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Components;

/// <summary>
/// Shows the current DO (tonic), plays it as a reference, and re-rolls it. All L1C1
/// drills are built relative to <see cref="Do"/>; pages read it and rebuild on
/// <see cref="DoChanged"/>.
/// </summary>
public partial class TonicHeader : ContentView
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();

    public int Do { get; private set; }
    public event EventHandler? DoChanged;

    public TonicHeader()
    {
        InitializeComponent();
        Roll();
    }

    private void Roll()
    {
        Do = Tonic.RandomDo(_rng);
        DoLabel.Text = $"DO = {Note.Name(Do)}";
    }

    private async void OnPlayDo(object? sender, EventArgs e)
    {
        var wav = AudioRenderer.RenderSequence(new[] { (await _samples.LoadAsync(Note.SampleFile(Do)), 2.0) });
        _audio.Play(wav);
    }

    private void OnNewDo(object? sender, EventArgs e)
    {
        Roll();
        DoChanged?.Invoke(this, EventArgs.Empty);
    }
}
