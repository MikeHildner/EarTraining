using NAudio.Lame;
using NAudio.Wave;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Hosting;

namespace EarTrainingLibrary.Utility
{
    public static class ExtensionMethods
    {
        private static Logger _log = LogManager.GetCurrentClassLogger();

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
            var mp3Stream = new MemoryStream();
            var rdr = new WaveFileReader(wavStream);
            CheckAddBinPath();
            var wtr = new LameMP3FileWriter(mp3Stream, rdr.WaveFormat, 128);
            rdr.CopyTo(wtr);

            mp3Stream.Position = 0;
            wavStream.Position = 0;

            return mp3Stream;
        }

        public static MemoryStream WavToMp3File(this Stream wavStream, out string fileName)
        {
            var mp3Stream = new MemoryStream();
            var rdr = new WaveFileReader(wavStream);
            CheckAddBinPath();
            var wtr = new LameMP3FileWriter(mp3Stream, rdr.WaveFormat, 128);
            rdr.CopyTo(wtr);
            mp3Stream.Position = 0;

            // Write the mp3 stream to disk.
            string tempFolder = HostingEnvironment.MapPath("~/Temp");
            CleanFolder(tempFolder);
            string guid = Guid.NewGuid().ToString();
            var fileNameOnly = guid + ".mp3";
            string mp3FileName = Path.Combine(tempFolder, fileNameOnly);

            using (FileStream file = new FileStream(mp3FileName, FileMode.Create))
            {
                mp3Stream.CopyTo(file);
            }

            mp3Stream.Position = 0;
            wavStream.Position = 0;

            fileName =  fileNameOnly;
            return mp3Stream;
        }

        //public static MemoryStream WavToMp4(this Stream wavStream)
        //{
        //    try
        //    {
        //        string tempFolder = HostingEnvironment.MapPath("~/Temp");
        //        CleanFolder(tempFolder);
        //        string guid = Guid.NewGuid().ToString();
        //        string wavFileName = Path.Combine(tempFolder, guid + ".wav");

        //        // Write the wav stream to disk.
        //        wavStream.CopyTo(new FileStream(wavFileName, FileMode.Create));
        //        string mp4FileName = Path.Combine(tempFolder, guid + ".mp4");

        //        CheckAddBinPath();

        //        var cmdLine = $"ffmpeg.exe -i {wavFileName} {mp4FileName}";
        //        var process = Process.Start("ffmpeg.exe", $"-i {wavFileName} {mp4FileName}");
        //        process.WaitForExit();

        //        var mp4Stream = new MemoryStream();

        //        using (FileStream file = new FileStream(mp4FileName, FileMode.Open, FileAccess.Read))
        //        {
        //            byte[] bytes = new byte[file.Length];
        //            file.Read(bytes, 0, (int)file.Length);
        //            mp4Stream.Write(bytes, 0, (int)file.Length);
        //            mp4Stream.Position = 0;
        //        }

        //        return mp4Stream;
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(ex, ex.Message);
        //        throw;
        //    }
        //}

        private static void CleanFolder(string tempFolder)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(tempFolder);
            DateTime cutoff = DateTime.Now.AddMinutes(-15);
            IEnumerable<FileInfo> files = dirInfo.GetFiles().Where(w => w.LastWriteTime < cutoff);
            foreach (var file in files)
            {
                File.Delete(file.FullName);
            }
        }

        public static void CheckAddBinPath()
        {
            // find path to 'bin' folder
            var binPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "bin" });
            // get current search path from environment
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";

            // add 'bin' folder to search path if not already present
            if (!path.Split(Path.PathSeparator).Contains(binPath, StringComparer.CurrentCultureIgnoreCase))
            {
                path = string.Join(Path.PathSeparator.ToString(), new string[] { path, binPath });
                Environment.SetEnvironmentVariable("PATH", path);
            }
        }

    }
}
