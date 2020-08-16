using EarTrainingLibrary.Enums;
using EarTrainingLibrary.NAudio;
using EarTrainingLibrary.Utility;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class L1C3Controller : BaseController
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

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
            if (samples[3] != null)  // Bass note.
            {
                msp.AddMixerInput(samples[3]);
            }

            IWaveProvider wp = msp.ToWaveProvider();
            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, wp);
            wavStream.Position = 0;

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }

        public ActionResult DictationTranscription()
        {
            //_log.Trace("===== Begin test =====");
            //for (int i = 0; i < 1000; i++)
            //{
            //    AudioAndDictation(1, "C", 60, 2, "EIGHTH", true);
            //}
            //_log.Trace("====== end test ======");

            ViewBag.ShowDo = false;
            return View();
        }

        public ActionResult AudioAndDictation(int intervalType, string keySignature, double bpm, int numberOfMeasures, string smallestRhythmicUnit, bool includeC2)
        {
            _log.Debug($"intervalType: {intervalType}, keySignature: {keySignature}, bpm: {bpm}, numberOfMeasures: {numberOfMeasures}, smallestRhythmicUnit: {smallestRhythmicUnit}, includeC2: {includeC2}");

            L1C3IntervalType intType = (L1C3IntervalType)intervalType;

            bool includeEighthNoteRhythms = smallestRhythmicUnit.ToUpper() == "EIGHTH";

            // We'll add stuff to the Dictionary and return as JSON.
            var dict = new Dictionary<string, string>();

            #region Audio

            //double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNoteMillis = (60 / bpm) * 1000;
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis);
            TimeSpan halfNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis * 2);
            TimeSpan dottedHalfNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis * 3);
            TimeSpan wholeNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis * 4);
            TimeSpan eighthNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis / 2);

            // Setup the scale note numbers.
            int[] scaleNoteNumbers = new int[] { 38, 39, 41, 43, 44, 46, 48, 50, 51, 53, 55, 56, 58, 60 };  // C Major, low TI to high SO.
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

            List<string> measureRhythms = GetNoteRhythms(includeEighthNoteRhythms);

            int randomInt;
            string measureRhythm1;
            string measureRhythm2;
            // Ensure there's at least 4 notes per 2 measures, as we need at least one C1 resolutiona and at least 1 C2 interval.
            // Ensure an even number of notes, so the interval is complete.
            // Ensure there's exactly one pair of eighth notes for each two measure phrase.
            while (true)
            {
                randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
                measureRhythm1 = measureRhythms[randomInt];
                randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
                measureRhythm2 = measureRhythms[randomInt];

                int totalNotes = measureRhythm1.Split(',').Count() + measureRhythm2.Split(',').Count();
                int totalEighthNotes = measureRhythm1.Split(',').Where(w => w == "8").Count() + measureRhythm2.Split(',').Where(w => w == "8").Count();

                // Ensure a minimum number of notes for the 2 measure phrase. This could be 4, or 6 if C2 intervals are included.
                int minimumNumberOfNotes = includeC2 ? 6 : 4;
                if (totalNotes < minimumNumberOfNotes)
                {
                    continue;
                }

                // Ensure an even number of notes.
                if (totalNotes % 2 != 0)
                {
                    continue;
                }

                // If quarter note is smallest rhythmic unit, ensure no eighth notes.
                if (!includeEighthNoteRhythms && totalEighthNotes > 0)
                {
                    continue;
                }

                // If eighth note is smallest rhythic unit, ensure just one pair of eighth notes.
                if (includeEighthNoteRhythms && totalEighthNotes != 2)
                {
                    continue;
                }

                break;
            }

            string measureRhythm3 = string.Empty;
            string measureRhythm4 = string.Empty;

            // TODO: Figure out how to refactor this. Kinda messy, but for now we need to populate measures 3 and 4 even if the user
            // choose just 2 measures.
            randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            measureRhythm3 = measureRhythms[randomInt];
            randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
            measureRhythm4 = measureRhythms[randomInt];

            //if (numberOfMeasures == 4)
            if (true)
            {
                while (true)
                {
                    randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
                    measureRhythm3 = measureRhythms[randomInt];
                    randomInt = NoteHelper.GetRandomInt(0, measureRhythms.Count);
                    measureRhythm4 = measureRhythms[randomInt];

                    int totalNotes = measureRhythm3.Split(',').Count() + measureRhythm4.Split(',').Count();
                    int totalEighthNotes = measureRhythm3.Split(',').Where(w => w == "8").Count() + measureRhythm4.Split(',').Where(w => w == "8").Count();

                    // Ensure a minimum number of notes for the 2 measure phrase. This could be 4, or 6 if C2 intervals are included.
                    int minimumNumberOfNotes = includeC2 ? 6 : 4;
                    if (totalNotes < minimumNumberOfNotes)
                    {
                        continue;
                    }

                    // Ensure an even number of notes.
                    if (totalNotes % 2 != 0)
                    {
                        continue;
                    }

                    // If quarter note is smallest rhythmic unit, ensure no eighth notes.
                    if (!includeEighthNoteRhythms && totalEighthNotes > 0)
                    {
                        continue;
                    }

                    // If eighth note is smallest rhythic unit, ensure just one pair of eighth notes.
                    if (includeEighthNoteRhythms && totalEighthNotes != 2)
                    {
                        continue;
                    }

                    break;
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

            int first2MeasuresNumberOfNotes = numberOfNotes1 + numberOfNotes2;
            int second2MeasuresNumberOfNotes = numberOfNotes3 + numberOfNotes4;

            Queue<int> noteNumberQueue = new Queue<int>();
            bool criteriaSatisfied = false;
            int numberOfTries = 0;
            int maxNumberOfTries = 1999;
            var parametersToString = string.Empty;

            try
            {
                while (!(noteNumberQueue.AllStepsWithinRange(12) && noteNumberQueue.AllNotesWithinRange(12) && criteriaSatisfied))
                {
                    numberOfTries++;

                    //_log.Info($"numberOfTries: {numberOfTries}");
                    //_log.Info($"measureRhythm1: {measureRhythm1}");
                    //_log.Info($"measureRhythm2: {measureRhythm2}");
                    if (numberOfTries > maxNumberOfTries)
                    {
                        throw new PhraseGenerationException($"Aborted phrase generation after {numberOfTries} tries.");
                    }

                    noteNumberQueue = GetIntervalIntQueue(scaleNoteNumbers, first2MeasuresNumberOfNotes, second2MeasuresNumberOfNotes, intType, out criteriaSatisfied, includeC2, out parametersToString);
                }
            }
            catch (PhraseGenerationException pgEx)
            {
                _log.Trace($"{numberOfTries} - {parametersToString}");

                _log.Error(pgEx, $"The maxNumberOfTries ({maxNumberOfTries}) has been exceeded.");
                dict.Add("hasError", "yep");
                dict.Add("numberOfTries", numberOfTries.ToString());
                var jsonEx = Json(dict, JsonRequestBehavior.AllowGet);
                return jsonEx;
            }

            _log.Trace(numberOfTries);

            int[] measureNoteNumbers1 = NoteHelper.PopulateNoteNumbersFromQueue(numberOfNotes1, noteNumberQueue);
            int[] measureNoteNumbers2 = NoteHelper.PopulateNoteNumbersFromQueue(numberOfNotes2, noteNumberQueue);
            int[] measureNoteNumbers3 = NoteHelper.PopulateNoteNumbersFromQueue(numberOfNotes3, noteNumberQueue);
            int[] measureNoteNumbers4 = NoteHelper.PopulateNoteNumbersFromQueue(numberOfNotes4, noteNumberQueue);

            NoteHelper.CreateSamplesFromRhythmsAndNoteNames(eighthNoteDuration, quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit1, notes1, measureNoteNumbers1);
            NoteHelper.CreateSamplesFromRhythmsAndNoteNames(eighthNoteDuration, quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit2, notes2, measureNoteNumbers2);
            NoteHelper.CreateSamplesFromRhythmsAndNoteNames(eighthNoteDuration, quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit3, notes3, measureNoteNumbers3);
            NoteHelper.CreateSamplesFromRhythmsAndNoteNames(eighthNoteDuration, quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit4, notes4, measureNoteNumbers4);

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
            if (numberOfMeasures == 4)
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
            dict.Add("numberOfTries", numberOfTries.ToString());

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

            string script1 = NoteHelper.GetEasyScoreScript3("transcription1", noteNames1, measureRhythmSplit1, keySignature, true);
            //string script1 = NoteHelper.GetMusicXmlScript("transcription1", noteNames1, measureRhythmSplit1, keySignature, false);

            dict.Add("theScript1", script1);
            string script2 = NoteHelper.GetEasyScoreScript3("transcription2", noteNames2, measureRhythmSplit2, keySignature, false);
            dict.Add("theScript2", script2);
            if (numberOfMeasures == 4)
            {
                string script3 = NoteHelper.GetEasyScoreScript3("transcription3", noteNames3, measureRhythmSplit3, keySignature, false);
                dict.Add("theScript3", script3);
                string script4 = NoteHelper.GetEasyScoreScript3("transcription4", noteNames4, measureRhythmSplit4, keySignature, false);
                dict.Add("theScript4", script4);
            }

            #endregion Notation

            var json = Json(dict, JsonRequestBehavior.AllowGet);
            return json;
        }

        private static List<string> GetNoteRhythms(bool includeEighthNotes)
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
            if (includeEighthNotes)
            {
                measureRhythms.Add("8,8,4,4,4");
                measureRhythms.Add("8,8,4,2");
                measureRhythms.Add("4,4,8,8,4");
                measureRhythms.Add("2,8,8,4");
            }

            return measureRhythms;
        }

        private Queue<int> GetIntervalIntQueue(int[] scaleNoteNumbers, int first2MeasuresNumberOfNotes, int second2MeasuresNumberOfNotes, L1C3IntervalType intervalType, out bool criteriaSatisfied, bool includeC2, out string parametersToString)
        {
            criteriaSatisfied = false;
            parametersToString = $"scaleNoteNumbers: {scaleNoteNumbers}, first2MeasuresNumberOfNotes: {first2MeasuresNumberOfNotes}, second2MeasuresNumberOfNotes: {second2MeasuresNumberOfNotes}, intervalType: {intervalType}, criteriaSatisfied: {criteriaSatisfied}, includeC2: {includeC2}";
            //_log.Trace(parameterString);

            int numberOfNotes = first2MeasuresNumberOfNotes + second2MeasuresNumberOfNotes;
            bool first2MeasuresHasC3Interval = false;
            bool first2MeasuresHasC1Resolution = false;
            bool first2MeasuresHasC2Interval = false;
            bool second2MeasuresHasC3Interval = false;
            bool second2MeasuresHasC1Resolution = false;
            bool second2MeasuresHasC2Interval = false;

            // C3 intervals.
            List<Tuple<int, int>> min3rdIntervals = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(2, 4),  // RE FA Min. 3rd.
                new Tuple<int, int>(3, 5),  // MI SO Min. 3rd.
                new Tuple<int, int>(6, 8),   // LA DO Min. 3rd.
                new Tuple<int, int>(7, 9)   // TI RE Min. 3rd.
            };

            List<Tuple<int, int>> maj6thIntervals = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1, 6),   // DO LA Maj. 6th.
                new Tuple<int, int>(2, 7),   // RE TI Maj. 6th.
                new Tuple<int, int>(4, 9),    // FA RE Maj. 6th.
                new Tuple<int, int>(5, 10)    // SO MI Maj. 6th.
            };

            // C1 resolutions.
            List<Tuple<int, int>> c1Resolutions = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(2, 1),  // RE DO.
                new Tuple<int, int>(4, 3),  // FA MI.
                new Tuple<int, int>(6, 5),  // LA SO.
                new Tuple<int, int>(7, 8),  // High TI DO.
                new Tuple<int, int>(0, 1)   // Low TI DO.
            };

            // C2 intervals.
            List<Tuple<int, int>> maj3rdIntervals = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(1, 3),  // DO MI Maj. 3rd.
                new Tuple<int, int>(4, 6),  // FA LA Maj. 3rd.
                new Tuple<int, int>(5, 7)   // SO TI Maj. 3rd.
            };

            List<Tuple<int, int>> min6thIntervals = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(3, 8),   // MI DO Min. 6th.
                new Tuple<int, int>(6, 11),  // LA FA Min. 6th.
                new Tuple<int, int>(7, 12)   // TI SO Min. 6th.
            };


            List<Tuple<int, int>> bothC3Intervals = min3rdIntervals.Concat(maj6thIntervals).ToList();
            List<Tuple<int, int>> bothC2Intervals = maj3rdIntervals.Concat(min6thIntervals).ToList();

            //_log.Info($"========== Getting intervals and resolutions, attempt # {numberOfTries} ==========");
            var q = new Queue<int>();
            int randomInt;
            int exclusiveUpperBound;
            Tuple<int, int> notes;
            while (q.Count < numberOfNotes)
            {
                //int randomInt = NoteHelper.GetRandomInt(0, intervals.Count);
                //_log.Info($"interval index: {randomInt}");

                //Tuple<int, int> t = intervals[randomInt];

                switch (intervalType)
                {
                    case L1C3IntervalType.Minor3rd:
                        exclusiveUpperBound = includeC2 ? 4 : 3;
                        randomInt = NoteHelper.GetRandomInt(1, exclusiveUpperBound);
                        if (randomInt == 1)
                        {
                            randomInt = NoteHelper.GetRandomInt(0, c1Resolutions.Count);
                            notes = c1Resolutions[randomInt];
                            if (q.Count < first2MeasuresNumberOfNotes)
                            {
                                first2MeasuresHasC1Resolution = true;
                            }
                            else
                            {
                                second2MeasuresHasC1Resolution = true;
                            }
                            q.Enqueue(scaleNoteNumbers[notes.Item1]);
                            q.Enqueue(scaleNoteNumbers[notes.Item2]);
                        }
                        else if (randomInt == 2)
                        {
                            randomInt = NoteHelper.GetRandomInt(0, min3rdIntervals.Count);
                            notes = min3rdIntervals[randomInt];
                            if (q.Count < first2MeasuresNumberOfNotes)
                            {
                                first2MeasuresHasC3Interval = true;
                            }
                            else
                            {
                                second2MeasuresHasC3Interval = true;
                            }

                            randomInt = NoteHelper.GetRandomInt(0, 2);
                            if (randomInt % 2 == 0)
                            {
                                q.Enqueue(scaleNoteNumbers[notes.Item1]);
                                q.Enqueue(scaleNoteNumbers[notes.Item2]);
                            }
                            else
                            {
                                q.Enqueue(scaleNoteNumbers[notes.Item2]);
                                q.Enqueue(scaleNoteNumbers[notes.Item1]);
                            }

                        }
                        else if (randomInt == 3)
                        {
                            randomInt = NoteHelper.GetRandomInt(0, maj3rdIntervals.Count);
                            notes = maj3rdIntervals[randomInt];
                            if (q.Count < first2MeasuresNumberOfNotes)
                            {
                                first2MeasuresHasC2Interval = true;
                            }
                            else
                            {
                                second2MeasuresHasC2Interval = true;
                            }

                            randomInt = NoteHelper.GetRandomInt(0, 2);
                            if (randomInt % 2 == 0)
                            {
                                q.Enqueue(scaleNoteNumbers[notes.Item1]);
                                q.Enqueue(scaleNoteNumbers[notes.Item2]);
                            }
                            else
                            {
                                q.Enqueue(scaleNoteNumbers[notes.Item2]);
                                q.Enqueue(scaleNoteNumbers[notes.Item1]);
                            }
                        }

                        else
                        {
                            throw new NotSupportedException($"randomInt {randomInt} is not supported.");
                        }

                        break;

                    case L1C3IntervalType.Major6th:
                        exclusiveUpperBound = includeC2 ? 4 : 3;
                        randomInt = NoteHelper.GetRandomInt(1, exclusiveUpperBound);
                        if (randomInt == 1)
                        {
                            randomInt = NoteHelper.GetRandomInt(0, c1Resolutions.Count);
                            notes = c1Resolutions[randomInt];
                            if (q.Count < first2MeasuresNumberOfNotes)
                            {
                                first2MeasuresHasC1Resolution = true;
                            }
                            else
                            {
                                second2MeasuresHasC1Resolution = true;
                            }
                            q.Enqueue(scaleNoteNumbers[notes.Item1]);
                            q.Enqueue(scaleNoteNumbers[notes.Item2]);
                        }

                        else if (randomInt == 2)
                        {
                            randomInt = NoteHelper.GetRandomInt(0, maj6thIntervals.Count);
                            notes = maj6thIntervals[randomInt];
                            if (q.Count < first2MeasuresNumberOfNotes)
                            {
                                first2MeasuresHasC3Interval = true;
                            }
                            else
                            {
                                second2MeasuresHasC3Interval = true;
                            }

                            randomInt = NoteHelper.GetRandomInt(0, 2);
                            if (randomInt % 2 == 0)
                            {
                                q.Enqueue(scaleNoteNumbers[notes.Item1]);
                                q.Enqueue(scaleNoteNumbers[notes.Item2]);
                            }
                            else
                            {
                                q.Enqueue(scaleNoteNumbers[notes.Item2]);
                                q.Enqueue(scaleNoteNumbers[notes.Item1]);
                            }
                        }
                        else if (randomInt == 3)
                        {
                            randomInt = NoteHelper.GetRandomInt(0, min6thIntervals.Count);
                            notes = min6thIntervals[randomInt];
                            if (q.Count < first2MeasuresNumberOfNotes)
                            {
                                first2MeasuresHasC2Interval = true;
                            }
                            else
                            {
                                second2MeasuresHasC2Interval = true;
                            }

                            randomInt = NoteHelper.GetRandomInt(0, 2);
                            if (randomInt % 2 == 0)
                            {
                                q.Enqueue(scaleNoteNumbers[notes.Item1]);
                                q.Enqueue(scaleNoteNumbers[notes.Item2]);
                            }
                            else
                            {
                                q.Enqueue(scaleNoteNumbers[notes.Item2]);
                                q.Enqueue(scaleNoteNumbers[notes.Item1]);
                            }
                        }
                        else
                        {
                            throw new NotSupportedException($"randomInt {randomInt} is not supported.");
                        }

                        break;

                    case L1C3IntervalType.Both:
                        exclusiveUpperBound = includeC2 ? 4 : 3;
                        randomInt = NoteHelper.GetRandomInt(1, exclusiveUpperBound);
                        if (randomInt == 1)
                        {
                            randomInt = NoteHelper.GetRandomInt(0, c1Resolutions.Count);
                            notes = c1Resolutions[randomInt];
                            if (q.Count < first2MeasuresNumberOfNotes)
                            {
                                first2MeasuresHasC1Resolution = true;
                            }
                            else
                            {
                                second2MeasuresHasC1Resolution = true;
                            }
                            q.Enqueue(scaleNoteNumbers[notes.Item1]);
                            q.Enqueue(scaleNoteNumbers[notes.Item2]);
                        }

                        else if (randomInt == 2)
                        {
                            randomInt = NoteHelper.GetRandomInt(0, bothC3Intervals.Count);
                            notes = bothC3Intervals[randomInt];
                            if (q.Count < first2MeasuresNumberOfNotes)
                            {
                                first2MeasuresHasC3Interval = true;
                            }
                            else
                            {
                                second2MeasuresHasC3Interval = true;
                            }

                            randomInt = NoteHelper.GetRandomInt(0, 2);
                            if (randomInt % 2 == 0)
                            {
                                q.Enqueue(scaleNoteNumbers[notes.Item1]);
                                q.Enqueue(scaleNoteNumbers[notes.Item2]);
                            }
                            else
                            {
                                q.Enqueue(scaleNoteNumbers[notes.Item2]);
                                q.Enqueue(scaleNoteNumbers[notes.Item1]);
                            }
                        }
                        else if (randomInt == 3)
                        {
                            randomInt = NoteHelper.GetRandomInt(0, bothC2Intervals.Count);
                            notes = bothC2Intervals[randomInt];
                            if (q.Count < first2MeasuresNumberOfNotes)
                            {
                                first2MeasuresHasC2Interval = true;
                            }
                            else
                            {
                                second2MeasuresHasC2Interval = true;
                            }

                            randomInt = NoteHelper.GetRandomInt(0, 2);
                            if (randomInt % 2 == 0)
                            {
                                q.Enqueue(scaleNoteNumbers[notes.Item1]);
                                q.Enqueue(scaleNoteNumbers[notes.Item2]);
                            }
                            else
                            {
                                q.Enqueue(scaleNoteNumbers[notes.Item2]);
                                q.Enqueue(scaleNoteNumbers[notes.Item1]);
                            }
                        }
                        else
                        {
                            throw new NotSupportedException($"randomInt {randomInt} is not supported.");
                        }

                        break;
                    default:
                        throw new NotSupportedException($"Interval type '{intervalType}' is not supported.");
                }
            }

            //_log.Info($"first2MeasuresHasC1Resolution: {first2MeasuresHasC1Resolution}, first2MeasuresHasC2Interval: {first2MeasuresHasC2Interval}, first2MeasuresHasC3Interval: {first2MeasuresHasC3Interval}, second2MeasuresHasC1Resolution: {second2MeasuresHasC1Resolution}, second2MeasuresHasC2Interval: {second2MeasuresHasC2Interval}, second2MeasuresHasC3Interval: {second2MeasuresHasC3Interval}");
            if (includeC2)
            {
                // If we don't have at least one C1 resolution, one C2 interval and and at least one C3 interval, keep going. Same thing for second two measures.
                if (!(first2MeasuresHasC1Resolution && first2MeasuresHasC2Interval && first2MeasuresHasC3Interval && second2MeasuresHasC1Resolution && second2MeasuresHasC2Interval && second2MeasuresHasC3Interval))
                {
                    criteriaSatisfied = false;
                }
                else
                {
                    criteriaSatisfied = true;
                }
            }
            else
            {
                // If we don't have at least one C1 resolution and at least one C3 interval, keep going. Same thing for second two measures.
                if (!(first2MeasuresHasC1Resolution && first2MeasuresHasC3Interval && second2MeasuresHasC1Resolution && second2MeasuresHasC3Interval))
                {
                    criteriaSatisfied = false;
                }
                else
                {
                    criteriaSatisfied = true;
                }
            }

            return q;
        }
    }
}