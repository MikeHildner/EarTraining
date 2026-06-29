using EarTraining.App.Services;
using Microsoft.Maui.Controls.Shapes;

namespace EarTraining.App.Pages;

/// <summary>
/// The app's landing hub: branding plus a two-level, single-open accordion. The top level is the
/// Level / group (Level 1, Level 2, Other drills, Experimental) — so Home opens as a short list of
/// four rows. Opening a Level reveals its chapters (each with a one-line description); opening a
/// chapter reveals its drills. "Other drills" / "Experimental" have no chapters, so they expand
/// straight to their drill buttons. Opening a row collapses its siblings; the open row is
/// highlighted. Each drill button carries its ShellContent route in CommandParameter.
/// </summary>
public partial class HomePage : ContentPage
{
    // Theme-aware (resolve to light/dark at build time; the accordion rebuilds on theme change).
    private static Color HeaderText => Theme.Accent;
    private static Color DescText => Theme.Muted;
    private static Color CollapsedBg => Theme.CardBg;
    private static Color CollapsedStroke => Theme.CardStroke;
    private static Color OpenBg => Theme.OpenBg;
    private static Color OpenStroke => Theme.Accent;

    private sealed record Drill(string Label, string Route);
    private sealed record Item(string Title, string? Desc, string? Route, Drill[]? Drills);
    private sealed record Group(string Title, Item[] Items);

    private static Item Chapter(string title, string desc, params Drill[] drills) => new(title, desc, null, drills);
    private static Item Leaf(string title, string route) => new(title, null, route, null);

    // Top-level groups → chapters (with descriptions, from the book) → drills. Adding a chapter =
    // one entry here (plus the matching ShellContent routes in AppShell).
    private static readonly Group[] Groups =
    {
        new("Level 1 · Foundations", new[]
        {
            Chapter("Chapter 1", "Solfège & pitch", new("Vocal Drills", "l1c1vocal"), new("Resolution ID", "l1c1resolution"), new("Pitch ID", "l1c1pitch"), new("Dictation", "l1c1dictation")),
            Chapter("Chapter 2", "Maj 3rd & min 6th", new("Vocal Drills", "l1c2vocal"), new("Melodic Intervals", "l1c2melodic"), new("Harmonic Intervals", "l1c2harmonic"), new("Dictation", "l1c2dictation")),
            Chapter("Chapter 3", "Min 3rd & maj 6th", new("Vocal Drills", "l1c3vocal"), new("Melodic Intervals", "l1c3melodic"), new("Harmonic Intervals", "l1c3harmonic"), new("Triad Recognition", "l1c3triad"), new("Dictation", "l1c3dictation")),
            Chapter("Chapter 4", "I, IV, V triads", new("Melody Harmonization", "l1c4harmonize"), new("Triad Progressions", "l1c4progressions")),
            Chapter("Chapter 5", "Diatonic 4ths & 5ths", new("Vocal Drills", "l1c5vocal"), new("Melodic Intervals", "l1c5melodic"), new("Harmonic Intervals", "l1c5harmonic"), new("Triad Recognition", "l1c5triad"), new("Triad Progressions", "l1c5progressions")),
            Chapter("Chapter 6", "Maj 2nd & min 7th", new("Vocal Drills", "l1c6vocal"), new("Melodic Intervals", "l1c6melodic"), new("Harmonic Intervals", "l1c6harmonic"), new("Triad Recognition", "l1c6triad"), new("Triad Progressions", "l1c6progressions")),
            Chapter("Chapter 7", "Min 2nd & maj 7th", new("Vocal Drills", "l1c7vocal"), new("Melodic Intervals", "l1c7melodic"), new("Harmonic Intervals", "l1c7harmonic"), new("Triad Recognition", "l1c7triad"), new("Triad Progressions", "l1c7progressions")),
        }),
        new("Level 2 · Progressions", new[]
        {
            Chapter("Chapter 4", "Major triad progressions", new Drill("Major Triad Progressions", "l2c4")),
            Chapter("Chapter 5", "Diatonic triad progressions", new Drill("Diatonic Triad Progressions", "l2c5")),
        }),
        new("Not in the Books", new[]
        {
            Leaf("Interval ID", "interval"),
            Leaf("Triad ID", "triad"),
            Leaf("Find the DO", "finddo"),
            Leaf("Solfège Syllables", "solfeg"),
            Leaf("Major Triad Movements", "l2major"),
        }),
    };

    private sealed class Acc
    {
        public required Label Chevron;
        public required View Content;
        public required Border Header;
    }

    private readonly List<Acc> _groups = new();
    private readonly Dictionary<Acc, List<Acc>> _chapters = new();

    public HomePage()
    {
        InitializeComponent();
        BuildGroups();
        if (Application.Current is { } app)
            app.RequestedThemeChanged += (_, _) => Rebuild();
    }

    private void Rebuild()
    {
        ChaptersContainer.Children.Clear();
        _groups.Clear();
        _chapters.Clear();
        BuildGroups();
    }

    private void BuildGroups()
    {
        foreach (var group in Groups)
        {
            var groupContent = new VerticalStackLayout { Spacing = 8, Margin = new Thickness(10, 8, 0, 2), IsVisible = false };
            var chapterAccs = new List<Acc>();

            foreach (var item in group.Items)
            {
                if (item.Drills is { } drills)
                {
                    var drillStack = new VerticalStackLayout { Spacing = 8, Margin = new Thickness(4, 6, 4, 2), IsVisible = false };
                    foreach (var d in drills)
                    {
                        var b = new Button { Text = d.Label, CommandParameter = d.Route };
                        b.Clicked += OnDrill;
                        drillStack.Add(b);
                    }

                    var (section, acc) = BuildCard(item.Title, item.Desc, drillStack, chapter: true);
                    groupContent.Add(section);
                    chapterAccs.Add(acc);

                    var siblings = chapterAccs;
                    var self = acc;
                    var tap = new TapGestureRecognizer();
                    tap.Tapped += (_, _) => ToggleAmong(siblings, self);
                    acc.Header.GestureRecognizers.Add(tap);
                }
                else if (item.Route is { } route)
                {
                    var b = new Button { Text = item.Title, CommandParameter = route };
                    b.Clicked += OnDrill;
                    groupContent.Add(b);
                }
            }

            var (groupSection, groupAcc) = BuildCard(group.Title, null, groupContent, chapter: false, wip: WipIconConverter.IsWip(group.Title));
            ChaptersContainer.Add(groupSection);
            _groups.Add(groupAcc);
            _chapters[groupAcc] = chapterAccs;

            var captured = groupAcc;
            var groupTap = new TapGestureRecognizer();
            groupTap.Tapped += (_, _) => ToggleGroup(captured);
            groupAcc.Header.GestureRecognizers.Add(groupTap);
        }
    }

    // Builds a header card over its (already-created) content as one section. Group cards are the
    // larger top-level rows; chapter cards are smaller, indented, and carry a description line.
    private (View section, Acc acc) BuildCard(string title, string? desc, View content, bool chapter, bool wip = false)
    {
        var chevron = new Label { Text = "▸", FontSize = chapter ? 15 : 18, TextColor = HeaderText, VerticalOptions = LayoutOptions.Center };

        var titleLabel = new Label { Text = title, FontAttributes = FontAttributes.Bold, FontSize = chapter ? 14 : 16, TextColor = HeaderText };
        View titleView;
        if (desc is null)
        {
            titleLabel.VerticalOptions = LayoutOptions.Center;
            titleView = titleLabel;
        }
        else
        {
            titleView = new VerticalStackLayout
            {
                Spacing = 1,
                VerticalOptions = LayoutOptions.Center,
                Children = { titleLabel, new Label { Text = desc, FontSize = 12, TextColor = DescText } },
            };
        }

        var header = new Grid { Padding = new Thickness(chapter ? 12 : 14, chapter ? 9 : 12), ColumnSpacing = 8 };
        header.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        header.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        header.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        Grid.SetColumn(titleView, 0);
        header.Add(titleView);
        if (wip)
        {
            var badge = new Label { Text = char.ConvertFromUtf32(0xF0AD), FontFamily = "FA", FontSize = 13, TextColor = Color.FromArgb("#BA7517"), VerticalOptions = LayoutOptions.Center };
            Grid.SetColumn(badge, 1);
            header.Add(badge);
        }
        Grid.SetColumn(chevron, 2);
        header.Add(chevron);

        var border = new Border
        {
            StrokeShape = new RoundRectangle { CornerRadius = chapter ? 9 : 10 },
            Stroke = CollapsedStroke,
            StrokeThickness = 1,
            BackgroundColor = CollapsedBg,
            Content = header,
        };

        var section = new VerticalStackLayout { Spacing = 0, Margin = new Thickness(0, 0, 0, chapter ? 0 : 8) };
        section.Add(border);
        section.Add(content);

        return (section, new Acc { Chevron = chevron, Content = content, Header = border });
    }

    // Single-open at the group tier; opening/closing a group also resets all chapters to collapsed.
    private void ToggleGroup(Acc group)
    {
        bool willOpen = !group.Content.IsVisible;
        foreach (var g in _groups)
        {
            SetOpen(g, g == group && willOpen);
            foreach (var ch in _chapters[g]) SetOpen(ch, false);
        }
    }

    // Single-open among a group's chapters.
    private static void ToggleAmong(List<Acc> siblings, Acc target)
    {
        bool willOpen = !target.Content.IsVisible;
        foreach (var s in siblings) SetOpen(s, s == target && willOpen);
    }

    private static void SetOpen(Acc a, bool open)
    {
        a.Content.IsVisible = open;
        a.Chevron.Text = open ? "▾" : "▸";
        a.Header.BackgroundColor = open ? OpenBg : CollapsedBg;
        a.Header.Stroke = open ? OpenStroke : CollapsedStroke;
        a.Header.StrokeThickness = open ? 1.5 : 1;
    }

    private async void OnDrill(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: string route })
            await Shell.Current.GoToAsync($"//{route}");
    }

    private async void OnAbout(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//about");

    private async void OnProgress(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//progress");

    private async void OnSettings(object? sender, TappedEventArgs e) =>
        await Shell.Current.GoToAsync("//settings");
}
