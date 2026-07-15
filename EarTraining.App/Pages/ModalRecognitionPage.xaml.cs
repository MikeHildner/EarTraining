using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

/// <summary>
/// L2C6 modal scale recognition (book Ch. 6, workbook pp. 72-73): Middle C plays as the DO
/// reference, then an ascending modal scale starting on C or C#; identify the mode from
/// seven buttons (the C# pool is the book's five). The reveal names the mode and its
/// implied relative major, matching the book's required answer format.
/// </summary>
public partial class ModalRecognitionPage : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new(); // mode -> button

    private ModalScaleDrill _drill = null!;
    private bool _answered;
    private bool _ready;

    private const double NoteSeconds = 0.6;
    private const double ReferenceSeconds = 2.0;

    public ModalRecognitionPage()
    {
        InitializeComponent();
        Automation.Target = this;
        Rebuild();
        _ready = true;
    }

    private IReadOnlyList<ModalScaleDrill> Pool =>
        CsRadio.IsChecked ? ModalScaleDrill.StartOnCs
        : BothRadio.IsChecked ? ModalScaleDrill.StartOnC.Concat(ModalScaleDrill.StartOnCs).ToList()
        : ModalScaleDrill.StartOnC;

    private void Rebuild()
    {
        // One button per mode available in the current pool, in scale-degree order.
        var available = Pool.Select(d => d.Mode).ToHashSet();
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        foreach (string mode in ModalScaleDrill.ModeNames)
        {
            if (!available.Contains(mode)) continue;
            string m = mode;
            var button = new Button { Text = m, Margin = new Thickness(4) };
            button.Style = (Style)Application.Current!.Resources["AnswerButton"];
            button.Clicked += (_, _) => OnAnswer(m);
            _answerButtons[m] = button;
            AnswersLayout.Children.Add(button);
        }
        NewDrill();
    }

    private void NewDrill()
    {
        var pool = Pool;
        _drill = pool[_rng.Next(pool.Count)];
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

    // Middle C reference (as the book prescribes), a breath, then the ascending scale.
    private async Task PlayDrillAsync()
    {
        var notes = new List<(byte[] sample, double seconds)>
        {
            (await _samples.LoadAsync(Note.SampleFile(ModalScaleDrill.ReferenceNote)), ReferenceSeconds),
        };
        for (int i = 0; i < _drill.Offsets.Count; i++)
            notes.Add((await _samples.LoadAsync(Note.SampleFile(_drill.StartNote + _drill.Offsets[i])),
                       i == _drill.Offsets.Count - 1 ? 1.5 : NoteSeconds));
        _audio.Play(AudioRenderer.RenderSequence(notes));
    }

    private void OnAnswer(string guess)
    {
        if (_answered) return;
        _answered = true;
        bool correct = guess == _drill.Mode;
        Gauge.Record(correct);
        foreach (var (mode, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (mode == _drill.Mode) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
            else if (mode == guess) { b.BackgroundColor = Colors.IndianRed; b.TextColor = Colors.White; }
        }
        StatusLabel.Text = correct ? $"Correct! {_drill.RevealText}" : $"Not Quite — {_drill.RevealText}";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    private void OnStartChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (!_ready || !e.Value) return;
        Rebuild();
        Gauge.Reset();   // pools differ, so tallies aren't comparable
    }

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        _ = PlayDrillAsync();
        return ReferenceSeconds + (_drill.Offsets.Count - 1) * NoteSeconds + 1.5;
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        foreach (var (mode, b) in _answerButtons)
        {
            b.IsEnabled = false;
            if (mode == _drill.Mode) { b.BackgroundColor = Colors.SeaGreen; b.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + _drill.RevealText;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
