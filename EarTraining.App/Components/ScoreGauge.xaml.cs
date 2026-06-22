using Microsoft.Maui.Graphics;

namespace EarTraining.App.Components;

/// <summary>
/// Reusable scoring control: running score, streak, and a tiered accuracy ring
/// (Good Start / Good Work / Very Good / Quite Good) matching the web app's thresholds
/// and colors. Pages call Record(correct) / Reset(); the gauge owns its own tally.
/// </summary>
public partial class ScoreGauge : ContentView
{
    private readonly GaugeDrawable _drawable = new();
    private int _correct, _total, _streak;

    public ScoreGauge()
    {
        InitializeComponent();
        GaugeView.Drawable = _drawable;
        Refresh();
    }

    public void Record(bool correct)
    {
        _total++;
        if (correct) { _correct++; _streak++; } else { _streak = 0; }
        Refresh();
    }

    public void Reset()
    {
        _correct = _total = _streak = 0;
        Refresh();
    }

    private void Refresh()
    {
        int pct = _total == 0 ? -1 : (int)Math.Round(100.0 * _correct / _total);
        var (color, label) = TierFor(pct);
        _drawable.Percent = pct;
        _drawable.FillColor = color;
        TierLabel.Text = label;
        TierLabel.TextColor = color;
        ScoreLabel.Text = $"Score: {_correct} / {_total}";
        StreakLabel.Text = $"Streak: {_streak}";
        GaugeView.Invalidate();
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
        public int Percent { get; set; } = -1;
        public Color FillColor { get; set; } = Color.FromArgb("#ADB5BD");

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

            int pct = Percent < 0 ? 0 : Percent;
            if (pct > 0)
            {
                canvas.StrokeColor = FillColor;
                canvas.DrawArc(box.X, box.Y, box.Width, box.Height, 90f, 90f - 360f * pct / 100f, true, false);
            }

            canvas.FontColor = Color.FromArgb("#2D1B69");
            canvas.FontSize = size * 0.24f;
            canvas.DrawString(Percent < 0 ? "—" : pct + "%",
                rect.X, rect.Y, rect.Width, rect.Height,
                HorizontalAlignment.Center, VerticalAlignment.Center);
        }
    }
}
