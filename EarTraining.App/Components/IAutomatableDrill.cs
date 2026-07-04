namespace EarTraining.App.Components;

/// <summary>
/// A drill page the <see cref="AutomationBar"/> can run automatically: it advances to a new
/// drill, plays it, waits, then reveals (and optionally scores) the answer, on a loop.
/// </summary>
public interface IAutomatableDrill
{
    /// <summary>Pick a new drill and start playing it; returns the audio length in seconds
    /// so the runner knows how long to wait before revealing.</summary>
    double AutoPlay();

    /// <summary>Reveal the answer. In scored mode, a drill the user didn't answer counts as a miss.</summary>
    void AutoReveal(bool scored);
}
