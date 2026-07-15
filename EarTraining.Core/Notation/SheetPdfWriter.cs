using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EarTraining.Core.Notation;

/// <summary>
/// Writes the Blank Sheet Music PDF directly from the preview's harvested SVG rows —
/// one Letter page, pure vector, no dependencies. (The platform print pipelines were a
/// dead end: Android's WebView print adapter lays pages out at an uncontrollable
/// phone-shaped viewport, so WYSIWYG paper output has to be produced by hand.) VexFlow's
/// SVG output here is only &lt;path&gt; elements — stroked staff lines and filled glyph
/// outlines with absolute M/L/Q/C/Z data — which map one-to-one onto PDF path operators;
/// quadratics are promoted to cubics.
/// </summary>
public static class SheetPdfWriter
{
    private const double Margin = 28.8;   // 0.4 in, matching the BlankSheet layout math

    /// <summary>
    /// <paramref name="rowsHtml"/> is the harvested markup: the preview's
    /// <c>&lt;div class="row"&gt;&lt;svg …&gt;…&lt;/svg&gt;&lt;/div&gt;</c> blocks.
    /// </summary>
    public static byte[] FromHarvestedRows(string rowsHtml, bool landscape)
    {
        var svgs = Regex.Matches(rowsHtml, "<svg[^>]*>(.*?)</svg>", RegexOptions.Singleline)
            .Select(m => m.Groups[1].Value)
            .ToList();
        if (svgs.Count == 0)
            throw new InvalidOperationException("No rendered staves to write.");

        double pageW = landscape ? 792 : 612;
        double pageH = landscape ? 612 : 792;

        // Self-describing geometry: the rows' viewBox carries the logical size BlankSheet
        // derived for the chosen staves-per-page, so scaling to the printable width makes
        // the N rows fill the printable height by construction.
        var vb = Regex.Match(rowsHtml, "viewBox=\"0 0 ([0-9.]+) ([0-9.]+)\"");
        double vbW = vb.Success ? double.Parse(vb.Groups[1].Value, CultureInfo.InvariantCulture) : 730;
        double vbH = vb.Success ? double.Parse(vb.Groups[2].Value, CultureInfo.InvariantCulture) : BlankSheet.RowHeight;
        double scale = (pageW - 2 * Margin) / vbW;
        double rowPitch = vbH * scale;

        var content = new StringBuilder();
        for (int r = 0; r < svgs.Count; r++)
        {
            double ty = pageH - Margin - r * rowPitch;   // top edge of this row, y-up page space
            content.Append("q\n");
            content.Append(F(scale)).Append(" 0 0 ").Append(F(-scale)).Append(' ')
                   .Append(F(Margin)).Append(' ').Append(F(ty)).Append(" cm\n");
            foreach (Match pm in Regex.Matches(svgs[r], "<path([^>]*?)/?>", RegexOptions.Singleline))
                AppendPath(content, Attrs(pm.Groups[1].Value));
            content.Append("Q\n");
        }

        return Assemble(pageW, pageH, content.ToString());
    }

    private static Dictionary<string, string> Attrs(string tag) =>
        Regex.Matches(tag, "([a-zA-Z-]+)=\"(.*?)\"")
            .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value);

    private static void AppendPath(StringBuilder sb, Dictionary<string, string> attrs)
    {
        if (!attrs.TryGetValue("d", out string? d) || string.IsNullOrWhiteSpace(d)) return;

        string fill = attrs.GetValueOrDefault("fill", "black");
        string stroke = attrs.GetValueOrDefault("stroke", "none");
        bool doFill = fill != "none";
        bool doStroke = stroke != "none";
        if (!doFill && !doStroke) return;

        if (doFill) sb.Append(Rgb(fill)).Append(" rg\n");
        if (doStroke)
        {
            sb.Append(Rgb(stroke)).Append(" RG\n");
            sb.Append(F(ParseNum(attrs.GetValueOrDefault("stroke-width", "1")))).Append(" w\n");
        }

        AppendSegments(sb, d);

        sb.Append(doFill && doStroke ? "B" : doFill ? "f" : "S").Append('\n');
    }

    // SVG path data -> PDF path operators. Handles absolute/relative M L H V Q C Z with
    // implicit command repetition; quadratics become cubics (c1 = p0 + 2/3(q-p0), etc.).
    private static void AppendSegments(StringBuilder sb, string d)
    {
        var nums = new List<double>();
        char cmd = ' ';
        double cx = 0, cy = 0, sx = 0, sy = 0;
        int i = 0;

        void Emit()
        {
            int k = 0;
            while (k < nums.Count)
            {
                bool rel = char.IsLower(cmd);
                switch (char.ToUpperInvariant(cmd))
                {
                    case 'M':
                        if (k + 1 >= nums.Count) { k = nums.Count; break; }
                        cx = rel ? cx + nums[k] : nums[k];
                        cy = rel ? cy + nums[k + 1] : nums[k + 1];
                        sx = cx; sy = cy;
                        sb.Append(F(cx)).Append(' ').Append(F(cy)).Append(" m\n");
                        k += 2;
                        cmd = rel ? 'l' : 'L';   // subsequent pairs are implicit LineTos
                        break;
                    case 'L':
                        if (k + 1 >= nums.Count) { k = nums.Count; break; }
                        cx = rel ? cx + nums[k] : nums[k];
                        cy = rel ? cy + nums[k + 1] : nums[k + 1];
                        sb.Append(F(cx)).Append(' ').Append(F(cy)).Append(" l\n");
                        k += 2;
                        break;
                    case 'H':
                        cx = rel ? cx + nums[k] : nums[k];
                        sb.Append(F(cx)).Append(' ').Append(F(cy)).Append(" l\n");
                        k += 1;
                        break;
                    case 'V':
                        cy = rel ? cy + nums[k] : nums[k];
                        sb.Append(F(cx)).Append(' ').Append(F(cy)).Append(" l\n");
                        k += 1;
                        break;
                    case 'C':
                        if (k + 5 >= nums.Count) { k = nums.Count; break; }
                        double c1x = rel ? cx + nums[k] : nums[k], c1y = rel ? cy + nums[k + 1] : nums[k + 1];
                        double c2x = rel ? cx + nums[k + 2] : nums[k + 2], c2y = rel ? cy + nums[k + 3] : nums[k + 3];
                        double ex = rel ? cx + nums[k + 4] : nums[k + 4], ey = rel ? cy + nums[k + 5] : nums[k + 5];
                        sb.Append(F(c1x)).Append(' ').Append(F(c1y)).Append(' ')
                          .Append(F(c2x)).Append(' ').Append(F(c2y)).Append(' ')
                          .Append(F(ex)).Append(' ').Append(F(ey)).Append(" c\n");
                        cx = ex; cy = ey;
                        k += 6;
                        break;
                    case 'Q':
                        if (k + 3 >= nums.Count) { k = nums.Count; break; }
                        double qx = rel ? cx + nums[k] : nums[k], qy = rel ? cy + nums[k + 1] : nums[k + 1];
                        double px = rel ? cx + nums[k + 2] : nums[k + 2], py = rel ? cy + nums[k + 3] : nums[k + 3];
                        double a1x = cx + 2.0 / 3.0 * (qx - cx), a1y = cy + 2.0 / 3.0 * (qy - cy);
                        double a2x = px + 2.0 / 3.0 * (qx - px), a2y = py + 2.0 / 3.0 * (qy - py);
                        sb.Append(F(a1x)).Append(' ').Append(F(a1y)).Append(' ')
                          .Append(F(a2x)).Append(' ').Append(F(a2y)).Append(' ')
                          .Append(F(px)).Append(' ').Append(F(py)).Append(" c\n");
                        cx = px; cy = py;
                        k += 4;
                        break;
                    default:
                        k = nums.Count;   // unsupported command: skip its numbers
                        break;
                }
            }
            nums.Clear();
        }

        while (i < d.Length)
        {
            char ch = d[i];
            if (char.IsLetter(ch))
            {
                Emit();
                if (char.ToUpperInvariant(ch) == 'Z')
                {
                    sb.Append("h\n");
                    cx = sx; cy = sy;
                    cmd = ' ';
                }
                else
                {
                    cmd = ch;
                }
                i++;
            }
            else if (ch is '-' or '+' or '.' || char.IsDigit(ch))
            {
                int start = i;
                i++;
                while (i < d.Length && (char.IsDigit(d[i]) || d[i] == '.' || d[i] == 'e' || d[i] == 'E' ||
                       ((d[i] == '-' || d[i] == '+') && (d[i - 1] == 'e' || d[i - 1] == 'E'))))
                    i++;
                nums.Add(double.Parse(d[start..i], CultureInfo.InvariantCulture));
            }
            else
            {
                i++;   // separators
            }
        }
        Emit();
    }

    private static double ParseNum(string s) =>
        double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double v) ? v : 1;

    private static string Rgb(string color)
    {
        double r = 0, g = 0, b = 0;
        if (color.StartsWith('#') && (color.Length == 7))
        {
            r = Convert.ToInt32(color.Substring(1, 2), 16) / 255.0;
            g = Convert.ToInt32(color.Substring(3, 2), 16) / 255.0;
            b = Convert.ToInt32(color.Substring(5, 2), 16) / 255.0;
        }
        // named colors in VexFlow output: "black" (and "white" never occurs) -> default 0,0,0
        return $"{F(r)} {F(g)} {F(b)}";
    }

    private static string F(double v) => v.ToString("0.###", CultureInfo.InvariantCulture);

    // Minimal single-page PDF: catalog, page tree, page, content stream. No fonts, no
    // compression — the staves are a few KB of operators and every viewer handles it.
    private static byte[] Assemble(double pageW, double pageH, string content)
    {
        byte[] stream = Encoding.ASCII.GetBytes(content);
        var sb = new StringBuilder();
        var offsets = new long[5];

        sb.Append("%PDF-1.4\n");
        offsets[1] = sb.Length;
        sb.Append("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n");
        offsets[2] = sb.Length;
        sb.Append("2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n");
        offsets[3] = sb.Length;
        sb.Append("3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 ")
          .Append(F(pageW)).Append(' ').Append(F(pageH))
          .Append("] /Contents 4 0 R /Resources << >> >>\nendobj\n");
        offsets[4] = sb.Length;
        sb.Append("4 0 obj\n<< /Length ").Append(stream.Length).Append(" >>\nstream\n");
        sb.Append(content);
        sb.Append("\nendstream\nendobj\n");

        long xref = sb.Length;
        sb.Append("xref\n0 5\n0000000000 65535 f \n");
        for (int i = 1; i <= 4; i++)
            sb.Append(offsets[i].ToString("0000000000", CultureInfo.InvariantCulture)).Append(" 00000 n \n");
        sb.Append("trailer\n<< /Size 5 /Root 1 0 R >>\nstartxref\n").Append(xref).Append("\n%%EOF");

        return Encoding.ASCII.GetBytes(sb.ToString());
    }
}
