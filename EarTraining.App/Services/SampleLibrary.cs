using Microsoft.Maui.Storage;

namespace EarTraining.App.Services;

/// <summary>Loads bundled piano sample WAVs (MauiAssets under Resources/Raw) as raw bytes.</summary>
public sealed class SampleLibrary
{
    public async Task<byte[]> LoadAsync(string fileName)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }
}
