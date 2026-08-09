using System.Text;
using EarTraining.App.Services;
using EarTraining.Core.Notation;
using EarTraining.Core.Theory;

namespace EarTraining.App.Pages;

/// <summary>
/// Blank Sheet Music (Extras): options + a live preview of one Letter page of empty
/// staves, and a Share button that renders the same page to a vector PDF (see
/// <see cref="SheetPdf"/>) and hands it to the system share sheet — print, email, text, save.
/// </summary>
public partial class BlankSheetPage : ContentPage
{
    private readonly NotationRenderer _notation = new();
    private bool _previewReady;
    private bool _suppress;   // guards the re-entrant SelectedIndexChanged while relabelling Staves

    // VexFlow clef names, keyed by the picker's label. "None" = plain 5-line staves.
    // Alto covers viola, tenor covers cello/trombone/bassoon — the app isn't just for
    // piano, guitar and bass players.
    private static readonly (string Label, string? Clef)[] Clefs =
    [
        ("None", null), ("Treble", "treble"), ("Bass", "bass"), ("Alto", "alto"), ("Tenor", "tenor"),
    ];

    public BlankSheetPage()
    {
        InitializeComponent();

        ClefPicker.ItemsSource = Clefs.Select(c => c.Label).ToList();
        ClefPicker.SelectedIndex = 0;       // start clean: no clef, no key
        KeyPicker.ItemsSource = new[] { "None" }.Concat(Keys.All).ToList();
        KeyPicker.SelectedIndex = 0;
        KeyPicker.IsEnabled = false;        // no clef yet, so nothing for a key signature to sit on
        TimePicker.ItemsSource = new List<string> { "None", "4/4" };
        TimePicker.SelectedIndex = 0;
        MeasuresPicker.ItemsSource = new List<string> { "Off", "2", "4" };
        MeasuresPicker.SelectedIndex = 0;   // default: classic unbarred manuscript
        BuildStavesPicker(0);               // "Default (10)" portrait / "Default (7)" landscape

        ClefPicker.SelectedIndexChanged += OnOptionChanged;
        KeyPicker.SelectedIndexChanged += OnOptionChanged;
        TimePicker.SelectedIndexChanged += OnOptionChanged;
        MeasuresPicker.SelectedIndexChanged += OnOptionChanged;
        StavesPicker.SelectedIndexChanged += OnOptionChanged;
        LandscapeSwitch.Toggled += OnOptionChanged;

        RebuildPreview();
    }

    private string? SelectedClef =>
        ClefPicker.SelectedIndex >= 0 ? Clefs[ClefPicker.SelectedIndex].Clef : null;

    private BlankSheetOptions Options => new(
        Clef: SelectedClef,
        KeySignature: SelectedClef is null || KeyPicker.SelectedIndex <= 0
            ? null
            : (string)KeyPicker.SelectedItem,
        FourFour: TimePicker.SelectedIndex == 1,
        MeasuresPerLine: MeasuresPicker.SelectedIndex switch { 1 => 2, 2 => 4, _ => 0 },
        Landscape: LandscapeSwitch.IsToggled,
        StavesPerPage: StavesPicker.SelectedIndex <= 0 ? 0 : int.Parse((string)StavesPicker.SelectedItem));

    // The first entry names the number it actually produces, so nobody has to guess what
    // "Default" means — and it follows the orientation (Core: 10 portrait, 7 landscape).
    private void BuildStavesPicker(int selectedIndex)
    {
        _suppress = true;
        StavesPicker.ItemsSource = new List<string>
        {
            LandscapeSwitch.IsToggled ? "Default (7)" : "Default (10)", "4", "5", "6", "7", "8", "10", "12",
        };
        StavesPicker.SelectedIndex = selectedIndex;
        _suppress = false;
    }

    private void OnOptionChanged(object? sender, EventArgs e)
    {
        if (_suppress) return;

        // A key signature needs a clef to sit on: Clef "None" disables the key picker and
        // snaps it to None (the re-entrant SelectedIndexChanged does the rebuild).
        bool clefless = SelectedClef is null;
        KeyPicker.IsEnabled = !clefless;
        if (clefless && KeyPicker.SelectedIndex != 0) { KeyPicker.SelectedIndex = 0; return; }

        // Flipping orientation changes what "Default" means; relabel it, keeping the choice.
        if (sender == LandscapeSwitch) BuildStavesPicker(StavesPicker.SelectedIndex);

        RebuildPreview();
    }

    private async void RebuildPreview()
    {
        try
        {
            _previewReady = false;
            StatusLabel.Text = string.Empty;
            string html = await _notation.BuildBlankSheetHtmlAsync(Options);
            NotationWeb.Source = new HtmlWebViewSource { Html = html };
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "Preview error: " + ex.Message;
        }
    }

    private async void OnNavigated(object? sender, WebNavigatedEventArgs e)
    {
        _previewReady = true;
        try
        {
            var result = await NotationWeb.EvaluateJavaScriptAsync("document.body.scrollHeight");
            if (int.TryParse(result, out int px) && px > 0)
                NotationWeb.HeightRequest = px + 8;
        }
        catch { /* keep the estimate */ }
    }

    private async void OnSharePdf(object? sender, EventArgs e)
    {
        if (!_previewReady)
        {
            StatusLabel.Text = "The preview is still rendering — try again in a moment.";
            return;
        }
        try
        {
            ShareButton.IsEnabled = false;
            StatusLabel.Text = "Creating PDF…";
            string rowsHtml = await HarvestRenderedRowsAsync();
            string path = await SheetPdf.RenderAsync(rowsHtml, LandscapeSwitch.IsToggled, "blank-sheet-music.pdf");
            StatusLabel.Text = string.Empty;
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Blank Sheet Music",
                File = new ShareFile(path),
            });
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "PDF error: " + ex.Message;
        }
        finally
        {
            ShareButton.IsEnabled = true;
        }
    }

    // Serialize the rendered SVG rows out of the preview. The base64 round-trip sidesteps
    // EvaluateJavaScriptAsync's quote/backslash mangling on large markup results; it also
    // proves VexFlow actually drew before we commit a PDF on either platform.
    private async Task<string> HarvestRenderedRowsAsync()
    {
        const string js =
            "(function(){var r=document.querySelectorAll('.row');var h='';" +
            "for(var i=0;i<r.length;i++){h+='<div class=\"row\">'+r[i].innerHTML+'</div>';}" +
            "return btoa(unescape(encodeURIComponent(h)));})()";
        string? b64 = (await NotationWeb.EvaluateJavaScriptAsync(js))?.Trim('"');
        if (string.IsNullOrEmpty(b64))
            throw new InvalidOperationException("The preview markup could not be captured.");
        string html = Encoding.UTF8.GetString(Convert.FromBase64String(b64));
        if (!html.Contains("<svg"))
            throw new InvalidOperationException("The preview has no rendered staves.");
        return html;
    }
}
