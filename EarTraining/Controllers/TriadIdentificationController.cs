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
    public class TriadIdentificationController : Controller
    {
        // GET: TriadIdentification
        public ActionResult Index()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;

            return View();
        }

        public FileResult GetDO(double frequency)
        {
            Stream stream = Solfeg.GetDONote(frequency);
            return new FileStreamResult(stream, "audio/wav");
        }

        public FileResult GetChord(double frequency, int type, int inverstion)
        {
            ChordType chordType = (ChordType)type;

            TimeSpan noteDuration = TimeSpan.FromSeconds(1);
            double gain = 0.2;
            SignalGeneratorType sgType = SignalGeneratorType.SawTooth;
            var solfeg = new Solfeg(frequency);

            List<ISampleProvider> notes;

            switch(chordType)
            {
                case ChordType.OneMajorTriad:
                    notes = new List<ISampleProvider>(3);
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration));
                    break;

                case ChordType.FourMajorTriad:
                    notes = new List<ISampleProvider>(3);
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.LaFrequency, sgType, noteDuration));
                    break;

                case ChordType.FiveMajorTriad:
                    notes = new List<ISampleProvider>(3);
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.LowTiFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.ReFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration));
                    break;

                case ChordType.SixMinorTriad:
                    notes = new List<ISampleProvider>(3);
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.LaFrequency, sgType, noteDuration));
                    break;

                default:
                    throw new NotSupportedException($"Chord type {chordType} is not supported.");
            }

            notes.Shuffle();

            MixingSampleProvider msp = new MixingSampleProvider(notes[0].WaveFormat);
            foreach (var note in notes)
            {
                msp.AddMixerInput(note);
            }
            
            var wp = msp.ToWaveProvider();
            MemoryStream ms = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(ms, wp);
            ms.Position = 0;

            return new FileStreamResult(ms, "audio/wav");
        }
    }
}