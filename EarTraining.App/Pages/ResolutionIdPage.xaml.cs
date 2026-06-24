using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Drills;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class ResolutionIdPage : ContentPage
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();
    private readonly Dictionary<ResolutionType, Button> _answerButtons = new();
    private ResolutionDrill _drill = null!;
    private bool _answered;

    public ResolutionIdPage()
    {
        InitializeComponent();
        DoHeader.DoChanged += (_, _) => NewDrill();
        Includes.Changed += (_, _) => { BuildAnswers(); NewDrill(); };
        Includes.Build(ResolutionDrill.All.Select(t => (t.ToString(), ResolutionDrill.ShortLabelOf(t))));
        BuildAnswers();
        NewDrill();
    }

    private IReadOnlyList<ResolutionType> IncludedTypes =>
        Includes.Included.Select(Enum.Parse<ResolutionType>).ToList();

    private void BuildAnswers()
    {
        AnswersLayout.Children.Clear();
        _answerButtons.Clear();
        foreach (var type in IncludedTypes)
        {
            var t = type;
            var button = new Button { Text = ResolutionDrill.ShortLabelOf(t), Margin = new Thickness(4) };
            button.Clicked += (_, _) => OnAnswer(t);
            _answerButtons[t] = button;
            AnswersLayout.Children.Add(button);
        }
    }

    private void NewDrill()
    {
        _drill = ResolutionDrill.Next(DoHeader.Do, IncludedTypes, _rng);
        _answered = false;
        StatusLabel.Text = string.Empty;
        foreach (var button in _answerButtons.Values)
        {
            button.IsEnabled = true;
            button.ClearValue(Button.BackgroundColorProperty);
        }
    }

    private async void OnPlay(object? sender, EventArgs e)
    {
        try
        {
            var notes = new List<(byte[] sample, double seconds)>();
            foreach (var (note, seconds) in _drill.ResolutionOnly)
                notes.Add((await _samples.LoadAsync(Note.SampleFile(note)), seconds));
            _audio.Play(AudioRenderer.RenderSequence(notes));
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "Audio error: " + ex.Message;
        }
    }

    private void OnAnswer(ResolutionType guess)
    {
        if (_answered) return;
        _answered = true;
        bool correct = guess == _drill.Type;
        Gauge.Record(correct);

        foreach (var (type, button) in _answerButtons)
        {
            button.IsEnabled = false;
            if (type == _drill.Type) button.BackgroundColor = Colors.SeaGreen;
            else if (type == guess) button.BackgroundColor = Colors.IndianRed;
        }

        StatusLabel.Text = correct ? "Correct!" : $"Nope — {_drill.ShortLabel}";
    }

    private void OnNext(object? sender, EventArgs e) => NewDrill();
}
