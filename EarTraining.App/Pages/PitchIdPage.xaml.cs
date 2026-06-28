using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class PitchIdPage : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new(); // syllable -> button

    // The web's 9 practice cells: semitone offset from DO -> include label.
    private static readonly (int offset, string label)[] PracticeNotes =
    {
        (-1, "TI (low)"), (0, "DO"), (2, "RE"), (4, "MI"), (5, "FA"),
        (7, "SO"), (9, "LA"), (11, "TI"), (12, "DO (8va)"),
    };

    private PitchDrill _drill = null!;
    private bool _answered;

    public PitchIdPage()
    {
        InitializeComponent();
        DoHeader.DoChanged += (_, _) => NewDrill();
        Includes.Changed += (_, _) => { BuildAnswers(); NewDrill(); };
        Includes.Build(PracticeNotes.Select(p => (p.offset.ToString(), p.label)));
        Automation.Target = this;
        BuildAnswers();
        NewDrill();
    }

    private IReadOnlyList<int> IncludedOffsets => Includes.Included.Select(int.Parse).ToList();

    private IReadOnlyList<string> AvailableSyllables =>
        Solfege.Syllables
            .Where(s => IncludedOffsets.Any(o => Solfege.PitchNotes.First(p => p.Offset == o).Syllable == s))
            .ToList();

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        foreach (var syllable in AvailableSyllables)
        {
            var s = syllable;
            var button = new Button { Text = s, WidthRequest = 84, Margin = new Thickness(4) };
            button.Style = (Style)Application.Current!.Resources["AnswerButton"];
            button.Clicked += (_, _) => OnAnswer(s);
            _answerButtons[s] = button;
            AnswersLayout.Children.Add(button);
        }
    }

    private void NewDrill()
    {
        _drill = PitchDrill.Next(DoHeader.Do, IncludedOffsets, _rng);
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
        var sample = await _samples.LoadAsync(Note.SampleFile(_drill.NoteNumber));
        _audio.Play(AudioRenderer.RenderSequence(new[] { (sample, PitchDrill.Seconds) }));
    }

    private void OnAnswer(string guess)
    {
        if (_answered) return;
        _answered = true;
        bool correct = guess == _drill.Syllable;
        Gauge.Record(correct);

        foreach (var (syllable, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (syllable == _drill.Syllable) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
            else if (syllable == guess) { button.BackgroundColor = Colors.IndianRed; button.TextColor = Colors.White; }
        }

        StatusLabel.Text = correct ? "Correct!" : $"Not Quite — {_drill.Syllable}";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        _ = PlayCurrentAsync();
        return PitchDrill.Seconds;
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        foreach (var (syllable, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (syllable == _drill.Syllable) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + _drill.Syllable;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
