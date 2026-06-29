using Microsoft.Maui.Graphics;

namespace EarTraining.App.Services;

/// <summary>
/// Theme-aware color palette for UI that's built in C# (HomePage, ProgressPage, ScoreGauge,
/// FlyoutIconConverter) — mirrors the <c>AppThemeBinding</c> color values in App.xaml. Each getter
/// returns the Light or Dark variant based on the app's current effective theme. (XAML uses
/// AppThemeBinding directly; this is the code-behind equivalent.) Code that caches these must refresh
/// on <c>Application.Current.RequestedThemeChanged</c>.
/// </summary>
public static class Theme
{
    public static bool IsDark => Application.Current?.RequestedTheme == AppTheme.Dark;

    private static Color Pick(string light, string dark) => Color.FromArgb(IsDark ? dark : light);

    public static Color Accent     => Pick("#512BD4", "#B7A6FF");
    public static Color PageBg     => Pick("#FFFFFF", "#15131C");
    public static Color CardBg     => Pick("#F5F3FC", "#211E30");
    public static Color CardStroke => Pick("#E0DCF5", "#383150");
    public static Color OpenBg     => Pick("#ECE6FB", "#2C2742");
    public static Color Heading    => Pick("#3C3489", "#CFC6F2");
    public static Color Body       => Pick("#444444", "#E4E2EA");
    public static Color Muted      => Pick("#6B7280", "#9AA0AA");
    public static Color GaugeTrack => Pick("#E9ECEF", "#3A3A42");
    public static Color GaugeText  => Pick("#2D1B69", "#E4E2EA");
}
