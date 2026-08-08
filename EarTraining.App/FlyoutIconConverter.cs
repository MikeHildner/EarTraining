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
            title == "Progress" ? 0xF201 :          // fa-line-chart
            title == "Settings" ? 0xF013 :          // fa-cog
            title == "Extras" ? 0xF03A :            // fa-list
            title == "About" ? 0xF05A :             // fa-info-circle
            title.StartsWith("Level 2") ? 0xF1B3 :  // fa-cubes (progressions)
            0xF001;                                 // fa-music (Level 1 chapters + default)

        return new FontImageSource
        {
            Glyph = char.ConvertFromUtf32(code),
            FontFamily = "FA",
            Color = Services.Theme.Accent,
            Size = 18,
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

// (WipIconConverter removed — the Level 2 "work in progress" wrench badge was dropped for the 1.1 release.)
