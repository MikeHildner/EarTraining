using EarTraining.Core.Notation;

namespace EarTraining.App.Services;

/// <summary>
/// Writes the Blank Sheet Music PDF into <see cref="FileSystem.CacheDirectory"/> and returns
/// its path for the system share sheet. The bytes come from Core's <see cref="SheetPdfWriter"/>,
/// fed with the SVG rows harvested from the rendered preview — one deterministic, dependency-free
/// implementation for both platforms (the platform print pipelines proved uncontrollable).
/// </summary>
public static class SheetPdf
{
    public static async Task<string> RenderAsync(string rowsHtml, bool landscape, string fileName)
    {
        byte[] pdf = SheetPdfWriter.FromHarvestedRows(rowsHtml, landscape);
        string path = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllBytesAsync(path, pdf);
        return path;
    }
}
