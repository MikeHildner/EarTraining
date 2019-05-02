using EarTrainingLibrary.Enums;
using EarTrainingLibrary.NAudio;
using EarTrainingLibrary.Utility;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class L1C1Controller : Controller
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
            var doNote = new SignalGenerator()
            {
                Gain = 0.2,
                Frequency = frequency,
                Type = SignalGeneratorType.Sin
            };

            var phrase = doNote.Take(TimeSpan.FromSeconds(1));

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream ms = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(ms, stwp);
            ms.Position = 0;
            return new FileStreamResult(ms, "audio/wav");
        }
        public FileResult GetResolution(double frequency, int type)
        {
            ResolutionType rt = (ResolutionType)type;
            MemoryStream ms = GetResolution(frequency, rt);
            return new FileStreamResult(ms, "audio/wav");
        }

        private MemoryStream GetResolution(double frequency, ResolutionType resolutionType)
        {
            double bpm = 100;
            double quarterNotemillis = (bpm / 60) * 1000;
            TimeSpan noteDuration = TimeSpan.FromMilliseconds(quarterNotemillis);
            SignalGeneratorType sgType = SignalGeneratorType.Sin;
            double gain = 0.2;

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

    //        var phrase = note1.Take(noteDuration)
    //.FollowedBy(shortRest)
    //.FollowedBy(note2.Take(noteDuration))
    //.FollowedBy(shortRest)
    //.FollowedBy(note3.Take(noteDuration))
    //.FollowedBy(shortRest)
    //.FollowedBy(note4.Take(noteDuration));

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream ms = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(ms, stwp);
            ms.Position = 0;

            return ms;
        }

        public FileResult MixingTest(double frequency)
        {
            var solfeg = new Solfeg(frequency);
            double gain = 0.2;
            var noteDuration = TimeSpan.FromSeconds(5);

            var note1 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, SignalGeneratorType.Sin, noteDuration);
            var note2 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, SignalGeneratorType.SawTooth, noteDuration);

            MixingSampleProvider msp = new MixingSampleProvider(note1.WaveFormat);
            msp.AddMixerInput(note1);
            msp.AddMixerInput(note2);
            

            var wp = msp.ToWaveProvider();
            MemoryStream ms = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(ms, wp);
            ms.Position = 0;

            return new FileStreamResult(ms, "audio/wav");
        }
    }
}