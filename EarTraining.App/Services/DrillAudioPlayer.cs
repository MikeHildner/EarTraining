using Plugin.Maui.Audio;

namespace EarTraining.App.Services;

/// <summary>
/// Plays a finished WAV (bytes from EarTraining.Core.AudioRenderer) through the
/// platform audio engine, stopping any in-flight sound first so taps can't overlap.
/// Named DrillAudioPlayer (not AudioPlayer) to avoid colliding with the plugin's
/// own Plugin.Maui.Audio.AudioPlayer type.
/// </summary>
public sealed class DrillAudioPlayer
{
    private readonly IAudioManager _audioManager;
    private IAudioPlayer? _current;
    private EventHandler? _endedForwarder;

    /// <summary>Raised when the current clip finishes on its own — never from Stop() or a replacing Play().</summary>
    public event EventHandler? PlaybackEnded;

    public DrillAudioPlayer(IAudioManager audioManager) => _audioManager = audioManager;

    public void Play(byte[] wav)
    {
        DetachCurrent();
        _current = _audioManager.CreatePlayer(new MemoryStream(wav));
        _endedForwarder = (_, _) => PlaybackEnded?.Invoke(this, EventArgs.Empty);
        _current.PlaybackEnded += _endedForwarder;
        _current.Volume = SettingsStore.Volume;   // master volume from user settings
        _current.Play();
    }

    /// <summary>Stop and release the in-flight sound, if any.</summary>
    public void Stop() => DetachCurrent();

    // Unhook the ended-event BEFORE stopping/disposing so a replaced or user-stopped
    // player can't raise a stale PlaybackEnded into the page.
    private void DetachCurrent()
    {
        if (_current is null) return;
        if (_endedForwarder is not null) _current.PlaybackEnded -= _endedForwarder;
        _endedForwarder = null;
        _current.Stop();
        _current.Dispose();
        _current = null;
    }
}
