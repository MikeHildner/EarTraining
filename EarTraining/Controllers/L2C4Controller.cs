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
    public class L2C4Controller : BaseController
    {
        public L2C4Controller()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
        }

        public ActionResult MajorTriadProgressions2(string @do)
        {
            if (!string.IsNullOrWhiteSpace(@do))
            {
                Pitch pitch = new Pitches().PitchesList.Single(s => s.PitchName.ToUpper().Split('/').Contains(@do.ToUpper()));
                ViewBag.Pitch = pitch;
            }

            return View();
        }

        public ActionResult MajorTriadProgressions3()
        {
            return View();
        }

        public ActionResult Get2ChordProgression(string doNoteName, int progressiontype)
        {
            var progressionType = (L2C4ProgressionType)progressiontype;

            TimeSpan noteDuration = TimeSpan.FromSeconds(2);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider[] samples1;
            ISampleProvider[] samples2;

            switch (progressionType)
            {
                case L2C4ProgressionType.Five2ndToOne1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.HighFirst, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    break;

                case L2C4ProgressionType.Five1stToOneRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    break;

                case L2C4ProgressionType.FiveRootToOne2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.HighSecond, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    break;

                case L2C4ProgressionType.Four1stToOne2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    break;

                case L2C4ProgressionType.FourRootToOne1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.HighFirst, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    break;

                case L2C4ProgressionType.Four2ndToOneRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    break;

                case L2C4ProgressionType.OneRootToFlatTwoRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpMinor2nd, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpAugmented5th);
                    break;

                case L2C4ProgressionType.OneRootToFlatTwo2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpMinor2nd, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpAugmented5th);
                    break;

                case L2C4ProgressionType.OneRootToSevenRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber + Interval.DownMinor2nd, doNoteNumber + Interval.UpMinor3rd, doNoteNumber + Interval.UpDiminished5th);
                    break;

                case L2C4ProgressionType.OneRootToSeven1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.HighFirst, noteDuration.Add(noteDuration), doNoteNumber + Interval.DownMinor2nd, doNoteNumber + Interval.UpMinor3rd, doNoteNumber + Interval.UpDiminished5th);
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

        public ActionResult Get3ChordProgression(string doNoteName, int progressiontype)
        {
            var progressionType = (L2C4ProgressionType)progressiontype;

            TimeSpan noteDuration = TimeSpan.FromSeconds(2);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider[] samples1, samples2, samples3;

            switch (progressionType)
            {
                case L2C4ProgressionType.OneThen5ThenHalfUp:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave);
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpDiminished5th, doNoteNumber + Interval.UpMinor7th, doNoteNumber + Interval.UpMinor9th);
                    break;

                case L2C4ProgressionType.OneThen4ThenHalfDown:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th);
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpDiminished5th, doNoteNumber + Interval.UpMinor7th, doNoteNumber + Interval.UpMinor9th);
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