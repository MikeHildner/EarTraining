using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class VocalDrillsPage : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<ResolutionType, Button> _answerButtons = new();
    private ResolutionDrill _drill = null!;
    private bool _answered;

    public VocalDrillsPage()
    {
        InitializeComponent();
        DoHeader.DoChanged += (_, _) => NewDrill();
        Includes.Changed += (_, _) => { BuildAnswers(); NewDrill(); };
        Includes.Build(ResolutionDrill.All.Select(t => (t.ToString(), ResolutionDrill.LabelOf(t))));
        Automation.Target = this;
        BuildAnswers();
        NewDrill();
    }

    private IReadOnlyList<ResolutionType> IncludedTypes =>
        Includes.Included.Select(Enum.Parse<ResolutionType>).ToList();

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        foreach (var type in IncludedTypes)
        {
            var t = type;
            var button = new Button { Text = ResolutionDrill.LabelOf(t), Margin = new Thickness(4) };
            button.Style = (Style)Application.Current!.Resources["AnswerButton"];
            button.Clicked += (_, _) => OnAnswer(t);
            _answerButtons[t] = button;
            AnswersLayout.Children.Add(button);
        }
    }

    private void NewDrill()
    {
        _drill = ResolutionDrill.Next(DoHeader.Do, IncludedTypes, _rng);
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
        var notes = new List<(byte[] sample, double seconds)>();
        foreach (var (note, seconds) in _drill.WithDo)
            notes.Add((await _samples.LoadAsync(Note.SampleFile(note)), seconds));
        _audio.Play(AudioRenderer.RenderSequence(notes));
    }

    private void OnAnswer(ResolutionType guess)
    {
        if (_answered) return;
        _answered = true;
        bool correct = guess == _drill.Type;
        Gauge.Record(correct);

        foreach (var (type, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (type == _drill.Type) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
            else if (type == guess) { button.BackgroundColor = Colors.IndianRed; button.TextColor = Colors.White; }
        }

        StatusLabel.Text = correct ? "Correct!" : $"Not Quite — {_drill.Label}";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        _ = PlayCurrentAsync();
        return _drill.WithDo.Sum(n => n.seconds);
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;            // user already answered this one
        _answered = true;
        if (scored) Gauge.Record(false);  // unanswered in scored mode = a miss
        foreach (var (type, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (type == _drill.Type) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + _drill.Label;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();   // don't keep running after leaving the page
    }
}
