using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

/// <summary>
/// L2C5 scored quiz (upgrades the old play-only explorer; book Ch. 5 workbook, pp. 69-71):
/// four random distinct diatonic triads — all seven degrees incl. vii° — voiced with the root
/// in the bass, in a random key each drill. DO is played as a reference first (as the book
/// prescribes), then the user taps the four chords in order from roman-numeral buttons; the
/// drill scores when all four are entered (all-correct = correct, like the book's written answers).
/// </summary>
public partial class L2C5ProgressionsQuizPage : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly List<Button> _chordButtons = new();
    private readonly List<int> _entry = new();

    private (IReadOnlyList<int> Degrees, IReadOnlyList<int> Roots, IReadOnlyList<IReadOnlyList<int>> Chords) _drill;
    private int _do;
    private bool _answered;

    public L2C5ProgressionsQuizPage()
    {
        InitializeComponent();
        BuildChordButtons();
        Automation.Target = this;
        NewDrill();
    }

    private void BuildChordButtons()
    {
        for (int degree = 1; degree <= 7; degree++)
        {
            int d = degree;
            var button = new Button { Text = L2Explorer.RomanLabels[d - 1], WidthRequest = 76, Margin = new Thickness(4) };
            button.Style = (Style)Application.Current!.Resources["AnswerButton"];
            button.Clicked += (_, _) => OnChordTapped(d);
            _chordButtons.Add(button);
            AnswersLayout.Children.Add(button);
        }
    }

    private void NewDrill()
    {
        _drill = L2Explorer.DiatonicQuizWalk(TonicFirstSwitch.IsToggled, _rng);
        _do = Tonic.RandomDo(_rng);
        _entry.Clear();
        _answered = false;
        EntryLabel.Text = "Your answer: —";
        StatusLabel.Text = string.Empty;
        foreach (var b in _chordButtons) b.IsEnabled = true;
    }

    private string AnswerText => string.Join(" – ", _drill.Degrees.Select(d => L2Explorer.RomanLabels[d - 1]));

    private void OnChordTapped(int degree)
    {
        if (_answered || _entry.Count >= 4) return;
        FlashTapped(_chordButtons[degree - 1]);
        _entry.Add(degree);
        EntryLabel.Text = "Your answer: " + string.Join(" – ", _entry.Select(d => L2Explorer.RomanLabels[d - 1]));
        if (_entry.Count < 4) return;

        _answered = true;
        bool correct = _entry.SequenceEqual(_drill.Degrees);
        Gauge.Record(correct);
        StatusLabel.Text = correct ? "Correct!" : $"Not Quite — {AnswerText}";
    }

    /// <summary>Brief press highlight so each tap visibly registers (Mark's feedback).
    /// Direct set + ClearValue — the same restore idiom as the answer-reveal colors;
    /// a SetAppThemeColor binding is NOT reliably removed by ClearValue on Android.</summary>
    private static async void FlashTapped(Button button)
    {
        button.BackgroundColor = Color.FromArgb("#512BD4");
        button.TextColor = Colors.White;
        await Task.Delay(180);
        button.ClearValue(Button.BackgroundColorProperty);
        button.ClearValue(Button.TextColorProperty);
    }

    private void OnClear(object? sender, EventArgs e)
    {
        if (_answered) return;
        _entry.Clear();
        EntryLabel.Text = "Your answer: —";
    }

    private async void OnPlay(object? sender, EventArgs e)
    {
        try { await PlayDrillAsync(); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    // DO reference first (per the book), then the four chords: bass root + voiced upper triad.
    private async Task PlayDrillAsync()
    {
        var steps = new List<(IReadOnlyList<byte[]> chord, double seconds)>
        {
            (new[] { await _samples.LoadAsync(Note.SampleFile(_do)) }, 2.0),
        };
        for (int i = 0; i < 4; i++)
        {
            var samples = new List<byte[]>
            {
                await _samples.LoadAsync(Note.SampleFile(Voicing.BassNoteNumber(_do + _drill.Roots[i]))),
            };
            foreach (int offset in _drill.Chords[i])
                samples.Add(await _samples.LoadAsync(Note.SampleFile(_do + offset)));
            steps.Add((samples, i == 3 ? 4.0 : 2.0));
        }
        _audio.Play(AudioRenderer.RenderProgression(steps));
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    private void OnTonicToggled(object? sender, ToggledEventArgs e) => NewDrill();

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        _ = PlayDrillAsync();
        return 2.0 + 2.0 * 3 + 4.0;   // DO reference + three 2s chords + the 4s final chord
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + AnswerText;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
