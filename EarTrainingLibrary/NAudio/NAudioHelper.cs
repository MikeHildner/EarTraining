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
            if(noteName.Contains("/"))
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

            var sample = GetSampleProviderFromFile(fileName, duration);

            return sample;
        }

        public static string GetFileNameFromNoteName(string noteName)
        {
            if (noteName.Contains("/"))
            {
                noteName = noteName.Split('/')[1];
            }

            noteName = SharpToFlat(noteName);

            string samplesFolder = HostingEnvironment.MapPath("~/Samples");
            string[] files = Directory.GetFiles(samplesFolder);
            string fileName = files.Single(s => s.Contains(noteName));

            return fileName;
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

        public static ISampleProvider GetSampleProviderFromFile(string fileName, TimeSpan duration)
        {
            // Read the sample from disk.
            var reader = new AudioFileReader(fileName);
            ISampleProvider inSample = reader.ToSampleProvider();

            // Shorten to specified duration.
            ISampleProvider outSample = inSample.Take(duration);

            return outSample;
        }

        public static string GetFileNameFromNoteNumber(int noteNumber)
        {
            string samplesFolder = HostingEnvironment.MapPath("~/Samples");
            string[] files = Directory.GetFiles(samplesFolder);
            string searchString = noteNumber.ToString() + ".";
            string fileName = files.Single(s => Path.GetFileName(s).StartsWith(searchString));

            return fileName;
        }
    }
}
