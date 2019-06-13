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
            switch(inversionType)
            {
                case InversionType.RootPosition:
                    // Do nothing - just make what we were given.
                    break;

                case InversionType.HighFirstInversion:
                    // Take the bottom note up an octave.
                    frequencies[0] *= 2;
                    break;

                case InversionType.HighSecondInversion:
                    // Take the bottom two notes up an octave.
                    frequencies[0] *= 2;
                    frequencies[1] *= 2;
                    break;

                case InversionType.LowSecondInversion:
                    // Take the top note down an octave.
                    frequencies[2] /= 2;
                    break;

                case InversionType.LowFirstInversion:
                    // Take the top two notes down an octave.
                    frequencies[1] /= 2;
                    frequencies[2] /= 2;
                    break;

                default:
                    throw new NotSupportedException($"InversionType {inversionType} is not supported.");
            }

            var outSamples = new ISampleProvider[frequencies.Length];
            for (int i = 0; i < frequencies.Length; i++)
            {
                outSamples[i] = NAudioHelper.GetSampleProvider(gain, frequencies[i], sgType, duration);
            }

            return outSamples;
        }

        public static ISampleProvider[] CreateTriadInversionEx(InversionType inversionType, TimeSpan duration, int firstNoteNumber, int secondNoteNumber, int thirdNoteNumber)
        {
            switch (inversionType)
            {
                case InversionType.RootPosition:
                    // Do nothing - just make what we were given.
                    break;

                case InversionType.HighFirstInversion:
                    // Take the bottom note up an octave.
                    firstNoteNumber += 12;
                    break;

                case InversionType.HighSecondInversion:
                    // Take the bottom two notes up an octave.
                    firstNoteNumber += 12;
                    secondNoteNumber += 12;
                    break;

                case InversionType.LowSecondInversion:
                    // Take the top note down an octave.
                    thirdNoteNumber -= 12;
                    break;

                case InversionType.LowFirstInversion:
                    // Take the top two notes down an octave.
                    secondNoteNumber -= 12;
                    thirdNoteNumber -= 12;
                    break;

                default:
                    throw new NotSupportedException($"InversionType {inversionType} is not supported.");
            }

            var samples = new ISampleProvider[3];
            samples[0] = NAudioHelper.GetSampleProvider(firstNoteNumber, duration);
            samples[1] = NAudioHelper.GetSampleProvider(secondNoteNumber, duration);
            samples[2] = NAudioHelper.GetSampleProvider(thirdNoteNumber, duration);

            return samples;
        }

        public static ISampleProvider[] Create2NoteInversionEx(InversionType inversionType, TimeSpan duration, int firstNoteNumber, int secondNoteNumber)
        {
            switch (inversionType)
            {
                case InversionType.RootPosition:
                    // Do nothing - just make what we were given.
                    break;

                case InversionType.HighFirstInversion:
                    // Take the bottom note up an octave.
                    firstNoteNumber = firstNoteNumber + Interval.Up1Octave;
                    break;

                case InversionType.LowFirstInversion:
                    // Take top note down an octave.
                    secondNoteNumber = secondNoteNumber + Interval.Down1Octave;
                    break;

                default:
                    throw new NotSupportedException($"InversionType {inversionType} is not supported.");
            }

            var samples = new ISampleProvider[2];
            samples[0] = NAudioHelper.GetSampleProvider(firstNoteNumber, duration);
            samples[1] = NAudioHelper.GetSampleProvider(secondNoteNumber, duration);

            return samples;
        }
    }
}
