using EarTraining.App.Services;
using Plugin.Maui.Audio;

namespace EarTraining.App.Pages;

/// <summary>
/// L2C2 reference — how this course defines circle-of-5ths (clockwise, successive V→I) and
/// circle-of-4ths (counter-clockwise, successive IV→I), and why that can contradict the
/// interval-based naming from traditional theory. Purely explanatory: a drawn circle, two
/// fixed two-triad demos, no scoring. Book Ch. 2, pp. 5–9.
/// </summary>
public partial class CircleOfFifthsPage : ContentPage
{
    private readonly SampleLibrary _samples = new();
    private readonly DrillAudioPlayer _audio = new(AudioManager.Current);
    private readonly CircleDrawable _drawable = new();

    // C4 major triad, then the V–I / IV–I destination triads below it.
    private static readonly int[] CTriad = [39, 43, 46];
    private static readonly int[] FTriad = [32, 36, 39];
    private static readonly int[] GTriad = [34, 38, 41];

    public CircleOfFifthsPage()
    {
        InitializeComponent();
        CircleView.Drawable = _drawable;
        ApplyTheme();
        if (Application.Current is { } app)
            app.RequestedThemeChanged += (_, _) => { ApplyTheme(); CircleView.Invalidate(); };
    }

    private void ApplyTheme()
    {
        _drawable.Ring = Theme.CardStroke;
        _drawable.NodeFill = Theme.CardBg;
        _drawable.Text = Theme.Body;
        _drawable.Muted = Theme.Muted;
        _drawable.Accent = Theme.Accent;
    }

    private async void OnPlayFifths(object? sender, EventArgs e) => await PlayPairAsync(CTriad, FTriad);
    private async void OnPlayFourths(object? sender, EventArgs e) => await PlayPairAsync(CTriad, GTriad);

    private async Task PlayPairAsync(int[] first, int[] second)
    {
        try
        {
            StatusLabel.Text = string.Empty;
            await DemoAudio.PlayAsync(_samples, _audio, [(first, 2.0), (second, 4.0)]);
        }
        catch (Exception ex) { StatusLabel.Text = "Audio error: " + ex.Message; }
    }

    /// <summary>
    /// The book's circle: C at the top and the FLAT keys running clockwise — the mirror of
    /// the traditional diagram — so that clockwise reads as successive V→I. Colors are pushed
    /// properties (an IDrawable can't use AppThemeBinding); the page re-pushes on theme change.
    /// </summary>
    private sealed class CircleDrawable : IDrawable
    {
        public Color Ring = Colors.Gray;
        public Color NodeFill = Colors.White;
        public Color Text = Colors.Black;
        public Color Muted = Colors.Gray;
        public Color Accent = Colors.Purple;

        private static readonly string[] KeyNames = ["C", "F", "Bb", "Eb", "Ab", "Db", "F#", "B", "E", "A", "D", "G"];
        private static readonly Dictionary<int, string> Enharmonics = new() { [5] = "(C#)", [6] = "(Gb)", [7] = "(Cb)" };

        public void Draw(ICanvas canvas, RectF rect)
        {
            float size = MathF.Min(rect.Width, rect.Height);
            float cx = rect.Center.X, cy = rect.Center.Y;
            float rNode = size * 0.058f;
            float radius = size / 2f - rNode - size * 0.075f; // leave room for the enharmonic sub-labels

            canvas.StrokeSize = MathF.Max(1.5f, size * 0.005f);
            canvas.StrokeColor = Ring;
            canvas.DrawCircle(cx, cy, radius); // full circles must be DrawCircle — a 360° arc draws nothing

            // Direction arrows in the top hemisphere (drawn before the nodes so ends tuck under them).
            float rArrow = radius * 0.72f;
            DrawArrowArc(canvas, cx, cy, rArrow, -78, -12, size);   // clockwise → 5ths
            DrawArrowArc(canvas, cx, cy, rArrow, -102, -168, size); // counter-clockwise → 4ths

            // Tiny labels in the pocket between arrow arc and ring.
            canvas.FontColor = Accent;
            canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
            canvas.FontSize = size * 0.036f;
            float rLabel = (rArrow + radius) / 2f;
            DrawCentered(canvas, "5ths", cx + rLabel * MathF.Cos(Rad(-45)), cy + rLabel * MathF.Sin(Rad(-45)), size);
            DrawCentered(canvas, "4ths", cx + rLabel * MathF.Cos(Rad(-135)), cy + rLabel * MathF.Sin(Rad(-135)), size);

            // The definitions, in the empty middle.
            canvas.FontSize = size * 0.034f;
            canvas.DrawString("clockwise = V→I", cx - radius, cy - size * 0.045f, radius * 2f, size * 0.06f,
                HorizontalAlignment.Center, VerticalAlignment.Center);
            canvas.FontColor = Text;
            canvas.Font = Microsoft.Maui.Graphics.Font.Default;
            canvas.DrawString("counter-clockwise = IV→I", cx - radius, cy + size * 0.005f, radius * 2f, size * 0.06f,
                HorizontalAlignment.Center, VerticalAlignment.Center);

            // Twelve key nodes, clockwise from the top: increasing screen angle sweeps
            // clockwise, which lays the flats out to the right exactly like the book.
            for (int i = 0; i < 12; i++)
            {
                float a = Rad(-90 + 30 * i);
                float x = cx + radius * MathF.Cos(a);
                float y = cy + radius * MathF.Sin(a);

                canvas.FillColor = NodeFill;
                canvas.FillCircle(x, y, rNode);
                canvas.StrokeColor = i == 0 ? Accent : Ring;
                canvas.DrawCircle(x, y, rNode);

                canvas.FontColor = i == 0 ? Accent : Text;
                canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
                canvas.FontSize = size * 0.046f;
                canvas.DrawString(KeyNames[i], x - rNode, y - rNode, rNode * 2f, rNode * 2f,
                    HorizontalAlignment.Center, VerticalAlignment.Center);

                if (Enharmonics.TryGetValue(i, out string? enh))
                {
                    canvas.FontColor = Muted;
                    canvas.Font = Microsoft.Maui.Graphics.Font.Default;
                    canvas.FontSize = size * 0.028f;
                    canvas.DrawString(enh, x - rNode * 2f, y + rNode + size * 0.004f, rNode * 4f, size * 0.045f,
                        HorizontalAlignment.Center, VerticalAlignment.Top);
                }
            }
        }

        private void DrawArrowArc(ICanvas canvas, float cx, float cy, float r, float fromDeg, float toDeg, float size)
        {
            const int samples = 24;
            var path = new PathF();
            float prevX = 0, prevY = 0, x = 0, y = 0;
            for (int i = 0; i <= samples; i++)
            {
                float a = Rad(fromDeg + (toDeg - fromDeg) * i / samples);
                prevX = x; prevY = y;
                x = cx + r * MathF.Cos(a);
                y = cy + r * MathF.Sin(a);
                if (i == 0) path.MoveTo(x, y); else path.LineTo(x, y);
            }
            canvas.StrokeColor = Accent;
            canvas.StrokeSize = MathF.Max(2f, size * 0.007f);
            canvas.DrawPath(path);

            // Arrowhead: a triangle continuing the arc's final direction.
            float dx = x - prevX, dy = y - prevY;
            float len = MathF.Sqrt(dx * dx + dy * dy);
            if (len < 0.001f) return;
            dx /= len; dy /= len;
            float head = size * 0.032f, half = size * 0.013f;
            var tip = new PathF();
            tip.MoveTo(x + dx * head, y + dy * head);
            tip.LineTo(x - dy * half, y + dx * half);
            tip.LineTo(x + dy * half, y - dx * half);
            tip.Close();
            canvas.FillColor = Accent;
            canvas.FillPath(tip);
        }

        private static void DrawCentered(ICanvas canvas, string text, float x, float y, float size) =>
            canvas.DrawString(text, x - size * 0.1f, y - size * 0.03f, size * 0.2f, size * 0.06f,
                HorizontalAlignment.Center, VerticalAlignment.Center);

        private static float Rad(float degrees) => degrees * MathF.PI / 180f;
    }
}
