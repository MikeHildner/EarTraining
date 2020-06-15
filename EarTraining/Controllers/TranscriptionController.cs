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
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace EarTraining.Controllers
{
    public class TranscriptionController : Controller
    {
        // GET: Transcription
        public ActionResult Test()
        {
            return View();
        }

        public ActionResult AudioAndDictation()
        {
            var dict = new Dictionary<string, string>();

            #region Audio

            double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNoteMillis = (60 / bpm) * 1000;
            double wholeNoteMillis = quarterNoteMillis * 4;
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis);
            TimeSpan wholeNoteDuration = TimeSpan.FromMilliseconds(wholeNoteMillis);

            ISampleProvider wholeDoNote;
            ISampleProvider[] ticks = new ISampleProvider[4];
            ISampleProvider note1;
            ISampleProvider note2;
            ISampleProvider note3;
            ISampleProvider note4;

            int[] noteNumbers = new int[] { 39, 41, 43, 44, 46, 48, 50, 51 };  // C Major.
            int firstNoteNumber = GetRandomNoteNumber(noteNumbers);
            int secondNoteNumber = GetRandomNoteNumber(noteNumbers);
            int thirdNoteNumber = GetRandomNoteNumber(noteNumbers);
            int fourthNoteNumber = GetRandomNoteNumber(noteNumbers);

            wholeDoNote = NAudioHelper.GetSampleProvider(noteNumbers[0], wholeNoteDuration);


            string tickFile = HostingEnvironment.MapPath($"~/Samples/Woodblock.wav");
            for (int i = 0; i < ticks.Length; i++)
            {
                //ticks[i] = NAudioHelper.GetSampleProvider(noteNumbers[0], quarterNoteDuration);
                ticks[i] = NAudioHelper.GetSampleProviderFromFile(tickFile, quarterNoteDuration);
            }

            note1 = NAudioHelper.GetSampleProvider(firstNoteNumber, quarterNoteDuration);
            note2 = NAudioHelper.GetSampleProvider(secondNoteNumber, quarterNoteDuration);
            note3 = NAudioHelper.GetSampleProvider(thirdNoteNumber, quarterNoteDuration);
            note4 = NAudioHelper.GetSampleProvider(fourthNoteNumber, quarterNoteDuration);

            ISampleProvider phrase =
                wholeDoNote;

            foreach (var tick in ticks)
            {
                phrase = phrase.FollowedBy(tick);
            }

            phrase = phrase
                .FollowedBy(note1)
                .FollowedBy(note2)
                .FollowedBy(note3)
                .FollowedBy(note4);

            SampleToWaveProvider stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            wavStream.WavToMp3File(out string fileName);
            dict.Add("src", fileName);

            #endregion Audio

            #region Notation

            string firstNoteName = NAudioHelper.GetNoteNameFromNoteNumber(firstNoteNumber);
            string secondNoteName = NAudioHelper.GetNoteNameFromNoteNumber(secondNoteNumber);
            string thirdNoteName = NAudioHelper.GetNoteNameFromNoteNumber(thirdNoteNumber);
            string fourthNoteName = NAudioHelper.GetNoteNameFromNoteNumber(fourthNoteNumber);

            var script = $@"
            const vf = new Vex.Flow.Factory({{
                //renderer: {{ elementId: 'transcription', width: 500, height: 200 }}
                renderer: {{ elementId: 'transcription' }}
            }});

            const score = vf.EasyScore();
            const system = vf.System();

            system.addStave({{
                voices: [
                    score.voice(score.notes('{firstNoteName}/q, {secondNoteName}, {thirdNoteName}, {fourthNoteName}', {{ stem: 'up' }})),
                    //score.voice(score.notes('C#4/h, C#4', {{ stem: 'down' }}))
                ]
            }}).addClef('treble').addTimeSignature('4/4');

            vf.draw();
            ";

            dict.Add("theScript", script);

            #endregion Notation

            var json = Json(dict, JsonRequestBehavior.AllowGet);
            return json;
        }

        public ActionResult AudioAndDictation2()
        {
            var dict = new Dictionary<string, string>();

            #region Audio

            double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNoteMillis = (60 / bpm) * 1000;
            double wholeNoteMillis = quarterNoteMillis * 4;
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis);
            TimeSpan wholeNoteDuration = TimeSpan.FromMilliseconds(wholeNoteMillis);

            int[] noteNumbers = new int[] { 39, 41, 43, 44, 46, 48, 50, 51 };  // C Major.
            ISampleProvider wholeDoNote = NAudioHelper.GetSampleProvider(noteNumbers[0], wholeNoteDuration);

            ISampleProvider[] ticks = new ISampleProvider[4];
            string tickFile = HostingEnvironment.MapPath($"~/Samples/Woodblock.wav");
            for (int i = 0; i < ticks.Length; i++)
            {
                ticks[i] = NAudioHelper.GetSampleProviderFromFile(tickFile, quarterNoteDuration);
            }

            ISampleProvider note1;
            ISampleProvider note2;
            ISampleProvider note3;
            ISampleProvider note4;


            int firstNoteNumber = GetRandomNoteNumber(noteNumbers);
            int secondNoteNumber = GetRandomNoteNumber(noteNumbers);
            int thirdNoteNumber = GetRandomNoteNumber(noteNumbers);
            int fourthNoteNumber = GetRandomNoteNumber(noteNumbers);

            note1 = NAudioHelper.GetSampleProvider(firstNoteNumber, quarterNoteDuration);
            note2 = NAudioHelper.GetSampleProvider(secondNoteNumber, quarterNoteDuration);
            note3 = NAudioHelper.GetSampleProvider(thirdNoteNumber, quarterNoteDuration);
            note4 = NAudioHelper.GetSampleProvider(fourthNoteNumber, quarterNoteDuration);

            ISampleProvider phrase =
                wholeDoNote;

            foreach (var tick in ticks)
            {
                phrase = phrase.FollowedBy(tick);
            }

            phrase = phrase
                .FollowedBy(note1)
                .FollowedBy(note2)
                .FollowedBy(note3)
                .FollowedBy(note4);

            SampleToWaveProvider stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            wavStream.WavToMp3File(out string fileName);
            dict.Add("src", fileName);

            #endregion Audio

            #region Notation

            string firstNoteName = NAudioHelper.GetNoteNameFromNoteNumber(firstNoteNumber);
            string secondNoteName = NAudioHelper.GetNoteNameFromNoteNumber(secondNoteNumber);
            string thirdNoteName = NAudioHelper.GetNoteNameFromNoteNumber(thirdNoteNumber);
            string fourthNoteName = NAudioHelper.GetNoteNameFromNoteNumber(fourthNoteNumber);

            string notes = $@"
                var notes = [
                    new VF.StaveNote({{clef: 'treble', keys: ['{firstNoteName.ToSlashNoteName()}'], duration: 'q' }}),

                    new VF.StaveNote({{ clef: 'treble', keys: ['{secondNoteName.ToSlashNoteName()}'], duration: 'q' }}),

                    new VF.StaveNote({{clef: 'treble', keys: ['{thirdNoteName.ToSlashNoteName()}'], duration: 'q' }}),

                    new VF.StaveNote({{clef: 'treble', keys: ['{fourthNoteName.ToSlashNoteName()}'], duration: 'q' }})
                ];
            ";

            string script = GetVexFlowScript("transcription1", notes);
            dict.Add("theScript1", script);

            script = GetVexFlowScript("transcription2", notes);
            dict.Add("theScript2", script);



            #endregion Notation

            var json = Json(dict, JsonRequestBehavior.AllowGet);
            return json;
        }

        public ActionResult AudioAndDictation3()
        {
            var dict = new Dictionary<string, string>();

            #region Audio

            double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNoteMillis = (60 / bpm) * 1000;
            double halfNoteMillis = quarterNoteMillis * 2;
            double dottedHalfNoteMillis = quarterNoteMillis * 3;
            double wholeNoteMillis = quarterNoteMillis * 4;
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis);
            TimeSpan halfNoteDuration = TimeSpan.FromMilliseconds(halfNoteMillis);
            TimeSpan dottedHalfNoteDuration = TimeSpan.FromMilliseconds(dottedHalfNoteMillis);
            TimeSpan wholeNoteDuration = TimeSpan.FromMilliseconds(wholeNoteMillis);

            // Setup the whole note and metronome ticks.
            int[] scaleNoteNumbers = new int[] { 39, 41, 43, 44, 46, 48, 50, 51 };  // C Major.
            ISampleProvider wholeDoNote = NAudioHelper.GetSampleProvider(scaleNoteNumbers[0], wholeNoteDuration);

            ISampleProvider[] ticks = new ISampleProvider[4];
            string tickFile = HostingEnvironment.MapPath($"~/Samples/Woodblock.wav");
            for (int i = 0; i < ticks.Length; i++)
            {
                ticks[i] = NAudioHelper.GetSampleProviderFromFile(tickFile, quarterNoteDuration);
            }


            List<string> measureRhythms = new List<string>();
            measureRhythms.Add("1");
            measureRhythms.Add("2,2");
            measureRhythms.Add("4,2.");
            measureRhythms.Add("2.,4");
            measureRhythms.Add("4,4,4,4");
            measureRhythms.Add("4,4,2");
            measureRhythms.Add("2,4,4");

            int randomInt = GetRandomInt(0, measureRhythms.Count);
            string measureRhythm = measureRhythms[randomInt];

            string[] measureRhythmSplit = measureRhythm.Split(',');
            int numberOfNotes = measureRhythmSplit.Length;

            ISampleProvider[] notes = new ISampleProvider[numberOfNotes];

            int[] measureNoteNumbers = new int[numberOfNotes];
            for (int i = 0; i < measureNoteNumbers.Length; i++)
            {
                measureNoteNumbers[i] = GetRandomNoteNumber(scaleNoteNumbers);
            }

            TimeSpan duration;
            for (int i = 0; i < notes.Length; i++)
            {
                switch (measureRhythmSplit[i])
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
                        throw new NotSupportedException($"Duration '{measureRhythmSplit[i]}' is not supported.");
                }

                notes[i] = NAudioHelper.GetSampleProvider(measureNoteNumbers[i], duration);
            }

            ISampleProvider phrase =
                wholeDoNote;

            foreach (var tick in ticks)
            {
                phrase = phrase.FollowedBy(tick);
            }

            foreach (var note in notes)
            {
                phrase = phrase.FollowedBy(note);
            }

            SampleToWaveProvider stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            wavStream.WavToMp3File(out string fileName);
            dict.Add("src", fileName);

            #endregion Audio

            #region Notation

            string[] noteNames = new string[numberOfNotes];
            for (int i = 0; i < noteNames.Length; i++)
            {
                noteNames[i] = NAudioHelper.GetNoteNameFromNoteNumber(measureNoteNumbers[i]);
            }

            var jsNotes = new StringBuilder();
            jsNotes.AppendLine("var notes = [");
            for (int i = 0; i < noteNames.Length; i++)
            {
                string staffNote = $"new VF.StaveNote({{clef: 'treble', keys: ['{noteNames[i].ToSlashNoteName()}'], duration: '{measureRhythmSplit[i]}' }}),";
                jsNotes.AppendLine(staffNote);
            }
            jsNotes.AppendLine("];");

            //string script = GetVexFlowScript("transcription1", jsNotes.ToString());
            string script = GetEasyScoreScript("transcription1", noteNames, measureRhythmSplit);
            dict.Add("theScript1", script);

            //script = GetVexFlowScript("transcription2", notes);
            //dict.Add("theScript2", script);



            #endregion Notation

            var json = Json(dict, JsonRequestBehavior.AllowGet);
            return json;
        }

        public ActionResult AudioAndDictation4()
        {
            var dict = new Dictionary<string, string>();

            #region Audio

            double bpm = double.Parse(ConfigurationManager.AppSettings["BPM"]);
            double quarterNoteMillis = (60 / bpm) * 1000;
            double halfNoteMillis = quarterNoteMillis * 2;
            double dottedHalfNoteMillis = quarterNoteMillis * 3;
            double wholeNoteMillis = quarterNoteMillis * 4;
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNoteMillis);
            TimeSpan halfNoteDuration = TimeSpan.FromMilliseconds(halfNoteMillis);
            TimeSpan dottedHalfNoteDuration = TimeSpan.FromMilliseconds(dottedHalfNoteMillis);
            TimeSpan wholeNoteDuration = TimeSpan.FromMilliseconds(wholeNoteMillis);

            // Setup the whole note and metronome ticks.
            int[] scaleNoteNumbers = new int[] { 39, 41, 43, 44, 46, 48, 50, 51 };  // C Major.
            ISampleProvider wholeDoNote = NAudioHelper.GetSampleProvider(scaleNoteNumbers[0], wholeNoteDuration);

            ISampleProvider[] ticks = new ISampleProvider[4];
            string tickFile = HostingEnvironment.MapPath($"~/Samples/Woodblock.wav");
            for (int i = 0; i < ticks.Length; i++)
            {
                ticks[i] = NAudioHelper.GetSampleProviderFromFile(tickFile, quarterNoteDuration);
            }


            List<string> measureRhythms = new List<string>();
            measureRhythms.Add("1");
            measureRhythms.Add("2,2");
            measureRhythms.Add("4,2.");
            measureRhythms.Add("2.,4");
            measureRhythms.Add("4,4,4,4");
            measureRhythms.Add("4,4,2");
            measureRhythms.Add("2,4,4");
            measureRhythms.Add("4,2,4");

            int randomInt = GetRandomInt(0, measureRhythms.Count);
            string measureRhythm1 = measureRhythms[randomInt];
            randomInt = GetRandomInt(0, measureRhythms.Count);
            string measureRhythm2 = measureRhythms[randomInt];


            string[] measureRhythmSplit1 = measureRhythm1.Split(',');
            int numberOfNotes1 = measureRhythmSplit1.Length;

            string[] measureRhythmSplit2 = measureRhythm2.Split(',');
            int numberOfNotes2 = measureRhythmSplit2.Length;

            ISampleProvider[] notes1 = new ISampleProvider[numberOfNotes1];
            ISampleProvider[] notes2 = new ISampleProvider[numberOfNotes2];

            int[] measureNoteNumbers1 = new int[numberOfNotes1];
            for (int i = 0; i < measureNoteNumbers1.Length; i++)
            {
                measureNoteNumbers1[i] = GetRandomNoteNumber(scaleNoteNumbers);
            }

            int[] measureNoteNumbers2 = new int[numberOfNotes2];
            for (int i = 0; i < measureNoteNumbers2.Length; i++)
            {
                measureNoteNumbers2[i] = GetRandomNoteNumber(scaleNoteNumbers);
            }

            TimeSpan duration;
            for (int i = 0; i < notes1.Length; i++)
            {
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
            for (int i = 0; i < notes2.Length; i++)
            {
                switch (measureRhythmSplit2[i])
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
                        throw new NotSupportedException($"Duration '{measureRhythmSplit2[i]}' is not supported.");
                }

                notes2[i] = NAudioHelper.GetSampleProvider(measureNoteNumbers2[i], duration);
            }

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

            SampleToWaveProvider stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            wavStream.WavToMp3File(out string fileName);
            dict.Add("src", fileName);

            #endregion Audio

            #region Notation

            string[] noteNames1 = new string[numberOfNotes1];
            for (int i = 0; i < noteNames1.Length; i++)
            {
                noteNames1[i] = NAudioHelper.GetNoteNameFromNoteNumber(measureNoteNumbers1[i]);
            }
            string[] noteNames2 = new string[numberOfNotes2];
            for (int i = 0; i < noteNames2.Length; i++)
            {
                noteNames2[i] = NAudioHelper.GetNoteNameFromNoteNumber(measureNoteNumbers2[i]);
            }

            var jsNotes1 = new StringBuilder();
            jsNotes1.AppendLine("var notes = [");
            for (int i = 0; i < noteNames1.Length; i++)
            {
                string staffNote = $"new VF.StaveNote({{clef: 'treble', keys: ['{noteNames1[i].ToSlashNoteName()}'], duration: '{measureRhythmSplit1[i]}' }}),";
                jsNotes1.AppendLine(staffNote);
            }
            jsNotes1.AppendLine("];");
            var jsNotes2 = new StringBuilder();
            jsNotes2.AppendLine("var notes = [");
            for (int i = 0; i < noteNames2.Length; i++)
            {
                string staffNote = $"new VF.StaveNote({{clef: 'treble', keys: ['{noteNames2[i].ToSlashNoteName()}'], duration: '{measureRhythmSplit2[i]}' }}),";
                jsNotes2.AppendLine(staffNote);
            }
            jsNotes2.AppendLine("];");

            //string script = GetVexFlowScript("transcription1", jsNotes.ToString());
            string script1 = GetEasyScoreScript("transcription1", noteNames1, measureRhythmSplit1);
            dict.Add("theScript1", script1);
            string script2 = GetEasyScoreScript("transcription2", noteNames2, measureRhythmSplit2);
            dict.Add("theScript2", script2);

            #endregion Notation

            var json = Json(dict, JsonRequestBehavior.AllowGet);
            return json;
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

        private string GetVexFlowScript(string elementID, string notes)
        {
            string script = $@"
            var VF = Vex.Flow;

            var div = document.getElementById('{elementID}');
            var renderer = new VF.Renderer(div, VF.Renderer.Backends.SVG);

            // Configure the rendering context.
            renderer.resize(500, 150);
            var context = renderer.getContext();
            //context.setFont('Arial', 10, "").setBackgroundFillStyle('#eed');

            // Create a stave of width 400 at position 10, 10 on the canvas.
            var stave = new VF.Stave(10, 10, 400);

            // Add a clef and time signature.
            stave.addClef('treble').addTimeSignature('4/4');

            // Connect it to the rendering context and draw!
            stave.setContext(context).draw();

            {notes}

            // Create a voice in 4/4 and add the notes from above
            var voice = new VF.Voice({{num_beats: 4,  beat_value: 4}});
            voice.addTickables(notes);

            // Format and justify the notes to 400 pixels.
            var formatter = new VF.Formatter().joinVoices([voice]).format([voice], 400);

            // Render voice
            voice.draw(context, stave);
            ";

            return script;
        }

        private string GetEasyScoreScript(string elementId, string[] noteNames, string[] measureRhythmSplit)
        {
            var easyScoreNotes = new StringBuilder();
            for (int i = 0; i < noteNames.Length; i++)
            {
                easyScoreNotes.Append($"{noteNames[i]}/{measureRhythmSplit[i]},");
            }

            string script = $@"
            const vf = new Vex.Flow.Factory({{
                renderer: {{ elementId: '{elementId}' }}
            }});

            const score = vf.EasyScore();
            const system = vf.System();

            system.addStave({{
                voices: [
                    score.voice(score.notes('{easyScoreNotes.ToString()}', {{ stem: 'up' }})),
                ]
            }}).addClef('treble').addTimeSignature('4/4');

            vf.draw();
            ";

            return script;
        }
    }
}