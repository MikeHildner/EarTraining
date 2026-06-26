using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class HarmonicIntervalIdL1C5Page : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new(); // category -> button
    private L1C5IntervalDrill _drill = null!;
    private bool _answered;
    private bool _ready;

    public HarmonicIntervalIdL1C5Page()
    {
        InitializeComponent();
        DoHeader.DoChanged += (_, _) => NewDrill();
        Includes.Changed += (_, _) => { BuildAnswers(); NewDrill(); };
        Automation.Target = this;
        Rebuild();
        _ready = true;
    }

    // "4th", "5th", or null = both
    private string? Group => FifthsRadio.IsChecked ? "5th" : BothRadio.IsChecked ? null : "4th";

    private List<(int idx, L1C5IntervalDrill drill)> Pool()
    {
        var group = Group;
        var pool = new List<(int, L1C5IntervalDrill)>();
        for (int i = 0; i < L1C5IntervalDrill.Harmonic.Count; i++)
        {
            var d = L1C5IntervalDrill.Harmonic[i];
            if (group is null || d.Group == group) pool.Add((i, d));
        }
        return pool;
    }

    private List<L1C5IntervalDrill> Playable()
    {
        var included = Includes.Included.Select(int.Parse).ToHashSet();
        return Pool().Where(p => included.Contains(p.idx)).Select(p => p.drill).ToList();
    }

    private void Rebuild()
    {
        bool both = Group is null;
        Includes.Build(Pool().Select(p => (p.idx.ToString(), both ? $"{p.drill.Label} ({p.drill.Group})" : p.drill.Label)));
        BuildAnswers();
        NewDrill();
    }

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        foreach (var drill in Playable())
        {
            string cat = drill.Category;
            if (_answerButtons.ContainsKey(cat)) continue;
            var button = new Button { Text = cat, Margin = new Thickness(4) };
            button.Style = (Style)Application.Current!.Resources["AnswerButton"];
            button.Clicked += (_, _) => OnAnswer(cat);
            _answerButtons[cat] = button;
            AnswersLayout.Children.Add(button);
        }
    }

    private void NewDrill()
    {
        var playable = Playable();
        _drill = playable[_rng.Next(playable.Count)];
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
        try { await PlayCurrentAsync(); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private async Task PlayCurrentAsync()
    {
        // Two notes played together (with a slight upward roll so both pitches are heard).
        var low = await _samples.LoadAsync(Note.SampleFile(DoHeader.Do + _drill.Offsets[0]));
        var high = await _samples.LoadAsync(Note.SampleFile(DoHeader.Do + _drill.Offsets[1]));
        _audio.Play(AudioRenderer.RenderHarmonic(new[] { low, high }, seconds: 3.0));
    }

    private void OnAnswer(string guess)
    {
        if (_answered) return;
        _answered = true;
        bool correct = guess == _drill.Category;
        Gauge.Record(correct);
        foreach (var (cat, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (cat == _drill.Category) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
            else if (cat == guess) { b.BackgroundColor = Colors.IndianRed; b.TextColor = Colors.White; }
        }
        StatusLabel.Text = correct ? "Correct!" : $"Not Quite — {_drill.Label} ({_drill.Category})";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    private void OnGroupChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (!_ready || !e.Value) return;
        Rebuild();
    }

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
        foreach (var (cat, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (cat == _drill.Category) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + $"{_drill.Label} ({_drill.Category})";
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
