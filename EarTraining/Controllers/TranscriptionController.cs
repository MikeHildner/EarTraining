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

        private int GetRandomNoteNumber(int[] noteNumbers)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] buffer = new byte[4];

                rng.GetBytes(buffer);
                int result = BitConverter.ToInt32(buffer, 0);
                int index = new Random(result).Next(0, noteNumbers.Length - 1);
                return noteNumbers[index];
            }
        }
    }
}