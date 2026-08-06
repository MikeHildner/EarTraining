using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class MelodyHarmonizationPage : ContentPage, IAutomatableDrill
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<string, Button> _answerButtons = new(); // prompt label -> button
    private readonly IReadOnlyList<MelodyHarmonizationDrill> _all = MelodyHarmonizationDrill.All;
    private MelodyHarmonizationDrill _drill = null!;
    private bool _answered;

    public MelodyHarmonizationPage()
    {
        InitializeComponent();
        DoHeader.DoChanged += (_, _) => NewDrill();
        Includes.Changed += (_, _) => { BuildAnswers(); NewDrill(); };
        Includes.Play = PlayKey;
        Automation.Target = this;
        Includes.Build(_all.Select(d => (d.Key, d.Label)));
        BuildAnswers();
        NewDrill();
    }

    private List<MelodyHarmonizationDrill> Playable()
    {
        var included = Includes.Included.ToHashSet();
        return _all.Where(d => included.Contains(d.Key)).ToList();
    }

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        // One button per included (note, triad) prompt — the answers mirror the include
        // list (Mark: identify the melody note AND the harmonizing triad together).
        foreach (var drill in Playable())
        {
            string label = drill.Label;
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

    // Plays one specific harmonization on demand from the Include list's ▶.
    private async void PlayKey(string key)
    {
        try { await PlayDrillAsync(_all.First(d => d.Key == key)); }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    private async Task PlayDrillAsync(MelodyHarmonizationDrill drill)
    {
        // Four voices: the root in the bass, then the three triad tones (melody note on top).
        var samples = new List<byte[]>
        {
            await _samples.LoadAsync(Note.SampleFile(Voicing.BassNoteNumber(DoHeader.Do + drill.BassRootOffset))),
        };
        foreach (var offset in drill.ToneOffsets)
            samples.Add(await _samples.LoadAsync(Note.SampleFile(DoHeader.Do + offset)));
        _audio.Play(AudioRenderer.RenderHarmonic(samples, seconds: 3.0));
    }

    private void OnAnswer(string guess)
    {
        if (_answered) return;
        _answered = true;
        bool correct = guess == _drill.Label;
        Gauge.Record(correct);
        foreach (var (name, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (name == _drill.Label) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
            else if (name == guess) { button.BackgroundColor = Colors.IndianRed; button.TextColor = Colors.White; }
        }
        StatusLabel.Text = correct ? "Correct!" : $"Not Quite — {_drill.Label}";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();

    // ── Automation (IAutomatableDrill) ──
    public double AutoPlay()
    {
        NewDrill();
        _ = PlayDrillAsync(_drill);
        return 3.0;
    }

    public void AutoReveal(bool scored)
    {
        if (_answered) return;
        _answered = true;
        if (scored) Gauge.Record(false);
        foreach (var (name, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (name == _drill.Label) { button.BackgroundColor = Colors.SeaGreen; button.TextColor = Colors.White; }
        }
        StatusLabel.Text = (scored ? "Time's up — " : "Answer: ") + _drill.Label;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Automation.Stop();
    }
}
