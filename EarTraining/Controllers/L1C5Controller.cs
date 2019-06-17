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
    public class L1C5Controller : Controller
    {
        public L1C5Controller()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
        }

        public ActionResult MelodicIntervals()
        {
            return View();
        }

        public ActionResult GetMelodicDrillEx(string doNoteName, int type)
        {
            L1C5MelodicDrillType drillType = (L1C5MelodicDrillType)type;
            var retMs = new MemoryStream();
            MemoryStream wavStream = GetMelodicDrillEx(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private MemoryStream GetMelodicDrillEx(string doNoteName, L1C5MelodicDrillType drillType)
        {
            double bpm = 90;
            double quarterNoteMillis = (60 / bpm) * 1000;
            double eigthNoteMillis = quarterNoteMillis / 2;

            TimeSpan eigthNoteDuration = TimeSpan.FromMilliseconds(eigthNoteMillis);
            TimeSpan quarterNoteDuration = eigthNoteDuration.Add(eigthNoteDuration);
            TimeSpan wholeNoteDuration = quarterNoteDuration.Add(quarterNoteDuration).Add(quarterNoteDuration).Add(quarterNoteDuration);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1, note2, note3, note4, note5, note6;

            switch (drillType)
            {
                #region 4ths.

                case L1C5MelodicDrillType.DoDoDoFaFaMi4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber, eigthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.ReReSoSoSoSo4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up2nd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up2nd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eigthNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.MiMiMiLaLaSo4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, eigthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.FaFaTiTiTiDo4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, eigthNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.SoSoSoDoDoDo4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eigthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.LaLaReReReDo4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, eigthNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.TiTiTiMiMiMi4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, eigthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, wholeNoteDuration);
                    break;

                #endregion 4ths.

                #region 5ths.

                case L1C5MelodicDrillType.DoDoDoSoSoSo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber, eigthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.ReReLaLaLaSo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up2nd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up2nd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, eigthNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.MiMiMiTiTiDo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, eigthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.FaFaDoDoDoDo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, eigthNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.SoSoSoReReDo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eigthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.Up1Octave, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.LaLaMiMiMiMi5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, eigthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, eigthNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.FaFaFaTiTiDo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, eigthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, eigthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownHalfStep, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownHalfStep, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                #endregion 5ths.

                default:
                    throw new NotSupportedException($"L1C5MelodicDrillType '{drillType}' is not supported.");
            }

            var phrase = note1
                .FollowedBy(note2)
                .FollowedBy(note3)
                .FollowedBy(note4)
                .FollowedBy(note5)
                .FollowedBy(note6);

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            return wavStream;
        }
    }
}