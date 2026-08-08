using EarTraining.App.Services;
using Microsoft.Maui.Controls.Shapes;

namespace EarTraining.App.Pages;

/// <summary>
/// The app's landing hub: branding plus a two-level, single-open accordion. The top level is the
/// Level / group (Level 1, Level 2, Extras) — so Home opens as a short list of three rows.
/// Opening a Level reveals its chapters (each with a one-line description); opening a chapter
/// reveals its drills. "Extras" has no chapters, so it expands straight to its drill buttons.
/// Opening a row collapses its siblings; the open row is highlighted. Each drill button carries
/// its ShellContent route in CommandParameter.
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

    private sealed record Drill(string Label, string Route, string? Keywords = null);
    private sealed record Item(string Title, string? Desc, string? Route, Drill[]? Drills, string? Keywords = null);
    private sealed record Group(string Title, Item[] Items);

    private static Item Chapter(string title, string desc, params Drill[] drills) => new(title, desc, null, drills);
    private static Item Leaf(string title, string route, string? keywords = null) => new(title, null, route, null, keywords);

    // Search keywords: the concepts a page teaches, so queries like "anticipation", "tritone",
    // or "mode" find it even though no label carries the word. Shared bases for the pages that
    // repeat across chapters; chapter-specific concepts are appended inline in the table.
    private const string KwDictation = "dictation transcription melody rhythm notation staff write";
    private const string KwDictEighths = KwDictation + " eighth notes";
    private const string KwDictDotted = KwDictEighths + " dotted";
    private const string KwDictAnticip = KwDictEighths + " anticipation anticipations syncopation";
    private const string KwBass = " bass clef";
    private const string KwVocal = "sing singing voice solfege patterns";
    private const string KwTriadRecog = "triad chord recognition inversion root I IV V";
    private const string KwTriadProg = "progression chords roman numerals I IV V";

    // Top-level groups → chapters (with descriptions, from the book) → drills. Adding a chapter =
    // one entry here (plus the matching ShellContent routes in AppShell).
    private static readonly Group[] Groups =
    {
        new("Level 1 · Foundations", new[]
        {
            Chapter("Chapter 1", "Solfège & pitch",
                new("Vocal Drills", "l1c1vocal", KwVocal),
                new("Resolution ID", "l1c1resolution", "resolve resolution tendency tones re do fa mi ti"),
                new("Pitch ID", "l1c1pitch", "pitch identify solfege syllable"),
                new("Dictation", "l1c1dictation", KwDictation)),
            Chapter("Chapter 2", "Maj 3rd & min 6th",
                new("Vocal Drills", "l1c2vocal", KwVocal),
                new("Melodic Intervals", "l1c2melodic", "interval major third minor sixth"),
                new("Harmonic Intervals", "l1c2harmonic", "interval major third minor sixth"),
                new("Dictation", "l1c2dictation", KwDictEighths)),
            Chapter("Chapter 3", "Min 3rd & maj 6th",
                new("Vocal Drills", "l1c3vocal", KwVocal),
                new("Melodic Intervals", "l1c3melodic", "interval minor third major sixth"),
                new("Harmonic Intervals", "l1c3harmonic", "interval minor third major sixth"),
                new("Triad Recognition", "l1c3triad", KwTriadRecog),
                new("Dictation", "l1c3dictation", KwDictEighths)),
            Chapter("Chapter 4", "I, IV, V triads",
                new("Melody Harmonization", "l1c4harmonize", "harmonize harmonization chord I IV V"),
                new("Triad Progressions", "l1c4progressions", KwTriadProg),
                new("Mixed Intervals", "l1c4intervals", "interval mixed major minor third sixth"),
                new("Dictation", "l1c4dictation", KwDictEighths)),
            Chapter("Chapter 5", "Diatonic 4ths & 5ths",
                new("Vocal Drills", "l1c5vocal", KwVocal),
                new("Melodic Intervals", "l1c5melodic", "interval perfect fourth fifth tritone augmented diminished"),
                new("Harmonic Intervals", "l1c5harmonic", "interval perfect fourth fifth tritone augmented diminished"),
                new("Triad Recognition", "l1c5triad", KwTriadRecog + " vi"),
                new("Triad Progressions", "l1c5progressions", KwTriadProg + " vi"),
                new("Dictation", "l1c5dictation", KwDictEighths),
                new("Bass Line Dictation", "l1c5bassline", KwDictation + KwBass)),
            Chapter("Chapter 6", "Maj 2nd & min 7th",
                new("Vocal Drills", "l1c6vocal", KwVocal),
                new("Melodic Intervals", "l1c6melodic", "interval major second minor seventh"),
                new("Harmonic Intervals", "l1c6harmonic", "interval major second minor seventh"),
                new("Triad Recognition", "l1c6triad", KwTriadRecog + " vi iii"),
                new("Triad Progressions", "l1c6progressions", KwTriadProg + " vi iii"),
                new("Dictation", "l1c6dictation", KwDictDotted),
                new("Bass Line Dictation", "l1c6bassline", KwDictDotted + KwBass)),
            Chapter("Chapter 7", "Min 2nd & maj 7th",
                new("Vocal Drills", "l1c7vocal", KwVocal),
                new("Melodic Intervals", "l1c7melodic", "interval minor second major seventh"),
                new("Harmonic Intervals", "l1c7harmonic", "interval minor second major seventh"),
                new("Triad Recognition", "l1c7triad", KwTriadRecog + " vi iii ii"),
                new("Triad Progressions", "l1c7progressions", KwTriadProg + " vi iii ii"),
                new("Dictation", "l1c7dictation", KwDictAnticip),
                new("Bass Line Dictation", "l1c7bassline", KwDictAnticip + KwBass)),
            Chapter("Chapter 8", "Review — all areas",
                new("All-Interval Review", "l1c8review", "interval review qualities tritone augmented diminished"),
                new("Dictation", "l1c8dictation", KwDictAnticip),
                new("Bass Line Dictation", "l1c8bassline", KwDictAnticip + KwBass)),
        }),
        new("Level 2 · Progressions", new[]
        {
            Chapter("Chapter 2", "The circle — 5ths & 4ths", new Drill("Circle of 5ths & 4ths", "l2c2", "circle fifths fourths fifth fourth reference terminology tetrachord clockwise explanation definition")),
            Chapter("Chapter 4", "Major triad progressions", new Drill("Major Triad Progressions", "l2c4", "major triad progression movement circle fifths fourths half-step")),
            Chapter("Chapter 5", "Diatonic triad progressions", new Drill("Diatonic Triad Progressions", "l2c5", "diatonic progression four chords roman numerals vii diminished")),
            Chapter("Chapter 6", "Modal scale recognition", new Drill("Modal Scales", "l2c6", "mode modes modal scale dorian phrygian lydian mixolydian aeolian ionian locrian relative major")),
            Chapter("Chapter 7", "II-V-I progressions",
                new Drill("The Five Movements", "l2c7movements", "five movements movement reference half-step commontone key change modulation listening cues"),
                new Drill("II-V-I Progressions", "l2c7", "jazz two five one II V I progression key change circle")),
            Chapter("Chapter 8", "7-3 melodic lines", new Drill("7-3 Lines", "l2c8", "seven three line guide tones voice leading")),
            Chapter("Chapter 9", "Vocal drills — the circle", new Drill("Vocal Drills", "l2c9", "sing circle fifths fourths fixed moveable do solfege")),
        }),
        new("Extras", new[]
        {
            Leaf("Blank Sheet Music", "blanksheet", "blank sheet staff paper manuscript print pdf share clef"),
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

    // Flat search index over the same Groups table (label + where it lives + the chapter's
    // description + curated concept keywords, so "min 6th", "anticipation", or "mode" all
    // find their pages), plus the utility pages. Words is the pre-split haystack for the
    // word-aware matching in TokenMatches.
    private sealed record SearchHit(string Label, string Context, string Route, string Haystack, string[] Words);
    private readonly List<SearchHit> _searchIndex = new();

    private static readonly char[] NonWord = [' ', '·', '-', '–', '—', ',', '.', '&', '/', '(', ')', '\''];

    public HomePage()
    {
        InitializeComponent();
        BuildGroups();
        BuildSearchIndex();
        if (Application.Current is { } app)
            app.RequestedThemeChanged += (_, _) => Rebuild();
    }

    private void Rebuild()
    {
        ChaptersContainer.Children.Clear();
        _groups.Clear();
        _chapters.Clear();
        BuildGroups();
        RenderSearch(SearchEntry.Text);   // re-render any active results in the new theme's colors
    }

    private void BuildSearchIndex()
    {
        void Add(string label, string context, string route, string haystack)
        {
            string lower = haystack.ToLowerInvariant();
            _searchIndex.Add(new(label, context, route, lower,
                lower.Split(NonWord, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray()));
        }

        foreach (var group in Groups)
        {
            string prefix = group.Title.Split('·')[0].Trim();   // "Level 1" / "Level 2" / "Extras"
            foreach (var item in group.Items)
            {
                if (item.Drills is { } drills)
                {
                    string context = $"{prefix} · {item.Title}";
                    foreach (var d in drills)
                        Add(d.Label, context, d.Route, $"{d.Label} {context} {item.Desc} {d.Keywords}");
                }
                else if (item.Route is { } route)
                {
                    Add(item.Title, prefix, route, $"{item.Title} {prefix} {item.Keywords}");
                }
            }
        }
        Add("Progress", "App", "progress", "progress stats statistics streak accuracy trend");
        Add("Settings", "App", "settings", "settings theme dark mode light volume practice key appearance");
        Add("About", "App", "about", "about version credits licenses");
    }

    // A token matches when the haystack contains it (mid-word hits like "6th"), a word starts
    // with it ("ant" -> "anticipation"), it's a slightly longer form of a word ("modes" ->
    // "mode"), or it shares a 4+ letter prefix with a word ("dictate" -> "dictation"). No
    // typo/edit-distance matching — results stay predictable.
    private static bool TokenMatches(SearchHit h, string token)
    {
        if (h.Haystack.Contains(token, StringComparison.Ordinal)) return true;
        foreach (string w in h.Words)
        {
            if (w.StartsWith(token, StringComparison.Ordinal)) return true;
            if (token.StartsWith(w, StringComparison.Ordinal) && token.Length - w.Length <= 2) return true;
            if (token.Length >= 4 && w.Length >= 4 && CommonPrefix(w, token) >= 4) return true;
        }
        return false;
    }

    private static int CommonPrefix(string a, string b)
    {
        int n = Math.Min(a.Length, b.Length), i = 0;
        while (i < n && a[i] == b[i]) i++;
        return i;
    }

    private void OnSearchChanged(object? sender, TextChangedEventArgs e) => RenderSearch(e.NewTextValue);

    private void OnSearchClear(object? sender, TappedEventArgs e) => SearchEntry.Text = "";

    private void RenderSearch(string? query)
    {
        string q = (query ?? "").Trim().ToLowerInvariant();
        bool active = q.Length > 0;
        SearchClear.IsVisible = active;
        SearchResults.IsVisible = active;
        ChaptersContainer.IsVisible = !active;
        SearchResults.Children.Clear();
        if (!active) return;

        string[] tokens = q.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var hits = _searchIndex
            .Where(h => tokens.All(t => TokenMatches(h, t)))
            .OrderBy(h => h.Label.StartsWith(q, StringComparison.OrdinalIgnoreCase) ? 0
                        : h.Label.Contains(q, StringComparison.OrdinalIgnoreCase) ? 1
                        : tokens.All(t => h.Label.ToLowerInvariant().Split(' ').Any(w => w.StartsWith(t))) ? 2 : 3)
            .Take(15)
            .ToList();

        if (hits.Count == 0)
        {
            SearchResults.Add(new Label { Text = "No drills match.", TextColor = DescText, HorizontalOptions = LayoutOptions.Center, Margin = new Thickness(0, 8) });
            return;
        }

        foreach (var hit in hits)
        {
            var row = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                Stroke = CollapsedStroke,
                StrokeThickness = 1,
                BackgroundColor = CollapsedBg,
                Padding = new Thickness(14, 10),
                Content = new VerticalStackLayout
                {
                    Spacing = 1,
                    Children =
                    {
                        new Label { Text = hit.Label, FontAttributes = FontAttributes.Bold, FontSize = 15, TextColor = HeaderText },
                        new Label { Text = hit.Context, FontSize = 12, TextColor = DescText },
                    },
                },
            };
            var route = hit.Route;
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (_, _) => await Shell.Current.GoToAsync($"//{route}");
            row.GestureRecognizers.Add(tap);
            SearchResults.Add(row);
        }
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

            var (groupSection, groupAcc) = BuildCard(group.Title, null, groupContent, chapter: false);
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
    private (View section, Acc acc) BuildCard(string title, string? desc, View content, bool chapter)
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
