using EarTraining.Core.Theory;

namespace EarTraining.App.Services;

/// <summary>
/// Defaults a dictation page's Key picker to the Settings practice key. The picker stays
/// freely changeable; on every page Loaded (Shell keeps pages alive) a CHANGED practice key
/// is re-applied — firing the picker's existing changed-handler, which regenerates the
/// drill — while an unchanged setting leaves the user's in-session choice alone. Switching
/// the setting back to Random keeps the current key, since dictation always needs a concrete
/// one. Mirrors TonicHeader's Loaded re-sync pattern.
/// </summary>
public sealed class PracticeKeyDefault
{
    private readonly Picker _keyPicker;
    private string _applied;

    public PracticeKeyDefault(ContentPage page, Picker keyPicker)
    {
        _keyPicker = keyPicker;
        _applied = SettingsStore.FixedKey;
        _keyPicker.SelectedIndex = Math.Max(0, IndexOf(_applied));   // C when random/unknown
        page.Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        string key = SettingsStore.FixedKey;
        if (key == _applied) return;
        _applied = key;
        int i = IndexOf(key);
        if (i >= 0)
            _keyPicker.SelectedIndex = i;   // fires the page's changed-handler → new drill
    }

    private static int IndexOf(string key)
    {
        for (int i = 0; i < Keys.All.Count; i++)
            if (Keys.All[i] == key) return i;
        return -1;
    }
}
