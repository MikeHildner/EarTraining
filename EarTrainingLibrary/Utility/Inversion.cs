using EarTrainingLibrary.Enums;
using EarTrainingLibrary.NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarTrainingLibrary.Utility
{
    public class Inversion
    {
        public static ISampleProvider[] CreateInversion(InversionType inversionType, double gain, TimeSpan duration, SignalGeneratorType sgType, params double[] frequencies)
        {
            var outSamples = new ISampleProvider[frequencies.Length];
            switch(inversionType)
            {
                case InversionType.RootPosition:
                    // Do nothing - just make what we were given.
                    break;

                case InversionType.HighFirstInversion:
                    // Take the bottom note up an octave;
                    frequencies[0] *= 2;
                    break;

                case InversionType.HighSecondInversion:
                    // Take the bottom two notes up an octave;
                    frequencies[0] *= 2;
                    frequencies[1] *= 2;
                    break;

                default:
                    throw new NotSupportedException($"InversionType {inversionType} is not supported.");
            }

            for (int i = 0; i < frequencies.Length; i++)
            {
                outSamples[i] = NAudioHelper.GetSampleProvider(gain, frequencies[i], sgType, duration);
            }

            return outSamples;
        }
    }
}
