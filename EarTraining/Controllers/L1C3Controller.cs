using EarTrainingLibrary.Enums;
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
    public class L1C3Controller : BaseController
    {
        // GET: L1C3
        public ActionResult Index()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;

            return View();
        }

        public FileResult GetTriad(double frequency, int triadtype, int inversion)
        {
            var triadType = (TriadType)triadtype;
            var inversionType = (InversionType)inversion;

            TimeSpan noteDuration = TimeSpan.FromSeconds(1);
            double gain = 0.2;
            SignalGeneratorType sgType = SignalGeneratorType.SawTooth;
            var solfeg = new Solfeg(frequency);

            ISampleProvider[] samples;
            switch(triadType)
            {
                case TriadType.OneMajorTriad:
                    samples = Inversion.CreateInversion(inversionType, gain, noteDuration, sgType, solfeg.DoFrequency, solfeg.MiFrequency, solfeg.SoFrequency);
                    break;

                case TriadType.FourMajorTriad:
                    samples = Inversion.CreateInversion(inversionType, gain, noteDuration, sgType, solfeg.FaFrequency, solfeg.LaFrequency, solfeg.SoFrequency);
                    break;

                case TriadType.FiveMajorTriad:
                    samples = Inversion.CreateInversion(inversionType, gain, noteDuration, sgType, solfeg.SoFrequency, solfeg.TiFrequency, solfeg.HighReFrequency);
                    break;

                default:
                    throw new NotSupportedException($"TriadType {triadType} is not supported.");
            }

            MixingSampleProvider msp = new MixingSampleProvider(samples[0].WaveFormat);
            msp.AddMixerInput(samples[0]);
            msp.AddMixerInput(samples[1]);
            msp.AddMixerInput(samples[2]);

            IWaveProvider wp = msp.ToWaveProvider();
            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, wp);
            wavStream.Position = 0;
            MemoryStream mp4Stream = wavStream.WavToMp4();

            return new FileStreamResult(mp4Stream, "audio/mpeg");
        }
    }
}