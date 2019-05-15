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
    public class TriadIdentificationController : BaseController
    {
        // GET: TriadIdentification
        public ActionResult Index()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;

            return View();
        }

        public ActionResult GetChord(double frequency, int type)
        {
            TriadType triadType = (TriadType)type;

            TimeSpan noteDuration = TimeSpan.FromSeconds(1);
            double gain = 0.2;
            SignalGeneratorType sgType = SignalGeneratorType.SawTooth;
            var solfeg = new Solfeg(frequency);

            List<ISampleProvider> notes;

            switch(triadType)
            {
                case TriadType.OneMajorTriad:
                    notes = new List<ISampleProvider>(3);
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration));
                    break;

                case TriadType.FourMajorTriad:
                    notes = new List<ISampleProvider>(3);
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.FaFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.LaFrequency, sgType, noteDuration));
                    break;

                case TriadType.FiveMajorTriad:
                    notes = new List<ISampleProvider>(3);
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.LowTiFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.ReFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.SoFrequency, sgType, noteDuration));
                    break;

                case TriadType.SixMinorTriad:
                    notes = new List<ISampleProvider>(3);
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.DoFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.MiFrequency, sgType, noteDuration));
                    notes.Add(NAudioHelper.GetSampleProvider(gain, solfeg.LaFrequency, sgType, noteDuration));
                    break;

                default:
                    throw new NotSupportedException($"Chord type {triadType} is not supported.");
            }

            MixingSampleProvider msp = new MixingSampleProvider(notes[0].WaveFormat);
            foreach (var note in notes)
            {
                msp.AddMixerInput(note);
            }
            
            var wp = msp.ToWaveProvider();
            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, wp);
            wavStream.Position = 0;

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }
    }
}