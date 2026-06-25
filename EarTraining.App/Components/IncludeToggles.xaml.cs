namespace EarTraining.App.Components;

/// <summary>
/// A row of per-pattern include switches. Pages call Build(items) with (key, label) pairs;
/// Included exposes the checked keys, and Changed fires on any toggle so the page can
/// re-filter the random drill and the quiz options.
/// </summary>
public partial class IncludeToggles : ContentView
{
    private readonly Dictionary<string, Switch> _switches = new();
    private bool _suppress;

    public event EventHandler? Changed;

    public IncludeToggles()
    {
        InitializeComponent();
    }

    public void Build(IEnumerable<(string key, string label)> items)
    {
        Container.Children.Clear();
        _switches.Clear();
        foreach (var (key, label) in items)
        {
            var toggle = new Switch { IsToggled = true };
            toggle.Toggled += (_, _) => { if (!_suppress) Changed?.Invoke(this, EventArgs.Empty); };
            _switches[key] = toggle;

            var row = new HorizontalStackLayout { Spacing = 4, Margin = new Thickness(0, 0, 14, 0) };
            row.Add(toggle);
            row.Add(new Label { Text = label, VerticalOptions = LayoutOptions.Center, FontSize = 13 });
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
