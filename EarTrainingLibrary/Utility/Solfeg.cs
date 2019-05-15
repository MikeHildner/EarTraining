using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarTrainingLibrary.Utility
{
    public class Solfeg
    {
        private readonly double _doFrequency;

        public Solfeg(double doFrequency)
        {
            _doFrequency = doFrequency;
        }

        public double DoFrequency
        {
            get
            {
                return _doFrequency;
            }
        }

        public double RaFrequency
        {
            get
            {
                double frequency = GetFrequencyByHalfStepsFromDo(1);
                return frequency;
            }
        }

        public double ReFrequency
        {
            get
            {
                double frequency = GetFrequencyByHalfStepsFromDo(2);
                return frequency;
            }
        }

        public double MaFrequency
        {
            get
            {
                double frequency = GetFrequencyByHalfStepsFromDo(3);
                return frequency;
            }
        }

        public double MiFrequency
        {
            get
            {
                double frequency = GetFrequencyByHalfStepsFromDo(4);
                return frequency;
            }
        }

        public double FaFrequency
        {
            get
            {
                double frequency = GetFrequencyByHalfStepsFromDo(5);
                return frequency;
            }
        }

        public double SeFrequency
        {
            get
            {
                double frequency = GetFrequencyByHalfStepsFromDo(6);
                return frequency;
            }
        }

        public double SoFrequency
        {
            get
            {
                double frequency = GetFrequencyByHalfStepsFromDo(7);
                return frequency;
            }
        }

        public double LeFrequency
        {
            get
            {
                double frequency = GetFrequencyByHalfStepsFromDo(8);
                return frequency;
            }
        }

        public double LaFrequency
        {
            get
            {
                double frequency = GetFrequencyByHalfStepsFromDo(9);
                return frequency;
            }
        }

        public double TeFrequency
        {
            get
            {
                double frequency = GetFrequencyByHalfStepsFromDo(10);
                return frequency;
            }
        }

        public double TiFrequency
        {
            get
            {
                double frequency = GetFrequencyByHalfStepsFromDo(11);
                return frequency;
            }
        }

        public double HighDoFrequency
        {
            get
            {
                double frequency = GetFrequencyByHalfStepsFromDo(12);
                return frequency;
            }
        }

        public double LowTiFrequency
        {
            get
            {
                return GetFrequencyByHalfStepsFromDo(-1);
            }
        }

        public double HighReFrequency
        {
            get
            {
                double frequency = GetFrequencyByHalfStepsFromDo(14);
                return frequency;
            }
        }

        public double GetFrequencyByHalfStepsFromDo(int halfStepsFromDo)
        {
            double frequencyRatio = Math.Pow(2, Math.Abs(halfStepsFromDo) / 12d);
            double thisFrequency;

            if (halfStepsFromDo >= 0)
            {
                thisFrequency = _doFrequency * frequencyRatio;
                return thisFrequency;
            }
            else
            {
                thisFrequency = _doFrequency / frequencyRatio;
                return thisFrequency;
            }
        }

        public static MemoryStream GetDONote(double frequency)
        {
            var doNote = new SignalGenerator()
            {
                Gain = 0.2,
                Frequency = frequency,
                Type = SignalGeneratorType.SawTooth
            };

            var phrase = doNote.Take(TimeSpan.FromSeconds(1));

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;

            MemoryStream mp3Stream = wavStream.WavToMp3Stream();

            return mp3Stream;
        }

    }
}
