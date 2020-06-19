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

        public ActionResult SolfegResolutionsDO()
        {
            return View();
        }

        public ActionResult SolfegResolutionsNoDO()
        {
            return View();
        }

        public ActionResult PitchIdentification()
        {
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
            int[] scaleNoteNumbers = new int[] { 39, 41, 43, 44, 46, 48, 50, 51 };  // C Major.
            scaleNoteNumbers = TransposeScaleNoteNumbers(scaleNoteNumbers, keySignature);

            // The initial DO.
            ISampleProvider wholeDoNote = NAudioHelper.GetSampleProvider(scaleNoteNumbers[0], wholeNoteDuration);

            // Four metronome ticks before the transcription part plays.
            ISampleProvider[] ticks = new ISampleProvider[4];
            string tickFile = HostingEnvironment.MapPath($"~/Samples/Woodblock.wav");
            for (int i = 0; i < ticks.Length; i++)
            {
                ticks[i] = NAudioHelper.GetSampleProviderFromFile(tickFile, quarterNoteDuration);
            }

            List<string> measureRhythms = GetQuarterNoteRhythms();

            int randomInt = GetRandomInt(0, measureRhythms.Count);
            string measureRhythm1 = measureRhythms[randomInt];
            randomInt = GetRandomInt(0, measureRhythms.Count);
            string measureRhythm2 = measureRhythms[randomInt];
            randomInt = GetRandomInt(0, measureRhythms.Count);
            string measureRhythm3 = measureRhythms[randomInt];
            randomInt = GetRandomInt(0, measureRhythms.Count);
            string measureRhythm4 = measureRhythms[randomInt];


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

            int[] measureNoteNumbers1 = PopulateNoteNumbersFromQueue(numberOfNotes1, noteNumberQueue);
            int[] measureNoteNumbers2 = PopulateNoteNumbersFromQueue(numberOfNotes2, noteNumberQueue);
            int[] measureNoteNumbers3 = PopulateNoteNumbersFromQueue(numberOfNotes3, noteNumberQueue);
            int[] measureNoteNumbers4 = PopulateNoteNumbersFromQueue(numberOfNotes4, noteNumberQueue);

            CreateSamplesFromRhythmsAndNoteNames(quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit1, notes1, measureNoteNumbers1);
            CreateSamplesFromRhythmsAndNoteNames(quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit2, notes2, measureNoteNumbers2);
            CreateSamplesFromRhythmsAndNoteNames(quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit3, notes3, measureNoteNumbers3);
            CreateSamplesFromRhythmsAndNoteNames(quarterNoteDuration, halfNoteDuration, dottedHalfNoteDuration, wholeNoteDuration, measureRhythmSplit4, notes4, measureNoteNumbers4);

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
            AdjustNoteNamesForKey(keySignature, measureNoteNumbers1, noteNames1);

            string[] noteNames2 = new string[numberOfNotes2];
            AdjustNoteNamesForKey(keySignature, measureNoteNumbers2, noteNames2);

            string[] noteNames3 = new string[numberOfNotes3];
            AdjustNoteNamesForKey(keySignature, measureNoteNumbers3, noteNames3);

            string[] noteNames4 = new string[numberOfNotes4];
            AdjustNoteNamesForKey(keySignature, measureNoteNumbers4, noteNames4);

            string script1 = GetEasyScoreScript("transcription1", noteNames1, measureRhythmSplit1, keySignature);
            dict.Add("theScript1", script1);
            string script2 = GetEasyScoreScript("transcription2", noteNames2, measureRhythmSplit2, keySignature);
            dict.Add("theScript2", script2);
            if(numberOfMeasures == 4)
            {
                string script3 = GetEasyScoreScript("transcription3", noteNames3, measureRhythmSplit3, keySignature);
                dict.Add("theScript3", script3);
                string script4 = GetEasyScoreScript("transcription4", noteNames4, measureRhythmSplit4, keySignature);
                dict.Add("theScript4", script4);

            }

            #endregion Notation

            var json = Json(dict, JsonRequestBehavior.AllowGet);
            return json;
        }

        private static List<string> GetQuarterNoteRhythms()
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

        private static void AdjustNoteNamesForKey(string keySignature, int[] measureNoteNumbers, string[] noteNames)
        {
            for (int i = 0; i < noteNames.Length; i++)
            {
                noteNames[i] = NAudioHelper.GetNoteNameFromNoteNumber(measureNoteNumbers[i]);
                if ((keySignature == "G" || keySignature == "A" || keySignature == "D" || keySignature == "E" || keySignature == "B") && noteNames[i].Contains("b"))
                {
                    noteNames[i] = noteNames[i].FlatToNaturalForSharpKeys();
                }
                if ((keySignature == "F" || keySignature == "Bb" || keySignature == "Eb" || keySignature == "Ab" || keySignature == "Db") && noteNames[i].Contains("b"))
                {
                    noteNames[i] = noteNames[i].FlatToNaturalForFlatKeys();
                }
                if (keySignature == "F#")
                {
                    noteNames[i] = noteNames[i].AdjustForFSharp();
                }
            }
        }

        private static void CreateSamplesFromRhythmsAndNoteNames(TimeSpan quarterNoteDuration, TimeSpan halfNoteDuration, TimeSpan dottedHalfNoteDuration, TimeSpan wholeNoteDuration, string[] measureRhythmSplit1, ISampleProvider[] notes1, int[] measureNoteNumbers1)
        {
            for (int i = 0; i < notes1.Length; i++)
            {
                TimeSpan duration;
                switch (measureRhythmSplit1[i])
                {
                    case "1":
                        duration = wholeNoteDuration;
                        break;

                    case "2":
                        duration = halfNoteDuration;
                        break;

                    case "2.":
                        duration = dottedHalfNoteDuration;
                        break;

                    case "4":
                        duration = quarterNoteDuration;
                        break;

                    default:
                        throw new NotSupportedException($"Duration '{measureRhythmSplit1[i]}' is not supported.");
                }

                notes1[i] = NAudioHelper.GetSampleProvider(measureNoteNumbers1[i], duration);
            }
        }

        private static int[] PopulateNoteNumbersFromQueue(int numberOfNotes, Queue<int> noteNumberQueue)
        {
            int[] measureNoteNumbers = new int[numberOfNotes];
            for (int i = 0; i < measureNoteNumbers.Length; i++)
            {
                measureNoteNumbers[i] = noteNumberQueue.Dequeue();
            }

            return measureNoteNumbers;
        }

        private int[] TransposeScaleNoteNumbers(int[] scaleNoteNumbers, string keySignature)
        {
            switch (keySignature)
            {
                case "F#":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += 6;
                    }
                    break;

                case "F":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += 5;
                    }
                    break;

                case "E":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += 4;
                    }
                    break;

                case "Eb":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += 3;
                    }
                    break;

                case "D":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += 2;
                    }
                    break;

                case "Db":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += 1;
                    }
                    break;

                case "C":
                    break;  // Do nothing as (hopefully!) we're passed in the C major scale notes number.

                case "B":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += -1;
                    }
                    break;

                case "Bb":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += -2;
                    }
                    break;

                case "A":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += -3;
                    }
                    break;

                case "Ab":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += -4;
                    }
                    break;

                case "G":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += -5;
                    }
                    break;

                default:
                    throw new NotSupportedException($"Key signature '{keySignature}' is not supported.");
            }

            return scaleNoteNumbers;
        }

        private string GetEasyScoreScript(string elementId, string[] noteNames, string[] measureRhythmSplit, string keySignature)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < noteNames.Length; i++)
            {
                sb.Append($"{noteNames[i]}/{measureRhythmSplit[i]},");
            }

            var easyScoreNotes = sb.ToString();
            easyScoreNotes = easyScoreNotes.TrimEnd(',');
            //easyScoreNotes += " |";

            string script = $@"
            const vf = new Vex.Flow.Factory({{
                renderer: {{ elementId: '{elementId}' }}
            }});

            const score = vf.EasyScore();
            //const system = vf.System();
            var system = vf.System({{width: 320}});

            system.addStave({{
                voices: [
                    score.voice(score.notes('{easyScoreNotes}', {{ stem: 'up' }})),
                ]
            }}).addClef('treble').addTimeSignature('4/4').addKeySignature('{keySignature}');
            system.addConnector('singleLeft');
            system.addConnector('singleRight');

            vf.draw();
            ";

            return script;
        }

        private int GetRandomInt(int min, int max)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] buffer = new byte[4];

                rng.GetBytes(buffer);
                int result = BitConverter.ToInt32(buffer, 0);
                int index = new Random(result).Next(min, max);
                return index;
            }
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
            resolutions.Add(new Tuple<int, int>(1, 0));
            resolutions.Add(new Tuple<int, int>(3, 2));
            resolutions.Add(new Tuple<int, int>(5, 4));
            resolutions.Add(new Tuple<int, int>(6, 7));

            var q = new Queue<int>();
            while (q.Count < numberOfNotes)
            {
                int randomInt = GetRandomInt(0, resolutions.Count);
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
                        int ri = GetRandomInt(0, 2);
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