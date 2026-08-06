using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

/// <summary>
/// Shared mixed/review interval-identification page: a Melodic / Harmonic mode toggle
/// over combined chapter tables, quizzing the specific solfège pair — one answer button
/// per included prompt, mirroring the include list (Mark's model, matching C2/C3).
/// Used by L1C4 "Mixed Intervals" (chapters 2+3, pp. 63-64) and the L1C8 "All-Interval
/// Review" (chapters 2-7, pp. 180-181).
/// </summary>
public partial class IntervalReviewPage : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new(); // include label -> button
    private readonly IReadOnlyList<ReviewIntervalDrill> _melodic;
    private readonly IReadOnlyList<ReviewIntervalDrill> _harmonic;
    private ReviewIntervalDrill _drill = null!;
    private bool _answered;
    private bool _ready;

    protected IntervalReviewPage(
        string heading, string bookNote,
        IReadOnlyList<ReviewIntervalDrill> melodic, IReadOnlyList<ReviewIntervalDrill> harmonic)
    {
        InitializeComponent();
        _melodic = melodic;
        _harmonic = harmonic;
        HeadingLabel.Text = heading;
        BookNote.Text = bookNote;
        DoHeader.DoChanged += (_, _) => NewDrill();
        Includes.Changed += (_, _) => { BuildAnswers(); NewDrill(); };
        Includes.Play = PlayKey;
        Automation.Target = this;
        Rebuild();
        _ready = true;
    }

    private bool Harmonic => HarmonicRadio.IsChecked;
    private IReadOnlyList<ReviewIntervalDrill> Table => Harmonic ? _harmonic : _melodic;

    private List<ReviewIntervalDrill> Playable()
    {
        var included = Includes.Included.Select(int.Parse).ToHashSet();
        var table = Table;
        var list = new List<ReviewIntervalDrill>();
        for (int i = 0; i < table.Count; i++)
            if (included.Contains(i)) list.Add(table[i]);
        return list;
    }

    private void Rebuild()
    {
        Includes.Build(Table.Select((d, i) => (i.ToString(), d.IncludeLabel)));
        BuildAnswers();
        NewDrill();
    }

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        // One button per included prompt, in table order — the answers mirror the
        // include list ("DO MI (Maj 3rd)"), so the pair and its quality are one answer.
        foreach (var drill in Playable())
        {
            string label = drill.IncludeLabel;
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
        if (playable.Count == 0) return;
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
        try { await PlayDrillAsync(Table[int.Parse(key)]); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private async Task PlayDrillAsync(ReviewIntervalDrill drill)
    {
        if (Harmonic)
        {
            // Two notes together (slight upward roll so both pitches are heard).
            var low = await _samples.LoadAsync(Note.SampleFile(DoHeader.Do + drill.Offsets[0]));
            var high = await _samples.LoadAsync(Note.SampleFile(DoHeader.Do + drill.Offsets[1]));
            _audio.Play(AudioRenderer.RenderHarmonic(new[] { low, high }, seconds: 3.0));
        }
        else
        {
            // Two notes in sequence (half notes).
            var notes = new List<(byte[] sample, double seconds)>();
            foreach (var offset in drill.Offsets)
                notes.Add((await _samples.LoadAsync(Note.SampleFile(DoHeader.Do + offset)), 2.0));
            _audio.Play(AudioRenderer.RenderSequence(notes));
        }
    }

    private void OnAnswer(string guess)
    {
        if (_answered) return;
        _answered = true;
        bool correct = guess == _drill.IncludeLabel;
        Gauge.Record(correct);
        foreach (var (label, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (label == _drill.IncludeLabel) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
            else if (label == guess) { b.BackgroundColor = Colors.IndianRed; b.TextColor = Colors.White; }
        }
        StatusLabel.Text = correct ? "Correct!" : $"Not Quite — {_drill.IncludeLabel}";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    private void OnModeChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (!_ready || !e.Value) return;   // ignore init + the cleared radio
        Rebuild();
        Gauge.Reset();                     // melodic and harmonic tallies aren't comparable
    }

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        _ = PlayDrillAsync(_drill);
        return Harmonic ? 3.0 : 4.0;
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        foreach (var (label, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (label == _drill.IncludeLabel) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + _drill.IncludeLabel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}

/// <summary>L1C4 mixed intervals: all Ma3/Mi6/Mi3/Ma6 prompts from chapters 2+3 (book pp. 63-64).</summary>
public sealed class MixedIntervalsL1C4Page : IntervalReviewPage
{
    public MixedIntervalsL1C4Page()
        : base("Mixed Intervals — 3rds & 6ths",
               "Selected from the interval identification questions on pp. 63–64.",
               ReviewIntervalDrill.MelodicC4, ReviewIntervalDrill.HarmonicC4) { Title = "L1 C4 · Mixed Intervals"; }
}

/// <summary>L1C8 review: every diatonic interval from chapters 2-7 (book pp. 180-181).</summary>
public sealed class IntervalReviewL1C8Page : IntervalReviewPage
{
    public IntervalReviewL1C8Page()
        : base("All-Interval Review",
               "Selected from the review interval identification questions on pp. 180–181.",
               ReviewIntervalDrill.MelodicC8, ReviewIntervalDrill.HarmonicC8) { Title = "L1 C8 · All-Interval Review"; }
}
