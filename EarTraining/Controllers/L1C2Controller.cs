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
    public class L1C2Controller : BaseController
    {
        public L1C2Controller()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
            ViewBag.ShowDo = true;
        }

        // GET: L1C2
        public ActionResult Index()
        {
            return View();
        }

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
    }
}
