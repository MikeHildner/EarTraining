using EarTraining.Core.Drills;

namespace EarTraining.App.Components;

/// <summary>
/// Maj 3rd / Min 6th / Both selector for the L1C2 interval drills. <see cref="Quality"/>
/// is the chosen quality, or null for "Both"; <see cref="Changed"/> fires on a change.
/// </summary>
public partial class IntervalTypeToggle : ContentView
{
    public event EventHandler? Changed;

    public IntervalTypeToggle()
    {
        InitializeComponent();
    }

    public IntervalQuality? Quality
    {
        get
        {
            if (Min6.IsChecked) return IntervalQuality.Minor6th;
            if (BothBtn.IsChecked) return null;
            return IntervalQuality.Major3rd;
        }
    }

    private void OnChanged(object? sender, CheckedChangedEventArgs e)
    {
        // CheckedChanged fires for both the cleared and the newly-set button; act once.
        if (e.Value) Changed?.Invoke(this, EventArgs.Empty);
    }
}
