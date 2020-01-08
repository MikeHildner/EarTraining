using EarTrainingLibrary.Enums;
using EarTrainingLibrary.NAudio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarTrainingLibrary.Utility
{
    public class Inversion
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static ISampleProvider[] CreateInversion(InversionType inversionType, double gain, TimeSpan duration, SignalGeneratorType sgType, params double[] frequencies)
        {
            switch(inversionType)
            {
                case InversionType.Root:
                    // Do nothing - just make what we were given.
                    break;

                case InversionType.HighFirst:
                    // Take the bottom note up an octave.
                    frequencies[0] *= 2;
                    break;

                case InversionType.HighSecond:
                    // Take the bottom two notes up an octave.
                    frequencies[0] *= 2;
                    frequencies[1] *= 2;
                    break;

                case InversionType.LowSecond:
                    // Take the top note down an octave.
                    frequencies[2] /= 2;
                    break;

                case InversionType.LowFirst:
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
            _log.Debug($"inversionType: {inversionType}, duration: {duration}, firstNoteNumber: {firstNoteNumber}, secondNoteNumber: {secondNoteNumber}, thirdNoteNumber: {thirdNoteNumber}");

            switch (inversionType)
            {
                case InversionType.Root:
                    // Do nothing - just make what we were given.
                    break;

                case InversionType.HighFirst:
                    // Take the bottom note up an octave.
                    firstNoteNumber += 12;
                    break;

                case InversionType.HighSecond:
                    // Take the bottom two notes up an octave.
                    firstNoteNumber += 12;
                    secondNoteNumber += 12;
                    break;

                case InversionType.LowSecond:
                    // Take the top note down an octave.
                    thirdNoteNumber -= 12;
                    break;

                case InversionType.LowFirst:
                    // Take the top two notes down an octave.
                    secondNoteNumber -= 12;
                    thirdNoteNumber -= 12;
                    break;

                default:
                    throw new NotSupportedException($"InversionType {inversionType} is not supported.");
            }

            _log.Debug($"Creating inversion using firstNoteNumber: {firstNoteNumber}, secondNoteNumber: {secondNoteNumber}, thirdNoteNumber: {thirdNoteNumber}");
            var samples = new ISampleProvider[3];
            samples[0] = NAudioHelper.GetSampleProvider(firstNoteNumber, duration);
            samples[1] = NAudioHelper.GetSampleProvider(secondNoteNumber, duration);
            samples[2] = NAudioHelper.GetSampleProvider(thirdNoteNumber, duration);

            return samples;
        }

        public static ISampleProvider[] CreateTriadInversionEx(int doNoteNumber, TriadType triadType, InversionType inversionType, TimeSpan duration)
        {
            int firstNoteNumber, secondNoteNumber, thirdNoteNumber;

            switch(triadType)
            {
                case TriadType.OneMajor:
                    firstNoteNumber = doNoteNumber;
                    secondNoteNumber = doNoteNumber + Interval.UpMajor3rd;
                    thirdNoteNumber = doNoteNumber + Interval.UpPerfect5th;
                    break;

                case TriadType.FourMajor:
                    firstNoteNumber = doNoteNumber + Interval.UpPerfect4th;
                    secondNoteNumber = doNoteNumber + Interval.UpMajor6th;
                    thirdNoteNumber = doNoteNumber + Interval.UpPerfectOctave;
                    break;

                case TriadType.FiveMajor:
                    firstNoteNumber = doNoteNumber + Interval.UpPerfect5th;
                    secondNoteNumber = doNoteNumber + Interval.UpMajor7th;
                    thirdNoteNumber = doNoteNumber + Interval.UpMajor9th;
                    break;

                case TriadType.SixMinor:
                    firstNoteNumber = doNoteNumber + Interval.UpMajor6th;
                    secondNoteNumber = doNoteNumber + Interval.UpPerfectOctave;
                    thirdNoteNumber = doNoteNumber + Interval.UpMajor10th;
                    break;

                case TriadType.ThreeMinor:
                    firstNoteNumber = doNoteNumber + Interval.UpMajor3rd;
                    secondNoteNumber = doNoteNumber + Interval.UpPerfect5th;
                    thirdNoteNumber = doNoteNumber + Interval.UpMajor7th;
                    break;

                case TriadType.TwoMinor:
                    firstNoteNumber = doNoteNumber + Interval.UpMajor2nd;
                    secondNoteNumber = doNoteNumber + Interval.UpPerfect4th;
                    thirdNoteNumber = doNoteNumber + Interval.UpMajor6th;
                    break;

                default:
                    throw new NotSupportedException($"TriadType {triadType} is not supported.");
            }

            switch (inversionType)
            {
                case InversionType.Root:
                    // Do nothing - just make what we were given.
                    break;

                case InversionType.HighFirst:
                    // Take the bottom note up an octave.
                    firstNoteNumber += 12;
                    break;

                case InversionType.HighSecond:
                    // Take the bottom two notes up an octave.
                    firstNoteNumber += 12;
                    secondNoteNumber += 12;
                    break;

                case InversionType.LowSecond:
                    // Take the top note down an octave.
                    thirdNoteNumber -= 12;
                    break;

                case InversionType.LowFirst:
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
                case InversionType.Root:
                    // Do nothing - just make what we were given.
                    break;

                case InversionType.HighFirst:
                    // Take the bottom note up an octave.
                    firstNoteNumber += Interval.UpPerfectOctave;
                    break;

                case InversionType.LowFirst:
                    // Take top note down an octave.
                    secondNoteNumber += Interval.DownPerfectOctave;
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
