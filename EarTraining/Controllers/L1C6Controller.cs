using EarTrainingLibrary.Enums;
using EarTrainingLibrary.NAudio;
using EarTrainingLibrary.Utility;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class L1C6Controller : Controller
    {
        public L1C6Controller()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
        }

        public ActionResult MelodicMaj2ndMin7th()
        {
            return View();
        }

        public ActionResult DiatonicTriadRecognition()
        {
            ViewBag.ShowPlayDoTriad = true;
            return View();
        }

        public ActionResult GetMelodicDrillEx(string doNoteName, int type)
        {
            L1C6MelodicDrillType drillType = (L1C6MelodicDrillType)type;
            MemoryStream wavStream = GetMelodicDrillEx(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private MemoryStream GetMelodicDrillEx(string doNoteName, L1C6MelodicDrillType drillType)
        {
            double bpm = double.Parse(ConfigurationManager.AppSettings["MelodicDrillBPM"]);
            double quarterNoteMillis = (60 / bpm) * 1000;
            double eigthNoteMillis = quarterNoteMillis / 2;
            TimeSpan eigthNoteDuration = TimeSpan.FromMilliseconds(eigthNoteMillis);
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis);
            TimeSpan dottedQuarterNoteDuration = quarterNoteDuration.Add(eigthNoteDuration);
            TimeSpan wholeNoteDuration = quarterNoteDuration.Add(quarterNoteDuration).Add(quarterNoteDuration).Add(quarterNoteDuration);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1, note2, note3, note4, note5;

            switch (drillType)
            {
                #region Major 2nds / min 7ths

                // Major 2nd intervals.
                case L1C6MelodicDrillType.DoDoReReDo2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.ReReMiMiMi2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, dottedQuarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.FaFaSoSoSo2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.SoSoLaLaSo2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, dottedQuarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.LaLaTiTiDo2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.ReReDoDoDo2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber, dottedQuarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.MiMiReReDo2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.SoSoFaFaMi2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, dottedQuarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.LaLaSoSoSo2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.TiTiLaLaSo2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, dottedQuarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                // Minor 7th intervals.
                case L1C6MelodicDrillType.ReReDoDoDo7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.MiMiReReDo7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, dottedQuarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.SoSoFaFaMi7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect11th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect11th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.LaLaSoSoSo7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect12th, dottedQuarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect12th, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect12th, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.TiTiLaLaSo7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor13th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor13th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect12th, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.DoDoReReDo7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, dottedQuarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.ReReMiMiMi7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.FaFaSoSoSo7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect11th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect11th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, dottedQuarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.SoSoLaLaSo7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect12th, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect12th, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C6MelodicDrillType.LaLaTiTiDo7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor13th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor13th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, dottedQuarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                #endregion Major 2nds / min 7ths

                default:
                    throw new NotSupportedException($"L1C6MelodicDrillType '{drillType}' is not supported.");
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
    }
}