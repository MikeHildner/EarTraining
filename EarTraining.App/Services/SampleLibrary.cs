using Microsoft.Maui.Storage;

namespace EarTraining.App.Services;

/// <summary>Loads bundled piano sample WAVs (MauiAssets under Resources/Raw) as raw bytes.
/// Bytes are cached app-wide after the first read — pages re-load the same handful of
/// samples on every play, and L2C9's full-circle track alone is ~140 loads per tap.
/// All callers are UI-thread event handlers, so the cache needs no lock.</summary>
public sealed class SampleLibrary
{
    private static readonly Dictionary<string, byte[]> Cache = new();

    public async Task<byte[]> LoadAsync(string fileName)
    {
        if (Cache.TryGetValue(fileName, out var cached)) return cached;
        using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        var bytes = ms.ToArray();
        Cache[fileName] = bytes;
        return bytes;
    }
}
