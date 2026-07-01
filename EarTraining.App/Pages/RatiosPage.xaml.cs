using EarTraining.App.Services;
using Microsoft.Maui.Graphics;

namespace EarTraining.App.Pages;

/// <summary>
/// "Ratios" reference (ported from the web Experimental/Ratios): each interval's just-intonation
/// frequency ratio drawn as numerator vs denominator circles, which animate in on appear. Purely
/// visual — no audio, no scoring. (The web page's external equal-temperament image is dropped.)
/// </summary>
public partial class RatiosPage : ContentPage
{
    private static readonly (string Name, int Num, int Den)[] Intervals =
    [
        ("Unison", 1, 1), ("Minor 2nd", 16, 15), ("Major 2nd", 9, 8), ("Minor 3rd", 6, 5),
        ("Major 3rd", 5, 4), ("Perfect 4th", 4, 3), ("Tritone", 10, 7), ("Perfect 5th", 3, 2),
        ("Minor 6th", 8, 5), ("Major 6th", 5, 3), ("Minor 7th", 16, 9), ("Major 7th", 15, 8),
        ("Perfect Octave", 2, 1),
    ];

    // Web Vibe colors: numerator (upper) blue-purple, denominator (lower) coral — legible light + dark.
    private static readonly Color NumColor = Color.FromArgb("#5B7CF6");
    private static readonly Color DenColor = Color.FromArgb("#F05575");

    private readonly List<(GraphicsView View, RatioDrawable Drawable, int Steps)> _rows = new();

    public RatiosPage()
    {
        InitializeComponent();
        foreach (var (name, num, den) in Intervals)
        {
            var title = new Label { Style = (Style)Application.Current!.Resources["Heading"], Text = name, VerticalOptions = LayoutOptions.Center };
            var ratio = new Label { Text = $"{num}:{den}", TextColor = Theme.Muted, FontSize = 14, VerticalOptions = LayoutOptions.Center };
            var header = new HorizontalStackLayout { Spacing = 8, Children = { title, ratio } };

            var drawable = new RatioDrawable { Num = num, Den = den };
            var view = new GraphicsView { Drawable = drawable, HeightRequest = 34, HorizontalOptions = LayoutOptions.Fill };

            _rows.Add((view, drawable, num + den));
            Rows.Children.Add(new VerticalStackLayout { Spacing = 6, Children = { header, view } });
        }
    }

    // Re-run the staggered circle reveal each time the page appears (mirrors the web's on-load animation).
    protected override void OnAppearing()
    {
        base.OnAppearing();
        for (int i = 0; i < _rows.Count; i++)
        {
            var (view, drawable, steps) = _rows[i];
            this.AbortAnimation($"reveal{i}");
            drawable.Reveal = 0;
            view.Invalidate();
            var v = view;
            var d = drawable;
            new Animation(x => { d.Reveal = x; v.Invalidate(); }, 0, 1)
                .Commit(this, $"reveal{i}", length: (uint)Math.Max(300, steps * 55),
                        finished: (_, _) => { d.Reveal = 1; v.Invalidate(); });
        }
    }

    // Draws two overlapping, centered rows of tangent circles (numerator + denominator), each circle
    // fading in as Reveal (0..1) advances — numerator group first, then denominator.
    private sealed class RatioDrawable : IDrawable
    {
        public int Num { get; init; }
        public int Den { get; init; }
        public double Reveal { get; set; }

        public void Draw(ICanvas canvas, RectF rect)
        {
            if (rect.Width < 16) return;
            int total = Num + Den;
            int maxCount = Math.Max(Num, Den);
            const float pad = 8f;
            float r = maxCount <= 1 ? 11.5f : Math.Min(11.5f, (rect.Width - pad) / (2f * maxCount));
            float cx = rect.Center.X;
            float cy = rect.Center.Y;

            canvas.StrokeSize = 1.5f;
            DrawGroup(canvas, cx, cy, r, Num, 0, total, NumColor);
            DrawGroup(canvas, cx, cy, r, Den, Num, total, DenColor);
        }

        private void DrawGroup(ICanvas canvas, float cx, float cy, float r, int count, int startIndex, int total, Color color)
        {
            if (count <= 0) return;
            float startX = cx - r * (count - 1);
            for (int j = 0; j < count; j++)
            {
                float a = (float)Math.Clamp(Reveal * total - (startIndex + j), 0, 1);
                if (a <= 0) continue;
                canvas.StrokeColor = color.WithAlpha(a);
                canvas.DrawCircle(startX + j * 2f * r, cy, r);
            }
        }
    }
}
