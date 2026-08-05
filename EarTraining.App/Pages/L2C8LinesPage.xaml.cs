using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

/// <summary>
/// L2C8 7-3 line recognition (book Ch. 8, workbook pp. 79-82): a single II-V-I in a random
/// key with one of the two 7-3 lines doubled on top as the melody; identify 7–3–7 vs 3–7–3.
/// The reveal shows the line's solfeg (DO–TI–TI / FA–FA–MI) per the book's teaching.
/// </summary>
public partial class L2C8LinesPage : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new(); // line -> button

    private L2C8Drill _drill = null!;
    private int _do;
    private bool _answered;

    private const double ChordSeconds = 2.0;
    private const double FinalSeconds = 3.5;
    private const double MelodyBoost = 1.7;   // top-line gain so the 7-3 melody cuts through on phone speakers (Mark)

    public L2C8LinesPage()
    {
        InitializeComponent();
        BuildAnswers();
        Automation.Target = this;
        NewDrill();
    }

    private void BuildAnswers()
    {
        foreach (var line in L2C8Drill.Lines)
        {
            string key = line.Line;
            var button = new Button { Text = key, WidthRequest = 120, Margin = new Thickness(4) };
            button.Style = (Style)Application.Current!.Resources["AnswerButton"];
            button.Clicked += (_, _) => OnAnswer(key);
            _answerButtons[key] = button;
            AnswersLayout.Children.Add(button);
        }
    }

    private void NewDrill()
    {
        _drill = L2C8Drill.Next(_rng);
        _do = Tonic.RandomDo(_rng);
        _answered = false;
        StatusLabel.Text = string.Empty;
        foreach (var b in _answerButtons.Values)
        {
            b.IsEnabled = true;
            b.ClearValue(Button.BackgroundColorProperty);
            b.ClearValue(Button.TextColorProperty);
        }
    }

    private async void OnPlay(object? sender, EventArgs e)
    {
        try { await PlayDrillAsync(); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private async Task PlayDrillAsync()
    {
        var chords = _drill.Chords();
        var steps = new List<(IReadOnlyList<byte[]> chord, double seconds)>();
        for (int i = 0; i < chords.Count; i++)
        {
            var samples = new List<byte[]>
            {
                await _samples.LoadAsync(Note.SampleFile(Voicing.BassNoteNumber(_do + chords[i].Root))),
            };
            foreach (int tone in chords[i].Tones)
                samples.Add(await _samples.LoadAsync(Note.SampleFile(_do + tone)));
            steps.Add((samples, i == chords.Count - 1 ? FinalSeconds : ChordSeconds));
        }
        _audio.Play(AudioRenderer.RenderProgression(steps, topGain: MelodyBoost));
    }

    private void OnAnswer(string guess)
    {
        if (_answered) return;
        _answered = true;
        bool correct = guess == _drill.Line;
        Gauge.Record(correct);
        foreach (var (line, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (line == _drill.Line) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
            else if (line == guess) { b.BackgroundColor = Colors.IndianRed; b.TextColor = Colors.White; }
        }
        StatusLabel.Text = correct
            ? $"Correct! {_drill.Line} ({_drill.Solfeg})"
            : $"Not Quite — {_drill.Line} ({_drill.Solfeg})";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        _ = PlayDrillAsync();
        return ChordSeconds * 2 + FinalSeconds;
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        foreach (var (line, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (line == _drill.Line) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + $"{_drill.Line} ({_drill.Solfeg})";
    }

    private async void OnReferenceLink(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//l2c7movements");

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
