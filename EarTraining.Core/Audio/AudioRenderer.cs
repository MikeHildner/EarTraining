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

    /// <summary>
    /// A melodic-dictation phrase: a DO whole-note lead-in, four count-in ticks, then
    /// the melody (each note held for its rhythmic duration), with a metronome click
    /// on every melody beat mixed underneath. Each note is fit to its exact duration
    /// (zero-padded if the sample is shorter than the beat), so timing stays correct
    /// regardless of sample length. Mirrors L1C1Controller.AudioAndDictation.
    /// </summary>
    public static byte[] RenderDictation(
        byte[] doSample,
        IReadOnlyList<(byte[] sample, double seconds)> melody,
        byte[] tick,
        double bpm,
        double gain = 0.7)
    {
        var doBuf = WavBuffer.Read(doSample);
        var tickBuf = WavBuffer.Read(tick);
        int sampleRate = doBuf.SampleRate, channels = doBuf.Channels;
        double quarter = 60.0 / bpm;

        // Audible voice: DO whole note, 4 count-in ticks, then the melody notes.
        var melodyParts = new List<WavBuffer> { Fit(doBuf, quarter * 4) };
        for (int i = 0; i < 4; i++) melodyParts.Add(Fit(tickBuf, quarter));
        foreach (var (sample, seconds) in melody)
            melodyParts.Add(Fit(WavBuffer.Read(sample), seconds));
        var melodyVoice = Concat(melodyParts);

        // Metronome voice: silent through the DO + count-in (8 beats), then a click on
        // each melody beat, scaled to sit under the melody.
        int melodyBeats = (int)Math.Round(melody.Sum(m => m.seconds) / quarter);
        var metroParts = new List<WavBuffer> { Silence(quarter * 8, sampleRate, channels) };
        for (int i = 0; i < melodyBeats; i++) metroParts.Add(Fit(tickBuf, quarter));
        var metroVoice = Gain(Concat(metroParts), 0.5);

        return Mix(gain, 0, [melodyVoice, metroVoice]).Write();
    }

    /// <summary>A bare melodic sequence: each note fit to its exact duration, then
    /// concatenated. Used by the resolution drills and the single-note pitch drill.</summary>
    public static byte[] RenderSequence(IReadOnlyList<(byte[] sample, double seconds)> notes)
    {
        var parts = notes.Select(n => Fit(WavBuffer.Read(n.sample), n.seconds)).ToList();
        return Concat(parts).Write();
    }

    /// <summary>
    /// A chord progression: each step is a chord (its note samples mixed as a block chord and
    /// fit to the step's duration), played one after another. Used by the L1C4 triad progressions.
    /// When <paramref name="topGain"/> ≠ 1, the LAST sample of each multi-note chord is treated
    /// as the top/melody voice and scaled by it — so a doubled melody line can sit above the
    /// pad (the L2C8 7-3 lines, per Mark's feedback that the melody didn't cut through).
    /// </summary>
    public static byte[] RenderProgression(IReadOnlyList<(IReadOnlyList<byte[]> chord, double seconds)> steps, double gain = 0.5, double topGain = 1.0)
    {
        var parts = steps
            .Select(step =>
            {
                var voices = step.chord.Select(b => Fit(WavBuffer.Read(b), step.seconds)).ToList();
                if (topGain != 1.0 && voices.Count > 1)
                    voices[^1] = Gain(voices[^1], topGain);
                return Mix(gain, 0, voices);
            })
            .ToList();
        return Concat(parts).Write();
    }

    /// <summary>
    /// A seamless metronome click loop: <paramref name="bars"/> bars of
    /// <paramref name="beatsPerBar"/> beats at <paramref name="bpm"/>, beat 1 of each
    /// bar at <paramref name="accentGain"/> and the rest at <paramref name="beatGain"/>.
    /// Every tick is placed at its absolute frame offset (round(i · rate·60/bpm)), so
    /// spacing carries no cumulative rounding drift, and the buffer is exactly
    /// round(totalBeats · framesPerBeat) frames so a looping player restarts on the
    /// grid. The tick is trimmed to at most one beat (500 ms cap) and anything that
    /// would spill past the loop edge wraps to the front — what the next pass would
    /// be playing anyway — keeping the seam clean at any tempo.
    /// </summary>
    public static byte[] RenderMetronome(byte[] tickWav, int bpm, int beatsPerBar, int bars, double accentGain = 1.0, double beatGain = 0.55)
    {
        var tick = WavBuffer.Read(tickWav);
        double secondsPerBeat = 60.0 / bpm;
        var hit = Slice(tick, Math.Min(0.5, secondsPerBeat));

        double framesPerBeat = tick.SampleRate * secondsPerBeat;
        int totalBeats = beatsPerBar * bars;
        int totalFrames = (int)Math.Round(totalBeats * framesPerBeat);
        int channels = tick.Channels;

        var acc = new double[totalFrames * channels];
        for (int i = 0; i < totalBeats; i++)
        {
            double gain = i % beatsPerBar == 0 ? accentGain : beatGain;
            int start = (int)Math.Round(i * framesPerBeat) * channels;
            for (int j = 0; j < hit.Samples.Length; j++)
                acc[(start + j) % acc.Length] += hit.Samples[j] * gain;
        }

        var s = new short[acc.Length];
        for (int i = 0; i < s.Length; i++)
            s[i] = (short)Math.Clamp(acc[i], short.MinValue, short.MaxValue);
        return new WavBuffer { SampleRate = tick.SampleRate, Channels = channels, Samples = s }.Write();
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

    /// <summary>Slice or zero-pad to exactly <paramref name="seconds"/> (whole frames).</summary>
    private static WavBuffer Fit(WavBuffer w, double seconds)
    {
        int target = (int)(seconds * w.SampleRate) * w.Channels;
        var s = new short[target];
        Array.Copy(w.Samples, s, Math.Min(target, w.Samples.Length));
        return new WavBuffer { SampleRate = w.SampleRate, Channels = w.Channels, Samples = s };
    }

    private static WavBuffer Silence(double seconds, int sampleRate, int channels)
    {
        int n = (int)(seconds * sampleRate) * channels;
        return new WavBuffer { SampleRate = sampleRate, Channels = channels, Samples = new short[n] };
    }

    private static WavBuffer Gain(WavBuffer w, double factor)
    {
        var s = new short[w.Samples.Length];
        for (int i = 0; i < s.Length; i++)
            s[i] = (short)Math.Clamp(w.Samples[i] * factor, short.MinValue, short.MaxValue);
        return new WavBuffer { SampleRate = w.SampleRate, Channels = w.Channels, Samples = s };
    }
}
