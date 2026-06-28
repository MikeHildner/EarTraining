using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class DictationL1C2Page : ContentPage
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly NotationRenderer _notation = new();
    private readonly Random _rng = new();

    private IntervalDictationDrill _drill = null!;

    public DictationL1C2Page()
    {
        InitializeComponent();
        BuildPickers();
        Toggle.Configure(IntervalQuality.Major3rd, IntervalQuality.Minor6th);
        // Wire change handlers after the initial selections are set, so a settings change
        // regenerates the dictation immediately without firing mid-setup.
        Toggle.Changed += OnSettingChanged;
        KeyPicker.SelectedIndexChanged += OnSettingChanged;
        BpmPicker.SelectedIndexChanged += OnSettingChanged;
        MeasuresPicker.SelectedIndexChanged += OnSettingChanged;
        RhythmPicker.SelectedIndexChanged += OnSettingChanged;
        NotationWeb.Navigated += OnNotationNavigated;
        NewDrill();
    }

    private void BuildPickers()
    {
        KeyPicker.ItemsSource = Keys.All.ToList();
        KeyPicker.SelectedIndex = 0; // C

        var bpms = new List<string>();
        for (int b = 50; b <= 100; b += 5) bpms.Add(b.ToString());
        BpmPicker.ItemsSource = bpms;
        BpmPicker.SelectedIndex = bpms.IndexOf("60");

        MeasuresPicker.ItemsSource = new List<string> { "2", "4" };
        MeasuresPicker.SelectedIndex = 0;

        RhythmPicker.ItemsSource = new List<string> { "Quarter", "Eighth" };
        RhythmPicker.SelectedIndex = 0;
    }

    private L1C2DictationInterval Interval => Toggle.Quality switch
    {
        IntervalQuality.Major3rd => L1C2DictationInterval.Major3rd,
        IntervalQuality.Minor6th => L1C2DictationInterval.Minor6th,
        _ => L1C2DictationInterval.Both,
    };
    private string Key => (string)KeyPicker.SelectedItem;
    private double Bpm => double.Parse((string)BpmPicker.SelectedItem);
    private int Measures => int.Parse((string)MeasuresPicker.SelectedItem);
    private bool IncludeEighths => RhythmPicker.SelectedIndex == 1;

    private void NewDrill()
    {
        _drill = IntervalDictationDrill.NextC2(Interval, Key, Bpm, Measures, IncludeEighths, _rng);
        NotationWeb.IsVisible = false;
        RevealButton.Text = "Reveal transcription";
        StatusLabel.Text = $"New dictation in {_drill.Key} at {_drill.Bpm:0} bpm — press Play.";
    }

    private void OnNewDictation(object? sender, EventArgs e) => NewDrill();

    private void OnSettingChanged(object? sender, EventArgs e) => NewDrill();

    private async void OnPlay(object? sender, EventArgs e)
    {
        try
        {
            StatusLabel.Text = "Loading…";
            var doSample = await _samples.LoadAsync(Note.SampleFile(_drill.DoNoteNumber));
            var tick = await _samples.LoadAsync("Woodblock.wav");

            var melody = new List<(byte[] sample, double seconds)>();
            foreach (var measure in _drill.Measures)
                for (int i = 0; i < measure.NoteNumbers.Count; i++)
                    melody.Add((await _samples.LoadAsync(Note.SampleFile(measure.NoteNumbers[i])),
                                Rhythm.Seconds(measure.Rhythms[i], _drill.Bpm)));

            var wav = AudioRenderer.RenderDictation(doSample, melody, tick, _drill.Bpm);
            _audio.Play(wav);
            StatusLabel.Text = $"Playing — {_drill.Key}, {_drill.Bpm:0} bpm.";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "Audio error: " + ex.Message;
        }
    }

    private async void OnReveal(object? sender, EventArgs e)
    {
        if (NotationWeb.IsVisible)
        {
            NotationWeb.IsVisible = false;
            RevealButton.Text = "Reveal transcription";
            return;
        }

        try
        {
            StatusLabel.Text = "Rendering notation…";
            string html = await _notation.BuildHtmlAsync(_drill);
            NotationWeb.HeightRequest = _drill.Measures.Count * 160 + 30; // generous upper bound; trimmed after render
            NotationWeb.Source = new HtmlWebViewSource { Html = html };
            NotationWeb.IsVisible = true;
            RevealButton.Text = "Hide transcription";
            StatusLabel.Text = string.Empty;
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "Notation error: " + ex.Message;
        }
    }

    // Once the notation has rendered, shrink the WebView to the exact content height.
    private async void OnNotationNavigated(object? sender, WebNavigatedEventArgs e)
    {
        try
        {
            var result = await NotationWeb.EvaluateJavaScriptAsync("document.body.scrollHeight");
            if (int.TryParse(result, out int px) && px > 0)
                NotationWeb.HeightRequest = px + 8;
        }
        catch { /* keep the estimate */ }
    }
}
