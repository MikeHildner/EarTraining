using System.Diagnostics;
using EarTraining.App.Services;
using EarTraining.Core.Audio;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

public partial class MetronomePage : ContentPage
{
    private static readonly int[] BeatsOptions = [2, 3, 4, 6];

    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly Stopwatch _clock = new();
    private readonly List<BoxView> _dots = new();
    private readonly List<DateTime> _taps = new();

    private IDispatcherTimer? _pulseTimer;
    private CancellationTokenSource? _restartDebounce;
    private byte[]? _tick;
    private bool _running;
    private bool _ready;

    public MetronomePage()
    {
        InitializeComponent();
        BeatsPicker.ItemsSource = BeatsOptions.Select(b => b.ToString()).ToList();
        int savedBeats = Array.IndexOf(BeatsOptions, Preferences.Get("met.beats", 4));
        BeatsPicker.SelectedIndex = savedBeats >= 0 ? savedBeats : 2;
        BpmSlider.Value = Math.Clamp(Preferences.Get("met.bpm", 100), 40, 208);
        UpdateBpmLabel();
        BuildPulseRow();
        _ready = true;
    }

    private int Bpm => (int)Math.Round(BpmSlider.Value);
    private int BeatsPerBar => BeatsOptions[Math.Max(0, BeatsPicker.SelectedIndex)];

    private void UpdateBpmLabel() => BpmLabel.Text = $"♩ = {Bpm}";

    // ── pulse row (visual only — audio timing lives in the rendered loop) ──

    private void BuildPulseRow()
    {
        PulseRow.Children.Clear();
        _dots.Clear();
        for (int i = 0; i < BeatsPerBar; i++)
        {
            var dot = new BoxView { WidthRequest = i == 0 ? 18 : 14, HeightRequest = i == 0 ? 18 : 14, CornerRadius = 9, VerticalOptions = LayoutOptions.Center };
            SetDim(dot);
            _dots.Add(dot);
            PulseRow.Children.Add(dot);
        }
    }

    private static void SetDim(BoxView dot) => dot.SetAppThemeColor(BoxView.ColorProperty, Color.FromArgb("#E0DCF5"), Color.FromArgb("#383150"));
    private static void SetLit(BoxView dot) => dot.SetAppThemeColor(BoxView.ColorProperty, Color.FromArgb("#512BD4"), Color.FromArgb("#B7A6FF"));

    private void OnPulseTick(object? sender, EventArgs e)
    {
        if (!_running) return;
        int beat = (int)(_clock.Elapsed.TotalSeconds * Bpm / 60.0) % BeatsPerBar;
        for (int i = 0; i < _dots.Count; i++)
        {
            if (i == beat) SetLit(_dots[i]);
            else SetDim(_dots[i]);
        }
    }

    // ── transport ──

    private async void OnStartStop(object? sender, EventArgs e)
    {
        if (_running) Stop();
        else await StartAsync();
    }

    private async Task StartAsync()
    {
        try
        {
            StatusLabel.Text = string.Empty;
            _tick ??= await _samples.LoadAsync("Woodblock.wav");
            int bpm = Bpm, beats = BeatsPerBar;
            // ~60 s per loop pass so the (Android) loop seam is at worst a once-a-minute event.
            int bars = Math.Max(1, (int)Math.Ceiling(60.0 / (beats * 60.0 / bpm)));
            var wav = await Task.Run(() => AudioRenderer.RenderMetronome(_tick, bpm, beats, bars));
            _audio.Play(wav, loop: true);
            _clock.Restart();
            _running = true;
            if (_pulseTimer is null)
            {
                _pulseTimer = Dispatcher.CreateTimer();
                _pulseTimer.Interval = TimeSpan.FromMilliseconds(40);
                _pulseTimer.Tick += OnPulseTick;
            }
            _pulseTimer.Start();
            DeviceDisplay.Current.KeepScreenOn = true;
            StartStopButton.Text = "◼ Stop";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "Audio error: " + ex.Message;
        }
    }

    private void Stop()
    {
        _running = false;
        _audio.Stop();
        _pulseTimer?.Stop();
        _clock.Reset();
        DeviceDisplay.Current.KeepScreenOn = false;
        StartStopButton.Text = "▶ Start";
        foreach (var dot in _dots) SetDim(dot);
    }

    /// <summary>Re-render + swap the loop after a settings change while running.</summary>
    private async Task RestartAsync()
    {
        if (!_running) return;
        await StartAsync();
    }

    // ── controls ──

    private void OnMinus(object? sender, EventArgs e) => BpmSlider.Value = Math.Max(40, Bpm - 1);
    private void OnPlus(object? sender, EventArgs e) => BpmSlider.Value = Math.Min(208, Bpm + 1);

    private void OnSliderChanged(object? sender, ValueChangedEventArgs e)
    {
        if (!_ready) return;
        UpdateBpmLabel();
        Preferences.Set("met.bpm", Bpm);
        if (!_running) return;
        // Debounce so dragging the slider doesn't re-render on every pixel.
        _restartDebounce?.Cancel();
        var cts = _restartDebounce = new CancellationTokenSource();
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(150, cts.Token);
                await MainThread.InvokeOnMainThreadAsync(RestartAsync);
            }
            catch (TaskCanceledException) { }
        });
    }

    private async void OnBeatsChanged(object? sender, EventArgs e)
    {
        if (!_ready) return;
        Preferences.Set("met.beats", BeatsPerBar);
        BuildPulseRow();
        await RestartAsync();
    }

    private void OnTap(object? sender, EventArgs e)
    {
        var now = DateTime.UtcNow;
        if (_taps.Count > 0 && (now - _taps[^1]).TotalSeconds > 2.5) _taps.Clear();
        _taps.Add(now);
        if (_taps.Count > 5) _taps.RemoveAt(0);
        if (_taps.Count < 2) return;

        double avgSeconds = (_taps[^1] - _taps[0]).TotalSeconds / (_taps.Count - 1);
        int bpm = Math.Clamp((int)Math.Round(60.0 / avgSeconds), 40, 208);
        BpmSlider.Value = bpm;   // label, prefs, and (if running) the debounced restart all flow from OnSliderChanged
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (_running) Stop();
    }
}
