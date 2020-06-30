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
using System.Web.Hosting;
using System.Web.Mvc;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class L1C2Controller : BaseController
    {
        public L1C2Controller()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
            ViewBag.ShowDo = true;
        }

        // GET: L1C2
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult VocalDrills()
        {
            return View();
        }

        public ActionResult MelodicMaj3rdMin6thNoDO()
        {
            return View();
        }

        public ActionResult HarmonicMaj3rdMin6th()
        {
            return View();
        }

        public ActionResult DictationTranscription()
        {
            ViewBag.ShowDo = false;
            return View();
        }

        public ActionResult GetMelodicDrillDO(string doNoteName, int type)
        {
            L1C2MelodicDrillType drillType = (L1C2MelodicDrillType)type;
            MemoryStream wavStream = GetMelodicDrillDO(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        public ActionResult GetMelodicDrillNoDO(string doNoteName, int type)
        {
            L1C2MelodicDrillType drillType = (L1C2MelodicDrillType)type;
            MemoryStream wavStream = GetMelodicDrillNoDO(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        public ActionResult GetHarmonicDrillEx(string doNoteName, int type)
        {
            L1C2HarmonicDrillType drillType = (L1C2HarmonicDrillType)type;
            MemoryStream wavStream = GetHarmonicDrillEx(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private MemoryStream GetMelodicDrillDO(string doNoteName, L1C2MelodicDrillType drillType)
        {
            double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNotemillis = (60 / bpm) * 1000;
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNotemillis);
            TimeSpan wholeNoteDuration = quarterNoteDuration.Add(quarterNoteDuration).Add(quarterNoteDuration).Add(quarterNoteDuration);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1, note2, note3, note4, note5;

            switch (drillType)
            {
                #region Major 3rds / min 6ths

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
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
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
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.LaLaFaFaMi6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor3rd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor3rd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.TiTiSoSoSo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.SoSoTiTiDo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, quarterNoteDuration);
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
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                #endregion Major 3rds / min 6ths

                #region Minor 3rds / maj 6ths.

                // Minor 3rd intervals.
                case L1C2MelodicDrillType.ReReFaFaMi3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.MiMiSoSoSo3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.LaLaDoDoDo3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.TiTiReReDo3High:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.TiTiReReDo3Low:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                // Major 6th intervals.
                case L1C2MelodicDrillType.DoDoLaLaSo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.ReReTiTiDo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.FaFaReReDo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.SoSoMiMiMi6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.LaLaDoDoDo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.TiTiReReDo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.ReReFaFaMi6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case L1C2MelodicDrillType.MiMiSoSoSo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownPerfect4th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownPerfect4th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownPerfect4th, wholeNoteDuration);
                    break;

                #endregion Minor 3rds / maj 6ths.

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

        private MemoryStream GetMelodicDrillNoDO(string doNoteName, L1C2MelodicDrillType drillType)
        {
            double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNotemillis = (60 / bpm) * 1000;
            TimeSpan halfNoteDuration = TimeSpan.FromMilliseconds(quarterNotemillis * 2);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1, note2;

            switch (drillType)
            {
                #region Major 3rds / min 6ths

                // Major 3rd intervals.
                case L1C2MelodicDrillType.DoMi3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.FaLa3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.SoTi3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.TiSo3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.LaFa3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.MiDo3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, halfNoteDuration);
                    break;

                // Minor 6th intervals.
                case L1C2MelodicDrillType.MiDo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.LaFa6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor3rd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.TiSo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.SoTi6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.FaLa6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor3rd, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.DoMi6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, halfNoteDuration);
                    break;

                #endregion Major 3rds / min 6ths

                #region Minor 3rds / maj 6ths.

                // Minor 3rd intervals.
                case L1C2MelodicDrillType.ReFa3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.MiSo3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.LaDo3:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.TiRe3High:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.TiRe3Low:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, halfNoteDuration);
                    break;

                // Major 6th intervals.
                case L1C2MelodicDrillType.DoLa6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.ReTi6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.FaRe6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.SoMi6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.LaDo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.TiRe6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.ReFa6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    break;

                case L1C2MelodicDrillType.MiSo6:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownPerfect4th, halfNoteDuration);
                    break;

                #endregion Minor 3rds / maj 6ths.

                default:
                    throw new NotSupportedException($"L1C2MelodicDrillType '{drillType}' is not supported.");
            }

            var phrase = note1
                .FollowedBy(note2);

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            return wavStream;
        }

        private MemoryStream GetHarmonicDrillEx(string doNoteName, L1C2HarmonicDrillType drillType)
        {
            var duration = TimeSpan.FromSeconds(4);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            int newDo;
            InversionType inversionType;
            int secondNoteNumber;

            switch (drillType)
            {
                // Major 3rd intervals.
                case L1C2HarmonicDrillType.DoMi3:
                    newDo = doNoteNumber;
                    secondNoteNumber = newDo + Interval.UpMajor3rd;
                    inversionType = InversionType.Root;
                    break;

                case L1C2HarmonicDrillType.FaLa3:
                    newDo = doNoteNumber + Interval.UpPerfect4th;
                    secondNoteNumber = newDo + Interval.UpMajor3rd;
                    inversionType = InversionType.Root;
                    break;

                case L1C2HarmonicDrillType.SoTi3:
                    newDo = doNoteNumber + Interval.UpPerfect5th;
                    secondNoteNumber = newDo + Interval.UpMajor3rd;
                    inversionType = InversionType.Root;
                    break;

                // Minor 6th intervals.
                case L1C2HarmonicDrillType.MiDo6:
                    newDo = doNoteNumber;
                    secondNoteNumber = newDo + Interval.UpMajor3rd;
                    inversionType = InversionType.HighFirst;
                    break;

                case L1C2HarmonicDrillType.LaFa6:
                    newDo = doNoteNumber + Interval.UpPerfect4th;
                    secondNoteNumber = newDo + Interval.UpMajor3rd;
                    inversionType = InversionType.HighFirst;
                    break;

                case L1C2HarmonicDrillType.TiSo6:
                    newDo = doNoteNumber + Interval.UpPerfect5th;
                    secondNoteNumber = newDo + Interval.UpMajor3rd;
                    inversionType = InversionType.HighFirst;
                    break;

                // Minor 3rd intervals.
                case L1C2HarmonicDrillType.ReFa3:
                    newDo = doNoteNumber + Interval.UpMajor2nd;
                    secondNoteNumber = newDo + Interval.UpMinor3rd;
                    inversionType = InversionType.Root;
                    break;

                case L1C2HarmonicDrillType.MiSo3:
                    newDo = doNoteNumber + Interval.UpMajor3rd;
                    secondNoteNumber = newDo + Interval.UpMinor3rd;
                    inversionType = InversionType.Root;
                    break;

                case L1C2HarmonicDrillType.LaDo3:
                    newDo = doNoteNumber + Interval.UpMajor6th;
                    secondNoteNumber = newDo + Interval.UpMinor3rd;
                    inversionType = InversionType.Root;
                    break;

                case L1C2HarmonicDrillType.TiRe3:
                    newDo = doNoteNumber + Interval.UpMajor7th;
                    secondNoteNumber = newDo + Interval.UpMinor3rd;
                    inversionType = InversionType.Root;
                    break;

                // Major 6th intervals.
                case L1C2HarmonicDrillType.FaRe6:
                    newDo = doNoteNumber + Interval.UpMajor2nd;
                    secondNoteNumber = newDo + Interval.UpMinor3rd;
                    inversionType = InversionType.HighFirst;
                    break;

                case L1C2HarmonicDrillType.SoMi6:
                    newDo = doNoteNumber + Interval.UpMajor3rd;
                    secondNoteNumber = newDo + Interval.UpMinor3rd;
                    inversionType = InversionType.HighFirst;
                    break;

                case L1C2HarmonicDrillType.DoLa6:
                    newDo = doNoteNumber + Interval.UpMajor6th;
                    secondNoteNumber = newDo + Interval.UpMinor3rd;
                    inversionType = InversionType.LowFirst;
                    break;

                case L1C2HarmonicDrillType.ReTi6:
                    newDo = doNoteNumber + Interval.UpMajor7th;
                    secondNoteNumber = newDo + Interval.UpMinor3rd;
                    inversionType = InversionType.LowFirst;
                    break;

                default:
                    throw new NotSupportedException($"L1C2HarmonicDrillType '{drillType}' is not supported.");
            }

            ISampleProvider[] samples = Inversion.Create2NoteInversionEx(inversionType, duration, newDo, secondNoteNumber);

            MixingSampleProvider msp = new MixingSampleProvider(samples[0].WaveFormat);
            msp.AddMixerInput(samples[0]);
            msp.AddMixerInput(samples[1]);

            IWaveProvider wp = msp.ToWaveProvider();
            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, wp);
            wavStream.Position = 0;

            return wavStream;
        }

        public ActionResult AudioAndDictation(int ascensionType, string keySignature, double bpm, int numberOfMeasures)
        {
            // We'll add stuff to the Dictionary and return as JSON.
            var dict = new Dictionary<string, string>();

            #region Audio

            //double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNoteMillis = (60 / bpm) * 1000;
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis);
            TimeSpan halfNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis * 2);
            TimeSpan dottedHalfNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis * 3);
            TimeSpan wholeNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis * 4);
            TimeSpan eigthNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis / 2);

            // Setup the scale note numbers.
            int[] scaleNoteNumbers = new int[] { 38, 39, 41, 43, 44, 46, 48, 50, 51, 53, 55, 56, 58, 60 };  // C Major, low TI to high SO.
            scaleNoteNumbers = NoteHelper.TransposeScaleNoteNumbers(scaleNoteNumbers, keySignature);

            // The initial DO.
            ISampleProvider wholeDoNote = NAudioHelper.GetSampleProvider(scaleNoteNumbers[1], wholeNoteDuration);

            // Four metronome ticks before the transcription part plays.
            ISampleProvider[] ticks = new ISampleProvider[4];
            string tickFile = HostingEnvironment.MapPath($"~/Samples/Woodblock.wav");
            for (int i = 0; i < ticks.Length; i++)
            {
                ticks[i] = NAudioHelper.GetSampleProviderFromFile(tickFile, quarterNoteDuration);
            }

            List<string> measureRhythms = GetEigthNoteRhythms();

            int randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            string measureRhythm1 = measureRhythms[randomInt];
            randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            string measureRhythm2 = measureRhythms[randomInt];

            // Ensure an even number of notes, so the interval is complete.
            // Ensure there's exactly one pair of eigth notes for each two measure phrase.
            while (true)
            {
                int totalNotes = measureRhythm1.Split(',').Count() + measureRhythm2.Split(',').Count();
                int totalEigthNotes = measureRhythm1.Split(',').Where(w => w == "8").Count() + measureRhythm2.Split(',').Where(w => w == "8").Count();

                if (totalNotes % 2 == 0 && totalEigthNotes == 2)
                {
                    break;
                }

                randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
                measureRhythm1 = measureRhythms[randomInt];
                randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
                measureRhythm2 = measureRhythms[randomInt];
            }

            randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            string measureRhythm3 = measureRhythms[randomInt];
            randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            string measureRhythm4 = measureRhythms[randomInt];

            if (numberOfMeasures == 4)
            {
                while (true)
                {
                    int totalNotes = measureRhythm3.Split(',').Count() + measureRhythm4.Split(',').Count();
                    int totalEigthNotes = measureRhythm3.Split(',').Where(w => w == "8").Count() + measureRhythm4.Split(',').Where(w => w == "8").Count();

                    if (totalNotes % 2 == 0 && totalEigthNotes == 2)
                    {
                        break;
                    }

                    randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
                    measureRhythm3 = measureRhythms[randomInt];
                    randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
                    measureRhythm4 = measureRhythms[randomInt];
                }
            }

            // Ensure that every two measures has exactly one pair of eigth notes.
            // Always take care of the first two measures, regardless of the number of measures.
            //while (measureRhythm1.Split(',').Where(w => w == "8").Count() + measureRhythm2.Split(',').Where(w => w == "8").Count() != 2)
            //{
            //    randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            //    measureRhythm1 = measureRhythms[randomInt];
            //    randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            //    measureRhythm2 = measureRhythms[randomInt];
            //}
            //if (numberOfMeasures == 4)
            //{
            //    while (measureRhythm3.Split(',').Where(w => w == "8").Count() + measureRhythm4.Split(',').Where(w => w == "8").Count() != 2)
            //    {
            //        randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            //        measureRhythm3 = measureRhythms[randomInt];
            //        randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            //        measureRhythm4 = measureRhythms[randomInt];
            //    }
            //}

            string[] measureRhythmSplit1 = measureRhythm1.Split(',');
            int numberOfNotes1 = measureRhythmSplit1.Length;
            string[] measureRhythmSplit2 = measureRhythm2.Split(',');
            int numberOfNotes2 = measureRhythmSplit2.Length;
            string[] measureRhythmSplit3 = measureRhythm3.Split(',');
            int numberOfNotes3 = measureRhythmSplit3.Length;
            string[] measureRhythmSplit4 = measureRhythm4.Split(',');
            int numberOfNotes4 = measureRhythmSplit4.Length;

            ISampleProvider[] notes1 = new ISampleProvider[numberOfNotes1];
            ISampleProvider[] notes2 = new ISampleProvider[numberOfNotes2];
            ISampleProvider[] notes3 = new ISampleProvider[numberOfNotes3];
            ISampleProvider[] notes4 = new ISampleProvider[numberOfNotes4];

            Queue<int> noteNumberQueue;
            switch (ascensionType)
            {
                case 1:
                    noteNumberQueue = GetIntervalIntQueue(scaleNoteNumbers, 32, 1);  // 32 notes max.
                    break;

                case 2:
                    noteNumberQueue = GetIntervalIntQueue(scaleNoteNumbers, 32, 2);
                    break;

                case 3:
                    noteNumberQueue = GetIntervalIntQueue(scaleNoteNumbers, 32, 3);
                    break;

                default:
                    throw new NotSupportedException($"AscensionType '{ascensionType}' is not supported.");
            }

            int[] measureNoteNumbers1 = NoteHelper.PopulateNoteNumbersFromQueue(numberOfNotes1, noteNumberQueue);
            int[] measureNoteNumbers2 = NoteHelper.PopulateNoteNumbersFromQueue(numberOfNotes2, noteNumberQueue);
            int[] measureNoteNumbers3 = NoteHelper.PopulateNoteNumbersFromQueue(numberOfNotes3, noteNumberQueue);
            int[] measureNoteNumbers4 = NoteHelper.PopulateNoteNumbersFromQueue(numberOfNotes4, noteNumberQueue);

            NoteHelper.CreateSamplesFromRhythmsAndNoteNames(eigthNoteDuration, quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit1, notes1, measureNoteNumbers1);
            NoteHelper.CreateSamplesFromRhythmsAndNoteNames(eigthNoteDuration, quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit2, notes2, measureNoteNumbers2);
            NoteHelper.CreateSamplesFromRhythmsAndNoteNames(eigthNoteDuration, quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit3, notes3, measureNoteNumbers3);
            NoteHelper.CreateSamplesFromRhythmsAndNoteNames(eigthNoteDuration, quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit4, notes4, measureNoteNumbers4);

            ISampleProvider phrase =
                wholeDoNote;

            foreach (var tick in ticks)
            {
                phrase = phrase.FollowedBy(tick);
            }

            foreach (var note1 in notes1)
            {
                phrase = phrase.FollowedBy(note1);
            }
            foreach (var note2 in notes2)
            {
                phrase = phrase.FollowedBy(note2);
            }
            if (numberOfMeasures == 4)
            {
                foreach (var note3 in notes3)
                {
                    phrase = phrase.FollowedBy(note3);
                }
                foreach (var note4 in notes4)
                {
                    phrase = phrase.FollowedBy(note4);
                }
            }

            // HACK: Use an empty note because without it, the audio gets cut short.
            ISampleProvider emptyNote = NAudioHelper.GetSampleProvider(0, 0, SignalGeneratorType.White, halfNoteDuration);
            phrase = phrase.FollowedBy(emptyNote);

            SampleToWaveProvider stwp = new SampleToWaveProvider(phrase);
            MixingSampleProvider msp = new MixingSampleProvider(stwp.WaveFormat);
            msp.AddMixerInput(stwp);

            int totalTicks = 8 + (4 * numberOfMeasures);
            ISampleProvider[] metronomeTicks = new ISampleProvider[totalTicks];

            // The first two measures have zero gain, as this is the initial DO whole note and metronome ticks.
            for (int i = 0; i < 8; i++)
            {
                metronomeTicks[i] = NAudioHelper.GetSampleProvider(0, 0, SignalGeneratorType.White, quarterNoteDuration);
            }
            for (int i = 8; i < totalTicks; i++)
            {
                metronomeTicks[i] = NAudioHelper.GetSampleProviderFromFile(tickFile, quarterNoteDuration);
            }

            ISampleProvider metronomePhrase = metronomeTicks[0];
            for (int i = 0; i < metronomeTicks.Length; i++)
            {
                metronomePhrase = metronomePhrase.FollowedBy(metronomeTicks[i]);
            }

            msp.AddMixerInput(metronomePhrase);

            IWaveProvider wp = msp.ToWaveProvider();

            MemoryStream wavStream = new MemoryStream();
            //WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            WaveFileWriter.WriteWavFileToStream(wavStream, wp);
            wavStream.Position = 0;
            wavStream.WavToMp3File(out string fileName);
            dict.Add("src", fileName);

            #endregion Audio

            #region Notation

            string[] noteNames1 = new string[numberOfNotes1];
            NoteHelper.AdjustNoteNamesForKey(keySignature, measureNoteNumbers1, noteNames1);

            string[] noteNames2 = new string[numberOfNotes2];
            NoteHelper.AdjustNoteNamesForKey(keySignature, measureNoteNumbers2, noteNames2);

            string[] noteNames3 = new string[numberOfNotes3];
            NoteHelper.AdjustNoteNamesForKey(keySignature, measureNoteNumbers3, noteNames3);

            string[] noteNames4 = new string[numberOfNotes4];
            NoteHelper.AdjustNoteNamesForKey(keySignature, measureNoteNumbers4, noteNames4);

            string script1 = NoteHelper.GetEasyScoreScript3("transcription1", noteNames1, measureRhythmSplit1, keySignature, true);
            //string script1 = NoteHelper.GetMusicXmlScript("transcription1", noteNames1, measureRhythmSplit1, keySignature, false);

            dict.Add("theScript1", script1);
            string script2 = NoteHelper.GetEasyScoreScript3("transcription2", noteNames2, measureRhythmSplit2, keySignature, false);
            dict.Add("theScript2", script2);
            if (numberOfMeasures == 4)
            {
                string script3 = NoteHelper.GetEasyScoreScript3("transcription3", noteNames3, measureRhythmSplit3, keySignature, false);
                dict.Add("theScript3", script3);
                string script4 = NoteHelper.GetEasyScoreScript3("transcription4", noteNames4, measureRhythmSplit4, keySignature, false);
                dict.Add("theScript4", script4);
            }

            #endregion Notation

            var json = Json(dict, JsonRequestBehavior.AllowGet);
            return json;
        }

        private static List<string> GetEigthNoteRhythms()
        {

            // Various rhythm possibilities for each measure.
            List<string> measureRhythms = new List<string>();
            measureRhythms.Add("1");
            measureRhythms.Add("2,2");
            measureRhythms.Add("4,2.");
            measureRhythms.Add("2.,4");
            measureRhythms.Add("4,4,4,4");
            measureRhythms.Add("4,4,2");
            measureRhythms.Add("2,4,4");
            measureRhythms.Add("4,2,4");
            measureRhythms.Add("8,8,4,4,4");
            measureRhythms.Add("8,8,4,2");
            measureRhythms.Add("4,4,8,8,4");
            measureRhythms.Add("2,8,8,4");

            return measureRhythms;
        }

        private Queue<int> GetIntervalIntQueue(int[] scaleNoteNumbers, int numberOfNotes, int ascensionType)
        {
            List<Tuple<int, int>> resolutions = new List<Tuple<int, int>>();
            resolutions.Add(new Tuple<int, int>(1, 3));  // DO MI Maj. 3rd.
            resolutions.Add(new Tuple<int, int>(4, 6));  // FA LA Maj. 3rd.
            resolutions.Add(new Tuple<int, int>(5, 7));  // SO TI Maj. 3rd.
            resolutions.Add(new Tuple<int, int>(3, 8));  // MI DO Min. 6th.
            resolutions.Add(new Tuple<int, int>(6, 11));  // LA FA Min. 6th.
            resolutions.Add(new Tuple<int, int>(7, 12));  // TI SO Min. 6th.

            var q = new Queue<int>();
            while (q.Count < numberOfNotes)
            {
                int randomInt = NoteHelper.GetRandomInt(0, resolutions.Count);
                Tuple<int, int> t = resolutions[randomInt];

                switch (ascensionType)
                {
                    case 1:
                        q.Enqueue(scaleNoteNumbers[t.Item1]);
                        q.Enqueue(scaleNoteNumbers[t.Item2]);
                        break;

                    case 2:
                        q.Enqueue(scaleNoteNumbers[t.Item2]);
                        q.Enqueue(scaleNoteNumbers[t.Item1]);
                        break;

                    case 3:
                        int ri = NoteHelper.GetRandomInt(0, 2);
                        if (ri % 2 == 0)
                        {
                            q.Enqueue(scaleNoteNumbers[t.Item1]);
                            q.Enqueue(scaleNoteNumbers[t.Item2]);
                        }
                        else
                        {
                            q.Enqueue(scaleNoteNumbers[t.Item2]);
                            q.Enqueue(scaleNoteNumbers[t.Item1]);
                        }
                        break;
                    default:
                        throw new NotSupportedException($"Ascension type '{ascensionType}' is not supported.");
                }
            }

            return q;
        }
    }
}
