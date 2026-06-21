using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class TriadPage : ContentPage
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _optionButtons = new();
    private TriadDrill _drill = null!;
    private bool _answered;
    private int _correct;
    private int _total;

    public TriadPage()
    {
        InitializeComponent();
        BuildOptions();
        NewDrill();
    }

    private void BuildOptions()
    {
        foreach (var option in TriadDrill.Options)
        {
            string name = option.Name;
            var button = new Button { Text = option.ShortName, WidthRequest = 92, Margin = new Thickness(4) };
            button.Clicked += (_, _) => OnAnswer(name);
            _optionButtons[name] = button;
            OptionsLayout.Children.Add(button);
        }
    }

    private void NewDrill()
    {
        _drill = TriadDrill.Next(_rng);
        _answered = false;
        StatusLabel.Text = string.Empty;
        NextButton.IsVisible = false;
        foreach (var button in _optionButtons.Values)
        {
            button.IsEnabled = true;
            button.ClearValue(Button.BackgroundColorProperty);
        }
    }

    private async void OnPlayChord(object? sender, EventArgs e) => await PlayAsync(harmonic: true);
    private async void OnPlayArpeggio(object? sender, EventArgs e) => await PlayAsync(harmonic: false);

    private async Task PlayAsync(bool harmonic)
    {
        var notes = new List<byte[]>();
        foreach (var noteNumber in _drill.NoteNumbers)
            notes.Add(await _samples.LoadAsync(Note.SampleFile(noteNumber)));

        var wav = harmonic ? AudioRenderer.RenderHarmonic(notes) : AudioRenderer.RenderMelodic(notes);
        _audio.Play(wav);
    }

    private void OnAnswer(string name)
    {
        if (_answered) return;
        _answered = true;
        _total++;
        bool correct = name == _drill.Answer.Name;
        if (correct) _correct++;

        foreach (var (qualityName, button) in _optionButtons)
        {
            button.IsEnabled = false;
            if (qualityName == _drill.Answer.Name) button.BackgroundColor = Colors.SeaGreen;
            else if (qualityName == name) button.BackgroundColor = Colors.IndianRed;
        }

        StatusLabel.Text = $"{(correct ? "Correct" : "Nope")} — {_drill.Answer.Name} ({_drill.RootName})";
        ScoreLabel.Text = $"Score: {_correct} / {_total}";
        NextButton.IsVisible = true;
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();
}
