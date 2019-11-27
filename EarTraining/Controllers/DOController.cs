using EarTrainingLibrary.NAudio;
using EarTrainingLibrary.Utility;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using System.Media;
using System.Web.Mvc;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class DOController : BaseController
    {
        // GET: DO
        public ActionResult Index()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
            ViewBag.ShowDo = true;

            return View();
        }

        public ActionResult GetDOEx(string doNoteName)
        {
            ISampleProvider note = NAudioHelper.GetSampleProvider(doNoteName, TimeSpan.FromSeconds(2));
            var stwp = new SampleToWaveProvider(note);
            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        public ActionResult GetNewDO()
        {
            Pitch pitch = new Pitches().Random();
            JsonResult jsonResult = Json(pitch, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
    }
}