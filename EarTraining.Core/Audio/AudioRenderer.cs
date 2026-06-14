namespace EarTraining.Core.Audio;

/// <summary>
/// Portable, dependency-free drill audio. Given the raw bytes of the piano note
/// samples, slice each to a duration then SEQUENCE them (melodic) or OVERLAY/mix
/// them (harmonic), returning a finished WAV as bytes. Same logic proven in the
/// AudioPortabilitySpike — no NAudio, no NAudio.Lame, no System.Web, no native
/// code, so it runs identically on the server, desktop, Android, and iOS.
/// </summary>
public static class AudioRenderer
{
    /// <summary>Notes played one after another.</summary>
    public static byte[] RenderMelodic(IReadOnlyList<byte[]> noteWavs, double secondsPerNote = 1.2)
    {
        var notes = noteWavs.Select(b => Slice(WavBuffer.Read(b), secondsPerNote)).ToArray();
        return Concat(notes).Write();
    }

    /// <summary>Notes played together (overlaid), with headroom to avoid clipping.</summary>
    public static byte[] RenderHarmonic(IReadOnlyList<byte[]> noteWavs, double seconds = 1.2, double gain = 0.7)
    {
        var notes = noteWavs.Select(b => Slice(WavBuffer.Read(b), seconds)).ToArray();
        return Mix(gain, notes).Write();
    }

    private static WavBuffer Slice(WavBuffer w, double seconds)
    {
        int count = Math.Min(w.Samples.Length, (int)(seconds * w.SampleRate) * w.Channels);
        var s = new short[count];
        Array.Copy(w.Samples, s, count);
        return new WavBuffer { SampleRate = w.SampleRate, Channels = w.Channels, Samples = s };
    }

    private static WavBuffer Concat(IReadOnlyList<WavBuffer> parts)
    {
        var first = parts[0];
        var s = new short[parts.Sum(p => p.Samples.Length)];
        int o = 0;
        foreach (var p in parts) { Array.Copy(p.Samples, 0, s, o, p.Samples.Length); o += p.Samples.Length; }
        return new WavBuffer { SampleRate = first.SampleRate, Channels = first.Channels, Samples = s };
    }

    private static WavBuffer Mix(double gain, IReadOnlyList<WavBuffer> voices)
    {
        var first = voices[0];
        int len = voices.Max(v => v.Samples.Length);
        var s = new short[len];
        for (int i = 0; i < len; i++)
        {
            double sum = 0;
            foreach (var v in voices)
                if (i < v.Samples.Length) sum += v.Samples[i] * gain;
            s[i] = (short)Math.Clamp(sum, short.MinValue, short.MaxValue);
        }
        return new WavBuffer { SampleRate = first.SampleRate, Channels = first.Channels, Samples = s };
    }
}
