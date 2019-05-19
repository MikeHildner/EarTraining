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
    public class L1C2Controller : Controller
    {
        // GET: L1C2
        public ActionResult Index()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;

            return View();
        }

        public ActionResult GetDrill(double frequency, int type)
        {
            L1C2DrillType drillType = (L1C2DrillType)type;
            var retMs = new MemoryStream();
            MemoryStream wavStream = GetDrill(frequency, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private MemoryStream GetDrill(double frequency, L1C2DrillType drillType)
        {
            double bpm = 100;
            double quarterNotemillis = (bpm / 60) * 1000;
            TimeSpan noteDuration = TimeSpan.FromMilliseconds(quarterNotemillis);
            double gain = 0.2;
            SignalGeneratorType sgType = SignalGeneratorType.SawTooth;

            ISampleProvider shortRest = new SignalGenerator()
            {
                Gain = 0
            }.Take(TimeSpan.FromMilliseconds(200));

            ISampleProvider note1, note2, note3, note4, note5;

            Solfeg solfeg = new Solfeg(frequency);

            switch (drillType)
            {
                // Major 3rd intervals.
                case L1C2DrillType.DoDoMiMiMi3:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    break;

                case L1C2DrillType.FaFaLaLaSo3:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.LaFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.LaFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    break;

                case L1C2DrillType.SoSoTiTiDo3:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.TiFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.TiFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.HighDoFrequency, sgType, noteDuration);
                    break;

                case L1C2DrillType.TiTiSoSoSo3:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.TiFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.TiFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    break;

                case L1C2DrillType.LaLaFaFaMi3:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.LaFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.LaFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    break;

                case L1C2DrillType.MiMiDoDoDo3:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    break;

                // Minor 6th intervals.
                case L1C2DrillType.MiMiDoDoDo6:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.HighDoFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.HighDoFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.HighDoFrequency, sgType, noteDuration);
                    break;

                case L1C2DrillType.LaLaFaFaMi6:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.LowLaFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.LowLaFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    break;

                case L1C2DrillType.TiTiSoSoSo6:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.LowTiFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.LowTiFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    break;

                case L1C2DrillType.SoSoTiTiDo6:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.LowTiFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.LowTiFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    break;

                case L1C2DrillType.FaFaLaLaSo6:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.LowLaFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.LowLaFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.LowSoFrequency, sgType, noteDuration);
                    break;

                case L1C2DrillType.DoDoMiMiMi6:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.HighDoFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.HighDoFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    break;

                default:
                    throw new NotSupportedException($"L1C2DrillType '{drillType}' is not supported.");
            }

            var phrase = note1
                .FollowedBy(shortRest)
                .FollowedBy(note2)
                .FollowedBy(shortRest)
                .FollowedBy(note3)
                .FollowedBy(shortRest)
                .FollowedBy(note4)
                .FollowedBy(shortRest)
                .FollowedBy(note5);

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            return wavStream;
        }
    }
}
