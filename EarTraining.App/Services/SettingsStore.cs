using Microsoft.Maui.Storage;

namespace EarTraining.App.Services;

/// <summary>
/// User preferences — theme, fixed practice key, playback volume — persisted on-device via
/// <see cref="Preferences"/> (no network, no account). Static singleton, matching the app's
/// no-DI service pattern (cf. <see cref="ProgressStore"/>). A few scalar keys, so no JSON blob.
/// </summary>
public static class SettingsStore
{
    /// <summary>Theme: AppTheme.Unspecified (follow system, default), Light, or Dark.</summary>
    public static AppTheme Theme
    {
        get => (AppTheme)Preferences.Get("set.theme", (int)AppTheme.Unspecified);
        set => Preferences.Set("set.theme", (int)value);
    }

    /// <summary>Fixed practice key (e.g. "C"); empty = random DO each drill (default).</summary>
    public static string FixedKey
    {
        get => Preferences.Get("set.key", "");
        set => Preferences.Set("set.key", value ?? "");
    }

    /// <summary>Playback volume 0..1 (default full).</summary>
    public static double Volume
    {
        get => Preferences.Get("set.volume", 1.0);
        set => Preferences.Set("set.volume", Math.Clamp(value, 0.0, 1.0));
    }

    /// <summary>Automation pacing — the silent gap after each phrase. Default Normal.</summary>
    public static AutomationPace AutoPace
    {
        get => (AutomationPace)Preferences.Get("set.autopace", (int)AutomationPace.Normal);
        set => Preferences.Set("set.autopace", (int)value);
    }
}

/// <summary>How long Automation pauses after each phrase (beat before + dwell after the answer reveal).</summary>
public enum AutomationPace { Relaxed, Normal, Quick }
