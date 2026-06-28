using EarTraining.Core.Drills;

namespace EarTraining.App.Components;

/// <summary>
/// A two-quality "interval type" selector (left / right / Both) for the interval drill pages.
/// Defaults to Maj 3rd / Min 6th (L1C2); call <see cref="Configure"/> to switch the pair
/// (e.g. Min 3rd / Maj 6th for L1C3). <see cref="Quality"/> is the chosen quality, or null
/// for "Both"; <see cref="Changed"/> fires on a change.
/// </summary>
public partial class IntervalTypeToggle : ContentView
{
    public event EventHandler? Changed;

    private IntervalQuality _left = IntervalQuality.Major3rd;
    private IntervalQuality _right = IntervalQuality.Minor6th;

    public IntervalTypeToggle()
    {
        InitializeComponent();
    }

    /// <summary>Choose the two qualities this toggle offers (defaults Maj 3rd / Min 6th).</summary>
    public void Configure(IntervalQuality left, IntervalQuality right)
    {
        _left = left;
        _right = right;
        LeftBtn.Content = left.Display();
        RightBtn.Content = right.Display();
    }

    public IntervalQuality? Quality
    {
        get
        {
            if (RightBtn.IsChecked) return _right;
            if (BothBtn.IsChecked) return null;
            return _left;
        }
    }

    private void OnChanged(object? sender, CheckedChangedEventArgs e)
    {
        // CheckedChanged fires for both the cleared and the newly-set button; act once.
        if (e.Value) Changed?.Invoke(this, EventArgs.Empty);
    }
}
