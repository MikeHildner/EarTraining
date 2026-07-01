using EarTraining.App.Services;

namespace EarTraining.App.Components;

/// <summary>
/// Hands-free drill runner. Set <see cref="Target"/> to the hosting page (which implements
/// <see cref="IAutomatableDrill"/>); "Automate" then loops: play a new drill, wait, reveal
/// (scoring misses if the "Timed" toggle is on), for <see cref="MaxIterations"/> rounds or until Stop.
/// </summary>
public partial class AutomationBar : ContentView
{
    public IAutomatableDrill? Target { get; set; }
    public int MaxIterations { get; set; } = 10;

    // Scored mode gives a fixed window to actually answer; the passive beat before the reveal and
    // the post-reveal dwell come from the user's Automation-pace setting (Relaxed/Normal/Quick).
    public double AnswerWindow { get; set; } = 4.0;

    private static (double pause, double dwell) PaceFor(AutomationPace p) => p switch
    {
        AutomationPace.Relaxed => (1.5, 2.0),
        AutomationPace.Quick => (0.5, 0.8),
        _ => (1.0, 1.0),   // Normal
    };

    private CancellationTokenSource? _cts;

    public AutomationBar()
    {
        InitializeComponent();
    }

    private async void OnGoStop(object? sender, EventArgs e)
    {
        if (_cts is not null) { Stop(); return; }   // running → stop
        if (Target is null) return;

        bool scored = ScoreSwitch.IsToggled;
        var (pause, dwell) = PaceFor(SettingsStore.AutoPace);
        ScoreSwitch.IsEnabled = false;
        GoButton.Text = "Stop ■";
        _cts = new CancellationTokenSource();
        var ct = _cts.Token;

        try
        {
            for (int i = 1; i <= MaxIterations; i++)
            {
                ct.ThrowIfCancellationRequested();
                StatusLabel.Text = $"Playing {i} of {MaxIterations}…";
                double len = Target.AutoPlay();
                await Task.Delay(TimeSpan.FromSeconds(len + (scored ? AnswerWindow : pause)), ct);
                Target.AutoReveal(scored);
                await Task.Delay(TimeSpan.FromSeconds(dwell), ct);   // let the answer land
            }
        }
        catch (OperationCanceledException) { /* Stop pressed or page left */ }
        finally { Reset(); }
    }

    /// <summary>Halt the run (also call from the page's OnDisappearing).</summary>
    public void Stop() => _cts?.Cancel();

    private void Reset()
    {
        _cts?.Dispose();
        _cts = null;
        GoButton.Text = "▶ Automate";
        ScoreSwitch.IsEnabled = true;
        StatusLabel.Text = string.Empty;
    }
}
