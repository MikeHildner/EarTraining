using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class TriadRecognitionL1C6Page : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new();
    private readonly IReadOnlyList<DiatonicTriadDrill> _all = DiatonicTriadDrill.All(5); // I/IV/V/vi/iii
    private DiatonicTriadDrill _drill = null!;
    private bool _answered;
    private bool _ready;

    public TriadRecognitionL1C6Page()
    {
        InitializeComponent();
        DoHeader.DoChanged += (_, _) => NewDrill();
        Includes.Changed += (_, _) => { BuildAnswers(); NewDrill(); };
        Automation.Target = this;
        Includes.Build(_all.Select(d => (d.Key, d.FullLabel)));
        BuildAnswers();
        NewDrill();
        _ready = true;
    }

    private bool ModeBoth => ScoreBothRadio.IsChecked;
    private string CorrectKey => ModeBoth ? _drill.Key : _drill.TriadIndex.ToString();
    private string AnswerText => ModeBoth ? _drill.FullLabel : _drill.TriadName;

    private List<DiatonicTriadDrill> Playable()
    {
        var included = Includes.Included.ToHashSet();
        return _all.Where(d => included.Contains(d.Key)).ToList();
    }

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        var included = Includes.Included.ToHashSet();
        if (ModeBoth)
        {
            foreach (var d in _all)
                if (included.Contains(d.Key)) AddAnswer(d.Key, d.FullLabel);
        }
        else
        {
            for (int t = 0; t < DiatonicTriadDrill.TriadNames.Length; t++)
            {
                bool any = false;
                for (int inv = 0; inv < DiatonicTriadDrill.InversionNames.Length; inv++)
                    if (included.Contains($"{t}-{inv}")) { any = true; break; }
                if (any) AddAnswer(t.ToString(), DiatonicTriadDrill.TriadNames[t]);
            }
        }
    }

    private void AddAnswer(string key, string label)
    {
        var button = new Button { Text = label, Margin = new Thickness(4) };
        button.Style = (Style)Application.Current!.Resources["AnswerButton"];
        button.Clicked += (_, _) => OnAnswer(key);
        _answerButtons[key] = button;
        AnswersLayout.Children.Add(button);
    }

    private void NewDrill()
    {
        var playable = Playable();
        _drill = playable[_rng.Next(playable.Count)];
        _answered = false;
        StatusLabel.Text = string.Empty;
        foreach (var button in _answerButtons.Values)
        {
            button.IsEnabled = true;
            button.ClearValue(Button.BackgroundColorProperty);
            button.ClearValue(Button.TextColorProperty);
        }
    }

    private async void OnPlay(object? sender, EventArgs e)
    {
        try { await PlayCurrentAsync(); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private async Task PlayCurrentAsync()
    {
        var samples = new List<byte[]>();
        foreach (var offset in _drill.Offsets)
            samples.Add(await _samples.LoadAsync(Note.SampleFile(DoHeader.Do + offset)));
        _audio.Play(AudioRenderer.RenderHarmonic(samples, seconds: 3.0));
    }

    private void OnAnswer(string guessKey)
    {
        if (_answered) return;
        _answered = true;
        bool correct = guessKey == CorrectKey;
        Gauge.Record(correct);
        foreach (var (key, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (key == CorrectKey) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
            else if (key == guessKey) { button.BackgroundColor = Colors.IndianRed; button.TextColor = Colors.White; }
        }
        StatusLabel.Text = correct ? "Correct!" : $"Not Quite — {AnswerText}";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    private void OnScoreModeChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (!_ready || !e.Value) return;
        BuildAnswers();
        Gauge.Reset();
        _answered = false;
        StatusLabel.Text = string.Empty;
    }

    private void OnInvert(object? sender, EventArgs e) => Includes.InvertAll();

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        _ = PlayCurrentAsync();
        return 3.0;
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        foreach (var (key, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (key == CorrectKey) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + AnswerText;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
