using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class TriadProgressionsPage : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new();
    private TriadProgressionDrill _drill = null!;
    private bool _answered;
    private bool _ready;

    public TriadProgressionsPage()
    {
        InitializeComponent();
        DoHeader.DoChanged += (_, _) => NewDrill();
        Includes.Changed += (_, _) => { BuildAnswers(); NewDrill(); };
        Automation.Target = this;
        Rebuild();
        _ready = true;
    }

    private bool ThreeChord => ThreeChordRadio.IsChecked;
    private IReadOnlyList<TriadProgressionDrill> Set =>
        ThreeChord ? TriadProgressionDrill.ThreeChord : TriadProgressionDrill.TwoChord;

    private List<TriadProgressionDrill> Playable()
    {
        var included = Includes.Included.Select(int.Parse).ToHashSet();
        return Set.Where((_, i) => included.Contains(i)).ToList();
    }

    private void Rebuild()
    {
        Includes.Build(Set.Select((d, i) => (i.ToString(), d.Label)));
        BuildAnswers();
        NewDrill();
    }

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        foreach (var d in Playable())
        {
            string label = d.Label;
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
        try { await PlayCurrentAsync(); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private async Task PlayCurrentAsync()
    {
        // Each chord = root in the bass + the 3 triad tones; the last chord rings longer.
        var steps = new List<(IReadOnlyList<byte[]> chord, double seconds)>();
        for (int i = 0; i < _drill.Chords.Count; i++)
        {
            var chord = _drill.Chords[i];
            var samples = new List<byte[]>
            {
                await _samples.LoadAsync(Note.SampleFile(Voicing.BassNoteNumber(DoHeader.Do + chord.BassRootOffset))),
            };
            foreach (var offset in chord.ToneOffsets)
                samples.Add(await _samples.LoadAsync(Note.SampleFile(DoHeader.Do + offset)));
            double seconds = i == _drill.Chords.Count - 1 ? 4.0 : 2.0;
            steps.Add((samples, seconds));
        }
        _audio.Play(AudioRenderer.RenderProgression(steps));
    }

    private void OnAnswer(string guess)
    {
        if (_answered) return;
        _answered = true;
        bool correct = guess == _drill.Label;
        Gauge.Record(correct);
        foreach (var (label, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (label == _drill.Label) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
            else if (label == guess) { button.BackgroundColor = Colors.IndianRed; button.TextColor = Colors.White; }
        }
        StatusLabel.Text = correct ? "Correct!" : $"Not Quite — {_drill.Label}";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    private void OnModeChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (!_ready || !e.Value) return;   // ignore init + the cleared radio
        Rebuild();
        Gauge.Reset();                     // 2- and 3-chord tallies aren't comparable
    }

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        _ = PlayCurrentAsync();
        return (_drill.Chords.Count - 1) * 2.0 + 4.0;
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        foreach (var (label, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (label == _drill.Label) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + _drill.Label;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
