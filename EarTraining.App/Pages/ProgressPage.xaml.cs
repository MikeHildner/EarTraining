using EarTraining.App.Services;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace EarTraining.App.Pages;

/// <summary>
/// Progress / stats screen: overall accuracy + daily streak, a last-N-days trend chart, and per-drill
/// all-time accuracy — all read from <see cref="ProgressStore"/> (local, on-device). Content is rebuilt
/// on each appearance so it reflects the latest practice. Card styling matches About/Home.
/// </summary>
public partial class ProgressPage : ContentPage
{
    private const int TrendWindow = 14;

    // Theme-aware (resolved at Build() time, which runs on each OnAppearing).
    private static Color CardBg => Theme.CardBg;
    private static Color CardStroke => Theme.CardStroke;
    private static Color Accent => Theme.Accent;
    private static Color Heading => Theme.Heading;
    private static Color Body => Theme.Body;
    private static Color Muted => Theme.Muted;

    public ProgressPage() => InitializeComponent();

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Build();
    }

    private void Build()
    {
        StatsHost.Children.Clear();

        if (!ProgressStore.HasData())
        {
            StatsHost.Children.Add(new Label
            {
                Text = "No stats yet — answer a few drills and your progress shows up here.",
                TextColor = Muted,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 24, 0, 0),
            });
            return;
        }

        StatsHost.Children.Add(SummaryCard());
        StatsHost.Children.Add(TrendCard());
        StatsHost.Children.Add(PerDrillCard());
        StatsHost.Children.Add(ResetButton());
    }

    private View SummaryCard()
    {
        var (c, t) = ProgressStore.Overall();
        int pct = t == 0 ? 0 : (int)Math.Round(100.0 * c / t);
        string fire = char.ConvertFromUtf32(0x1F525); // 🔥 (ASCII-safe in source)

        var stack = new VerticalStackLayout { Spacing = 3 };
        stack.Add(new Label { Text = $"{pct}%", FontSize = 40, FontAttributes = FontAttributes.Bold, TextColor = Accent, HorizontalOptions = LayoutOptions.Center });
        stack.Add(new Label { Text = $"overall accuracy · {c} / {t} answered", FontSize = 13, TextColor = Muted, HorizontalOptions = LayoutOptions.Center });
        stack.Add(new Label
        {
            Text = $"{fire} {ProgressStore.CurrentStreak()}-day streak   ·   best {ProgressStore.BestStreak()}",
            FontSize = 15, FontAttributes = FontAttributes.Bold, TextColor = Heading,
            HorizontalOptions = LayoutOptions.Center, Margin = new Thickness(0, 8, 0, 0),
        });
        return Card(stack);
    }

    private View TrendCard()
    {
        var trend = ProgressStore.Trend(TrendWindow);
        int activeDays = trend.Count(d => d.Answered > 0);

        var stack = new VerticalStackLayout { Spacing = 8 };
        stack.Add(Heading2($"Last {TrendWindow} days"));
        stack.Add(new GraphicsView { Drawable = new TrendDrawable(trend), HeightRequest = 110 });
        stack.Add(new Label
        {
            Text = $"Practiced {activeDays} of the last {TrendWindow} days. Bar height = drills answered; color = that day's accuracy.",
            FontSize = 12, TextColor = Muted,
        });
        return Card(stack);
    }

    private View PerDrillCard()
    {
        var stack = new VerticalStackLayout { Spacing = 7 };
        stack.Add(Heading2("By drill"));
        foreach (var (route, name, correct, total) in ProgressStore.PerDrill())
        {
            int pct = (int)Math.Round(100.0 * correct / total);
            // Whole row taps through to that drill; the › chevron signals it navigates.
            var row = new Grid { ColumnSpacing = 8, Padding = new Thickness(0, 3) };
            row.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            row.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            row.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            row.Add(new Label { Text = name, FontSize = 13.5, TextColor = Body, VerticalOptions = LayoutOptions.Center });
            var right = new Label
            {
                Text = $"{pct}%  ({correct}/{total})",
                FontSize = 13.5, FontAttributes = FontAttributes.Bold, TextColor = TierColor(pct),
                VerticalOptions = LayoutOptions.Center,
            };
            Grid.SetColumn(right, 1);
            row.Add(right);

            var chevron = new Label
            {
                Text = "›", FontSize = 18, TextColor = Accent,
                VerticalOptions = LayoutOptions.Center, Margin = new Thickness(2, 0, 0, 0),
            };
            Grid.SetColumn(chevron, 2);
            row.Add(chevron);

            var r = route;
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (_, _) => await Shell.Current.GoToAsync($"//{r}");
            row.GestureRecognizers.Add(tap);

            stack.Add(row);
        }
        return Card(stack);
    }

    private View ResetButton()
    {
        var btn = new Button
        {
            Text = "Reset progress",
            BackgroundColor = Colors.Transparent,
            TextColor = Color.FromArgb("#B00020"),
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 4, 0, 0),
        };
        btn.Clicked += async (_, _) =>
        {
            bool ok = await DisplayAlertAsync("Reset progress?", "This permanently clears all saved stats on this device.", "Reset", "Cancel");
            if (ok) { ProgressStore.Reset(); Build(); }
        };
        return btn;
    }

    private static Label Heading2(string text) =>
        new() { Text = text, FontAttributes = FontAttributes.Bold, FontSize = 16, TextColor = Heading };

    private static Border Card(View content) => new()
    {
        StrokeShape = new RoundRectangle { CornerRadius = 12 },
        Stroke = CardStroke,
        StrokeThickness = 1,
        BackgroundColor = CardBg,
        Padding = 16,
        Content = content,
    };

    private static Color TierColor(int pct) => pct switch
    {
        >= 90 => Color.FromArgb("#C99700"),
        >= 70 => Color.FromArgb("#28A745"),
        >= 50 => Color.FromArgb("#FD7E14"),
        _ => Color.FromArgb("#DC3545"),
    };

    /// <summary>A compact daily bar chart: a faint full-height track per day, with a colored bar whose
    /// height is the day's drill count (relative to the busiest day) and color is that day's accuracy tier.</summary>
    private sealed class TrendDrawable : IDrawable
    {
        private readonly IReadOnlyList<(DateTime Date, int Answered, int Correct)> _days;
        public TrendDrawable(IReadOnlyList<(DateTime, int, int)> days) => _days = days;

        public void Draw(ICanvas canvas, RectF rect)
        {
            int n = _days.Count;
            if (n == 0) return;
            int max = Math.Max(1, _days.Max(d => d.Answered));

            const float pad = 6f, gap = 4f, labelBand = 16f;
            float w = (rect.Width - pad * 2 - gap * (n - 1)) / n;
            float top = pad;
            float baseY = rect.Height - labelBand;
            float maxH = baseY - top;

            for (int i = 0; i < n; i++)
            {
                var d = _days[i];
                float x = pad + i * (w + gap);
                canvas.FillColor = Theme.GaugeTrack;
                canvas.FillRoundedRectangle(x, top, w, maxH, 2);
                if (d.Answered > 0)
                {
                    float h = Math.Max(3f, maxH * d.Answered / max);
                    int pct = (int)Math.Round(100.0 * d.Correct / d.Answered);
                    canvas.FillColor = TierColor(pct);
                    canvas.FillRoundedRectangle(x, baseY - h, w, h, 2);
                }
            }

            canvas.FontColor = Theme.Muted;
            canvas.FontSize = 10;
            canvas.DrawString(_days[0].Date.ToString("M/d"), rect.X + pad, baseY + 2, 44, 12, HorizontalAlignment.Left, VerticalAlignment.Top);
            canvas.DrawString(_days[n - 1].Date.ToString("M/d"), rect.Right - 48, baseY + 2, 44, 12, HorizontalAlignment.Right, VerticalAlignment.Top);
        }
    }
}
