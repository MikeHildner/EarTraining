using System.Globalization;

namespace EarTraining.App;

/// <summary>
/// Maps a flyout item's Title to a FontAwesome (4.7) glyph so the Shell flyout rows show a leading
/// icon without hand-assigning one to every FlyoutItem. Used by AppShell's flyout ItemTemplate.
/// Codepoints are the FA 4.7 private-use-area values (kept as hex so the source stays plain ASCII).
/// </summary>
public sealed class FlyoutIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string title = value as string ?? string.Empty;
        int code =
            title == "Home" ? 0xF015 :              // fa-home
            title == "Not in the Books" ? 0xF03A :  // fa-list
            title == "About" ? 0xF05A :             // fa-info-circle
            title.StartsWith("Level 2") ? 0xF1B3 :  // fa-cubes (progressions)
            0xF001;                                 // fa-music (Level 1 chapters + default)

        return new FontImageSource
        {
            Glyph = char.ConvertFromUtf32(code),
            FontFamily = "FA",
            Color = Color.FromArgb("#512BD4"),
            Size = 18,
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>
/// Returns an amber FontAwesome "wrench" glyph for nav items that are still a work in progress
/// (Level 2 + Experimental), or null otherwise — used as a trailing badge in AppShell's flyout.
/// </summary>
public sealed class WipIconConverter : IValueConverter
{
    public static bool IsWip(string title) => title.StartsWith("Level 2");

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string title = value as string ?? string.Empty;
        return IsWip(title)
            ? new FontImageSource { Glyph = char.ConvertFromUtf32(0xF0AD), FontFamily = "FA", Color = Color.FromArgb("#BA7517"), Size = 13 }
            : null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
