using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class IntervalPage : ContentPage
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<int, Button> _optionButtons = new();
    private IntervalDrill _drill = null!;
    private bool _answered;
    private int _correct;
    private int _total;

    public IntervalPage()
    {
        InitializeComponent();
        BuildOptions();
        NewDrill();
    }

    private void BuildOptions()
    {
        foreach (var option in IntervalDrill.Options)
        {
            int semitones = option.Semitones;
            var button = new Button { Text = option.ShortName, WidthRequest = 78, Margin = new Thickness(4) };
            button.Style = (Style)Application.Current!.Resources["AnswerButton"];
            button.Clicked += (_, _) => OnAnswer(semitones);
            _optionButtons[semitones] = button;
            OptionsLayout.Children.Add(button);
        }
    }

    private void NewDrill()
    {
        _drill = IntervalDrill.Next(_rng);
        _answered = false;
        StatusLabel.Text = string.Empty;
        NextButton.IsVisible = false;
        foreach (var button in _optionButtons.Values)
        {
            button.IsEnabled = true;
            button.ClearValue(Button.BackgroundColorProperty);
            button.ClearValue(Button.TextColorProperty);
        }
    }

    private async void OnPlayMelodic(object? sender, EventArgs e) => await PlayAsync(melodic: true);
    private async void OnPlayHarmonic(object? sender, EventArgs e) => await PlayAsync(melodic: false);

    private async Task PlayAsync(bool melodic)
    {
        var low = await _samples.LoadAsync(Note.SampleFile(_drill.LowNote));
        var high = await _samples.LoadAsync(Note.SampleFile(_drill.HighNote));
        var notes = new[] { low, high };
        var wav = melodic ? AudioRenderer.RenderMelodic(notes) : AudioRenderer.RenderHarmonic(notes);
        _audio.Play(wav);
    }

    private void OnAnswer(int semitones)
    {
        if (_answered) return;
        _answered = true;
        _total++;
        bool correct = semitones == _drill.Answer.Semitones;
        if (correct) _correct++;
        ProgressStore.Record("interval", correct);

        foreach (var (semis, button) in _optionButtons)
        {
            button.IsEnabled = false;
            if (semis == _drill.Answer.Semitones) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
            else if (semis == semitones) { button.BackgroundColor = Colors.IndianRed; button.TextColor = Colors.White; }
        }

        StatusLabel.Text = $"{(correct ? "Correct" : "Not Quite")} — {_drill.Answer.Name} ({_drill.LowName} to {_drill.HighName})";
        ScoreLabel.Text = $"Score: {_correct} / {_total}";
        NextButton.IsVisible = true;
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();
}
