using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class DictationPage : ContentPage
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly NotationRenderer _notation = new();
    private readonly Random _rng = new();

    private static readonly (string Label, int Value)[] ResolutionOptions =
        [("Resolutions", 1), ("Reverse Resolutions", 2), ("Both", 3)];

    private DictationDrill _drill = null!;

    public DictationPage()
    {
        InitializeComponent();
        BuildPickers();
        NewDrill();
    }

    private void BuildPickers()
    {
        ResolutionPicker.ItemsSource = ResolutionOptions.Select(o => o.Label).ToList();
        ResolutionPicker.SelectedIndex = 0;

        KeyPicker.ItemsSource = Keys.All.ToList();
        KeyPicker.SelectedIndex = 0; // C

        var bpms = new List<string>();
        for (int b = 50; b <= 100; b += 5) bpms.Add(b.ToString());
        BpmPicker.ItemsSource = bpms;
        BpmPicker.SelectedIndex = bpms.IndexOf("60");

        MeasuresPicker.ItemsSource = new List<string> { "2", "4" };
        MeasuresPicker.SelectedIndex = 0;
    }

    private int ResolutionType => ResolutionOptions[Math.Max(0, ResolutionPicker.SelectedIndex)].Value;
    private string Key => (string)KeyPicker.SelectedItem;
    private double Bpm => double.Parse((string)BpmPicker.SelectedItem);
    private int Measures => int.Parse((string)MeasuresPicker.SelectedItem);

    private void NewDrill()
    {
        _drill = DictationDrill.Next(ResolutionType, Key, Bpm, Measures, _rng);
        NotationWeb.IsVisible = false;
        RevealButton.Text = "Reveal transcription";
        StatusLabel.Text = $"New dictation in {_drill.Key} at {_drill.Bpm:0} bpm — press Play.";
    }

    private void OnNewDictation(object? sender, EventArgs e) => NewDrill();

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
            NotationWeb.HeightRequest = _drill.Measures.Count * 200 + 40;
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
}
