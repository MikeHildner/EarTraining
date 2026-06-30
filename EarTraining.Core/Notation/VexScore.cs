using System.Text;

namespace EarTraining.Core.Notation;

/// <summary>
/// Builds a VexFlow EasyScore script that draws one measure on a treble staff.
/// Ported verbatim from the web app's NoteHelper.GetEasyScoreScript so the same
/// bundled vexflow-min.js renders identically — here the script is eval'd inside a
/// small WebView rather than the browser DOM.
/// </summary>
public static class VexScore
{
    public static string EasyScore(string elementId, IReadOnlyList<string> noteNames, IReadOnlyList<string> rhythms, string key, bool showTimeSignature)
    {
        string timeSignature = showTimeSignature ? ".addTimeSignature('4/4')" : string.Empty;

        var sb = new StringBuilder();
        for (int i = 0; i < noteNames.Count; i++)
            sb.Append($"{noteNames[i]}/{rhythms[i]},");
        string easyScoreNotes = sb.ToString().TrimEnd(',');

        return $@"
        const vf = new Vex.Flow.Factory({{
            renderer: {{ elementId: '{elementId}' }}
        }});

        const score = vf.EasyScore();
        var system = vf.System({{width: Math.min(320, (window.innerWidth || 320) - 12)}});

        system.addStave({{
            voices: [
                score.voice(score.notes('{easyScoreNotes}', {{ stem: 'up' }})),
            ]
        }}).addClef('treble'){timeSignature}.addKeySignature('{key}');
        system.addConnector('singleLeft');
        system.addConnector('singleRight');

        vf.draw();

        // Crop the SVG to its actual content (clef + staff + notes). The Factory canvas
        // defaults to ~200px tall — far more than one treble staff needs — which otherwise
        // leaves a big empty band below each measure.
        var _el = document.getElementById('{elementId}');
        var _svg = _el ? _el.querySelector('svg') : null;
        if (_svg && _svg.getBBox) {{
            var _bb = _svg.getBBox();
            if (_bb.height > 0) {{
                var _m = 6;
                var _w = Math.ceil(_bb.width + _m * 2);
                var _h = Math.ceil(_bb.height + _m * 2);
                _svg.setAttribute('viewBox', (_bb.x - _m) + ' ' + (_bb.y - _m) + ' ' + _w + ' ' + _h);
                _svg.setAttribute('width', _w);
                _svg.setAttribute('height', _h);
            }}
        }}
        ";
    }

    /// <summary>
    /// Like <see cref="EasyScore"/> but beams consecutive eighth-note pairs (for the L1C3
    /// dictation). Ported from the web's NoteHelper.GetEasyScoreScript3 — a pair of "8"s
    /// becomes <c>score.beam(score.notes('a/8,b/8'), {{ autoStem: true }})</c>; everything
    /// else is a plain note. Same getBBox crop so measures sit snug.
    /// </summary>
    public static string EasyScoreBeamed(string elementId, IReadOnlyList<string> noteNames, IReadOnlyList<string> rhythms, string key, bool showTimeSignature)
    {
        string timeSignature = showTimeSignature ? ".addTimeSignature('4/4')" : string.Empty;

        var sb = new StringBuilder();
        for (int i = 0; i < noteNames.Count; i++)
        {
            if (i + 1 < noteNames.Count && rhythms[i] == "8" && rhythms[i + 1] == "8")
            {
                sb.Append($".concat(score.beam(score.notes('{noteNames[i]}/{rhythms[i]},{noteNames[i + 1]}/{rhythms[i + 1]}'), {{ autoStem: true }}))");
                i++;
            }
            else
            {
                sb.Append($".concat(score.notes('{noteNames[i]}/{rhythms[i]},'))");
            }
        }
        string easyScoreNotes = sb.ToString();

        return $@"
        const vf = new Vex.Flow.Factory({{
            renderer: {{ elementId: '{elementId}' }}
        }});

        const score = vf.EasyScore();
        var system = vf.System({{width: Math.min(320, (window.innerWidth || 320) - 12)}});

        system.addStave({{
            voices: [
                score.voice(score.notes('') {easyScoreNotes}),
            ]
        }}).addClef('treble'){timeSignature}.addKeySignature('{key}');
        system.addConnector('singleLeft');
        system.addConnector('singleRight');

        vf.draw();

        var _el = document.getElementById('{elementId}');
        var _svg = _el ? _el.querySelector('svg') : null;
        if (_svg && _svg.getBBox) {{
            var _bb = _svg.getBBox();
            if (_bb.height > 0) {{
                var _m = 6;
                var _w = Math.ceil(_bb.width + _m * 2);
                var _h = Math.ceil(_bb.height + _m * 2);
                _svg.setAttribute('viewBox', (_bb.x - _m) + ' ' + (_bb.y - _m) + ' ' + _w + ' ' + _h);
                _svg.setAttribute('width', _w);
                _svg.setAttribute('height', _h);
            }}
        }}
        ";
    }
}
