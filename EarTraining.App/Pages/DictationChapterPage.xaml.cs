using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

/// <summary>
/// Shared melodic-dictation page for L1 chapters 4-7 (book coverage Tier 1): the interval
/// pool is fixed per chapter (cumulative, via <see cref="IntervalDictationDrill.NextChapter"/>)
/// and the Rhythms picker offers the styles the chapter has reached — eighth pairs everywhere,
/// the dotted quarter-eighth figure from C6, anticipations from C7. One thin subclass per
/// chapter supplies the pool/heading/book page (Shell templates need parameterless types).
/// </summary>
public partial class DictationChapterPage : ContentPage
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly NotationRenderer _notation = new();
    private readonly Random _rng = new();

    private readonly L1DictationChapter _chapter;
    private IntervalDictationDrill _drill = null!;

    protected DictationChapterPage(
        L1DictationChapter chapter, string heading, int bookPage, DictationRhythmStyle maxStyle)
    {
        InitializeComponent();
        _chapter = chapter;
        HeadingLabel.Text = heading;
        BookNote.Text = $"Selected from the melodic dictation / transcription questions on p. {bookPage}.";
        BuildPickers(maxStyle);
        KeyPicker.SelectedIndexChanged += OnSettingChanged;
        BpmPicker.SelectedIndexChanged += OnSettingChanged;
        MeasuresPicker.SelectedIndexChanged += OnSettingChanged;
        RhythmPicker.SelectedIndexChanged += OnSettingChanged;
        NotationWeb.Navigated += OnNotationNavigated;
        // Pre-warm the WebView while hidden so the first Reveal isn't a cold white flash (esp. Android).
        NotationWeb.Source = new HtmlWebViewSource { Html = "<!doctype html><html><body style=\"margin:0;background:#fff\"></body></html>" };
        NewDrill();
    }

    private void BuildPickers(DictationRhythmStyle maxStyle)
    {
        KeyPicker.ItemsSource = Keys.All.ToList();
        new PracticeKeyDefault(this, KeyPicker);   // Settings practice key, else C

        var bpms = new List<string>();
        for (int b = 50; b <= 100; b += 5) bpms.Add(b.ToString());
        BpmPicker.ItemsSource = bpms;
        BpmPicker.SelectedIndex = bpms.IndexOf("60");

        MeasuresPicker.ItemsSource = new List<string> { "2", "4" };
        MeasuresPicker.SelectedIndex = 0;

        // Item index == DictationRhythmStyle value, so the mapping is a cast.
        var styles = new List<string> { "Quarters & halves", "Eighth notes" };
        if (maxStyle >= DictationRhythmStyle.Dotted) styles.Add("Dotted quarter–eighth");
        if (maxStyle >= DictationRhythmStyle.Anticipations) styles.Add("Anticipations");
        RhythmPicker.ItemsSource = styles;
        RhythmPicker.SelectedIndex = 0;
    }

    private string Key => (string)KeyPicker.SelectedItem;
    private double Bpm => double.Parse((string)BpmPicker.SelectedItem);
    private int Measures => int.Parse((string)MeasuresPicker.SelectedItem);
    private DictationRhythmStyle Style => (DictationRhythmStyle)RhythmPicker.SelectedIndex;

    private void NewDrill()
    {
        _drill = IntervalDictationDrill.NextChapter(_chapter, Key, Bpm, Measures, Style, _rng);
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

/// <summary>L1C4 dictation: 3rds &amp; 6ths (Ma3/Mi6/Mi3/Ma6) + C1 resolutions; rhythms through eighth pairs.</summary>
public sealed class DictationL1C4Page : DictationChapterPage
{
    public DictationL1C4Page()
        : base(L1DictationChapter.C4, "Melodic Dictation — 3rds & 6ths", 64, DictationRhythmStyle.Eighths) { }
}

/// <summary>L1C5 dictation: adds 4ths &amp; 5ths (incl. the FA-TI / TI-FA tritones).</summary>
public sealed class DictationL1C5Page : DictationChapterPage
{
    public DictationL1C5Page()
        : base(L1DictationChapter.C5, "Melodic Dictation — adds 4ths & 5ths", 98, DictationRhythmStyle.Eighths) { }
}

/// <summary>L1C6 dictation: adds Maj 2nd / Min 7th and the dotted quarter-eighth figure.</summary>
public sealed class DictationL1C6Page : DictationChapterPage
{
    public DictationL1C6Page()
        : base(L1DictationChapter.C6, "Melodic Dictation — adds 2nds & 7ths", 131, DictationRhythmStyle.Dotted) { }
}

/// <summary>L1C7 dictation: all diatonic intervals, with eighth-note anticipations.</summary>
public sealed class DictationL1C7Page : DictationChapterPage
{
    public DictationL1C7Page()
        : base(L1DictationChapter.C7, "Melodic Dictation — all diatonic intervals", 162, DictationRhythmStyle.Anticipations) { }
}

/// <summary>L1C8 review dictation: the full C7 pool + every rhythm style (book p. 182).</summary>
public sealed class DictationL1C8Page : DictationChapterPage
{
    public DictationL1C8Page()
        : base(L1DictationChapter.C7, "Melodic Dictation — review", 182, DictationRhythmStyle.Anticipations) { }
}
