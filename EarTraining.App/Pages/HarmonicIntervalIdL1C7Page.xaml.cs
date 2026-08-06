using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class HarmonicIntervalIdL1C7Page : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new(); // prompt label -> button
    private L1C7IntervalDrill _drill = null!;
    private bool _answered;
    private bool _ready;

    public HarmonicIntervalIdL1C7Page()
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

    private List<(int idx, L1C7IntervalDrill drill)> Pool()
    {
        var group = Group;
        var pool = new List<(int, L1C7IntervalDrill)>();
        for (int i = 0; i < L1C7IntervalDrill.Harmonic.Count; i++)
        {
            var d = L1C7IntervalDrill.Harmonic[i];
            if (group is null || d.Group == group) pool.Add((i, d));
        }
        return pool;
    }

    private List<L1C7IntervalDrill> Playable()
    {
        var included = Includes.Included.Select(int.Parse).ToHashSet();
        return Pool().Where(p => included.Contains(p.idx)).Select(p => p.drill).ToList();
    }

    // The include-row / answer-button text for a prompt: the bare pair in single-group
    // mode, "PAIR (Group)" in Both mode. Includes.Build and BuildAnswers share this —
    // the answers mirror the include list.
    private string LabelFor(L1C7IntervalDrill d) => Group is null ? $"{d.Label} ({d.Group})" : d.Label;

    private void Rebuild()
    {
        Includes.Build(Pool().Select(p => (p.idx.ToString(), LabelFor(p.drill))));
        BuildAnswers();
        NewDrill();
    }

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        // One button per included prompt, in table order — the answers mirror the include list.
        foreach (var drill in Playable())
        {
            string label = LabelFor(drill);
            if (_answerButtons.ContainsKey(label)) continue;
            // Width from the text, not from auto-measure: MAUI's FlexLayout sizes auto-width
            // buttons inconsistently on Android and can pack a row tight enough to clip a
            // label's suffix — which would make "SO RE (4th)" and "SO RE (5th)" look alike.
            var button = new Button { Margin = new Thickness(4) };
            button.Style = (Style)Application.Current!.Resources["AnswerButton"];
            button.Text = label;
            button.WidthRequest = 30 + 8.0 * label.Length;
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

    // Plays one specific prompt on demand from the Include list's ▶.
    private async void PlayKey(string key)
    {
        try { await PlayDrillAsync(L1C7IntervalDrill.Harmonic[int.Parse(key)]); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private async Task PlayDrillAsync(L1C7IntervalDrill drill)
    {
        var low = await _samples.LoadAsync(Note.SampleFile(DoHeader.Do + drill.Offsets[0]));
        var high = await _samples.LoadAsync(Note.SampleFile(DoHeader.Do + drill.Offsets[1]));
        _audio.Play(AudioRenderer.RenderHarmonic(new[] { low, high }, seconds: 3.0));
    }

    private void OnAnswer(string guess)
    {
        if (_answered) return;
        _answered = true;
        bool correct = guess == LabelFor(_drill);
        Gauge.Record(correct);
        foreach (var (label, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (label == LabelFor(_drill)) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
            else if (label == guess) { b.BackgroundColor = Colors.IndianRed; b.TextColor = Colors.White; }
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
        _ = PlayDrillAsync(_drill);
        return 3.0;
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        foreach (var (label, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (label == LabelFor(_drill)) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + $"{_drill.Label} ({_drill.Category})";
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
