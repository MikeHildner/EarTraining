using EarTrainingLibrary.Enums;
using EarTrainingLibrary.NAudio;
using EarTrainingLibrary.Utility;
using NAudio.Lame;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class L1C1Controller : BaseController
    {
        // GET: L1C1
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

        public FileResult GetResolution(double frequency, int type)
        {
            ResolutionType rt = (ResolutionType)type;
            var retMs = new MemoryStream();
            MemoryStream ms = GetResolution(frequency, rt);
            var rdr = new WaveFileReader(ms);
            CheckAddBinPath();
            var wtr = new LameMP3FileWriter(retMs, rdr.WaveFormat, 128);
            rdr.CopyTo(wtr);
            retMs.Position = 0;
            return new FileStreamResult(retMs, "audio/mpeg");
        }

        private MemoryStream GetResolution(double frequency, ResolutionType resolutionType)
        {
            double bpm = 100;
            double quarterNotemillis = (bpm / 60) * 1000;
            TimeSpan noteDuration = TimeSpan.FromMilliseconds(quarterNotemillis);
            double gain = 0.2;
            SignalGeneratorType sgType = SignalGeneratorType.Sin;

            ISampleProvider shortRest = new SignalGenerator()
            {
                Gain = 0
            }.Take(TimeSpan.FromMilliseconds(200));

            ISampleProvider note1;
            ISampleProvider note2;
            ISampleProvider note3;
            ISampleProvider note4;

            Solfeg solfeg = new Solfeg(frequency);

            switch (resolutionType)
            {

                case ResolutionType.DoDoReDo:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.ReFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    break;

                case ResolutionType.DoDoFaMi:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    break;

                case ResolutionType.DoDoLaSo:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.LaFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    break;

                case ResolutionType.DoDoTiHighDo:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.TiFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.HighDoFrequency, sgType, noteDuration);
                    break;

                case ResolutionType.DoDoLowTiDo:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.LowTiFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    break;

                default:
                    throw new NotSupportedException($"ResolutionType '{resolutionType}' is not supported.");
            }

            var phrase = note1
                .FollowedBy(shortRest)
                .FollowedBy(note2)
                .FollowedBy(shortRest)
                .FollowedBy(note3)
                .FollowedBy(shortRest)
                .FollowedBy(note4);

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream ms = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(ms, stwp);
            ms.Position = 0;

            return ms;
        }

        public static void CheckAddBinPath()
        {
            // find path to 'bin' folder
            var binPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "bin" });
            // get current search path from environment
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";

            // add 'bin' folder to search path if not already present
            if (!path.Split(Path.PathSeparator).Contains(binPath, StringComparer.CurrentCultureIgnoreCase))
            {
                path = string.Join(Path.PathSeparator.ToString(), new string[] { path, binPath });
                Environment.SetEnvironmentVariable("PATH", path);
            }
        }
    }
}