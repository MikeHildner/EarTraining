using EarTraining.App.Components;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

/// <summary>
/// Shared bass-line dictation page for L1 chapters 5-7 (book coverage Tier 1): melodies in
/// the bass register whose strong beats carry chord tones of an implied diatonic triad
/// (book Ch. 5 §1.15-1.21), revealed on a bass clef. The triad pool and rhythm styles grow
/// with the chapter; one thin subclass per chapter supplies them (Shell templates need
/// parameterless types). Same play/reveal flow as the melodic dictation pages.
/// </summary>
public partial class BassLineDictationPage : ContentPage
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly NotationRenderer _notation = new();
    private readonly Random _rng = new();

    private readonly L1DictationChapter _chapter;
    private BassLineDictationDrill _drill = null!;

    protected BassLineDictationPage(
        L1DictationChapter chapter, string heading, int bookPage, DictationRhythmStyle maxStyle)
    {
        InitializeComponent();
        _chapter = chapter;
        HeadingLabel.Text = heading;
        BookNote.Text = $"Selected from the bass line dictation / transcription questions on p. {bookPage}.";
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
        var styles = new List<string> { "Halves & quarters" };
        if (maxStyle >= DictationRhythmStyle.Eighths) styles.Add("Eighth notes");
        if (maxStyle >= DictationRhythmStyle.Dotted) styles.Add("Dotted quarter–eighth");
        if (maxStyle >= DictationRhythmStyle.Anticipations) styles.Add("Anticipations");
        RhythmPicker.ItemsSource = styles;
        RhythmPicker.SelectedIndex = 0;
        RhythmPicker.IsEnabled = styles.Count > 1;
    }

    private string Key => (string)KeyPicker.SelectedItem;
    private double Bpm => double.Parse((string)BpmPicker.SelectedItem);
    private int Measures => int.Parse((string)MeasuresPicker.SelectedItem);
    private DictationRhythmStyle Style => (DictationRhythmStyle)RhythmPicker.SelectedIndex;

    private void NewDrill()
    {
        _drill = BassLineDictationDrill.Next(_chapter, Key, Bpm, Measures, Style, _rng);
        NotationWeb.IsVisible = false;
        RevealButton.Text = "Reveal transcription";
        StatusLabel.Text = $"New bass line in {_drill.Key} at {_drill.Bpm:0} bpm — press Play.";
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

/// <summary>L1C5 bass lines: I/IV/V/VI implied triads, halves &amp; quarters (book p. 102).</summary>
public sealed class BassLineL1C5Page : BassLineDictationPage
{
    public BassLineL1C5Page()
        : base(L1DictationChapter.C5, "Bass Line Dictation — halves & quarters", 102, DictationRhythmStyle.Basic) { Title = "L1 C5 · Bass Line Dictation"; }
}

/// <summary>L1C6 bass lines: adds the III triad, eighth pairs, and the dotted figure (book p. 135).</summary>
public sealed class BassLineL1C6Page : BassLineDictationPage
{
    public BassLineL1C6Page()
        : base(L1DictationChapter.C6, "Bass Line Dictation — adds eighths & dotted", 135, DictationRhythmStyle.Dotted) { Title = "L1 C6 · Bass Line Dictation"; }
}

/// <summary>L1C7 bass lines: all six diatonic triads, with anticipations (book p. 165).</summary>
public sealed class BassLineL1C7Page : BassLineDictationPage
{
    public BassLineL1C7Page()
        : base(L1DictationChapter.C7, "Bass Line Dictation — adds anticipations", 165, DictationRhythmStyle.Anticipations) { Title = "L1 C7 · Bass Line Dictation"; }
}

/// <summary>L1C8 review bass lines: all six triads + every rhythm style (book p. 185).</summary>
public sealed class BassLineL1C8Page : BassLineDictationPage
{
    public BassLineL1C8Page()
        : base(L1DictationChapter.C7, "Bass Line Dictation — review", 185, DictationRhythmStyle.Anticipations) { Title = "L1 C8 · Bass Line Dictation"; }
}
