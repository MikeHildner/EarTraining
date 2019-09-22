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

        public ActionResult MelodicIntervals()
        {
            return View();
        }

        public ActionResult HarmonicIntervals()
        {
            return View();
        }

        public ActionResult DiatonicTriadRecognition()
        {
            ViewBag.ShowPlayDoTriad = true;
            return View();
        }

        public ActionResult DiatonicTriadProgressions()
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

        public ActionResult GetHarmonicDrillEx(string doNoteName, int type)
        {
            L1C6HarmonicDrillType drillType = (L1C6HarmonicDrillType)type;
            MemoryStream wavStream = GetHarmonicDrillEx(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private MemoryStream GetHarmonicDrillEx(string doNoteName, L1C6HarmonicDrillType drillType)
        {
            TimeSpan duration = TimeSpan.FromSeconds(3);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1, note2;

            switch (drillType)
            {
                // Major 2nds.

                case L1C6HarmonicDrillType.DoRe2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, duration);
                    break;

                case L1C6HarmonicDrillType.ReMi2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, duration);
                    break;

                case L1C6HarmonicDrillType.FaSo2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, duration);
                    break;

                case L1C6HarmonicDrillType.SoLa2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, duration);
                    break;

                case L1C6HarmonicDrillType.LaTi2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, duration);
                    break;

                // Minor 7ths.
                
                case L1C6HarmonicDrillType.ReDo7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, duration);
                    break;

                case L1C6HarmonicDrillType.MiRe7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, duration);
                    break;

                case L1C6HarmonicDrillType.SoFa7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect11th, duration);
                    break;

                case L1C6HarmonicDrillType.LaSo7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect12th, duration);
                    break;

                case L1C6HarmonicDrillType.TiLa7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor13th, duration);
                    break;

                default:
                    throw new NotSupportedException($"L1C6HarmonicDrillType '{drillType}' is not supported.");
            }

            MixingSampleProvider msp = new MixingSampleProvider(note1.WaveFormat);
            msp.AddMixerInput(note1);
            msp.AddMixerInput(note2);

            var stwp = new SampleToWaveProvider(msp);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            return wavStream;
        }

        public ActionResult Get2ChordProgressionEx(string doNoteName, int progressiontype)
        {
            var progressionType = (ProgressionType2)progressiontype;

            TimeSpan noteDuration1 = TimeSpan.FromSeconds(2);
            TimeSpan noteDuration2 = noteDuration1;

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider[] samples1;
            ISampleProvider[] samples2;

            switch (progressionType)
            {
                case ProgressionType2.OneRootToSixMin1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration1, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowFirstInversion, noteDuration2, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th);
                    break;

                case ProgressionType2.OneRootToFour2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration1, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecondInversion, noteDuration2, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave);
                    break;

                case ProgressionType2.FourRootToFiveRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration1, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration2, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th);
                    break;

                case ProgressionType2.One2ndToThreeMin1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecondInversion, noteDuration1, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowFirstInversion, noteDuration2, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th);
                    break;

                case ProgressionType2.ThreeMin1stToSixMinRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.HighFirstInversion, noteDuration1, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration2, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th);
                    break;

                case ProgressionType2.Five2ndToOne1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.HighSecondInversion, noteDuration1, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.HighFirstInversion, noteDuration2, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    break;

                case ProgressionType2.One1stToFour1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowFirstInversion, noteDuration1, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowFirstInversion, noteDuration2, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave);
                    break;

                case ProgressionType2.One2ndToSixMinRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.HighSecondInversion, noteDuration1, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration2, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th);
                    break;

                case ProgressionType2.ThreeMinRootToFive2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration1, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecondInversion, noteDuration2, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th);
                    break;

                case ProgressionType2.Five2ndToSixMin2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecondInversion, noteDuration1, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecondInversion, noteDuration2, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th);
                    break;

                case ProgressionType2.Four2ndToThreeMin2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecondInversion, noteDuration1, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecondInversion, noteDuration2, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th);
                    break;

                default:
                    throw new NotSupportedException($"ProgressionType {progressionType} is not supported.");
            }

            MixingSampleProvider msp1 = new MixingSampleProvider(samples1[0].WaveFormat);
            msp1.AddMixerInput(samples1[0]);
            msp1.AddMixerInput(samples1[1]);
            msp1.AddMixerInput(samples1[2]);

            MixingSampleProvider msp2 = new MixingSampleProvider(samples2[0].WaveFormat);
            msp2.AddMixerInput(samples2[0]);
            msp2.AddMixerInput(samples2[1]);
            msp2.AddMixerInput(samples2[2]);

            var phrase = msp1
                .FollowedBy(msp2);

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        public ActionResult Get3ChordProgressionEx(string doNoteName, int progressiontype)
        {
            var progressionType = (ProgressionType2)progressiontype;

            TimeSpan noteDuration = TimeSpan.FromSeconds(2);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider[] samples1;
            ISampleProvider[] samples2;
            ISampleProvider[] samples3;

            switch (progressionType)
            {
                case ProgressionType2.OneRootFive1stSixMin1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowFirstInversion, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th);
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.LowFirstInversion, noteDuration, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th);
                    break;

                case ProgressionType2.One1stThreeMinRootFourRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.HighFirstInversion, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th);
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave);
                    break;

                case ProgressionType2.SixMin2ndFive2ndFour2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.HighSecondInversion, noteDuration, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.HighSecondInversion, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th);
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.HighSecondInversion, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave);
                    break;

                default:
                    throw new NotSupportedException($"ProgressionType {progressionType} is not supported.");
            }

            MixingSampleProvider msp1 = new MixingSampleProvider(samples1[0].WaveFormat);
            msp1.AddMixerInput(samples1[0]);
            msp1.AddMixerInput(samples1[1]);
            msp1.AddMixerInput(samples1[2]);

            MixingSampleProvider msp2 = new MixingSampleProvider(samples2[0].WaveFormat);
            msp2.AddMixerInput(samples2[0]);
            msp2.AddMixerInput(samples2[1]);
            msp2.AddMixerInput(samples2[2]);

            MixingSampleProvider msp3 = new MixingSampleProvider(samples3[0].WaveFormat);
            msp3.AddMixerInput(samples3[0]);
            msp3.AddMixerInput(samples3[1]);
            msp3.AddMixerInput(samples3[2]);

            var phrase = msp1
                .FollowedBy(msp2)
                .FollowedBy(msp3);

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }
    }
}