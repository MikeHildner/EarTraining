using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

/// <summary>
/// L2C7 II-V-I pairs quiz (book Ch. 7, workbook pp. 75-76): two four-part II-V-I sequences
/// in different keys, root-3-7 voicings, random key each drill; identify the key-center
/// movement (Circle of 5ths / 4ths / half-step up / down / root commontone) — the same
/// movement-category model as L2C4, since the exact chords aren't guessable in a random key.
/// </summary>
public partial class L2C7ProgressionsPage : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new(); // category -> button

    private L2C7Drill _drill = null!;
    private int _do;
    private bool _answered;

    private const double ChordSeconds = 1.6;
    private const double FinalSeconds = 3.0;
    private const double FirstResolveSeconds = 3.2;   // hold the first key's I ~4 beats before the next key (Mark's original audio)

    public L2C7ProgressionsPage()
    {
        InitializeComponent();
        Includes.Changed += (_, _) => { BuildAnswers(); NewDrill(); };
        Automation.Target = this;
        Includes.Build(L2C7Drill.Movements.Select(m => (m.Category, m.Category)));
        BuildAnswers();
        NewDrill();
    }

    private List<string> IncludedCategories()
    {
        var included = Includes.Included.ToHashSet();
        return L2C7Drill.Movements.Select(m => m.Category).Where(included.Contains).ToList();
    }

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        foreach (string category in IncludedCategories())
        {
            string cat = category;
            var button = new Button { Text = cat, Margin = new Thickness(4) };
            button.Style = (Style)Application.Current!.Resources["AnswerButton"];
            button.Clicked += (_, _) => OnAnswer(cat);
            _answerButtons[cat] = button;
            AnswersLayout.Children.Add(button);
        }
    }

    private void NewDrill()
    {
        var included = IncludedCategories();
        if (included.Count == 0) return;
        _drill = L2C7Drill.Next(included, _rng);
        _do = Tonic.RandomDo(_rng);
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
        try { await PlayDrillAsync(); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private async Task PlayDrillAsync()
    {
        var chords = _drill.Chords();
        var steps = new List<(IReadOnlyList<byte[]> chord, double seconds)>();
        for (int i = 0; i < chords.Count; i++)
        {
            var samples = new List<byte[]>
            {
                await _samples.LoadAsync(Note.SampleFile(Voicing.BassNoteNumber(_do + chords[i].Root))),
            };
            foreach (int upper in chords[i].Upper)
                samples.Add(await _samples.LoadAsync(Note.SampleFile(_do + upper)));
            double seconds = i == 2 ? FirstResolveSeconds                      // first key's I: let it settle
                           : i == chords.Count - 1 ? FinalSeconds : ChordSeconds;
            steps.Add((samples, seconds));
        }
        _audio.Play(AudioRenderer.RenderProgression(steps));
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
        StatusLabel.Text = correct ? "Correct!" : $"Not Quite — {_drill.Category}";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        _ = PlayDrillAsync();
        return ChordSeconds * 4 + FirstResolveSeconds + FinalSeconds;
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
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + _drill.Category;
    }

    private async void OnReferenceLink(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//l2c7movements");

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
