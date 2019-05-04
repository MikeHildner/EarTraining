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
    public class PitchIdentificationController : BaseController
    {
        // GET: PitchIdentification
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

        public FileResult GetPitch(double frequency, int type)
        {
            SolfegPitch solfegPitch = (SolfegPitch)type;

            TimeSpan noteDuration = TimeSpan.FromSeconds(1);
            double gain = 0.2;
            SignalGeneratorType sgType = SignalGeneratorType.Sin;
            var solfeg = new Solfeg(frequency);

            double pitchFrequency;

            switch(solfegPitch)
            {
                case SolfegPitch.Do:
                    pitchFrequency = solfeg.DoFrequency;
                    break;
                case SolfegPitch.Ra:
                    pitchFrequency = solfeg.RaFrequency;
                    break;
                case SolfegPitch.Re:
                    pitchFrequency = solfeg.ReFrequency;
                    break;
                case SolfegPitch.Ma:
                    pitchFrequency = solfeg.MaFrequency;
                    break;
                case SolfegPitch.Mi:
                    pitchFrequency = solfeg.MiFrequency;
                    break;
                case SolfegPitch.Fa:
                    pitchFrequency = solfeg.FaFrequency;
                    break;
                case SolfegPitch.Se:
                    pitchFrequency = solfeg.SeFrequency;
                    break;
                case SolfegPitch.So:
                    pitchFrequency = solfeg.SoFrequency;
                    break;
                case SolfegPitch.Le:
                    pitchFrequency = solfeg.LeFrequency;
                    break;
                case SolfegPitch.La:
                    pitchFrequency = solfeg.LaFrequency;
                    break;
                case SolfegPitch.Te:
                    pitchFrequency = solfeg.TeFrequency;
                    break;
                case SolfegPitch.Ti:
                    pitchFrequency = solfeg.TiFrequency;
                    break;
                case SolfegPitch.HighDo:
                    pitchFrequency = solfeg.HighDoFrequency;
                    break;

                default:
                    throw new NotSupportedException($"Solfeg pitch {solfegPitch} is not supported.");
            }
            var note = NAudioHelper.GetSampleProvider(gain, pitchFrequency, sgType, noteDuration);

            var stwp = new SampleToWaveProvider(note);

            MemoryStream ms = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(ms, stwp);
            ms.Position = 0;

            return new FileStreamResult(ms, "audio/wav");
        }
    }
}