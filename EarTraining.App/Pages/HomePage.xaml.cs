using Microsoft.Maui.Controls.Shapes;

namespace EarTraining.App.Pages;

/// <summary>
/// The app's landing hub: branding plus a single-open accordion of the chapters. Each chapter
/// is a tappable card that expands to reveal its drills (tapping another chapter collapses the
/// previous one), so Home stays a short, scannable list as chapters grow. Each drill button
/// carries its ShellContent route in CommandParameter; tapping navigates there via Shell.
/// </summary>
public partial class HomePage : ContentPage
{
    // Chapters and their drills, in display order. Adding a chapter = one entry here
    // (plus the matching ShellContent routes in AppShell).
    private static readonly (string Title, (string Label, string Route)[] Drills)[] Chapters =
    {
        ("Level 1 · Chapter 1", new[] { ("Vocal Drills", "l1c1vocal"), ("Resolution ID", "l1c1resolution"), ("Pitch ID", "l1c1pitch"), ("Dictation", "l1c1dictation") }),
        ("Level 1 · Chapter 2", new[] { ("Vocal Drills", "l1c2vocal"), ("Melodic Intervals", "l1c2melodic"), ("Harmonic Intervals", "l1c2harmonic") }),
        ("Level 1 · Chapter 3", new[] { ("Vocal Drills", "l1c3vocal"), ("Melodic Intervals", "l1c3melodic"), ("Harmonic Intervals", "l1c3harmonic"), ("Triad Recognition", "l1c3triad"), ("Dictation", "l1c3dictation") }),
        ("Level 1 · Chapter 4", new[] { ("Melody Harmonization", "l1c4harmonize"), ("Triad Progressions", "l1c4progressions") }),
        ("Other drills", new[] { ("Interval ID", "interval"), ("Triad ID", "triad") }),
    };

    private readonly List<(Label Chevron, View Content)> _sections = new();

    public HomePage()
    {
        InitializeComponent();
        BuildChapters();
    }

    private void BuildChapters()
    {
        for (int i = 0; i < Chapters.Length; i++)
        {
            var (title, drills) = Chapters[i];
            bool expanded = i == 0;   // open the first chapter by default

            var content = new VerticalStackLayout { Spacing = 8, Margin = new Thickness(4, 6, 4, 0), IsVisible = expanded };
            foreach (var (label, route) in drills)
            {
                var button = new Button { Text = label, CommandParameter = route };
                button.Clicked += OnDrill;
                content.Add(button);
            }

            var chevron = new Label
            {
                Text = expanded ? "▾" : "▸",
                FontSize = 18,
                TextColor = Color.FromArgb("#512BD4"),
                VerticalOptions = LayoutOptions.Center,
            };
            var titleLabel = new Label
            {
                Text = title,
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                TextColor = Color.FromArgb("#512BD4"),
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
                Stroke = Color.FromArgb("#E0DCF5"),
                BackgroundColor = Color.FromArgb("#F5F3FC"),
                Content = header,
            };
            int index = i;
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => Toggle(index);
            headerBorder.GestureRecognizers.Add(tap);

            var section = new VerticalStackLayout { Spacing = 0, Margin = new Thickness(0, 0, 0, 8) };
            section.Add(headerBorder);
            section.Add(content);
            ChaptersContainer.Add(section);

            _sections.Add((chevron, content));
        }
    }

    // Single-open accordion: opening a chapter collapses the others; tapping the open one closes it.
    private void Toggle(int idx)
    {
        bool willOpen = !_sections[idx].Content.IsVisible;
        for (int i = 0; i < _sections.Count; i++)
        {
            bool open = i == idx && willOpen;
            _sections[i].Content.IsVisible = open;
            _sections[i].Chevron.Text = open ? "▾" : "▸";
        }
    }

    private async void OnDrill(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: string route })
            await Shell.Current.GoToAsync($"//{route}");
    }

    private async void OnAbout(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//about");
}
