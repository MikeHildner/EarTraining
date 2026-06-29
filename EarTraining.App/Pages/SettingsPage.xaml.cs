using EarTraining.App.Services;
using EarTraining.Core.Theory;

namespace EarTraining.App.Pages;

/// <summary>
/// User settings: theme (System/Light/Dark), fixed practice key (or random DO), and playback volume.
/// Reads/writes <see cref="SettingsStore"/>; the theme choice is applied live via UserAppTheme. Change
/// handlers are wired after the initial selections so they don't fire mid-setup.
/// </summary>
public partial class SettingsPage : ContentPage
{
    private static readonly AppTheme[] Themes = { AppTheme.Unspecified, AppTheme.Light, AppTheme.Dark };

    public SettingsPage()
    {
        InitializeComponent();

        ThemePicker.ItemsSource = new List<string> { "System", "Light", "Dark" };
        ThemePicker.SelectedIndex = Math.Max(0, Array.IndexOf(Themes, SettingsStore.Theme));

        var keys = new List<string> { "Random" };
        keys.AddRange(Keys.All);
        KeyPicker.ItemsSource = keys;
        KeyPicker.SelectedIndex = string.IsNullOrEmpty(SettingsStore.FixedKey)
            ? 0
            : Math.Max(0, keys.IndexOf(SettingsStore.FixedKey));

        VolumeSlider.Value = SettingsStore.Volume;
        UpdateVolumeLabel(SettingsStore.Volume);

        ThemePicker.SelectedIndexChanged += OnThemeChanged;
        KeyPicker.SelectedIndexChanged += OnKeyChanged;
        VolumeSlider.ValueChanged += OnVolumeChanged;
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        var theme = Themes[Math.Max(0, ThemePicker.SelectedIndex)];
        SettingsStore.Theme = theme;
        if (Application.Current is { } app) app.UserAppTheme = theme;   // apply live
    }

    private void OnKeyChanged(object? sender, EventArgs e)
    {
        int i = KeyPicker.SelectedIndex;
        SettingsStore.FixedKey = i <= 0 ? "" : Keys.All[i - 1];
    }

    private void OnVolumeChanged(object? sender, ValueChangedEventArgs e)
    {
        SettingsStore.Volume = e.NewValue;
        UpdateVolumeLabel(e.NewValue);
    }

    private void UpdateVolumeLabel(double v) => VolumeLabel.Text = $"{v * 100:0}%";
}
