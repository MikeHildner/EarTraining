using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

/// <summary>
/// "Find the DO" — a scorable, absolute-pitch drill: play a random (hidden) tonic and name its pitch
/// class. The DO is always random, independent of the Settings practice key, and is never shown until
/// the answer. Gauge + Automation, mirroring the other quiz pages (e.g. PitchIdPage).
/// </summary>
public partial class FindTheDoPage : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new(); // pitch class -> button

    // Pitch-class names in chromatic (flat) order, matching Note's spelling; index = (note + 21) % 12.
    private static readonly string[] Classes =
        { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B" };

    private const double PlaySeconds = 2.0;

    private int _do;
    private bool _answered;

    public FindTheDoPage()
    {
        InitializeComponent();
        Automation.Target = this;
        BuildAnswers();
        NewDrill();
    }

    private static string ClassOf(int noteNumber) => Classes[(noteNumber + 21) % 12];

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        foreach (var name in Classes)
        {
            var n = name;
            var button = new Button { Text = n, WidthRequest = 64, Margin = new Thickness(4) };
            button.Style = (Style)Application.Current!.Resources["AnswerButton"];
            button.Clicked += (_, _) => OnAnswer(n);
            _answerButtons[n] = button;
            AnswersLayout.Children.Add(button);
        }
    }

    // A fresh random tonic each drill — deliberately independent of SettingsStore.FixedKey, so a
    // fixed practice key never pins this drill (that would give the answer away).
    private void NewDrill()
    {
        _do = Tonic.RandomDo(_rng);
        _answered = false;
        StatusLabel.Text = "Name the tonic.";
        foreach (var button in _answerButtons.Values)
        {
            button.IsEnabled = true;
            button.ClearValue(Button.BackgroundColorProperty);
            button.ClearValue(Button.TextColorProperty);
        }
    }

    private async Task PlayAsync()
    {
        var sample = await _samples.LoadAsync(Note.SampleFile(_do));
        _audio.Play(AudioRenderer.RenderSequence(new[] { (sample, PlaySeconds) }));
    }

    private async void OnPlay(object? sender, EventArgs e)
    {
        try { await PlayAsync(); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private async void OnNext(object? sender, EventArgs e)
    {
        NewDrill();
        try { await PlayAsync(); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private void OnAnswer(string guess)
    {
        if (_answered) return;
        _answered = true;
        string answer = ClassOf(_do);
        bool correct = guess == answer;
        Gauge.Record(correct);

        foreach (var (name, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (name == answer) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
            else if (name == guess) { button.BackgroundColor = Colors.IndianRed; button.TextColor = Colors.White; }
        }

        StatusLabel.Text = correct ? $"Correct! DO = {answer}" : $"Not Quite — DO = {answer}";
    }

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        _ = PlayAsync();
        return PlaySeconds;
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        string answer = ClassOf(_do);
        foreach (var (name, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (name == answer) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + $"DO = {answer}";
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
