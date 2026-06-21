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

    /// <summary>
    /// Notes played together (overlaid), with headroom to avoid clipping. Each
    /// successive (higher) voice enters <paramref name="staggerSeconds"/> later — a
    /// slight upward roll. A small onset asynchrony is the ear's strongest cue for
    /// hearing two pitches instead of one fused tone (the octave is the worst case:
    /// the upper fundamental lands on the lower note's 2nd harmonic). Pass 0 for a
    /// dead-simultaneous block chord.
    /// </summary>
    public static byte[] RenderHarmonic(IReadOnlyList<byte[]> noteWavs, double seconds = 1.2, double gain = 0.6, double staggerSeconds = 0.05)
    {
        var notes = noteWavs.Select(b => Slice(WavBuffer.Read(b), seconds)).ToArray();
        return Mix(gain, staggerSeconds, notes).Write();
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

    private static WavBuffer Mix(double gain, double staggerSeconds, IReadOnlyList<WavBuffer> voices)
    {
        var first = voices[0];
        // Per-voice delay in interleaved samples; computed in whole frames then
        // x channels so L/R stay aligned. Voice v starts at v * stagger.
        int stagger = (int)(staggerSeconds * first.SampleRate) * first.Channels;
        int len = 0;
        for (int v = 0; v < voices.Count; v++)
            len = Math.Max(len, v * stagger + voices[v].Samples.Length);

        var acc = new double[len];
        for (int v = 0; v < voices.Count; v++)
        {
            int offset = v * stagger;
            var samples = voices[v].Samples;
            for (int i = 0; i < samples.Length; i++)
                acc[offset + i] += samples[i] * gain;
        }

        var s = new short[len];
        for (int i = 0; i < len; i++)
            s[i] = (short)Math.Clamp(acc[i], short.MinValue, short.MaxValue);
        return new WavBuffer { SampleRate = first.SampleRate, Channels = first.Channels, Samples = s };
    }
}
