using EarTrainingLibrary.Enums;
using EarTrainingLibrary.NAudio;
using EarTrainingLibrary.Utility;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

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
            _log.Debug($"pitchName: {pitchName}, chordProgression: {chordProgression}, movementProgression: {movementProgression}");

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
            _log.Debug($"doNoteName: {doNoteName}, chordAndInversion: {chordAndInversion}, movement: {movement ?? "null"}");
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
            switch (inversion)
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

            string thisChordRootNote = GetClosestNote(doNoteName, chord);
            //thisChordRootNote = chord + doNoteRegister.ToString();
            _log.Debug($"doNoteName: {doNoteName}, thisChordRootNote: {thisChordRootNote}");
            string doFileName = NAudioHelper.GetFileNameFromNoteName(thisChordRootNote);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider[] samples;
            samples = Inversion.CreateTriadInversionEx(inversionType, noteDuration, doNoteNumber, doNoteNumber + Interval.UpMajor3rd, doNoteNumber + Interval.UpPerfect5th);

            MixingSampleProvider msp = new MixingSampleProvider(samples[0].WaveFormat);
            msp.AddMixerInput(samples[0]);
            msp.AddMixerInput(samples[1]);
            msp.AddMixerInput(samples[2]);

            return msp;
        }

        private string GetClosestNote(string doNoteName, string secondNote)
        {
            string doNoteRegisterString = doNoteName[doNoteName.Length - 1].ToString();
            int doNoteRegister = int.Parse(doNoteRegisterString);

            PitchNumber doNotePitchNumber = (PitchNumber)Enum.Parse(typeof(PitchNumber), doNoteName);
            //int doNoteNumber = (int)doNotePitchNumber;

            string lowerRegisterSecondNoteName = secondNote + (doNoteRegister - 1).ToString();
            string sameRegisterSecondNoteName = secondNote + (doNoteRegister).ToString();
            string higherRegisterSecondNoteName = secondNote + (doNoteRegister + 1).ToString();

            PitchNumber lowerRegisterPitchNumber, sameRegisterPitchNumber, higherRegisterPitchNumber;
            int lowerRegisterSecondNoteNumber, sameRegisterSecondNoteNumber, higherRegisterSecondNoteNumber;

            // Note: Because we're limiting the initial notes two be the middle two octaves (C3 through B4 in Pitches.cs),
            // we don't have to worry about (for now) lower or higher registers being out of the range (e.g. A-1 or C10).

            // See https://docs.microsoft.com/en-us/dotnet/api/system.enum.tryparse?view=netframework-4.8 to do this robustly.

            Enum.TryParse(lowerRegisterSecondNoteName, out lowerRegisterPitchNumber);
            lowerRegisterSecondNoteNumber = (int)lowerRegisterPitchNumber;

            Enum.TryParse(sameRegisterSecondNoteName, out sameRegisterPitchNumber);
            sameRegisterSecondNoteNumber = (int)sameRegisterPitchNumber;

            Enum.TryParse(higherRegisterSecondNoteName, out higherRegisterPitchNumber);
            higherRegisterSecondNoteNumber = (int)higherRegisterPitchNumber;


            int lowerRegisterDiff = Math.Abs(sameRegisterSecondNoteNumber - lowerRegisterSecondNoteNumber);
            int sameRegisterDiff = Math.Abs(sameRegisterSecondNoteNumber - sameRegisterSecondNoteNumber);
            int higherRegisterDiff = Math.Abs(sameRegisterSecondNoteNumber - higherRegisterSecondNoteNumber);

            string returnedSecondNoteName;

            if(lowerRegisterDiff <= 6)
            {
                returnedSecondNoteName = lowerRegisterSecondNoteName;
            }
            else if(sameRegisterDiff <= 6)
            {
                returnedSecondNoteName = sameRegisterSecondNoteName;
            }
            else if(higherRegisterDiff <= 6)
            {
                returnedSecondNoteName = higherRegisterSecondNoteName;
            }
            else
            {
                throw new Exception("Could not find a closest note name.");
            }

            _log.Debug($"doNoteName: {doNoteName}, secondNote: {secondNote}, returnedSecondNoteName: {returnedSecondNoteName}");

            return returnedSecondNoteName;
        }
    }
}
