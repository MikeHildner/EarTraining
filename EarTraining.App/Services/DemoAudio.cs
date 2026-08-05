using EarTraining.Core.Audio;
using EarTraining.Core.Theory;

namespace EarTraining.App.Services;

/// <summary>
/// Plays a fixed teaching example on the reference pages: absolute note numbers in, one
/// rendered progression out. Unlike the drill pages there's no randomness here — a demo
/// always sounds the same, so the reader can connect the prose to the sound.
/// </summary>
public static class DemoAudio
{
    public static async Task PlayAsync(SampleLibrary samples, DrillAudioPlayer audio,
        IReadOnlyList<(IReadOnlyList<int> Notes, double Seconds)> steps)
    {
        var rendered = new List<(IReadOnlyList<byte[]> chord, double seconds)>();
        foreach (var (notes, seconds) in steps)
        {
            var buffers = new List<byte[]>();
            foreach (int n in notes)
                buffers.Add(await samples.LoadAsync(Note.SampleFile(n)));
            rendered.Add((buffers, seconds));
        }
        audio.Play(AudioRenderer.RenderProgression(rendered));
    }
}
