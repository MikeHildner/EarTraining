using System.Text;

namespace EarTraining.Core.Notation;

/// <summary>Options for a printable page of blank staves (the Blank Sheet Music page).
/// <paramref name="Clef"/> is "treble", "bass", or null for plain 5-line staves;
/// <paramref name="KeySignature"/> is a <see cref="Theory.Keys"/> name or null (and is
/// ignored when there is no clef — accidental positions need a clef); a
/// <paramref name="MeasuresPerLine"/> of 0 means classic unbarred manuscript paper.</summary>
public sealed record BlankSheetOptions(
    string? Clef,
    string? KeySignature,
    bool FourFour,
    int MeasuresPerLine,
    bool Landscape);

/// <summary>
/// Builds the VexFlow scripts for a page of blank staves — raw <c>Vex.Flow.Stave</c> API,
/// no voices and no getBBox crop (the geometry is fixed, so each row sets its own viewBox;
/// without one the <c>max-width:100%</c> scaling breaks). Sized for one Letter page with
/// 0.4-inch margins at 96 CSS px/in: the printable box is 739×979 px portrait / 979×739
/// landscape, so 10 rows × 730 px (portrait) or 7 rows × 970 px (landscape) at 94 px per
/// row. A stave at y = −6 puts its 5 lines at 34..74 within the row, leaving headroom for
/// the clef and key signature and ~0.56 in of writing room between systems.
/// </summary>
public static class BlankSheet
{
    public const int PortraitRows = 10, LandscapeRows = 7;
    public const int PortraitWidth = 730, LandscapeWidth = 970;
    public const int RowHeight = 94;
    private const int StaveY = -6;

    public static int Rows(bool landscape) => landscape ? LandscapeRows : PortraitRows;
    public static int Width(bool landscape) => landscape ? LandscapeWidth : PortraitWidth;

    /// <summary>
    /// The script for one row div. Unbarred rows are a single full-width stave with both
    /// barlines suppressed; barred rows split into equal staves whose interior junctions
    /// come from each stave's default single end barline (all begin barlines are NONE so
    /// junctions aren't double-struck). Clef/key sit on every row's first measure, per
    /// engraving convention; the 4/4 goes on the first row only.
    /// </summary>
    public static string RowScript(string elementId, BlankSheetOptions o, bool firstRow)
    {
        int w = Width(o.Landscape);
        int n = Math.Max(1, o.MeasuresPerLine);
        int mw = w / n;

        var sb = new StringBuilder();
        sb.Append($@"
        var div = document.getElementById('{elementId}');
        var renderer = new Vex.Flow.Renderer(div, Vex.Flow.Renderer.Backends.SVG);
        renderer.resize({w}, {RowHeight});
        var ctx = renderer.getContext();
");
        for (int m = 0; m < n; m++)
        {
            int x = m * mw;
            int width = m == n - 1 ? w - x : mw;   // last stave absorbs the integer-division remainder
            sb.Append($@"
        var stave{m} = new Vex.Flow.Stave({x}, {StaveY}, {width});
        stave{m}.setBegBarType(Vex.Flow.Barline.type.NONE);");
            if (o.MeasuresPerLine == 0)
                sb.Append($@"
        stave{m}.setEndBarType(Vex.Flow.Barline.type.NONE);");
            if (m == 0)
            {
                if (o.Clef is not null)
                {
                    sb.Append($@"
        stave{m}.addClef('{o.Clef}');");
                    if (o.KeySignature is not null)
                        sb.Append($@"
        stave{m}.addKeySignature('{o.KeySignature}');");
                }
                if (firstRow && o.FourFour)
                    sb.Append($@"
        stave{m}.addTimeSignature('4/4');");
            }
            sb.Append($@"
        stave{m}.setContext(ctx).draw();
");
        }
        // Same responsive-svg recipe as the dictation reveals: intrinsic width/height
        // ATTRIBUTES (print engines size the document from them — the row then fills the
        // 739px printable width) + viewBox, with the page stylesheet's max-width:100% /
        // height:auto scaling the preview by aspect ratio. VexFlow's resize() also pins an
        // INLINE style, which would letterbox the scaled row in a fixed-height box — drop it.
        sb.Append($@"
        var svg = div.querySelector('svg');
        if (svg) {{
            svg.setAttribute('viewBox', '0 0 {w} {RowHeight}');
            svg.setAttribute('width', '{w}');
            svg.setAttribute('height', '{RowHeight}');
            svg.removeAttribute('style');
        }}
");
        return sb.ToString();
    }

}
