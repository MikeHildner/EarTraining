using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace EarTrainingLibrary.NAudio
{
    public class NAudioHelper
    {
        private static string _timbre = "Piano";

        public static ISampleProvider GetSampleProvider(double gain, double frequency, SignalGeneratorType signalGeneratorType, TimeSpan duration)
        {
            var note = new SignalGenerator()
            {
                Gain = gain,
                Frequency = frequency,
                Type = signalGeneratorType
            }.Take(duration);

            return note;
        }

        public static ISampleProvider GetSampleProvider(string noteName, TimeSpan duration)
        {
            // Get the file name based on the note name.
            if (noteName.Contains("/"))
            {
                noteName = noteName.Split('/')[1];
            }

            string fileName = GetFileNameFromNoteName(noteName);

            var sample = GetSampleProviderFromFile(fileName, duration);

            return sample;
        }

        public static ISampleProvider GetSampleProvider(int noteNumber, TimeSpan duration)
        {
            string fileName = GetFileNameFromNoteNumber(noteNumber);

            ISampleProvider sample = GetSampleProviderFromFile(fileName, duration);

            return sample;
        }

        public static string GetFileNameFromNoteName(string noteName)
        {
            if (noteName.Contains("/"))
            {
                noteName = noteName.Split('/')[1];
            }

            noteName = SharpToFlat(noteName);

            string samplesFolder = HostingEnvironment.MapPath($"~/Samples/{_timbre}");
            var files = Directory.GetFiles(samplesFolder);//.Where(w => w.EndsWith(".mp3"));
            string fileName = files.Single(s => s.Contains(noteName));

            return fileName;
        }

        public static string GetNoteNameFromNoteNumber(int noteNumber)
        {
            string startsWith = noteNumber.ToString() + ".";
            string samplesFolder = HostingEnvironment.MapPath($"~/Samples/{_timbre}");
            var files = Directory.GetFiles(samplesFolder);
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            string fileName = files.Single(s => s.StartsWith(startsWith));
            string noteName = fileName.Split('.')[1];
            return noteName;
        }

        private static string SharpToFlat(string noteName)
        {
            if (!noteName.Contains("#")) { return noteName; }

            if (noteName.Contains("C#")) { return noteName.Replace("C#", "Db"); }
            if (noteName.Contains("D#")) { return noteName.Replace("D#", "Eb"); }
            if (noteName.Contains("F#")) { return noteName.Replace("F#", "Gb"); }
            if (noteName.Contains("G#")) { return noteName.Replace("G#", "Ab"); }
            if (noteName.Contains("A#")) { return noteName.Replace("A#", "Bb"); }

            return noteName;
        }

        public static ISampleProvider GetSampleProviderFromFile(string fileName, TimeSpan duration, float? volume = null)
        {
            // Read the sample from disk.
            var reader = new AudioFileReader(fileName);
            //var reader = new Mp3FileReader(fileName);
            //var reader = new MediaFoundationReader(fileName);
            if (volume == null)
            {
                string sVolume = System.Configuration.ConfigurationManager.AppSettings["PlaybackVolume"];
                volume = float.Parse(sVolume);
            }
            reader.Volume = volume.Value;
            ISampleProvider inSample = reader.ToSampleProvider();

            // Shorten to specified duration.
            ISampleProvider outSample = inSample.Take(duration);
            return outSample;
        }

        public static string GetFileNameFromNoteNumber(int noteNumber)
        {
            string samplesFolder = HostingEnvironment.MapPath($"~/Samples/{_timbre}");
            string[] files = Directory.GetFiles(samplesFolder);
            string searchString = noteNumber.ToString() + ".";
            string fileName = files.Single(s => Path.GetFileName(s).StartsWith(searchString));
            //string fileName = files.Single(s => Path.GetFileName(s).StartsWith(searchString) && Path.GetFileName(s).EndsWith(".mp3"));

            return fileName;
        }
    }
}
