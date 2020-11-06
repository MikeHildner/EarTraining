using EarTrainingLibrary.Enums;
using EarTrainingLibrary.NAudio;
using EarTrainingLibrary.Utility;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Windows.Forms;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class L1C1Controller : BaseController
    {
        public L1C1Controller()
        {
            Pitch pitch = new Pitches().Random();
            //pitch = new Pitches().PitchesList.Single(s => s.PitchName == "E4");
            ViewBag.Pitch = pitch;
            ViewBag.ShowDo = true;
        }

        public ActionResult SolfegResolutionsDO(string @do)
        {
            if(!string.IsNullOrWhiteSpace(@do))
            {
                Pitch pitch = new Pitches().PitchesList.Single(s => s.PitchName == @do.ToUpper());
                ViewBag.Pitch = pitch;
            }

            return View();
        }

        public ActionResult SolfegResolutionsNoDO(string @do)
        {
            if (!string.IsNullOrWhiteSpace(@do))
            {
                Pitch pitch = new Pitches().PitchesList.Single(s => s.PitchName == @do.ToUpper());
                ViewBag.Pitch = pitch;
            }

            return View();
        }

        public ActionResult PitchIdentification(string @do)
        {
            if (!string.IsNullOrWhiteSpace(@do))
            {
                Pitch pitch = new Pitches().PitchesList.Single(s => s.PitchName == @do.ToUpper());
                ViewBag.Pitch = pitch;
            }

            return View();
        }

        public ActionResult DictationTranscription()
        {
            ViewBag.ShowDo = false;
            return View();
        }

        public ActionResult Experimental()
        {
            return View();
        }

        public ActionResult GetResolutionDO(string doNoteName, int type)
        {
            ResolutionType rt = (ResolutionType)type;
            MemoryStream wavStream = GetResolutionDO(doNoteName, rt);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");

        }

        private MemoryStream GetResolutionDO(string doNoteName, ResolutionType resolutionType)
        {
            double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNotemillis = (60 / bpm) * 1000;
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNotemillis);
            TimeSpan halfNoteDuration = quarterNoteDuration.Add(quarterNoteDuration);
            TimeSpan wholeNoteDuration = halfNoteDuration.Add(halfNoteDuration);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1;
            ISampleProvider note2;
            ISampleProvider note3;
            ISampleProvider note4;

            switch (resolutionType)
            {

                case ResolutionType.DoDoReDo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, halfNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoFaMi:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoLaSo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoTiHighDo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + 12, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoLowTiDo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, halfNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                default:
                    throw new NotSupportedException($"ResolutionType '{resolutionType}' is not supported.");
            }

            ISampleProvider phrase = note1
                .FollowedBy(note2)
                .FollowedBy(note3)
                .FollowedBy(note4);

            SampleToWaveProvider stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            return wavStream;
        }

        public ActionResult GetResolutionNoDO(string doNoteName, int type)
        {
            ResolutionType rt = (ResolutionType)type;
            MemoryStream wavStream = GetResolutionNoDO(doNoteName, rt);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");

        }

        private MemoryStream GetResolutionNoDO(string doNoteName, ResolutionType resolutionType)
        {
            double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNotemillis = (60 / bpm) * 1000;
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNotemillis);
            TimeSpan halfNoteDuration = quarterNoteDuration.Add(quarterNoteDuration);
            TimeSpan wholeNoteDuration = halfNoteDuration.Add(halfNoteDuration);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1;
            ISampleProvider note2;

            switch (resolutionType)
            {

                case ResolutionType.DoDoReDo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoFaMi:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoLaSo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoTiHighDo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + 12, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoLowTiDo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                default:
                    throw new NotSupportedException($"ResolutionType '{resolutionType}' is not supported.");
            }

            ISampleProvider phrase = note1
                .FollowedBy(note2);

            SampleToWaveProvider stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            return wavStream;
        }

        public ActionResult GetNoteEx(string doNoteName, int type)
        {
            int doNoteNumber = NoteHelper.NoteNumberFromNoteName(doNoteName);
            int thisNoteNumber = doNoteNumber + type;
            ISampleProvider note = NAudioHelper.GetSampleProvider(thisNoteNumber, TimeSpan.FromSeconds(3));

            var stwp = new SampleToWaveProvider(note);
            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        public ActionResult AudioAndDictation(int resolutionType, string keySignature, double bpm, int numberOfMeasures)
        {
            // We'll add stuff to the Dictionary and return as JSON.
            var dict = new Dictionary<string, string>();

            #region Audio

            //double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNoteMillis = (60 / bpm) * 1000;
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis);
            TimeSpan halfNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis * 2);
            TimeSpan dottedHalfNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis * 3);
            TimeSpan wholeNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis * 4);

            // Setup the scale note numbers.
            int[] scaleNoteNumbers = new int[] { 38, 39, 41, 43, 44, 46, 48, 50, 51 };  // C Major, with a low TI.
            scaleNoteNumbers = NoteHelper.TransposeScaleNoteNumbers(scaleNoteNumbers, keySignature);

            // The initial DO.
            ISampleProvider wholeDoNote = NAudioHelper.GetSampleProvider(scaleNoteNumbers[1], wholeNoteDuration);

            // Four metronome ticks before the transcription part plays.
            ISampleProvider[] ticks = new ISampleProvider[4];
            string tickFile = HostingEnvironment.MapPath($"~/Samples/Woodblock.wav");
            for (int i = 0; i < ticks.Length; i++)
            {
                ticks[i] = NAudioHelper.GetSampleProviderFromFile(tickFile, quarterNoteDuration);
            }

            List<string> measureRhythms = GetNoteRhythms();

            int randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            string measureRhythm1 = measureRhythms[randomInt];
            randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            string measureRhythm2 = measureRhythms[randomInt];
            if(numberOfMeasures == 2)
            {
                while ((measureRhythm1.Split(',').Count() + measureRhythm2.Split(',').Count()) % 2 == 1)  // Ensure an even number of notes, so there's always a (reverse) resolution.
                {
                    randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
                    measureRhythm2 = measureRhythms[randomInt];
                }
            }
            randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            string measureRhythm3 = measureRhythms[randomInt];
            randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            string measureRhythm4 = measureRhythms[randomInt];
            if (numberOfMeasures == 4)
            {
                while ((measureRhythm1.Split(',').Count() + measureRhythm2.Split(',').Count() + measureRhythm3.Split(',').Count() + measureRhythm4.Split(',').Count()) % 2 == 1)
                {
                    randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
                    measureRhythm4 = measureRhythms[randomInt];
                }
            }

            string[] measureRhythmSplit1 = measureRhythm1.Split(',');
            int numberOfNotes1 = measureRhythmSplit1.Length;
            string[] measureRhythmSplit2 = measureRhythm2.Split(',');
            int numberOfNotes2 = measureRhythmSplit2.Length;
            string[] measureRhythmSplit3 = measureRhythm3.Split(',');
            int numberOfNotes3 = measureRhythmSplit3.Length;
            string[] measureRhythmSplit4 = measureRhythm4.Split(',');
            int numberOfNotes4 = measureRhythmSplit4.Length;

            ISampleProvider[] notes1 = new ISampleProvider[numberOfNotes1];
            ISampleProvider[] notes2 = new ISampleProvider[numberOfNotes2];
            ISampleProvider[] notes3 = new ISampleProvider[numberOfNotes3];
            ISampleProvider[] notes4 = new ISampleProvider[numberOfNotes4];

            Queue<int> noteNumberQueue;
            switch (resolutionType)
            {
                case 1:
                    noteNumberQueue = GetResolutionIntQueue(scaleNoteNumbers, 16, 1);  // 16 notes max.
                    break;

                case 2:
                    noteNumberQueue = GetResolutionIntQueue(scaleNoteNumbers, 16, 2);
                    break;

                case 3:
                    noteNumberQueue = GetResolutionIntQueue(scaleNoteNumbers, 16, 3);
                    break;

                default:
                    throw new NotSupportedException($"ResolutionType '{resolutionType}' is not supported.");
            }

            int[] measureNoteNumbers1 = NoteHelper.PopulateNoteNumbersFromQueue(numberOfNotes1, noteNumberQueue);
            int[] measureNoteNumbers2 = NoteHelper.PopulateNoteNumbersFromQueue(numberOfNotes2, noteNumberQueue);
            int[] measureNoteNumbers3 = NoteHelper.PopulateNoteNumbersFromQueue(numberOfNotes3, noteNumberQueue);
            int[] measureNoteNumbers4 = NoteHelper.PopulateNoteNumbersFromQueue(numberOfNotes4, noteNumberQueue);

            NoteHelper.CreateSamplesFromRhythmsAndNoteNames(new TimeSpan(), quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit1, notes1, measureNoteNumbers1);
            NoteHelper.CreateSamplesFromRhythmsAndNoteNames(new TimeSpan(), quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit2, notes2, measureNoteNumbers2);
            NoteHelper.CreateSamplesFromRhythmsAndNoteNames(new TimeSpan(), quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit3, notes3, measureNoteNumbers3);
            NoteHelper.CreateSamplesFromRhythmsAndNoteNames(new TimeSpan(), quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit4, notes4, measureNoteNumbers4);

            ISampleProvider phrase =
                wholeDoNote;

            foreach (var tick in ticks)
            {
                phrase = phrase.FollowedBy(tick);
            }

            foreach (var note1 in notes1)
            {
                phrase = phrase.FollowedBy(note1);
            }
            foreach (var note2 in notes2)
            {
                phrase = phrase.FollowedBy(note2);
            }
            if(numberOfMeasures == 4)
            {
                foreach (var note3 in notes3)
                {
                    phrase = phrase.FollowedBy(note3);
                }
                foreach (var note4 in notes4)
                {
                    phrase = phrase.FollowedBy(note4);
                }
            }

            // HACK: Use an empty note because without it, the audio gets cut short.
            ISampleProvider emptyNote = NAudioHelper.GetSampleProvider(0, 0, SignalGeneratorType.White, halfNoteDuration);
            phrase = phrase.FollowedBy(emptyNote);

            SampleToWaveProvider stwp = new SampleToWaveProvider(phrase);
            MixingSampleProvider msp = new MixingSampleProvider(stwp.WaveFormat);
            msp.AddMixerInput(stwp);

            int totalTicks = 8 + (4 * numberOfMeasures);
            ISampleProvider[] metronomeTicks = new ISampleProvider[totalTicks];

            // The first two measures have zero gain, as this is the initial DO whole note and metronome ticks.
            for (int i = 0; i < 8; i++)
            {
                metronomeTicks[i] = NAudioHelper.GetSampleProvider(0, 0, SignalGeneratorType.White, quarterNoteDuration);
            }
            for (int i = 8; i < totalTicks; i++)
            {
                metronomeTicks[i] = NAudioHelper.GetSampleProviderFromFile(tickFile, quarterNoteDuration);
            }

            ISampleProvider metronomePhrase = metronomeTicks[0];
            for (int i = 0; i < metronomeTicks.Length; i++)
            {
                metronomePhrase = metronomePhrase.FollowedBy(metronomeTicks[i]);
            }

            msp.AddMixerInput(metronomePhrase);

            IWaveProvider wp = msp.ToWaveProvider();

            MemoryStream wavStream = new MemoryStream();
            //WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            WaveFileWriter.WriteWavFileToStream(wavStream, wp);
            wavStream.Position = 0;
            wavStream.WavToMp3File(out string fileName);
            dict.Add("src", fileName);

            #endregion Audio

            #region Notation

            string[] noteNames1 = new string[numberOfNotes1];
            NoteHelper.AdjustNoteNamesForKey(keySignature, measureNoteNumbers1, noteNames1);

            string[] noteNames2 = new string[numberOfNotes2];
            NoteHelper.AdjustNoteNamesForKey(keySignature, measureNoteNumbers2, noteNames2);

            string[] noteNames3 = new string[numberOfNotes3];
            NoteHelper.AdjustNoteNamesForKey(keySignature, measureNoteNumbers3, noteNames3);

            string[] noteNames4 = new string[numberOfNotes4];
            NoteHelper.AdjustNoteNamesForKey(keySignature, measureNoteNumbers4, noteNames4);

            string script1 = NoteHelper.GetEasyScoreScript("transcription1", noteNames1, measureRhythmSplit1, keySignature, true);
            dict.Add("theScript1", script1);
            string script2 = NoteHelper.GetEasyScoreScript("transcription2", noteNames2, measureRhythmSplit2, keySignature, false);
            dict.Add("theScript2", script2);
            if(numberOfMeasures == 4)
            {
                string script3 = NoteHelper.GetEasyScoreScript("transcription3", noteNames3, measureRhythmSplit3, keySignature, false);
                dict.Add("theScript3", script3);
                string script4 = NoteHelper.GetEasyScoreScript("transcription4", noteNames4, measureRhythmSplit4, keySignature, false);
                dict.Add("theScript4", script4);
            }

            #endregion Notation

            var json = Json(dict, JsonRequestBehavior.AllowGet);
            return json;
        }

        private static List<string> GetNoteRhythms()
        {
            // Various rhythm possibilities for each measure.
            List<string> measureRhythms = new List<string>();
            measureRhythms.Add("1");
            measureRhythms.Add("2,2");
            measureRhythms.Add("4,2.");
            measureRhythms.Add("2.,4");
            measureRhythms.Add("4,4,4,4");
            measureRhythms.Add("4,4,2");
            measureRhythms.Add("2,4,4");
            measureRhythms.Add("4,2,4");
            return measureRhythms;
        }

        private Queue<int> GetRandomIntQueue(int[] scaleNoteNumbers, int numberOfNotes)
        {
            var q = new Queue<int>();
            int noteNumber;
            for (int i = 0; i < numberOfNotes; i++)
            {
                noteNumber = GetRandomNoteNumber(scaleNoteNumbers);
                q.Enqueue(noteNumber);
            }
            return q;
        }

        private Queue<int> GetResolutionIntQueue(int[] scaleNoteNumbers, int numberOfNotes, int resolutionType)
        {
            List<Tuple<int, int>> resolutions = new List<Tuple<int, int>>();
            resolutions.Add(new Tuple<int, int>(2, 1));  // RE DO.
            resolutions.Add(new Tuple<int, int>(4, 3));  // FA MI.
            resolutions.Add(new Tuple<int, int>(6, 5));  // LA SO.
            resolutions.Add(new Tuple<int, int>(7, 8));  // High TI DO.
            resolutions.Add(new Tuple<int, int>(0, 1));  // Low TI DO.

            var q = new Queue<int>();
            while (q.Count < numberOfNotes)
            {
                int randomInt = NoteHelper.GetRandomInt(0, resolutions.Count);
                Tuple<int, int> t = resolutions[randomInt];

                switch(resolutionType)
                {
                    case 1:
                        q.Enqueue(scaleNoteNumbers[t.Item1]);
                        q.Enqueue(scaleNoteNumbers[t.Item2]);
                        break;

                    case 2:
                        q.Enqueue(scaleNoteNumbers[t.Item2]);
                        q.Enqueue(scaleNoteNumbers[t.Item1]);
                        break;

                    case 3:
                        int ri = NoteHelper.GetRandomInt(0, 2);
                        if (ri % 2 == 0)
                        {
                            q.Enqueue(scaleNoteNumbers[t.Item1]);
                            q.Enqueue(scaleNoteNumbers[t.Item2]);
                        }
                        else
                        {
                            q.Enqueue(scaleNoteNumbers[t.Item2]);
                            q.Enqueue(scaleNoteNumbers[t.Item1]);
                        }
                        break;
                    default:
                        throw new NotSupportedException($"Resolution type '{resolutionType}' is not supported.");
                }
            }

            return q;
        }

        private int GetRandomNoteNumber(int[] noteNumbers)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] buffer = new byte[4];

                rng.GetBytes(buffer);
                int result = BitConverter.ToInt32(buffer, 0);
                int index = new Random(result).Next(0, noteNumbers.Length);
                return noteNumbers[index];
            }
        }
    }
}