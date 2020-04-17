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
    public class L1C4Controller : BaseController
    {
        public L1C4Controller()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
            ViewBag.ShowDo = true;
        }

        // GET: L1C4
        public ActionResult Index()
        {
            ViewBag.ShowPlayDoTriad = true;

            return View();
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

            int bassNoteNumber;

            switch (progressionType)
            {
                case ProgressionType.Four2ndToOneRoot:
                    bassNoteNumber = (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber();
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, bassNoteNumber);
                    bassNoteNumber = doNoteNumber.BassNoteNumber();
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, bassNoteNumber);
                    break;

                case ProgressionType.Five1stToOneRoot:
                    bassNoteNumber = (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber();
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, bassNoteNumber);
                    bassNoteNumber = doNoteNumber.BassNoteNumber();
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, bassNoteNumber);
                    break;

                case ProgressionType.FiveRootToFourRoot:
                    bassNoteNumber = (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber();
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, bassNoteNumber);
                    bassNoteNumber = (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber();
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, bassNoteNumber);
                    break;

                case ProgressionType.OneRootToFour2nd:
                    bassNoteNumber = doNoteNumber.BassNoteNumber();
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, bassNoteNumber);
                    bassNoteNumber = (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber();
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, bassNoteNumber);
                    break;

                case ProgressionType.OneRootToFive1st:
                    bassNoteNumber = doNoteNumber.BassNoteNumber();
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, bassNoteNumber);
                    bassNoteNumber = (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber();
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, bassNoteNumber);
                    break;

                default:
                    throw new NotSupportedException($"ProgressionType {progressionType} is not supported.");
            }

            MixingSampleProvider msp1 = new MixingSampleProvider(samples1[0].WaveFormat);
            msp1.AddMixerInput(samples1[0]);
            msp1.AddMixerInput(samples1[1]);
            msp1.AddMixerInput(samples1[2]);
            if(samples1[3] != null)  // Bass note.
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

                case ProgressionType.Five1stFour2ndOneRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration.Add(noteDuration), doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    break;

                case ProgressionType.OneRootFour2ndFive1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    break;

                case ProgressionType.Five1stOneRootFour2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    break;

                case ProgressionType.OneRootFive1stFour2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    
                    break;

                case ProgressionType.Four2ndOneRootFive1st:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecond, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave, (doNoteNumber + Interval.UpPerfect4th).BassNoteNumber());
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th, doNoteNumber.BassNoteNumber());
                    samples3 = Inversion.CreateTriadInversionEx(InversionType.LowFirst, noteDuration.Add(noteDuration), doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th, (doNoteNumber + Interval.UpPerfect5th).BassNoteNumber());
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
    }
}