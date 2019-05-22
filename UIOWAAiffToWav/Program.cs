using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIOWAAiffToWav
{
    class Program
    {
        static void Main(string[] args)
        {
            ConvertFiles();
        }

        private static void ConvertFiles()
        {
            // These UIOW files have some dead space at the beginning, and are very long (apx. 30 seconds).
            // I want an immediate attack and smaller files, so chop off the dead space at the beginning
            // and shorten the duration of the sample.

            string[] aiffFiles = Directory.GetFiles(@"E:\Source\Repos\EarTraining\EarTraining\UIOWA_AIFFs");
            var skipDuration = TimeSpan.FromMilliseconds(500);
            var takeDuration = TimeSpan.FromSeconds(4);

            foreach (var aiffFile in aiffFiles)
            {
                // Read the sample from disk.
                var reader = new AudioFileReader(aiffFile);
                ISampleProvider inSample = reader.ToSampleProvider();

                // Chop off beginning and ending.
                ISampleProvider outSample = inSample.Skip(skipDuration).Take(takeDuration);

                // Turn into a wav.
                var stwp = new SampleToWaveProvider(outSample);

                // New file name.
                string newFileName = Path.GetFileNameWithoutExtension(aiffFile);
                newFileName = newFileName.Replace("Piano.ff.", string.Empty);
                newFileName = NumberFromNoteName(newFileName);
                newFileName = Path.Combine(@"e:\temp", newFileName + ".wav");

                // Write to disk.
                using (var file = new FileStream(newFileName, FileMode.Create))
                {
                    WaveFileWriter.WriteWavFileToStream(file, stwp);
                }
            }
        }

        private static string NumberFromNoteName(string noteName)
        {
            int n;

            switch(noteName)
            {
                case "A0":
                    n = 0;
                    break;

                case "Bb0":
                    n = 1;
                    break;

                case "B0":
                    n = 2;
                    break;

                case "C1":
                    n = 3;
                    break;

                case "Db1":
                    n = 4;
                    break;

                case "D1":
                    n = 5;
                    break;

                case "Eb1":
                    n = 6;
                    break;

                case "E1":
                    n = 7;
                    break;

                case "F1":
                    n = 8;
                    break;

                case "Gb1":
                    n = 9;
                    break;

                case "G1":
                    n = 10;
                    break;

                case "Ab1":
                    n = 11;
                    break;

                case "A1":
                    n = 12;
                    break;

                case "Bb1":
                    n = 13;
                    break;

                case "B1":
                    n = 14;
                    break;

                case "C2":
                    n = 15;
                    break;

                case "Db2":
                    n = 16;
                    break;

                case "D2":
                    n = 17;
                    break;

                case "Eb2":
                    n = 18;
                    break;

                case "E2":
                    n = 19;
                    break;

                case "F2":
                    n = 20;
                    break;

                case "Gb2":
                    n = 21;
                    break;

                case "G2":
                    n = 22;
                    break;

                case "Ab2":
                    n = 23;
                    break;

                case "A2":
                    n = 24;
                    break;

                case "Bb2":
                    n = 25;
                    break;

                case "B2":
                    n = 26;
                    break;

                case "C3":
                    n = 27;
                    break;

                case "Db3":
                    n = 28;
                    break;

                case "D3":
                    n = 29;
                    break;

                case "Eb3":
                    n = 30;
                    break;

                case "E3":
                    n = 31;
                    break;

                case "F3":
                    n = 32;
                    break;

                case "Gb3":
                    n = 33;
                    break;

                case "G3":
                    n = 34;
                    break;

                case "Ab3":
                    n = 35;
                    break;

                case "A3":
                    n = 36;
                    break;

                case "Bb3":
                    n = 37;
                    break;

                case "B3":
                    n = 38;
                    break;

                case "C4":
                    n = 39;
                    break;

                case "Db4":
                    n = 40;
                    break;

                case "D4":
                    n = 41;
                    break;

                case "Eb4":
                    n = 42;
                    break;

                case "E4":
                    n = 43;
                    break;

                case "F4":
                    n = 44;
                    break;

                case "Gb4":
                    n = 45;
                    break;

                case "G4":
                    n = 46;
                    break;

                case "Ab4":
                    n = 47;
                    break;

                case "A4":
                    n = 48;
                    break;

                case "Bb4":
                    n = 49;
                    break;

                case "B4":
                    n = 50;
                    break;

                case "C5":
                    n = 51;
                    break;

                case "Db5":
                    n = 52;
                    break;

                case "D5":
                    n = 53;
                    break;

                case "Eb5":
                    n = 54;
                    break;

                case "E5":
                    n = 55;
                    break;

                case "F5":
                    n = 56;
                    break;

                case "Gb5":
                    n = 57;
                    break;

                case "G5":
                    n = 58;
                    break;

                case "Ab5":
                    n = 59;
                    break;

                case "A5":
                    n = 60;
                    break;

                case "Bb5":
                    n = 61;
                    break;

                case "B5":
                    n = 62;
                    break;

                case "C6":
                    n = 63;
                    break;

                case "Db6":
                    n = 64;
                    break;

                case "D6":
                    n = 65;
                    break;

                case "Eb6":
                    n = 66;
                    break;

                case "E6":
                    n = 67;
                    break;

                case "F6":
                    n = 68;
                    break;

                case "Gb6":
                    n = 69;
                    break;

                case "G6":
                    n = 70;
                    break;

                case "Ab6":
                    n = 71;
                    break;

                case "A6":
                    n = 72;
                    break;

                case "Bb6":
                    n = 73;
                    break;

                case "B6":
                    n = 74;
                    break;

                case "C7":
                    n = 75;
                    break;

                case "Db7":
                    n = 76;
                    break;

                case "D7":
                    n = 77;
                    break;

                case "Eb7":
                    n = 78;
                    break;

                case "E7":
                    n = 79;
                    break;

                case "F7":
                    n = 80;
                    break;

                case "Gb7":
                    n = 81;
                    break;

                case "G7":
                    n = 82;
                    break;

                case "Ab7":
                    n = 83;
                    break;

                case "A7":
                    n = 84;
                    break;

                case "Bb7":
                    n = 85;
                    break;

                case "B7":
                    n = 86;
                    break;

                case "C8":
                    n = 87;
                    break;

                default:
                    throw new NotSupportedException($"Note name '{noteName}' is not supported.");
            }

            return $"{n.ToString()}.{noteName}";
        }
    }
}
