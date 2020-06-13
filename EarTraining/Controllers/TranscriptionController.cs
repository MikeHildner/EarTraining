using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EarTraining.Controllers
{
    public class TranscriptionController : Controller
    {
        // GET: Transcription
        public ActionResult Test()
        {
            return View();
        }

        public ActionResult AudioAndDictation()
        {
            var dict = new Dictionary<string, string>();

            var script = @"
            const vf = new Vex.Flow.Factory({
                renderer: { elementId: 'boo', width: 500, height: 200 }
            });

            const score = vf.EasyScore();
            const system = vf.System();

            system.addStave({
                voices: [
                    score.voice(score.notes('C#5/q, B4, A4, G#4', { stem: 'up' })),
                    score.voice(score.notes('C#4/h, C#4', { stem: 'down' }))
                ]
            }).addClef('treble').addTimeSignature('4/4');

            vf.draw();
            ";
            
            dict.Add("theScript", script);

            var json = Json(dict, JsonRequestBehavior.AllowGet);
            return json;
        }
    }
}