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
    public class MajorOrMinorTriadController : Controller
    {
        // GET: MajorOrMinorTriad
        public ActionResult Index()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;

            return View();
        }

        public FileResult GetDO(double frequency)
        {
            Stream stream = Solfeg.GetDONote(frequency);
            return new FileStreamResult(stream, "audio/wav");
        }

        public FileResult GetChord(double frequency, int type)
        {
            TimeSpan noteDuration = TimeSpan.FromSeconds(1);
            double gain = 0.2;
            SignalGeneratorType sgType = SignalGeneratorType.SawTooth;
            var solfeg = new Solfeg(frequency);

            var note1 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
            double thirdFrequency = type == 0 ? solfeg.MiFrequency : solfeg.MaFrequency;
            var note2 = NAudioHelper.GetSampleProvider(gain, thirdFrequency, sgType, noteDuration);
            var note3 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);

            MixingSampleProvider msp = new MixingSampleProvider(note1.WaveFormat);
            msp.AddMixerInput(note1);
            msp.AddMixerInput(note2);
            msp.AddMixerInput(note3);

            var wp = msp.ToWaveProvider();
            MemoryStream ms = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(ms, wp);
            ms.Position = 0;

            return new FileStreamResult(ms, "audio/wav");
        }
    }
}