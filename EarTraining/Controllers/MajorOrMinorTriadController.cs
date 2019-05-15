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
    public class MajorOrMinorTriadController : BaseController
    {
        // GET: MajorOrMinorTriad
        public ActionResult Index()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;

            return View();
        }

        public ActionResult GetChord(double frequency, int type)
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
            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, wp);
            wavStream.Position = 0;

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }
    }
}