using Microsoft.Maui.Controls.Shapes;

namespace EarTraining.App.Pages;

/// <summary>
/// The app's landing hub: branding plus a single-open accordion of the chapters. Each chapter
/// is a tappable card that expands to reveal its drills; opening one collapses the others, and
/// the open card is highlighted. All chapters start collapsed, so Home stays a short, scannable
/// list as chapters grow. Each drill button carries its ShellContent route in CommandParameter.
/// </summary>
public partial class HomePage : ContentPage
{
    private static readonly Color HeaderText = Color.FromArgb("#512BD4");
    private static readonly Color CollapsedBg = Color.FromArgb("#F5F3FC");
    private static readonly Color CollapsedStroke = Color.FromArgb("#E0DCF5");
    private static readonly Color OpenBg = Color.FromArgb("#ECE6FB");
    private static readonly Color OpenStroke = Color.FromArgb("#512BD4");

    // Chapters and their drills, in display order. Adding a chapter = one entry here
    // (plus the matching ShellContent routes in AppShell).
    private static readonly (string Title, (string Label, string Route)[] Drills)[] Chapters =
    {
        ("Level 1 · Chapter 1", new[] { ("Vocal Drills", "l1c1vocal"), ("Resolution ID", "l1c1resolution"), ("Pitch ID", "l1c1pitch"), ("Dictation", "l1c1dictation") }),
        ("Level 1 · Chapter 2", new[] { ("Vocal Drills", "l1c2vocal"), ("Melodic Intervals", "l1c2melodic"), ("Harmonic Intervals", "l1c2harmonic"), ("Dictation", "l1c2dictation") }),
        ("Level 1 · Chapter 3", new[] { ("Vocal Drills", "l1c3vocal"), ("Melodic Intervals", "l1c3melodic"), ("Harmonic Intervals", "l1c3harmonic"), ("Triad Recognition", "l1c3triad"), ("Dictation", "l1c3dictation") }),
        ("Level 1 · Chapter 4", new[] { ("Melody Harmonization", "l1c4harmonize"), ("Triad Progressions", "l1c4progressions") }),
        ("Level 1 · Chapter 5", new[] { ("Vocal Drills", "l1c5vocal"), ("Melodic Intervals", "l1c5melodic"), ("Harmonic Intervals", "l1c5harmonic"), ("Triad Recognition", "l1c5triad"), ("Triad Progressions", "l1c5progressions") }),
        ("Level 1 · Chapter 6", new[] { ("Vocal Drills", "l1c6vocal"), ("Melodic Intervals", "l1c6melodic"), ("Harmonic Intervals", "l1c6harmonic"), ("Triad Recognition", "l1c6triad"), ("Triad Progressions", "l1c6progressions") }),
        ("Level 1 · Chapter 7", new[] { ("Vocal Drills", "l1c7vocal"), ("Melodic Intervals", "l1c7melodic"), ("Harmonic Intervals", "l1c7harmonic"), ("Triad Recognition", "l1c7triad"), ("Triad Progressions", "l1c7progressions") }),
        ("Level 2", new[] { ("Major Triad Progressions", "l2c4"), ("Major Triad Movements", "l2major"), ("Diatonic Progressions", "l2c5") }),
        ("Other drills", new[] { ("Interval ID", "interval"), ("Triad ID", "triad") }),
    };

    private readonly List<(Label Chevron, View Content, Border Header)> _sections = new();

    public HomePage()
    {
        InitializeComponent();
        BuildChapters();
    }

    private void BuildChapters()
    {
        foreach (var (title, drills) in Chapters)
        {
            var content = new VerticalStackLayout { Spacing = 8, Margin = new Thickness(4, 6, 4, 0), IsVisible = false };
            foreach (var (label, route) in drills)
            {
                var button = new Button { Text = label, CommandParameter = route };
                button.Clicked += OnDrill;
                content.Add(button);
            }

            var chevron = new Label { Text = "▸", FontSize = 18, TextColor = HeaderText, VerticalOptions = LayoutOptions.Center };
            var titleLabel = new Label
            {
                Text = title,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                TextColor = HeaderText,
                VerticalOptions = LayoutOptions.Center,
            };

            var header = new Grid { Padding = new Thickness(14, 12), ColumnSpacing = 8 };
            header.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            header.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            Grid.SetColumn(titleLabel, 0);
            Grid.SetColumn(chevron, 1);
            header.Add(titleLabel);
            header.Add(chevron);

            var headerBorder = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                Stroke = CollapsedStroke,
                StrokeThickness = 1,
                BackgroundColor = CollapsedBg,
                Content = header,
            };
            int index = _sections.Count;
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => Toggle(index);
            headerBorder.GestureRecognizers.Add(tap);

            var section = new VerticalStackLayout { Spacing = 0, Margin = new Thickness(0, 0, 0, 8) };
            section.Add(headerBorder);
            section.Add(content);
            ChaptersContainer.Add(section);

            _sections.Add((chevron, content, headerBorder));
        }
    }

    // Single-open accordion: opening a chapter collapses the others; tapping the open one closes it.
    private void Toggle(int idx)
    {
        bool willOpen = !_sections[idx].Content.IsVisible;
        for (int i = 0; i < _sections.Count; i++)
            SetOpen(i, i == idx && willOpen);
    }

    private void SetOpen(int i, bool open)
    {
        var (chevron, content, header) = _sections[i];
        content.IsVisible = open;
        chevron.Text = open ? "▾" : "▸";
        header.BackgroundColor = open ? OpenBg : CollapsedBg;
        header.Stroke = open ? OpenStroke : CollapsedStroke;
        header.StrokeThickness = open ? 1.5 : 1;
    }

    private async void OnDrill(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: string route })
            await Shell.Current.GoToAsync($"//{route}");
    }

    private async void OnAbout(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//about");
}
