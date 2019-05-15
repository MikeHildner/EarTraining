using EarTrainingLibrary.NAudio;
using EarTrainingLibrary.Utility;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using System.Media;
using System.Web.Mvc;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class DOController : BaseController
    {
        // GET: DO
        public ActionResult Index()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;

            return View();
        }

        public FileResult GetDO(double frequency)
        {
            ISampleProvider note = NAudioHelper.GetSampleProvider(0.2, frequency, SignalGeneratorType.SawTooth, TimeSpan.FromSeconds(1));
            var stwp = new SampleToWaveProvider(note);
            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            // Just doing this so we can write the mp3 to disk.
            MemoryStream mp3Stream = wavStream.WavToMp3();
            MemoryStream mp4Stream = wavStream.WavToMp4();
            return new FileStreamResult(mp4Stream, "audio/mpeg");

            //byte[] buff;
            //WaveGenerator wave = new WaveGenerator(WaveExampleType.ExampleSineWave, frequency);
            //buff = wave.GetBytes();
            //MemoryStream wavStream = new MemoryStream(buff);
            //Stream mp3Stream = wavStream.WavToMp3();
            //return new FileStreamResult(mp3Stream, "audio/mpeg");
            //MemoryStream mp4Stream = wavStream.WavToMp4();
            //return new FileStreamResult(mp4Stream, "audio/mpeg");
        }
    }
}