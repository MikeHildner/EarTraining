using EarTrainingLibrary.NAudio;
using EarTrainingLibrary.Utility;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class L2Controller : BaseController
    {
        public L2Controller()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
        }

        public ActionResult MajorTriadProgressions()
        {
            return View();
        }

        public ActionResult CreateProgression(string pitchName, string chordProgression)
        {
            string chord = "";
            string inversion = "";
            MixingSampleProvider msp = CreateMajorTriad(pitchName, chord, inversion);

            ISampleProvider note = NAudioHelper.GetSampleProvider(pitchName, TimeSpan.FromSeconds(2));
            var stwp = new SampleToWaveProvider(note);
            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;

            wavStream.WavToMp3File(out string fileName);
            var path = $"temp/{fileName}";
            JsonResult jsonResult = Json(path, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
    }
}
