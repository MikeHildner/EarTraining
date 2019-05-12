using EarTrainingLibrary.Enums;
using EarTrainingLibrary.Utility;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class L1C4Controller : BaseController
    {
        // GET: L1C4
        public ActionResult Index()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;

            return View();
        }

        public FileResult GetProgression(double frequency, int progressiontype)
        {
            var progressionType = (ProgressionType)progressiontype;

            TimeSpan noteDuration = TimeSpan.FromSeconds(1);
            double gain = 0.2;
            SignalGeneratorType sgType = SignalGeneratorType.SawTooth;
            var solfeg = new Solfeg(frequency);

            ISampleProvider[] samples1;
            ISampleProvider[] samples2;

            switch (progressionType)
            {
                case ProgressionType.Four2ndToOneRoot:
                    samples1 = Inversion.CreateInversion(InversionType.LowSecondInversion, gain, noteDuration, sgType, solfeg.FaFrequency, solfeg.LaFrequency, solfeg.HighDoFrequency);
                    samples2 = Inversion.CreateInversion(InversionType.RootPosition, gain, noteDuration, sgType, solfeg.DoFrequency, solfeg.MiFrequency, solfeg.SoFrequency);
                    break;

                case ProgressionType.Five1stToOneRoot:
                    samples1 = Inversion.CreateInversion(InversionType.LowFirstInversion, gain, noteDuration, sgType, solfeg.SoFrequency, solfeg.TiFrequency, solfeg.HighReFrequency);
                    samples2 = Inversion.CreateInversion(InversionType.RootPosition, gain, noteDuration, sgType, solfeg.DoFrequency, solfeg.MiFrequency, solfeg.SoFrequency);
                    break;

                case ProgressionType.FiveRootToFourRoot:
                    samples1 = Inversion.CreateInversion(InversionType.RootPosition, gain, noteDuration, sgType, solfeg.SoFrequency, solfeg.TiFrequency, solfeg.HighReFrequency);
                    samples2 = Inversion.CreateInversion(InversionType.RootPosition, gain, noteDuration, sgType, solfeg.FaFrequency, solfeg.LaFrequency, solfeg.HighDoFrequency);
                    break;

                case ProgressionType.OneRootToFour2nd:
                    samples1 = Inversion.CreateInversion(InversionType.RootPosition, gain, noteDuration, sgType, solfeg.DoFrequency, solfeg.MiFrequency, solfeg.SoFrequency);
                    samples2 = Inversion.CreateInversion(InversionType.LowSecondInversion, gain, noteDuration, sgType, solfeg.FaFrequency, solfeg.LaFrequency, solfeg.HighDoFrequency);
                    
                    break;

                default:
                    throw new NotSupportedException($"ProgressionType {progressionType} is not supported.");
            }

            MixingSampleProvider msp1 = new MixingSampleProvider(samples1[0].WaveFormat);
            msp1.AddMixerInput(samples1[0]);
            msp1.AddMixerInput(samples1[1]);
            msp1.AddMixerInput(samples1[2]);

            ISampleProvider shortRest = new SignalGenerator()
            {
                Gain = 0
            }.Take(TimeSpan.FromMilliseconds(200));

            MixingSampleProvider msp2 = new MixingSampleProvider(samples2[0].WaveFormat);
            msp2.AddMixerInput(samples2[0]);
            msp2.AddMixerInput(samples2[1]);
            msp2.AddMixerInput(samples2[2]);

            var phrase = msp1
                .FollowedBy(shortRest)
                .FollowedBy(msp2);

            var stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            MemoryStream mp4Stream = wavStream.WavToMp4();

            return new FileStreamResult(mp4Stream, "audio/mpeg");
        }
    }
}