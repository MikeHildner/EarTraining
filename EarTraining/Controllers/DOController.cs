using System.IO;
using System.Media;
using System.Web.Mvc;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class DOController : Controller
    {
        // GET: DO
        public ActionResult Index()
        {
            //string filePath = @"C:\temp\test.wav";
            //WaveGenerator wave = new WaveGenerator(WaveExampleType.ExampleSineWave);
            //wave.SaveToFile(filePath);

            //SoundPlayer player = new SoundPlayer(filePath);
            //player.Play();

            return View();
        }

        public FileResult GetFileStreamResult()
        {
            byte[] buff;
            WaveGenerator wave = new WaveGenerator(WaveExampleType.ExampleSineWave);
            buff = wave.GetBytes();
            MemoryStream stream = new MemoryStream(buff);
            return new FileStreamResult(stream, "audio/wav");
        }
    }
}