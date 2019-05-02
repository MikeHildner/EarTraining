using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
