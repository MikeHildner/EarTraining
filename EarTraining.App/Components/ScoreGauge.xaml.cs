using Microsoft.Maui.Graphics;

namespace EarTraining.App.Components;

/// <summary>
/// Reusable scoring control: running score, streak, and a tiered accuracy ring
/// (Good Start / Good Work / Very Good / Quite Good) matching the web app's thresholds
/// and colors. On each answer the ring sweeps from empty up to the new value (ease-out)
/// and the gauge gives a brief scale "pop" — so it reacts even when the percentage is
/// unchanged (e.g. consecutive correct answers at 100%). Pages call Record(correct) /
/// Reset(); the gauge owns its own tally.
/// </summary>
public partial class ScoreGauge : ContentView
{
    private readonly GaugeDrawable _drawable = new();
    private int _correct, _total, _streak;

    public ScoreGauge()
    {
        InitializeComponent();
        GaugeView.Drawable = _drawable;
        Refresh(animate: false);
    }

    /// <summary>Welcome/hero use: hide the score + streak tally, leaving just the ring + tier label.</summary>
    public bool ShowTally
    {
        get => ScoreLabel.IsVisible;
        set { ScoreLabel.IsVisible = value; StreakLabel.IsVisible = value; }
    }

    /// <summary>Ring diameter in device-independent units (default 130).</summary>
    public double GaugeSize
    {
        get => GaugeView.WidthRequest;
        set { GaugeView.WidthRequest = value; GaugeView.HeightRequest = value; }
    }

    /// <summary>Color of the centered percent text (default dark; set light on dark backgrounds).</summary>
    public Color PercentTextColor
    {
        get => _drawable.PercentColor;
        set { _drawable.PercentColor = value; GaugeView.Invalidate(); }
    }

    public void Record(bool correct)
    {
        _total++;
        if (correct) { _correct++; _streak++; } else { _streak = 0; }
        Refresh(animate: true);
    }

    public void Reset()
    {
        _correct = _total = _streak = 0;
        Refresh(animate: false);
    }

    private void Refresh(bool animate)
    {
        int pct = _total == 0 ? -1 : (int)Math.Round(100.0 * _correct / _total);
        var (color, label) = TierFor(pct);

        // Percent text, tier, color, and score/streak snap to the final value immediately;
        // only the ring sweeps.
        _drawable.DisplayPercent = pct;
        _drawable.FillColor = color;
        TierLabel.Text = label;
        TierLabel.TextColor = color;
        ScoreLabel.Text = $"Score: {_correct} / {_total}";
        StreakLabel.Text = $"Streak: {_streak}";

        double target = pct < 0 ? 0 : pct;
        this.AbortAnimation("gaugeSweep");

        if (animate && pct >= 0)
        {
            _drawable.SweepPercent = 0;
            new Animation(v => { _drawable.SweepPercent = v; GaugeView.Invalidate(); }, 0, target, Easing.CubicOut)
                .Commit(this, "gaugeSweep", length: 700, finished: (_, _) => { _drawable.SweepPercent = target; GaugeView.Invalidate(); });
            _ = PopAsync();
        }
        else
        {
            _drawable.SweepPercent = target;
            GaugeView.Invalidate();
        }
    }

    private async Task PopAsync()
    {
        await GaugeView.ScaleToAsync(1.08, 120, Easing.CubicOut);
        await GaugeView.ScaleToAsync(1.0, 250, Easing.CubicOut);
    }

    private static (Color color, string label) TierFor(int pct) => pct switch
    {
        < 0 => (Color.FromArgb("#ADB5BD"), "Play a drill"),
        >= 90 => (Color.FromArgb("#C99700"), "Quite Good"),
        >= 70 => (Color.FromArgb("#28A745"), "Very Good"),
        >= 50 => (Color.FromArgb("#FD7E14"), "Good Work"),
        _ => (Color.FromArgb("#DC3545"), "Good Start"),
    };

    private sealed class GaugeDrawable : IDrawable
    {
        public int DisplayPercent { get; set; } = -1;   // -1 = no data; drives the center text
        public double SweepPercent { get; set; }          // 0..100, animated; drives the arc
        public Color FillColor { get; set; } = Color.FromArgb("#ADB5BD");
        public Color PercentColor { get; set; } = Color.FromArgb("#2D1B69");

        public void Draw(ICanvas canvas, RectF rect)
        {
            float size = Math.Min(rect.Width, rect.Height);
            float cx = rect.Center.X, cy = rect.Center.Y;
            float stroke = size * 0.12f;
            float r = size / 2f - stroke;
            var box = new RectF(cx - r, cy - r, r * 2f, r * 2f);

            canvas.StrokeSize = stroke;
            canvas.StrokeLineCap = LineCap.Round;
            canvas.StrokeColor = Color.FromArgb("#E9ECEF");
            canvas.DrawCircle(cx, cy, r);

            double sweep = SweepPercent;
            if (sweep >= 99.95)
            {
                // A 360° arc has start == end and draws nothing — use a full circle.
                canvas.StrokeColor = FillColor;
                canvas.DrawCircle(cx, cy, r);
            }
            else if (sweep > 0.05)
            {
                canvas.StrokeColor = FillColor;
                canvas.DrawArc(box.X, box.Y, box.Width, box.Height, 90f, (float)(90.0 - 360.0 * sweep / 100.0), true, false);
            }

            canvas.FontColor = PercentColor;
            canvas.FontSize = size * 0.24f;
            canvas.DrawString(DisplayPercent < 0 ? "—" : DisplayPercent + "%",
                rect.X, rect.Y, rect.Width, rect.Height,
                HorizontalAlignment.Center, VerticalAlignment.Center);
        }
    }
}
