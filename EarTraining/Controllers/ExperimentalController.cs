using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace EarTraining.Controllers
{
    public class ExperimentalController : Controller
    {
        // GET: Experimental
        public ActionResult Osmd()
        {
            return View();
        }

        public ActionResult OsmdTest()
        {
            var dict = new Dictionary<string, string>();

            string xml = GetXml().Replace(Environment.NewLine, string.Empty);
            var theScript1 = $@"
            var osmd = new opensheetmusicdisplay.OpenSheetMusicDisplay('div-container');
            var loadPromise = osmd.load('{xml}');
            loadPromise.then(function() {{
                osmd.render();
            }});
            ";
            dict.Add("theScript1", theScript1);

            var json = Json(dict, JsonRequestBehavior.AllowGet);
            return json;

        }

        private string GetXml()
        {
            string xml = @"
<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!DOCTYPE score-partwise PUBLIC
    ""-//Recordare//DTD MusicXML 3.1 Partwise//EN""
    ""http://www.musicxml.org/dtds/partwise.dtd"">
<score-partwise version=""3.1"">
  <part-list>
    <score-part id=""P1"">
      <part-name>Music</part-name>
    </score-part>
  </part-list>
  <part id=""P1"">
    <measure number=""1"">
      <attributes>
        <divisions>1</divisions>
        <key>
          <fifths>1</fifths>
        </key>
        <time>
          <beats>4</beats>
          <beat-type>4</beat-type>
        </time>
        <clef>
          <sign>G</sign>
          <line>2</line>
        </clef>
      </attributes>
      <note>
        <pitch>
          <step>D</step>
          <octave>4</octave>
        </pitch>
        <duration>4</duration>
        <type>whole</type>
      </note>
    </measure>
  </part>
</score-partwise>
";

            return xml;
        }
    }
}
