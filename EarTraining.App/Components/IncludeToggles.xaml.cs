using EarTraining.App.Services;

namespace EarTraining.App.Components;

/// <summary>
/// A row of per-pattern include switches. Pages call Build(items) with (key, label) pairs;
/// Included exposes the checked keys, and Changed fires on any toggle so the page can
/// re-filter the random drill and the quiz options. If a page sets <see cref="Play"/>, each
/// row also gets a ▶ that plays that specific pattern on demand (restores the website's
/// per-pattern playback) — independent of the include switch.
/// </summary>
public partial class IncludeToggles : ContentView
{
    private readonly Dictionary<string, Switch> _switches = new();
    private bool _suppress;

    public event EventHandler? Changed;

    /// <summary>Optional: when set, each row shows a ▶ that calls this with the row's key to play it.</summary>
    public Action<string>? Play { get; set; }

    public IncludeToggles()
    {
        InitializeComponent();
    }

    public void Build(IEnumerable<(string key, string label)> items)
    {
        Container.Children.Clear();
        _switches.Clear();
        Hint.IsVisible = Play is not null;
        foreach (var (key, label) in items)
        {
            var toggle = new Switch { IsToggled = true };
            toggle.Toggled += (_, _) => { if (!_suppress) Changed?.Invoke(this, EventArgs.Empty); };
            _switches[key] = toggle;

            var row = new HorizontalStackLayout { Spacing = 4, Margin = new Thickness(0, 0, 14, 0) };
            row.Add(toggle);
            row.Add(new Label { Text = label, VerticalOptions = LayoutOptions.Center, FontSize = 13 });
            if (Play is { } play)
            {
                var k = key;
                // Compact outline button (overrides the global filled Button style) so it reads as
                // a control, not a floating glyph.
                var playBtn = new Button
                {
                    Text = "▶",
                    FontSize = 13,
                    BackgroundColor = Colors.Transparent,
                    TextColor = Theme.Accent,
                    BorderColor = Theme.Accent,
                    BorderWidth = 1,
                    CornerRadius = 8,
                    Padding = new Thickness(12, 0),
                    MinimumHeightRequest = 0,
                    MinimumWidthRequest = 0,
                    HeightRequest = 32,
                    VerticalOptions = LayoutOptions.Center,
                };
                playBtn.Clicked += (_, _) => play(k);
                row.Add(playBtn);
            }
            Container.Add(row);
        }
    }

    /// <summary>Keys whose switch is on. At least one is kept on (the last one can't be turned off).</summary>
    public IReadOnlyList<string> Included
    {
        get
        {
            var on = _switches.Where(kv => kv.Value.IsToggled).Select(kv => kv.Key).ToList();
            return on.Count > 0 ? on : _switches.Keys.Take(1).ToList();
        }
    }

    /// <summary>Flip every switch (for an "Invert Selections" button); fires <see cref="Changed"/> once.</summary>
    public void InvertAll()
    {
        _suppress = true;
        foreach (var sw in _switches.Values) sw.IsToggled = !sw.IsToggled;
        _suppress = false;
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
