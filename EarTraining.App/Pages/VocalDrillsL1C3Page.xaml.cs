using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class VocalDrillsL1C3Page : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new();
    private L1C3Drill _drill = null!;
    private bool _answered;

    public VocalDrillsL1C3Page()
    {
        InitializeComponent();
        DoHeader.DoChanged += (_, _) => NewDrill();
        Toggle.Changed += (_, _) => Rebuild();
        Includes.Changed += (_, _) => { BuildAnswers(); NewDrill(); };
        Includes.Play = PlayKey;
        Automation.Target = this;
        Toggle.Configure(IntervalQuality.Minor3rd, IntervalQuality.Major6th);
        Rebuild();
    }

    private List<(int idx, L1C3Drill drill)> Pool()
    {
        var quality = Toggle.Quality;
        var pool = new List<(int, L1C3Drill)>();
        for (int i = 0; i < L1C3Drill.Vocal.Count; i++)
        {
            var d = L1C3Drill.Vocal[i];
            if (quality is null || d.Quality == quality) pool.Add((i, d));
        }
        return pool;
    }

    private List<L1C3Drill> Playable()
    {
        var included = Includes.Included.Select(int.Parse).ToHashSet();
        return Pool().Where(p => included.Contains(p.idx)).Select(p => p.drill).ToList();
    }

    private void Rebuild()
    {
        Includes.Build(Pool().Select(p => (p.idx.ToString(), Toggle.Quality is null ? p.drill.QuizLabel : p.drill.Label)));
        BuildAnswers();
        NewDrill();
    }

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        foreach (var drill in Playable())
        {
            string label = drill.QuizLabel;
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
        foreach (var button in _answerButtons.Values)
        {
            button.IsEnabled = true;
            button.ClearValue(Button.BackgroundColorProperty);
            button.ClearValue(Button.TextColorProperty);
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
        try { await PlayDrillAsync(L1C3Drill.Vocal[int.Parse(key)]); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private async Task PlayDrillAsync(L1C3Drill drill)
    {
        // 5 notes: four quarters then a whole note (1 s / 4 s at 60 bpm).
        var notes = new List<(byte[] sample, double seconds)>();
        for (int i = 0; i < drill.Offsets.Count; i++)
            notes.Add((await _samples.LoadAsync(Note.SampleFile(DoHeader.Do + drill.Offsets[i])), i < 4 ? 1.0 : 4.0));
        _audio.Play(AudioRenderer.RenderSequence(notes));
    }

    private void OnAnswer(string guess)
    {
        if (_answered) return;
        _answered = true;
        bool correct = guess == _drill.QuizLabel;
        Gauge.Record(correct);
        foreach (var (label, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (label == _drill.QuizLabel) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
            else if (label == guess) { button.BackgroundColor = Colors.IndianRed; button.TextColor = Colors.White; }
        }
        StatusLabel.Text = correct ? "Correct!" : $"Not Quite — {_drill.QuizLabel}";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        _ = PlayDrillAsync(_drill);
        double total = 0;
        for (int i = 0; i < _drill.Offsets.Count; i++) total += i < 4 ? 1.0 : 4.0;
        return total;
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        foreach (var (label, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (label == _drill.QuizLabel) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + _drill.QuizLabel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
