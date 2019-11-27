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
    public class L1C3Controller : BaseController
    {
        public L1C3Controller()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
            ViewBag.ShowDo = true;
        }

        // GET: L1C3
        public ActionResult Index()
        {
            ViewBag.ShowPlayDoTriad = true;

            return View();
        }

        public ActionResult VocalDrills()
        {
            return View();
        }

        public ActionResult MelodicMin3rdMaj6thNoDO()
        {
            return View();
        }

        public ActionResult HarmonicMin3rdMaj6th()
        {
            return View();
        }

        public ActionResult GetTriadEx(string doNoteName, int triadtype, int inversion)
        {
            var triadType = (TriadType)triadtype;
            var inversionType = (InversionType)inversion;

            TimeSpan noteDuration = TimeSpan.FromSeconds(3);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider[] samples;
            int newDoNoteNumber;  // Used for other chords besides the I.
            switch (triadType)
            {
                case TriadType.OneMajor:
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    break;

                case TriadType.FourMajor:
                    newDoNoteNumber = doNoteNumber + Interval.UpPerfect4th;
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMajor3rd, newDoNoteNumber + Interval.UpPerfect5th);
                    break;

                case TriadType.FiveMajor:
                    newDoNoteNumber = doNoteNumber + Interval.UpPerfect5th;
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMajor3rd, newDoNoteNumber + Interval.UpPerfect5th);
                    break;

                case TriadType.SixMinor:
                    newDoNoteNumber = doNoteNumber + Interval.UpMajor6th;
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMinor3rd, newDoNoteNumber + Interval.UpPerfect5th);
                    break;

                case TriadType.ThreeMinor:
                    newDoNoteNumber = doNoteNumber + Interval.UpMajor3rd;
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMinor3rd, newDoNoteNumber + Interval.UpPerfect5th);
                    break;

                case TriadType.TwoMinor:
                    newDoNoteNumber = doNoteNumber + Interval.UpMajor2nd;
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, newDoNoteNumber, newDoNoteNumber + Interval.UpMinor3rd, newDoNoteNumber + Interval.UpPerfect5th);
                    break;

                default:
                    throw new NotSupportedException($"TriadType {triadType} is not supported.");
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
    }
}