using EarTrainingLibrary.Enums;
using EarTrainingLibrary.NAudio;
using EarTrainingLibrary.Utility;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class L2Controller : BaseController
    {
        public L2Controller()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
        }

        public ActionResult MajorTriadProgressions()
        {
            return View();
        }

        public ActionResult CreateProgression(string pitchName, string chordProgression, string movementProgression)
        {
            Debug.WriteLine("CreateProgression()");
            Debug.WriteLine($"pitchName: {pitchName}, chordProgression: {chordProgression}, movementProgression: {movementProgression}");

            string[] chordsAndInversions = chordProgression.Split('-');
            string[] movements = movementProgression.Split(',');
            MixingSampleProvider[] msps = new MixingSampleProvider[chordsAndInversions.Length];
            for (int i = 0; i < chordsAndInversions.Length; i++)
            {
                string chordAndInversion;
                string movement;

                chordAndInversion = chordsAndInversions[i];
                if (i == 0)
                {
                    movement = null;
                }
                else
                {
                    movement = movements[i - 1];
                }

                msps[i] = CreateMajorTriad(pitchName, chordAndInversion, movement);
            }

            //ISampleProvider note = NAudioHelper.GetSampleProvider(pitchName, TimeSpan.FromSeconds(2));
            //var stwp = new SampleToWaveProvider(note);
            ISampleProvider phrase = msps[0];
            for (int i = 1; i < msps.Length; i++)
            {
                phrase = phrase.FollowedBy(msps[i]);
            }
            var stwp = new SampleToWaveProvider(phrase);
            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;

            wavStream.WavToMp3File(out string fileName);
            var path = $"temp/{fileName}";
            JsonResult jsonResult = Json(path, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }

        private MixingSampleProvider CreateMajorTriad(string doNoteName, string chordAndInversion, string movement)
        {
            Debug.WriteLine("CreateMajorTriad()");
            Debug.WriteLine($"doNoteName: {doNoteName}, chordAndInversion: {chordAndInversion}, movement: {movement ?? "null"}");
            double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNoteMillis = (60 / bpm) * 1000;
            double halfNoteMillis = quarterNoteMillis * 2;
            TimeSpan noteDuration = TimeSpan.FromMilliseconds(halfNoteMillis);

            string doNoteRegisterString = Regex.Replace(doNoteName, "[A-G#b]", "");
            int doNoteRegister = int.Parse(doNoteRegisterString);
            string[] parts = chordAndInversion.Trim().Split(' ');
            string chord = parts[0].Trim();

            string inversion = parts[1].Trim();
            InversionType inversionType;
            switch(inversion)
            {
                case "(root)":
                    inversionType = InversionType.Root;
                    break;

                case "(1st)":
                    inversionType = InversionType.LowFirst;
                    break;

                case "(2nd)":
                    inversionType = InversionType.LowSecond;
                    break;

                default:
                    inversionType = InversionType.Root;
                    break;
            }

            //bool secondNoteIsHigher = IsSecondNoteHigher(doNoteName, chord);

            //string thisChordRootNote;
            //if (secondNoteIsHigher)
            //{
            //    thisChordRootNote = chord + (doNoteRegister + 1).ToString();
            //}
            //else
            //{
            //    thisChordRootNote = chord + doNoteRegister.ToString();
            //}
            string thisChordRootNote = chord + doNoteRegister.ToString();
            string doFileName = NAudioHelper.GetFileNameFromNoteName(thisChordRootNote);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider[] samples;
            samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, doNoteNumber + Interval.UpPerfect4th, doNoteNumber + Interval.UpMajor6th, doNoteNumber + Interval.UpPerfectOctave);

            MixingSampleProvider msp = new MixingSampleProvider(samples[0].WaveFormat);
            msp.AddMixerInput(samples[0]);
            msp.AddMixerInput(samples[1]);
            msp.AddMixerInput(samples[2]);

            return msp;
        }

        private bool IsSecondNoteHigher(string doNoteName, string chord)
        {
            bool isHigher = false;
            return isHigher;
        }
    }
}
