using System.Text;

namespace EarTraining.Core.Audio;

/// <summary>
/// In-memory 16-bit PCM WAV. I/O-free and fully portable: bytes in, bytes out.
/// The mobile app supplies the piano sample bytes (from MauiAssets); this type
/// never touches the file system or any platform API.
/// </summary>
public sealed class WavBuffer
{
    public int SampleRate { get; init; }
    public int Channels { get; init; }
    public short[] Samples { get; init; } = Array.Empty<short>(); // interleaved L,R,L,R...

    public double Duration => (double)(Samples.Length / Math.Max(1, Channels)) / Math.Max(1, SampleRate);

    public static WavBuffer Read(byte[] b)
    {
        if (b.Length < 12 || Encoding.ASCII.GetString(b, 0, 4) != "RIFF" || Encoding.ASCII.GetString(b, 8, 4) != "WAVE")
            throw new InvalidDataException("Not a WAV stream.");

        int channels = 0, sampleRate = 0, bits = 0;
        byte[]? data = null;
        int pos = 12;
        while (pos + 8 <= b.Length)
        {
            string id = Encoding.ASCII.GetString(b, pos, 4);
            int size = BitConverter.ToInt32(b, pos + 4);
            int body = pos + 8;
            if (id == "fmt ")
            {
                channels = BitConverter.ToUInt16(b, body + 2);
                sampleRate = (int)BitConverter.ToUInt32(b, body + 4);
                bits = BitConverter.ToUInt16(b, body + 14);
            }
            else if (id == "data")
            {
                int n = Math.Min(size, b.Length - body);
                data = new byte[n];
                Array.Copy(b, body, data, 0, n);
            }
            pos = body + size + (size & 1); // chunks are word-aligned
        }
        if (data is null || bits != 16)
            throw new InvalidDataException("Expected 16-bit PCM WAV.");

        var samples = new short[data.Length / 2];
        Buffer.BlockCopy(data, 0, samples, 0, samples.Length * 2);
        return new WavBuffer { SampleRate = sampleRate, Channels = channels, Samples = samples };
    }

    public byte[] Write()
    {
        int dataBytes = Samples.Length * 2;
        using var ms = new MemoryStream(44 + dataBytes);
        using var w = new BinaryWriter(ms);
        w.Write(Encoding.ASCII.GetBytes("RIFF"));
        w.Write(36 + dataBytes);
        w.Write(Encoding.ASCII.GetBytes("WAVE"));
        w.Write(Encoding.ASCII.GetBytes("fmt "));
        w.Write(16);                            // PCM fmt chunk size
        w.Write((short)1);                      // audio format = PCM
        w.Write((short)Channels);
        w.Write(SampleRate);
        w.Write(SampleRate * Channels * 2);     // byte rate
        w.Write((short)(Channels * 2));         // block align
        w.Write((short)16);                     // bits per sample
        w.Write(Encoding.ASCII.GetBytes("data"));
        w.Write(dataBytes);
        var bytes = new byte[dataBytes];
        Buffer.BlockCopy(Samples, 0, bytes, 0, dataBytes);
        w.Write(bytes);
        w.Flush();
        return ms.ToArray();
    }
}
