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

    public DrillAudioPlayer(IAudioManager audioManager) => _audioManager = audioManager;

    public void Play(byte[] wav, bool loop = false)
    {
        _current?.Stop();
        _current?.Dispose();
        _current = _audioManager.CreatePlayer(new MemoryStream(wav));
        _current.Loop = loop;                     // the metronome loops its click track
        _current.Volume = SettingsStore.Volume;   // master volume from user settings
        _current.Play();
    }

    /// <summary>Stop and release the in-flight sound, if any.</summary>
    public void Stop()
    {
        _current?.Stop();
        _current?.Dispose();
        _current = null;
    }
}
