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
        // GET: L1C4
        public ActionResult Index()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;

            return View();
        }

        public ActionResult GetProgressionEx(string doNoteName, int progressiontype)
        {
            var progressionType = (ProgressionType)progressiontype;

            TimeSpan noteDuration = TimeSpan.FromSeconds(1);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider[] samples1;
            ISampleProvider[] samples2;

            switch (progressionType)
            {
                case ProgressionType.Four2ndToOneRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowSecondInversion, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.Up1Octave);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    break;

                case ProgressionType.Five1stToOneRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.LowFirstInversion, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    break;

                case ProgressionType.FiveRootToFourRoot:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration, doNoteNumber + Interval.UpPerfect5th, doNoteNumber + Interval.UpMajor7th, doNoteNumber + Interval.UpMajor9th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.Up1Octave);
                    break;

                case ProgressionType.OneRootToFour2nd:
                    samples1 = Inversion.CreateTriadInversionEx(InversionType.RootPosition, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    samples2 = Inversion.CreateTriadInversionEx(InversionType.LowSecondInversion, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.Up1Octave);

                    break;

                default:
                    throw new NotSupportedException($"ProgressionType {progressionType} is not supported.");
            }

            MixingSampleProvider msp1 = new MixingSampleProvider(samples1[0].WaveFormat);
            msp1.AddMixerInput(samples1[0]);
            msp1.AddMixerInput(samples1[1]);
            msp1.AddMixerInput(samples1[2]);

            ISampleProvider shortRest = new SignalGenerator()
            {
                Gain = 0
            }.Take(TimeSpan.FromMilliseconds(200));

            MixingSampleProvider msp2 = new MixingSampleProvider(samples2[0].WaveFormat);
            msp2.AddMixerInput(samples2[0]);
            msp2.AddMixerInput(samples2[1]);
            msp2.AddMixerInput(samples2[2]);

            var phrase = msp1
                //.FollowedBy(shortRest)
                .FollowedBy(msp2);

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;

            MemoryStream mp3Stream = wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }
    }
}