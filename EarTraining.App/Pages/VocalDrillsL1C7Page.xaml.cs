using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class VocalDrillsL1C7Page : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new(); // pattern label -> button
    private L1C7VocalDrill _drill = null!;
    private bool _answered;
    private bool _ready;

    public VocalDrillsL1C7Page()
    {
        InitializeComponent();
        DoHeader.DoChanged += (_, _) => NewDrill();
        Includes.Changed += (_, _) => { BuildAnswers(); NewDrill(); };
        Includes.Play = PlayKey;
        Automation.Target = this;
        Rebuild();
        _ready = true;
    }

    // "Min 2nd", "Maj 7th", or null = both
    private string? Group => SeventhsRadio.IsChecked ? "Maj 7th" : BothRadio.IsChecked ? null : "Min 2nd";

    private List<(int idx, L1C7VocalDrill drill)> Pool()
    {
        var group = Group;
        var pool = new List<(int, L1C7VocalDrill)>();
        for (int i = 0; i < L1C7VocalDrill.All.Count; i++)
        {
            var d = L1C7VocalDrill.All[i];
            if (group is null || d.Group == group) pool.Add((i, d));
        }
        return pool;
    }

    private List<L1C7VocalDrill> Playable()
    {
        var included = Includes.Included.Select(int.Parse).ToHashSet();
        return Pool().Where(p => included.Contains(p.idx)).Select(p => p.drill).ToList();
    }

    private void Rebuild()
    {
        // Every L1C7 vocal label is unique across both intervals, so no group suffix is needed.
        Includes.Build(Pool().Select(p => (p.idx.ToString(), p.drill.Label)));
        BuildAnswers();
        NewDrill();
    }

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        foreach (var drill in Playable())
        {
            string label = drill.Label;
            if (_answerButtons.ContainsKey(label)) continue;
            var button = new Button { Text = label, Margin = new Thickness(4) };
            button.Style = (Style)Application.Current!.Resources["AnswerButton"];
            button.Clicked += (_, _) => OnAnswer(label);
            _answerButtons[label] = button;
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
        try { await PlayDrillAsync(_drill); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    // Plays one specific pattern on demand from the Include list's ▶.
    private async void PlayKey(string key)
    {
        try { await PlayDrillAsync(L1C7VocalDrill.All[int.Parse(key)]); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private async Task PlayDrillAsync(L1C7VocalDrill drill)
    {
        var notes = new List<(byte[] sample, double seconds)>();
        for (int i = 0; i < drill.Offsets.Count; i++)
            notes.Add((await _samples.LoadAsync(Note.SampleFile(DoHeader.Do + drill.Offsets[i])), drill.Rhythm[i]));
        _audio.Play(AudioRenderer.RenderSequence(notes));
    }

    private void OnAnswer(string guess)
    {
        if (_answered) return;
        _answered = true;
        bool correct = guess == _drill.Label;
        Gauge.Record(correct);
        foreach (var (label, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (label == _drill.Label) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
            else if (label == guess) { b.BackgroundColor = Colors.IndianRed; b.TextColor = Colors.White; }
        }
        StatusLabel.Text = correct ? "Correct!" : $"Not Quite — {_drill.Label}";
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
        _ = PlayDrillAsync(_drill);
        return _drill.Rhythm.Sum();
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        foreach (var (label, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (label == _drill.Label) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + _drill.Label;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
