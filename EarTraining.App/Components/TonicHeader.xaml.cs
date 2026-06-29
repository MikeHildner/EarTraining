using EarTraining.App.Services;
using EarTraining.Core.Audio;
using EarTraining.Core.Theory;
using Plugin.Maui.Audio;

namespace EarTraining.App.Components;

/// <summary>
/// Shows the current DO (tonic), plays it as a reference, and re-rolls it. All L1C1
/// drills are built relative to <see cref="Do"/>; pages read it and rebuild on
/// <see cref="DoChanged"/>.
/// </summary>
public partial class TonicHeader : ContentView
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Random _rng = new();

    public int Do { get; private set; }
    public event EventHandler? DoChanged;

    public TonicHeader()
    {
        InitializeComponent();
        Roll();
        SyncToSettings();
        // Shell keeps drill pages alive, so Loaded re-fires on every re-appear — re-sync then
        // (e.g. the user changed the practice key in Settings and came back to a cached page).
        Loaded += (_, _) => SyncToSettings();
    }

    private void Roll()
    {
        // Honor a fixed practice key from Settings; otherwise a fresh random DO each drill.
        var key = SettingsStore.FixedKey;
        Do = string.IsNullOrEmpty(key) ? Tonic.RandomDo(_rng) : Keys.DoNote(key);
        UpdateLabel();
    }

    // Reflects the current Settings practice key: hides the now-pointless "New DO" when the key
    // is fixed, and (for cached pages) re-rolls to a newly chosen fixed key, notifying the page.
    // With no fixed key we leave the existing random DO untouched — only the chrome updates — so
    // revisiting a page doesn't silently change the tonic.
    private void SyncToSettings()
    {
        var key = SettingsStore.FixedKey;
        bool isFixed = !string.IsNullOrEmpty(key);
        NewDoButton.IsVisible = !isFixed;

        bool reRolled = false;
        if (isFixed)
        {
            int target = Keys.DoNote(key);
            if (Do != target) { Do = target; reRolled = true; }
        }

        UpdateLabel();
        if (reRolled) DoChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateLabel() =>
        DoLabel.Text = string.IsNullOrEmpty(SettingsStore.FixedKey)
            ? $"DO = {Note.Name(Do)}"
            : $"DO = {Note.Name(Do)} · fixed";

    private async void OnPlayDo(object? sender, EventArgs e)
    {
        var wav = AudioRenderer.RenderSequence(new[] { (await _samples.LoadAsync(Note.SampleFile(Do)), 2.0) });
        _audio.Play(wav);
    }

    private void OnNewDo(object? sender, EventArgs e)
    {
        Roll();
        DoChanged?.Invoke(this, EventArgs.Empty);
    }
}
