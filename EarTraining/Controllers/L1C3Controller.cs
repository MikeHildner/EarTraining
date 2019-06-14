﻿using EarTrainingLibrary.Enums;
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
        }

        // GET: L1C3
        public ActionResult Index()
        {
            ViewBag.ShowPlayDoTriad = true;

            return View();
        }

        public ActionResult MelodicMin3rdMaj6th()
        {
            return View();
        }

        public ActionResult HarmonicMin3rdMaj6th()
        {
            return View();
        }

        public ActionResult GetTriad(double frequency, int triadtype, int inversion)
        {
            var triadType = (TriadType)triadtype;
            var inversionType = (InversionType)inversion;

            TimeSpan noteDuration = TimeSpan.FromSeconds(1);
            double gain = 0.2;
            SignalGeneratorType sgType = SignalGeneratorType.SawTooth;
            var solfeg = new Solfeg(frequency);

            ISampleProvider[] samples;
            switch(triadType)
            {
                case TriadType.OneMajorTriad:
                    samples = Inversion.CreateInversion(inversionType, gain, noteDuration, sgType, solfeg.DoFrequency, solfeg.MiFrequency, solfeg.SoFrequency);
                    break;

                case TriadType.FourMajorTriad:
                    samples = Inversion.CreateInversion(inversionType, gain, noteDuration, sgType, solfeg.FaFrequency, solfeg.LaFrequency, solfeg.HighDoFrequency);
                    break;

                case TriadType.FiveMajorTriad:
                    samples = Inversion.CreateInversion(inversionType, gain, noteDuration, sgType, solfeg.SoFrequency, solfeg.TiFrequency, solfeg.HighReFrequency);
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
                case TriadType.OneMajorTriad:
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);
                    break;

                case TriadType.FourMajorTriad:
                    newDoNoteNumber = doNoteNumber + Interval.UpPerfect4th;
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, newDoNoteNumber, newDoNoteNumber + 4, newDoNoteNumber + 7);
                    break;

                case TriadType.FiveMajorTriad:
                    newDoNoteNumber = doNoteNumber + Interval.UpPerfect5th;
                    samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, newDoNoteNumber, newDoNoteNumber + 4, newDoNoteNumber + 7);
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