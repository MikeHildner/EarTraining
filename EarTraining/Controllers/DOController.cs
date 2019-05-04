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

        public FileResult GetFileStreamResult(double hertz)
        {
            byte[] buff;
            WaveGenerator wave = new WaveGenerator(WaveExampleType.ExampleSineWave, hertz);
            buff = wave.GetBytes();
            MemoryStream stream = new MemoryStream(buff);
            return new FileStreamResult(stream, "audio/wav");
        }
    }
}