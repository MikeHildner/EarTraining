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
    public class L2C5Controller : BaseController
    {
        public L2C5Controller()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;

            ViewBag.ShowPlayDoTriad = true;
            ViewBag.ShowDo = true;
        }

        public ActionResult DiatonicTriadProgressions4()
        {
            return View();
        }

        public ActionResult GetTriad(string doNoteName, int triadtype)
        {
            var triadType = (L2C5TriadType)triadtype;
            InversionType inversionType = GetRandomInversion();

            TimeSpan noteDuration = TimeSpan.FromSeconds(3);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider[] samples;
            int newDoNoteNumber;  // Used for other chords besides the I.
            switch (triadType)
            {
                case L2C5TriadType.OneMajor:
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    break;

                case L2C5TriadType.TwoMinor:
                    newDoNoteNumber = doNoteNumber + Interval.UpMajor2nd;
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMinor3rd, newDoNoteNumber + Interval.UpPerfect5th);
                    break;

                case L2C5TriadType.ThreeMinor:
                    newDoNoteNumber = doNoteNumber + Interval.UpMajor3rd;
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMinor3rd, newDoNoteNumber + Interval.UpPerfect5th);
                    break;

                case L2C5TriadType.FourMajor:
                    newDoNoteNumber = doNoteNumber + Interval.UpPerfect4th;
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMajor3rd, newDoNoteNumber + Interval.UpPerfect5th);
                    break;

                case L2C5TriadType.FiveMajor:
                    newDoNoteNumber = doNoteNumber + Interval.UpPerfect5th;
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMajor3rd, newDoNoteNumber + Interval.UpPerfect5th);
                    break;

                case L2C5TriadType.SixMinor:
                    newDoNoteNumber = doNoteNumber + Interval.UpMajor6th;
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMinor3rd, newDoNoteNumber + Interval.UpPerfect5th);
                    break;

                case L2C5TriadType.SevenDiminished:
                    newDoNoteNumber = doNoteNumber + Interval.UpMajor7th;
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMinor3rd, newDoNoteNumber + Interval.UpDiminished5th);
                    break;

                default:
                    throw new NotSupportedException($"L2C5TriadType {triadType} is not supported.");
            }

            MixingSampleProvider msp = new MixingSampleProvider(samples[0].WaveFormat);
            msp.AddMixerInput(samples[0]);
            msp.AddMixerInput(samples[1]);
            msp.AddMixerInput(samples[2]);

            IWaveProvider wp = msp.ToWaveProvider();
            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, wp);
            wavStream.Position = 0;

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private InversionType GetRandomInversion()
        {
            var values = Enum.GetValues(typeof(InversionType)).Cast<int>();
            int min = values.Min();
            int max = values.Max() + 1;

            Random r = new Random();
            int rint = r.Next(min, max);

            return (InversionType)rint;
        }

        public ActionResult Get4ChordProgression(string doNoteName, string progression)
        {
            TimeSpan noteDuration = TimeSpan.FromSeconds(2);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            string[] chords = progression.Split('-');
            ISampleProvider[] samples1, samples2, samples3, samples4;

            // First chord.
            string[] chordParts = chords[0].Split(' ');
            int degree = int.Parse(chordParts[0]);
            int newDoNoteNumber = GetDoNoteNumber(doNoteNumber, degree);
            samples1 = CreateTriad(newDoNoteNumber, degree, noteDuration);

            // Second chord.
            chordParts = chords[1].Split(' ');
            degree = int.Parse(chordParts[0]);
            newDoNoteNumber = GetDoNoteNumber(doNoteNumber, degree);
            samples2 = CreateTriad(newDoNoteNumber, degree, noteDuration);

            // Third chord.
            chordParts = chords[2].Split(' ');
            degree = int.Parse(chordParts[0]);
            newDoNoteNumber = GetDoNoteNumber(doNoteNumber, degree);
            samples3 = CreateTriad(newDoNoteNumber, degree, noteDuration);

            // Fourth chord.
            chordParts = chords[3].Split(' ');
            degree = int.Parse(chordParts[0]);
            newDoNoteNumber = GetDoNoteNumber(doNoteNumber, degree);
            samples4 = CreateTriad(newDoNoteNumber, degree, noteDuration);

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

            MixingSampleProvider msp4 = new MixingSampleProvider(samples4[0].WaveFormat);
            msp4.AddMixerInput(samples4[0]);
            msp4.AddMixerInput(samples4[1]);
            msp4.AddMixerInput(samples4[2]);

            var phrase = msp1
                .FollowedBy(msp2)
                .FollowedBy(msp3)
                .FollowedBy(msp4);

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        private ISampleProvider[] CreateTriad(int newDoNoteNumber, int degree, TimeSpan noteDuration)
        {
            ISampleProvider[] samples;

            switch (degree)
            {
                case 1:
                case 4:
                case 5:
                    samples = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMajor3rd, newDoNoteNumber + Interval.UpPerfect5th);
                    break;

                case 2:
                case 3:
                case 6:
                    samples = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMinor3rd, newDoNoteNumber + Interval.UpPerfect5th);
                    break;

                case 7:
                    samples = Inversion.CreateTriadInversionEx(InversionType.Root, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMinor3rd, newDoNoteNumber + Interval.UpDiminished5th);
                    break;

                default:
                    throw new NotImplementedException($"degree '{degree}' is not supported.");
            }

            return samples;
        }

        private int GetDoNoteNumber(int doNoteNumber, int degree)
        {
            int rv;

            switch(degree)
            {
                case 1:
                    rv = 1;
                    break;

                case 2:
                    rv = doNoteNumber + Interval.UpMajor2nd;
                    break;

                case 3:
                    rv = doNoteNumber + Interval.UpMajor3rd;
                    break;

                case 4:
                    rv = doNoteNumber + Interval.UpPerfect4th;
                    break;

                case 5:
                    rv = doNoteNumber + Interval.UpPerfect5th;
                    break;

                case 6:
                    rv = doNoteNumber + Interval.UpMajor6th;
                    break;

                case 7:
                    rv = doNoteNumber + Interval.UpMajor7th;
                    break;

                default:
                    throw new NotSupportedException($"degree '{degree}' is not supported.");
            }

            return rv;
        }
    }
}
