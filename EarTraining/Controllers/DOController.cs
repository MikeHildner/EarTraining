using EarTrainingLibrary.Utility;
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
            byte[] buff;
            WaveGenerator wave = new WaveGenerator(WaveExampleType.ExampleSineWave, frequency);
            buff = wave.GetBytes();
            MemoryStream wavStream = new MemoryStream(buff);
            //Stream mp3Stream = wavStream.WavToMp3();
            //return new FileStreamResult(mp3Stream, "audio/mpeg");
            MemoryStream mp4Stream = wavStream.WavToMp4();
            return new FileStreamResult(mp4Stream, "audio/mpeg");
        }
    }
}