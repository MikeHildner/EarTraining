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
    public class L1C7Controller : BaseController
    {
        public L1C7Controller()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
            ViewBag.ShowDo = true;
        }

        public ActionResult VocalDrills()
        {
            return View();
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

        public ActionResult GetMelodicDrill(string doNoteName, int type)
        {
            L1C7MelodicDrillType drillType = (L1C7MelodicDrillType)type;
            MemoryStream wavStream = GetMelodicDrill(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private MemoryStream GetMelodicDrill(string doNoteName, L1C7MelodicDrillType drillType)
        {
            double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNoteMillis = (60 / bpm) * 1000;

            TimeSpan eighthNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis / 2);
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis);
            TimeSpan dottedQuarterNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis).Add(eighthNoteDuration);
            TimeSpan halfNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis * 2);
            TimeSpan halfNoteAndEighthNoteDuration = halfNoteDuration.Add(eighthNoteDuration);
            TimeSpan wholeNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis * 4);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1, note2, note3, note4 = null;

            switch (drillType)
            {
                // Minor 2nd intervals.
                case L1C7MelodicDrillType.MiMiFaMiMin2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, eighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteAndEighthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case L1C7MelodicDrillType.TiDoDoMin2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, halfNoteAndEighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case L1C7MelodicDrillType.FaFaMiMiMin2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, eighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, halfNoteAndEighthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case L1C7MelodicDrillType.DoTiDoMin2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, halfNoteAndEighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                // Major 7th intervals.
                case L1C7MelodicDrillType.DoDoTiDoMaj7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, eighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteAndEighthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C7MelodicDrillType.FaMiMiMaj7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, halfNoteAndEighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, wholeNoteDuration);
                    break;

                case L1C7MelodicDrillType.TiTiDoDoMaj7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, eighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber, halfNoteAndEighthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case L1C7MelodicDrillType.MiFaMiMaj7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, dottedQuarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteAndEighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                default:
                    throw new NotSupportedException($"L1C7MelodicDrillType '{drillType}' is not supported.");
            }

            var phrase = note1
                .FollowedBy(note2)
                .FollowedBy(note3);
            if(note4 != null)
            {
                phrase = phrase.FollowedBy(note4);
            }

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            return wavStream;
        }

        public ActionResult GetHarmonicDrill(string doNoteName, int type)
        {
            L1C7HarmonicDrillType drillType = (L1C7HarmonicDrillType)type;
            MemoryStream wavStream = GetHarmonicDrill(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private MemoryStream GetHarmonicDrill(string doNoteName, L1C7HarmonicDrillType drillType)
        {
            TimeSpan duration = TimeSpan.FromSeconds(3);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1, note2;

            switch (drillType)
            {
                // Minor 2nds.
                case L1C7HarmonicDrillType.MiFa2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, duration);
                    break;

                case L1C7HarmonicDrillType.TiDo2nd:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, duration);
                    break;

                // Major 7ths.
                case L1C7HarmonicDrillType.DoTi7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, duration);
                    break;

                case L1C7HarmonicDrillType.FaMi7th:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, duration);
                    break;

                default:
                    throw new NotSupportedException($"L1C7HarmonicDrillType '{drillType}' is not supported.");
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
            var progressionType = (ProgressionType3)progressiontype;

            TimeSpan noteDuration = TimeSpan.FromSeconds(2);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider[] samples1;
            ISampleProvider[] samples2;

            switch (progressionType)
            {
                case ProgressionType3.Four2ndOneRoot:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FourMajor, InversionType.LowSecond, noteDuration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.OneMajor, InversionType.Root, noteDuration, true);
                    break;

                case ProgressionType3.TwoMinRootFive1st:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.TwoMinor, InversionType.Root, noteDuration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FiveMajor, InversionType.LowFirst, noteDuration, true);
                    break;

                case ProgressionType3.One2ndTwoMin1st:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.OneMajor, InversionType.HighSecond, noteDuration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.TwoMinor, InversionType.HighFirst, noteDuration, true);
                    break;

                case ProgressionType3.SixMinRootFour1st:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.SixMinor, InversionType.Root, noteDuration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FourMajor, InversionType.LowFirst, noteDuration, true);
                    break;

                case ProgressionType3.OneFirstFive2nd:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.OneMajor, InversionType.HighFirst, noteDuration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FiveMajor, InversionType.LowSecond, noteDuration, true);
                    break;

                case ProgressionType3.OneRootSixMin1st:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.OneMajor, InversionType.Root, noteDuration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.SixMinor, InversionType.LowFirst, noteDuration, true);
                    break;

                default:
                    throw new NotSupportedException($"ProgressionType {progressionType} is not supported.");
            }

            MixingSampleProvider msp1 = new MixingSampleProvider(samples1[0].WaveFormat);
            msp1.AddMixerInput(samples1[0]);
            msp1.AddMixerInput(samples1[1]);
            msp1.AddMixerInput(samples1[2]);
            if (samples1[3] != null)  // Bass note.
            {
                msp1.AddMixerInput(samples1[3]);
            }

            MixingSampleProvider msp2 = new MixingSampleProvider(samples2[0].WaveFormat);
            msp2.AddMixerInput(samples2[0]);
            msp2.AddMixerInput(samples2[1]);
            msp2.AddMixerInput(samples2[2]);
            if (samples2[3] != null)  // Bass note.
            {
                msp2.AddMixerInput(samples2[3]);
            }

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
            var progressionType = (ProgressionType3)progressiontype;

            TimeSpan duration = TimeSpan.FromSeconds(2);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider[] samples1;
            ISampleProvider[] samples2;
            ISampleProvider[] samples3;

            switch (progressionType)
            {
                case ProgressionType3.One2ndTwoMin1stThreeMin1st:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.OneMajor, InversionType.HighSecond, duration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.TwoMinor, InversionType.HighFirst, duration, true);
                    samples3 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.ThreeMinor, InversionType.HighFirst, duration, true);
                    break;

                case ProgressionType3.One1stFive2ndSixMin2nd:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.OneMajor, InversionType.HighFirst, duration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FiveMajor, InversionType.LowSecond, duration, true);
                    samples3 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.SixMinor, InversionType.LowSecond, duration, true);
                    break;

                case ProgressionType3.Four2ndFive1stSixMin1st:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FourMajor, InversionType.LowSecond, duration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FiveMajor, InversionType.LowFirst, duration, true);
                    samples3 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.SixMinor, InversionType.LowFirst, duration, true);
                    break;

                case ProgressionType3.ThreeMin1stOne2ndTwoMin1st:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.ThreeMinor, InversionType.HighFirst, duration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.OneMajor, InversionType.HighSecond, duration, true);
                    samples3 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.TwoMinor, InversionType.HighFirst, duration, true);
                    break;

                case ProgressionType3.FourRootFive2ndThreeMinRoot:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FourMajor, InversionType.Root, duration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FiveMajor, InversionType.LowSecond, duration, true);
                    samples3 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.ThreeMinor, InversionType.Root, duration, true);
                    break;

                case ProgressionType3.OneRootTwoMin2ndFour1st:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.OneMajor, InversionType.Root, duration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.TwoMinor, InversionType.LowSecond, duration, true);
                    samples3 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FourMajor, InversionType.LowFirst, duration, true);
                    break;

                case ProgressionType3.One1stThreeMinRootTwoMinFirst:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.OneMajor, InversionType.HighFirst, duration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.ThreeMinor, InversionType.Root, duration, true);
                    samples3 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.TwoMinor, InversionType.HighFirst, duration, true);
                    break;

                case ProgressionType3.Four2ndOneRootFive1st:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FourMajor, InversionType.LowSecond, duration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.OneMajor, InversionType.Root, duration, true);
                    samples3 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FiveMajor, InversionType.LowFirst, duration, true);
                    break;

                case ProgressionType3.SixMinRootFour1stOne2nd:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.SixMinor, InversionType.Root, duration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FourMajor, InversionType.HighFirst, duration, true);
                    samples3 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.OneMajor, InversionType.HighSecond, duration, true);
                    break;

                case ProgressionType3.Five2ndOneRootTwoRoot:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FiveMajor, InversionType.LowSecond, duration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.OneMajor, InversionType.Root, duration, true);
                    samples3 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.TwoMinor, InversionType.Root, duration, true);
                    break;

                case ProgressionType3.One1stFive2ndFourRoot:
                    samples1 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.OneMajor, InversionType.HighFirst, duration, true);
                    samples2 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FiveMajor, InversionType.LowSecond, duration, true);
                    samples3 = Inversion.CreateTriadInversionEx(doNoteNumber, TriadType.FourMajor, InversionType.Root, duration, true);
                    break;

                default:
                    throw new NotSupportedException($"ProgressionType {progressionType} is not supported.");
            }

            MixingSampleProvider msp1 = new MixingSampleProvider(samples1[0].WaveFormat);
            msp1.AddMixerInput(samples1[0]);
            msp1.AddMixerInput(samples1[1]);
            msp1.AddMixerInput(samples1[2]);
            if (samples1[3] != null)  // Bass note.
            {
                msp1.AddMixerInput(samples1[3]);
            }

            MixingSampleProvider msp2 = new MixingSampleProvider(samples2[0].WaveFormat);
            msp2.AddMixerInput(samples2[0]);
            msp2.AddMixerInput(samples2[1]);
            msp2.AddMixerInput(samples2[2]);
            if (samples2[3] != null)  // Bass note.
            {
                msp2.AddMixerInput(samples2[3]);
            }

            MixingSampleProvider msp3 = new MixingSampleProvider(samples3[0].WaveFormat);
            msp3.AddMixerInput(samples3[0]);
            msp3.AddMixerInput(samples3[1]);
            msp3.AddMixerInput(samples3[2]);
            if (samples3[3] != null)  // Bass note.
            {
                msp3.AddMixerInput(samples3[3]);
            }

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

        public ActionResult GetMelodicInterval(string doNoteName, int type)
        {
            L1C7MelodicDrillType drillType = (L1C7MelodicDrillType)type;
            MemoryStream wavStream = GetMelodicInterval(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private MemoryStream GetMelodicInterval(string doNoteName, L1C7MelodicDrillType drillType)
        {
            double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNoteMillis = (60 / bpm) * 1000;
            TimeSpan halfNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis * 2);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1, note2;

            switch (drillType)
            {
                #region Minor 2nds / minor 7ths

                // Minor 2nd intervals.
                case L1C7MelodicDrillType.MiFa2Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    break;

                case L1C7MelodicDrillType.TiDo2Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, halfNoteDuration);
                    break;

                case L1C7MelodicDrillType.FaMi2Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, halfNoteDuration);
                    break;

                case L1C7MelodicDrillType.DoTi2Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    break;

                // Major 7th intervals.
                case L1C7MelodicDrillType.DoTi7Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    break;

                case L1C7MelodicDrillType.FaMi7Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, halfNoteDuration);
                    break;

                case L1C7MelodicDrillType.TiDo7Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, halfNoteDuration);
                    break;

                case L1C7MelodicDrillType.MiFa7Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    break;

                #endregion Major 2nds / min 7ths

                default:
                    throw new NotSupportedException($"L1C6MelodicDrillType '{drillType}' is not supported.");
            }

            var phrase = note1
                .FollowedBy(note2);

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            return wavStream;
        }
    }
}