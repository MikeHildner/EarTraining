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
        var system = vf.System({{width: 320}});

        system.addStave({{
            voices: [
                score.voice(score.notes('{easyScoreNotes}', {{ stem: 'up' }})),
            ]
        }}).addClef('treble'){timeSignature}.addKeySignature('{key}');
        system.addConnector('singleLeft');
        system.addConnector('singleRight');

        vf.draw();
        ";
    }
}
