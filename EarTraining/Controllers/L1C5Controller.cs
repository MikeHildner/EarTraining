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
    public class L1C5Controller : BaseController
    {
        public L1C5Controller()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
            ViewBag.ShowDo = true;
        }

        public ActionResult VocalDrills(string @do)
        {
            if (!string.IsNullOrWhiteSpace(@do))
            {
                Pitch pitch = new Pitches().PitchesList.Single(s => s.PitchName == @do.ToUpper());
                ViewBag.Pitch = pitch;
            }

            return View();
        }

        public ActionResult MelodicIntervals(string @do)
        {
            if (!string.IsNullOrWhiteSpace(@do))
            {
                Pitch pitch = new Pitches().PitchesList.Single(s => s.PitchName == @do.ToUpper());
                ViewBag.Pitch = pitch;
            }

            return View();
        }

        public ActionResult HarmonicIntervals(string @do)
        {
            if (!string.IsNullOrWhiteSpace(@do))
            {
                Pitch pitch = new Pitches().PitchesList.Single(s => s.PitchName == @do.ToUpper());
                ViewBag.Pitch = pitch;
            }

            return View();
        }

        public ActionResult DiatonicTriadRecognition(string @do)
        {
            if (!string.IsNullOrWhiteSpace(@do))
            {
                Pitch pitch = new Pitches().PitchesList.Single(s => s.PitchName == @do.ToUpper());
                ViewBag.Pitch = pitch;
            }

            ViewBag.ShowPlayDoTriad = true;
            return View();
        }

        public ActionResult DiatonicTriadProgressions(string @do)
        {
            if (!string.IsNullOrWhiteSpace(@do))
            {
                Pitch pitch = new Pitches().PitchesList.Single(s => s.PitchName == @do.ToUpper());
                ViewBag.Pitch = pitch;
            }

            ViewBag.ShowPlayDoTriad = true;
            return View();
        }

        public ActionResult GetMelodicDrillEx(string doNoteName, int type)
        {
            L1C5MelodicDrillType drillType = (L1C5MelodicDrillType)type;
            MemoryStream wavStream = GetMelodicDrillEx(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private MemoryStream GetMelodicDrillEx(string doNoteName, L1C5MelodicDrillType drillType)
        {
            double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNoteMillis = (60 / bpm) * 1000;
            double eighthNoteMillis = quarterNoteMillis / 2;

            TimeSpan eighthNoteDuration = TimeSpan.FromMilliseconds(eighthNoteMillis);
            TimeSpan quarterNoteDuration = eighthNoteDuration.Add(eighthNoteDuration);
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
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, eighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber, eighthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.ReReSoSoSoSo4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eighthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eighthNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.MiMiMiLaLaSo4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, eighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, eighthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.FaFaTiTiTiDo4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, eighthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, eighthNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.SoSoSoDoDoDo4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eighthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.LaLaReReReDo4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, eighthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, eighthNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.TiTiTiMiMiMi4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, eighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, eighthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, wholeNoteDuration);
                    break;

                #endregion 4ths.

                #region 5ths.

                case L1C5MelodicDrillType.DoDoDoSoSoSo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, eighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber, eighthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.ReReLaLaLaSo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, eighthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, eighthNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.MiMiMiTiTiDo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, eighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, eighthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.FaFaDoDoDoDo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, eighthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, eighthNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.SoSoSoReReDo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, eighthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, quarterNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.LaLaMiMiMiMi5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, quarterNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, eighthNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, eighthNoteDuration);
                    note6 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, wholeNoteDuration);
                    break;

                case L1C5MelodicDrillType.FaFaFaTiTiDo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, eighthNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, eighthNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, quarterNoteDuration);
                    note5 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, quarterNoteDuration);
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

        public ActionResult GetHarmonicDrillEx(string doNoteName, int type)
        {
            L1C5HarmonicDrillType drillType = (L1C5HarmonicDrillType)type;
            MemoryStream wavStream = GetHarmonicDrillEx(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private MemoryStream GetHarmonicDrillEx(string doNoteName, L1C5HarmonicDrillType drillType)
        {
            TimeSpan duration = TimeSpan.FromSeconds(3);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1, note2;

            switch (drillType)
            {
                #region 4ths.

                case L1C5HarmonicDrillType.DoFa4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, duration);
                    break;

                case L1C5HarmonicDrillType.ReSo4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, duration);
                    break;

                case L1C5HarmonicDrillType.MiLa4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, duration);
                    break;

                case L1C5HarmonicDrillType.FaTi4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, duration);
                    break;

                case L1C5HarmonicDrillType.SoDo4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, duration);
                    break;

                case L1C5HarmonicDrillType.LaRe4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, duration);
                    break;

                case L1C5HarmonicDrillType.TiMi4:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, duration);
                    break;

                #endregion 4ths.

                #region 5ths.

                case L1C5HarmonicDrillType.DoSo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, duration);
                    break;

                case L1C5HarmonicDrillType.ReLa5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, duration);
                    break;

                case L1C5HarmonicDrillType.MiTi5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, duration);
                    break;

                case L1C5HarmonicDrillType.FaDo5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, duration);
                    break;

                case L1C5HarmonicDrillType.SoRe5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, duration);
                    break;

                case L1C5HarmonicDrillType.LaMi5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, duration);
                    break;

                case L1C5HarmonicDrillType.TiFa5:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, duration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect11th, duration);
                    break;

                #endregion 5ths.

                default:
                    throw new NotSupportedException($"L1C5HarmonicDrillType '{drillType}' is not supported.");
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
            var progressionType = (ProgressionType)progressiontype;

            TimeSpan noteDuration = TimeSpan.FromSeconds(2);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider[] samples1;
            ISampleProvider[] samples2;

            switch (progressionType)
            {
                case ProgressionType.Four2ndToOneRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    break;

                case ProgressionType.Five1stToOneRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    break;

                case ProgressionType.OneRootToFour2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    break;

                case ProgressionType.SixMin2ndToFourRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th, (doNoteNumber + Interval.UpMajor6th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    break;

                case ProgressionType.FiveRootToSixMinRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th, (doNoteNumber + Interval.UpMajor6th).BassNoteNumber());
                    break;

                case ProgressionType.SixMin1stToOneRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th, (doNoteNumber + Interval.UpMajor6th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    break;

                case ProgressionType.FourRootToFiveRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    break;

                case ProgressionType.SixMinRootToFiveRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th, (doNoteNumber + Interval.UpMajor6th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    break;

                case ProgressionType.FourRootToSixMin2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th, (doNoteNumber + Interval.UpMajor6th).BassNoteNumber());
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
            var progressionType = (ProgressionType)progressiontype;

            TimeSpan noteDuration = TimeSpan.FromSeconds(2);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider[] samples1;
            ISampleProvider[] samples2;
            ISampleProvider[] samples3;

            switch (progressionType)
            {
                case ProgressionType.Four2ndFive1stOneRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    break;

                case ProgressionType.OneRootFour2ndFive1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    break;


                case ProgressionType.Four2ndOneRootFive1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    break;

                case ProgressionType.OneRootFive1stSixMin1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th, (doNoteNumber + Interval.UpMajor6th).BassNoteNumber());
                    break;

                case ProgressionType.SixMin2ndFourRootOne1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th, (doNoteNumber + Interval.UpMajor6th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.HighFirst, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    break;

                case ProgressionType.OneRootSixMin1stFive1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th, (doNoteNumber + Interval.UpMajor6th).BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    break;

                case ProgressionType.FiveRootFourRootSixMin2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th, (doNoteNumber + Interval.UpMajor6th).BassNoteNumber());
                    break;

                case ProgressionType.FourRootFiveRootSixMinRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th, (doNoteNumber + Interval.UpMajor6th).BassNoteNumber());
                    break;

                case ProgressionType.SixMin1stOne2ndFour1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th, (doNoteNumber + Interval.UpMajor6th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    break;

                case ProgressionType.Five2ndSixMin2ndOneRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, doNoteNumber + Interval.UpMajor10th, (doNoteNumber + Interval.UpMajor6th).BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    break;

                default:
                    throw new NotSupportedException($"ProgressionType {progressionType} is not supported.");
            }

            // First chord.
            MixingSampleProvider msp1 = new MixingSampleProvider(samples1[0].WaveFormat);
            msp1.AddMixerInput(samples1[0]);
            msp1.AddMixerInput(samples1[1]);
            msp1.AddMixerInput(samples1[2]);
            if (samples1[3] != null)  // Bass note.
            {
                msp1.AddMixerInput(samples1[3]);
            }

            // Second chord.
            MixingSampleProvider msp2 = new MixingSampleProvider(samples2[0].WaveFormat);
            msp2.AddMixerInput(samples2[0]);
            msp2.AddMixerInput(samples2[1]);
            msp2.AddMixerInput(samples2[2]);
            if (samples2[3] != null)  // Bass note.
            {
                msp2.AddMixerInput(samples2[3]);
            }

            // Third chord.
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
            L1C5MelodicDrillType drillType = (L1C5MelodicDrillType)type;
            MemoryStream wavStream = GetMelodicInterval(doNoteName, drillType);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private MemoryStream GetMelodicInterval(string doNoteName, L1C5MelodicDrillType drillType)
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
                #region 4ths.

                case L1C5MelodicDrillType.DoFa4Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.ReSo4Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.MiLa4Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.FaTi4Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.SoDo4Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.LaRe4Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.TiMi4Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.FaDo4Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.SoRe4Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.LaMi4Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.TiFa4Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.DoSo4Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.ReLa4Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.MiTi4Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    break;

                #endregion 4ths.

                #region 5ths.

                case L1C5MelodicDrillType.DoSo5Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.ReLa5Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.MiTi5Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.FaDo5Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.SoRe5Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.LaMi5Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.TiFa5Asc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect11th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.SoDo5Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.LaRe5Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.TiMi5Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.DoFa5Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfectOctave, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.ReSo5Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor9th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.MiLa5Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor10th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    break;

                case L1C5MelodicDrillType.FaTi5Desc:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect11th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    break;

                #endregion 5ths.

                default:
                    throw new NotSupportedException($"L1C5MelodicDrillType '{drillType}' is not supported.");
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