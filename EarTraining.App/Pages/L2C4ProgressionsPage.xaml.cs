using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class L2C4ProgressionsPage : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new(); // category -> button
    private L2C4Drill? _drill;
    private int _do; // random key per drill (note number of DO)
    private bool _answered;
    private bool _ready;

    public L2C4ProgressionsPage()
    {
        InitializeComponent();
        Includes.Changed += (_, _) => { BuildAnswers(); NewDrill(); };
        Automation.Target = this;
        Rebuild();
        _ready = true;
    }

    private bool ThreeChord => ThreeChordRadio.IsChecked;
    private IReadOnlyList<L2C4Drill> Set => ThreeChord ? L2C4Drill.ThreeChord : L2C4Drill.TwoChord;
    private string[] Categories => ThreeChord ? L2C4Drill.ThreeChordCategories : L2C4Drill.TwoChordCategories;

    private List<L2C4Drill> Playable()
    {
        var included = Includes.Included.ToHashSet();
        return Set.Where(d => included.Contains(d.Category)).ToList();
    }

    private void Rebuild()
    {
        // Includes (and the quiz answers) are the movement categories of the current mode.
        Includes.Build(Categories.Select(c => (c, c)));
        BuildAnswers();
        NewDrill();
    }

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        var included = Includes.Included.ToHashSet();
        foreach (var cat in Categories)
        {
            if (!included.Contains(cat)) continue;
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
        if (playable.Count == 0) { _drill = null; StatusLabel.Text = "Pick at least one movement to drill."; return; }
        _drill = playable[_rng.Next(playable.Count)];
        _do = _rng.Next(34, 46); // random key (G3..F#4), re-randomized each drill like the web
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
        if (_drill is null) return;
        var steps = new List<(IReadOnlyList<byte[]> chord, double seconds)>();
        for (int i = 0; i < _drill.Chords.Count; i++)
        {
            var tones = new List<byte[]>();
            foreach (var offset in _drill.Chords[i])
                tones.Add(await _samples.LoadAsync(Note.SampleFile(_do + offset)));
            double seconds = i == _drill.Chords.Count - 1 ? 4.0 : 2.0; // hold the final chord
            steps.Add((tones, seconds));
        }
        _audio.Play(AudioRenderer.RenderProgression(steps));
    }

    private void OnAnswer(string guess)
    {
        if (_answered || _drill is null) return;
        _answered = true;
        bool correct = guess == _drill.Category;
        Gauge.Record(correct);
        foreach (var (cat, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (cat == _drill.Category) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
            else if (cat == guess) { b.BackgroundColor = Colors.IndianRed; b.TextColor = Colors.White; }
        }
        StatusLabel.Text = correct ? "Correct!" : $"Not Quite — {_drill.Category}";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    private void OnModeChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (!_ready || !e.Value) return;
        Rebuild();
        Gauge.Reset();
    }

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        if (_drill is null) return 0.5;
        _ = PlayCurrentAsync();
        return (_drill.Chords.Count - 1) * 2.0 + 4.0;
    }

    public void AutoReveal(bool scored)
    {
        if (_answered || _drill is null) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        foreach (var (cat, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (cat == _drill.Category) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + _drill.Category;
    }

    private async void OnReferenceLink(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//l2c2");

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
