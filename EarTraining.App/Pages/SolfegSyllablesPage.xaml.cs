using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

/// <summary>
/// "All Solfeg Syllables" reference (ported from the web Solfeg/Index + AllPitches): the full
/// chromatic set of solfège syllables from DO to the octave DO, each tappable to hear it relative
/// to the current DO. No scoring — a reference/explorer.
/// </summary>
public partial class SolfegSyllablesPage : ContentPage
{
    // Offset from DO → syllable name (chromatic, ascending; chromatic tones show both spellings).
    private static readonly (int Offset, string Name)[] Syllables =
    [
        (0, "DO"), (1, "DI / RA"), (2, "RE"), (3, "RI / ME"), (4, "MI"), (5, "FA"),
        (6, "FI / SE"), (7, "SO"), (8, "SI / LE"), (9, "LA"), (10, "LI / TE"), (11, "TI"), (12, "DO (8ve)"),
    ];

    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);

    public SolfegSyllablesPage()
    {
        InitializeComponent();
        foreach (var (offset, name) in Syllables)
        {
            var button = new Button { Text = name, Margin = new Thickness(4) };
            button.Style = (Style)Application.Current!.Resources["AnswerButton"];
            int off = offset;
            button.Clicked += async (_, _) => await PlayAsync(off);
            SyllablesLayout.Children.Add(button);
        }
    }

    private async Task PlayAsync(int offset)
    {
        try
        {
            var sample = await _samples.LoadAsync(Note.SampleFile(DoHeader.Do + offset));
            _audio.Play(AudioRenderer.RenderSequence(new[] { (sample, 2.0) }));
        }
        catch { /* ignore playback errors */ }
    }
}
