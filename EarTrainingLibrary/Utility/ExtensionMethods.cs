using NAudio.Lame;
using NAudio.Wave;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Hosting;

namespace EarTrainingLibrary.Utility
{
    public static class ExtensionMethods
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do
                {
                    provider.GetBytes(box);
                }
                while (!(box[0] < n * (byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static MemoryStream WavToMp3Stream(this Stream wavStream)
        {
            MemoryStream mp3Stream = new MemoryStream();
            WaveFileReader rdr = new WaveFileReader(wavStream);
            CheckAddBinPath();
            LameMP3FileWriter wtr = new LameMP3FileWriter(mp3Stream, rdr.WaveFormat, 128);
            rdr.CopyTo(wtr);

            mp3Stream.Position = 0;
            wavStream.Position = 0;

            return mp3Stream;
        }

        public static MemoryStream WavToMp3FileOld(this Stream wavStream, out string fileName)
        {
            MemoryStream mp3Stream = new MemoryStream();
            WaveFileReader rdr = new WaveFileReader(wavStream);
            CheckAddBinPath();
            LameMP3FileWriter wtr = new LameMP3FileWriter(mp3Stream, rdr.WaveFormat, 128);
            rdr.CopyTo(wtr);
            mp3Stream.Position = 0;

            // Write the mp3 stream to disk.
            string tempFolder = HostingEnvironment.MapPath("~/Temp");
            //CleanFolder(tempFolder);
            string guid = Guid.NewGuid().ToString();
            string fileNameOnly = guid + ".mp3";
            string mp3FileName = Path.Combine(tempFolder, fileNameOnly);

            using (FileStream file = new FileStream(mp3FileName, FileMode.Create))
            {
                mp3Stream.CopyTo(file);
            }

            mp3Stream.Position = 0;
            wavStream.Position = 0;

            fileName = fileNameOnly;
            return mp3Stream;
        }

        public static void WavToMp3File(this Stream wavStream, out string fileName)
        {
            CheckAddBinPath();

            using (MemoryStream mp3Stream = new MemoryStream())
            using (WaveFileReader rdr = new WaveFileReader(wavStream))
            using (LameMP3FileWriter wtr = new LameMP3FileWriter(mp3Stream, rdr.WaveFormat, 128))
            {
                rdr.CopyTo(wtr);
                mp3Stream.Position = 0;

                // Write the mp3 stream to disk.
                string tempFolder = HostingEnvironment.MapPath("~/Temp");
                string guid = Guid.NewGuid().ToString();
                string fileNameOnly = guid + ".mp3";
                string mp3FileName = Path.Combine(tempFolder, fileNameOnly);

                using (FileStream file = new FileStream(mp3FileName, FileMode.Create))
                {
                    mp3Stream.CopyTo(file);
                }

                fileName = fileNameOnly;
            }
        }

        public static void CheckAddBinPath()
        {
            // find path to 'bin' folder
            string binPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "bin" });
            // get current search path from environment
            string path = Environment.GetEnvironmentVariable("PATH") ?? "";

            // add 'bin' folder to search path if not already present
            if (!path.Split(Path.PathSeparator).Contains(binPath, StringComparer.CurrentCultureIgnoreCase))
            {
                path = string.Join(Path.PathSeparator.ToString(), new string[] { path, binPath });
                Environment.SetEnvironmentVariable("PATH", path);
            }
        }

        /// <summary>
        /// Gets the bass note number from the given note number.
        /// Bass notes should be between C2 and C3 inclusive.
        /// </summary>
        /// <param name="noteNumber"></param>
        /// <returns></returns>
        public static int BassNoteNumber(this int noteNumber)
        {
            int lowest = 15;   // C2.
            int highest = 27;  // C3.

            int bassNoteNumber = noteNumber;

            while(bassNoteNumber > highest || bassNoteNumber < lowest)
            {
                if(bassNoteNumber > highest)
                {
                    bassNoteNumber -= 12;
                }

                if(bassNoteNumber < lowest)
                {
                    bassNoteNumber += 12;
                }
            }

            return bassNoteNumber;
        }

        /// <summary>
        /// VexFlow wants note names like C/4 instead of C4.
        /// </summary>
        /// <param name="noteName">The note name in the format such as C4.</param>
        /// <returns>string in the format such as C/4.</returns>
        public static string ToSlashNoteName(this string noteName)
        {
            int noteNameLength = noteName.Length;
            string nName = noteName.Substring(0, noteName.Length - 1);  // Hopefully we won't have to deal with super-high registers.
            string octave = noteName.Last().ToString();

            string slashNoteName = $"{nName}/{octave}";

            return slashNoteName;
        }
    }
}
