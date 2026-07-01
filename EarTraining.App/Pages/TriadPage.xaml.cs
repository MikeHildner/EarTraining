using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class TriadPage : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _optionButtons = new();
    private TriadDrill _drill = null!;
    private bool _answered;

    private const double PlaySeconds = 1.5;   // chord render (~1.2s) + margin; drives the automation timer

    public TriadPage()
    {
        InitializeComponent();
        BuildOptions();
        Automation.Target = this;
        NewDrill();
    }

    private void BuildOptions()
    {
        foreach (var option in TriadDrill.Options)
        {
            string name = option.Name;
            var button = new Button { Text = option.ShortName, WidthRequest = 92, Margin = new Thickness(4) };
            button.Style = (Style)Application.Current!.Resources["AnswerButton"];
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
        foreach (var button in _optionButtons.Values)
        {
            button.IsEnabled = true;
            button.ClearValue(Button.BackgroundColorProperty);
            button.ClearValue(Button.TextColorProperty);
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
        bool correct = name == _drill.Answer.Name;
        Gauge.Record(correct);

        foreach (var (qualityName, button) in _optionButtons)
        {
            button.IsEnabled = false;
            if (qualityName == _drill.Answer.Name) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
            else if (qualityName == name) { button.BackgroundColor = Colors.IndianRed; button.TextColor = Colors.White; }
        }

        StatusLabel.Text = $"{(correct ? "Correct" : "Not Quite")} — {_drill.Answer.Name} ({_drill.RootName})";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        _ = PlayAsync(harmonic: true);   // chord — all notes together
        return PlaySeconds;
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        foreach (var (qualityName, button) in _optionButtons)
        {
            button.IsEnabled = false;
            if (qualityName == _drill.Answer.Name) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + $"{_drill.Answer.Name} ({_drill.RootName})";
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
