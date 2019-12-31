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
            int max = values.Max() +1;

            Random r = new Random();
            int rint = r.Next(min, max);

            return (InversionType)rint;
        }
    }
}
