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
    public class L1C2Controller : BaseController
    {
        // GET: L1C2
        public ActionResult Index()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;

            return View();
        }

        public ActionResult MelodicMaj3rdMin6th()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;

            return View();
        }

        public ActionResult HarmonicMaj3rdMin6th()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;

            return View();
        }

        public ActionResult GetDrill(double frequency, int type)
        {
            L1C2MelodicDrillType drillType = (L1C2MelodicDrillType)type;
            var retMs = new MemoryStream();
            MemoryStream wavStream = GetDrill(frequency, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        public ActionResult GetMelodicDrillEx(string doNoteName, int type)
        {
            L1C2MelodicDrillType drillType = (L1C2MelodicDrillType)type;
            var retMs = new MemoryStream();
            MemoryStream wavStream = GetMelodicDrillEx(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        public ActionResult GetHarmonicDrillEx(string doNoteName, int type)
        {
            L1C2HarmonicDrillType drillType = (L1C2HarmonicDrillType)type;
            var retMs = new MemoryStream();
            MemoryStream wavStream = GetHarmonicDrillEx(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private MemoryStream GetDrill(double frequency, L1C2MelodicDrillType drillType)
        {
            double bpm = 100;
            double quarterNotemillis = (60 / bpm) * 1000;
            TimeSpan noteDuration = TimeSpan.FromMilliseconds(quarterNotemillis);
            double gain = 0.2;
            SignalGeneratorType sgType = SignalGeneratorType.SawTooth;

             ISampleProvider note1, note2, note3, note4, note5;

            Solfeg solfeg = new Solfeg(frequency);

            switch (drillType)
            {
                // Major 3rd intervals.
                case L1C2MelodicDrillType.DoDoMiMiMi3:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    break;

                case L1C2MelodicDrillType.FaFaLaLaSo3:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.LaFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.LaFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    break;

                case L1C2MelodicDrillType.SoSoTiTiDo3:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.TiFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.TiFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.HighDoFrequency, sgType, noteDuration);
                    break;

                case L1C2MelodicDrillType.TiTiSoSoSo3:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.TiFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.TiFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    break;

                case L1C2MelodicDrillType.LaLaFaFaMi3:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.LaFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.LaFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    break;

                case L1C2MelodicDrillType.MiMiDoDoDo3:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    break;

                // Minor 6th intervals.
                case L1C2MelodicDrillType.MiMiDoDoDo6:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.HighDoFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.HighDoFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.HighDoFrequency, sgType, noteDuration);
                    break;

                case L1C2MelodicDrillType.LaLaFaFaMi6:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.LowLaFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.LowLaFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration);
                    break;

                case L1C2MelodicDrillType.TiTiSoSoSo6:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.LowTiFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.LowTiFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    break;

                case L1C2MelodicDrillType.SoSoTiTiDo6:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.LowTiFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.LowTiFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration);
                    break;

                case L1C2MelodicDrillType.FaFaLaLaSo6:
                    note1 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note2 = NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration);
                    note3 = NAudioHelper.GetSampleProvider(gain, solfeg.LowLaFrequency, sgType, noteDuration);
                    note4 = NAudioHelper.GetSampleProvider(gain, solfeg.LowLaFrequency, sgType, noteDuration);
                    note5 = NAudioHelper.GetSampleProvider(gain, solfeg.LowSoFrequency, sgType, noteDuration);
                    break;

                case L1C2MelodicDrillType.DoDoMiMiMi6:
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
                .FollowedBy(note2)
                .FollowedBy(note3)
                .FollowedBy(note4)
                .FollowedBy(note5);

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            return wavStream;
        }

        private MemoryStream GetMelodicDrillEx(string doNoteName, L1C2MelodicDrillType drillType)
        {
            double bpm = 90;
            double quarterNotemillis = (60 / bpm) * 1000;
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNotemillis);
            TimeSpan wholeNoteDuration = quarterNoteDuration.Add(quarterNoteDuration).Add(quarterNoteDuration).Add(quarterNoteDuration);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1, note2, note3, note4, note5;

            switch (drillType)
            {
                // Major 3rd intervals.
                case L1C2MelodicDrillType.DoDoMiMiMi3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.FaFaLaLaSo3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.SoSoTiTiDo3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.TiTiSoSoSo3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.LaLaFaFaMi3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.MiMiDoDoDo3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                // Minor 6th intervals.
                case L1C2MelodicDrillType.MiMiDoDoDo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.LaLaFaFaMi6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor3rd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor3rd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.TiTiSoSoSo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownHalfStep, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownHalfStep, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.SoSoTiTiDo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownHalfStep, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownHalfStep, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.FaFaLaLaSo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor3rd, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor3rd, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownPerfect4th, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.DoDoMiMiMi6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                default:
                    throw new NotSupportedException($"L1C2MelodicDrillType '{drillType}' is not supported.");
            }

            var phrase = note1
                .FollowedBy(note2)
                .FollowedBy(note3)
                .FollowedBy(note4)
                .FollowedBy(note5);

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            return wavStream;
        }

        private MemoryStream GetHarmonicDrillEx(string doNoteName, L1C2HarmonicDrillType drillType)
        {
            var duration = TimeSpan.FromSeconds(2);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1, note2;

            switch (drillType)
            {
                // Major 3rd intervals.
                case L1C2HarmonicDrillType.DoMi3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + 4, duration);
                    break;

                default:
                    throw new NotSupportedException($"L1C2HarmonicDrillType '{drillType}' is not supported.");
            }

            MixingSampleProvider msp = new MixingSampleProvider(note1.WaveFormat);
            msp.AddMixerInput(note1);
            msp.AddMixerInput(note2);

            IWaveProvider wp = msp.ToWaveProvider();
            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, wp);
            wavStream.Position = 0;

            return wavStream;
        }
    }
}
