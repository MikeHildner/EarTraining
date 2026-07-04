using System.Text;
using EarTraining.Core.Drills;
using EarTraining.Core.Notation;
using Microsoft.Maui.Storage;

namespace EarTraining.App.Services;

/// <summary>
/// Renders a dictation's answer as a VexFlow staff for a WebView. The bundled vexflow-min.js
/// is read once and inlined into a self-contained HTML document (one div + EasyScore script
/// per measure), so the WebView is a pure, offline drawing surface — no network and no app
/// navigation. The L1C3 dictation may contain beamed eighth-note pairs, so it uses the
/// beaming script variant.
/// </summary>
public sealed class NotationRenderer
{
    private string? _vexFlowJs;

    private async Task<string> VexFlowJsAsync()
    {
        if (_vexFlowJs is null)
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("vexflow-min.js");
            using var reader = new StreamReader(stream);
            _vexFlowJs = await reader.ReadToEndAsync();
        }
        return _vexFlowJs;
    }

    /// <summary>L1C1 dictation (quarter/half/whole — no eighth beaming).</summary>
    public Task<string> BuildHtmlAsync(DictationDrill drill) =>
        BuildHtmlAsync(drill.Measures, drill.Key, beamed: false);

    /// <summary>L1C3 dictation (may contain beamed eighth-note pairs).</summary>
    public Task<string> BuildHtmlAsync(IntervalDictationDrill drill) =>
        BuildHtmlAsync(drill.Measures, drill.Key, beamed: true);

    /// <summary>Bass-line dictation (bass clef; may contain beamed eighth-note pairs).</summary>
    public Task<string> BuildHtmlAsync(BassLineDictationDrill drill) =>
        BuildHtmlAsync(drill.Measures, drill.Key, beamed: true, clef: "bass");

    /// <summary>Blank Sheet Music: one Letter page of empty staves, one div+script per row.</summary>
    public async Task<string> BuildBlankSheetHtmlAsync(BlankSheetOptions options)
    {
        var divs = new StringBuilder();
        var scripts = new StringBuilder();
        int rows = BlankSheet.Rows(options);
        for (int i = 0; i < rows; i++)
        {
            string id = $"sheetrow{i + 1}";
            divs.Append($"<div class=\"row\" id=\"{id}\"></div>");
            scripts.Append("(function(){");
            scripts.Append(BlankSheet.RowScript(id, options, firstRow: i == 0));
            scripts.Append("})();\n");
        }

        string vexFlow = await VexFlowJsAsync();
        // Preview-only surface: the shared PDF is written by SheetPdfWriter from the rows
        // harvested out of this page, so this shell just has to display well on screen.
        return $@"<!DOCTYPE html>
<html>
<head><meta name=""viewport"" content=""width=device-width, initial-scale=1""><style>svg{{display:block;margin:0 auto;max-width:100%;height:auto}}</style></head>
<body style=""margin:0;background:#ffffff;"">
{divs}
<script>{vexFlow}</script>
<script>
try {{
{scripts}
}} catch (e) {{ document.body.innerHTML += '<pre style=""color:red"">' + e + '</pre>'; }}
</script>
</body>
</html>";
    }

    private async Task<string> BuildHtmlAsync(IReadOnlyList<DictationMeasure> measures, string key, bool beamed, string clef = "treble")
    {
        var divs = new StringBuilder();
        var scripts = new StringBuilder();
        for (int i = 0; i < measures.Count; i++)
        {
            string id = $"transcription{i + 1}";
            divs.Append($"<div id=\"{id}\"></div>");
            var m = measures[i];
            string script = beamed
                ? VexScore.EasyScoreBeamed(id, m.NoteNames, m.Rhythms, key, showTimeSignature: i == 0, clef)
                : VexScore.EasyScore(id, m.NoteNames, m.Rhythms, key, showTimeSignature: i == 0, clef);
            // Each snippet declares `const vf`/`const score`; wrap in an IIFE so concatenating
            // measures doesn't throw "Identifier 'vf' has already been declared".
            scripts.Append("(function(){");
            scripts.Append(script);
            scripts.Append("})();\n");
        }

        string vexFlow = await VexFlowJsAsync();
        return $@"<!DOCTYPE html>
<html>
<head><meta name=""viewport"" content=""width=device-width, initial-scale=1""><style>svg{{display:block;margin:0 auto;max-width:100%;height:auto}}</style></head>
<body style=""margin:0;background:#ffffff;"">
{divs}
<script>{vexFlow}</script>
<script>
try {{
{scripts}
}} catch (e) {{ document.body.innerHTML += '<pre style=""color:red"">' + e + '</pre>'; }}
</script>
</body>
</html>";
    }
}
