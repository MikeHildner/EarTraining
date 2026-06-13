using System.Runtime.InteropServices;
using System.Text;

namespace AudioPortabilitySpike;

// SPIKE: prove the ear-training audio engine (sample-based piano rendering)
// runs on modern, cross-platform .NET with ZERO Windows / native / 3rd-party
// dependencies. It mirrors what the ASP.NET app does today — load piano WAV
// samples, slice to a duration, then SEQUENCE them (melodic) or OVERLAY them
// (harmonic) — but using only System.IO. No NAudio, no NAudio.Lame, no
// System.Web, no ffmpeg. If this renders correct WAVs, the audio layer is
// portable to iOS/Android (the only platform-specific piece left is handing
// the finished PCM buffer to the OS to play).

internal static class Program
{
    private static int Main(string[] args)
    {
        string samplesDir = args.Length > 0 ? args[0] : Path.Combine("..", "EarTraining", "Samples", "Piano");
        string outDir = args.Length > 1 ? args[1] : ".";

        Console.WriteLine($"Runtime : {RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"OS      : {RuntimeInformation.OSDescription}");
        Console.WriteLine($"Samples : {Path.GetFullPath(samplesDir)}");
        Console.WriteLine();

        // C-major triad. File names match the app's "<index>.<Note><octave>.wav".
        var c4 = Slice(ReadWav(Path.Combine(samplesDir, "39.C4.wav")), 1.2);
        var e4 = Slice(ReadWav(Path.Combine(samplesDir, "43.E4.wav")), 1.2);
        var g4 = Slice(ReadWav(Path.Combine(samplesDir, "46.G4.wav")), 1.2);
        Console.WriteLine($"Loaded C4/E4/G4: {c4.Channels}ch {c4.SampleRate}Hz {c4.BitsPerSample}-bit, {Seconds(c4):0.00}s each");

        // Melodic drill: notes one after another.
        var melodic = Concat(c4, e4, g4);
        string melodicPath = Path.Combine(outDir, "melodic_C_E_G.wav");
        WriteWav(melodicPath, melodic);
        Report(melodicPath, melodic);

        // Harmonic drill: notes together (overlay), with headroom to avoid clipping.
        var harmonic = Mix(0.7, c4, e4, g4);
        string harmonicPath = Path.Combine(outDir, "harmonic_C_E_G.wav");
        WriteWav(harmonicPath, harmonic);
        Report(harmonicPath, harmonic);

        Console.WriteLine();
        Console.WriteLine("OK: rendered melodic + harmonic drills with no NAudio, no System.Web, no native code.");
        return 0;
    }

    private static void Report(string path, Wav w) =>
        Console.WriteLine($"  -> {Path.GetFileName(path),-20} {Seconds(w):0.00}s  {new FileInfo(path).Length:N0} bytes");

    // ---- minimal 16-bit PCM WAV model + I/O (pure BCL) ----

    private sealed class Wav
    {
        public int SampleRate;
        public int Channels;
        public int BitsPerSample;
        public short[] Samples = Array.Empty<short>(); // interleaved L,R,L,R...
    }

    private static double Seconds(Wav w) => (double)(w.Samples.Length / w.Channels) / w.SampleRate;

    private static Wav ReadWav(string path)
    {
        byte[] b = File.ReadAllBytes(path);
        if (Encoding.ASCII.GetString(b, 0, 4) != "RIFF" || Encoding.ASCII.GetString(b, 8, 4) != "WAVE")
            throw new InvalidDataException($"Not a WAV file: {path}");

        var w = new Wav();
        byte[]? data = null;
        int pos = 12;
        while (pos + 8 <= b.Length)
        {
            string id = Encoding.ASCII.GetString(b, pos, 4);
            int size = BitConverter.ToInt32(b, pos + 4);
            int body = pos + 8;
            if (id == "fmt ")
            {
                w.Channels = BitConverter.ToUInt16(b, body + 2);
                w.SampleRate = (int)BitConverter.ToUInt32(b, body + 4);
                w.BitsPerSample = BitConverter.ToUInt16(b, body + 14);
            }
            else if (id == "data")
            {
                int n = Math.Min(size, b.Length - body);
                data = new byte[n];
                Array.Copy(b, body, data, 0, n);
            }
            pos = body + size + (size & 1); // chunks are word-aligned
        }
        if (data is null || w.BitsPerSample != 16)
            throw new InvalidDataException($"Expected 16-bit PCM data in {path}");

        w.Samples = new short[data.Length / 2];
        Buffer.BlockCopy(data, 0, w.Samples, 0, w.Samples.Length * 2);
        return w;
    }

    private static Wav Slice(Wav w, double seconds)
    {
        int count = Math.Min(w.Samples.Length, (int)(seconds * w.SampleRate) * w.Channels);
        var s = new short[count];
        Array.Copy(w.Samples, s, count);
        return new Wav { SampleRate = w.SampleRate, Channels = w.Channels, BitsPerSample = 16, Samples = s };
    }

    private static Wav Concat(params Wav[] parts)
    {
        var first = parts[0];
        var s = new short[parts.Sum(p => p.Samples.Length)];
        int o = 0;
        foreach (var p in parts) { Array.Copy(p.Samples, 0, s, o, p.Samples.Length); o += p.Samples.Length; }
        return new Wav { SampleRate = first.SampleRate, Channels = first.Channels, BitsPerSample = 16, Samples = s };
    }

    private static Wav Mix(double gain, params Wav[] voices)
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
        return new Wav { SampleRate = first.SampleRate, Channels = first.Channels, BitsPerSample = 16, Samples = s };
    }

    private static void WriteWav(string path, Wav w)
    {
        int dataBytes = w.Samples.Length * 2;
        using var fs = new FileStream(path, FileMode.Create);
        using var bw = new BinaryWriter(fs);
        bw.Write(Encoding.ASCII.GetBytes("RIFF"));
        bw.Write(36 + dataBytes);
        bw.Write(Encoding.ASCII.GetBytes("WAVE"));
        bw.Write(Encoding.ASCII.GetBytes("fmt "));
        bw.Write(16);                                 // PCM fmt chunk size
        bw.Write((short)1);                           // audio format = PCM
        bw.Write((short)w.Channels);
        bw.Write(w.SampleRate);
        bw.Write(w.SampleRate * w.Channels * 2);      // byte rate
        bw.Write((short)(w.Channels * 2));            // block align
        bw.Write((short)16);                          // bits per sample
        bw.Write(Encoding.ASCII.GetBytes("data"));
        bw.Write(dataBytes);
        byte[] bytes = new byte[dataBytes];
        Buffer.BlockCopy(w.Samples, 0, bytes, 0, dataBytes);
        bw.Write(bytes);
    }
}
