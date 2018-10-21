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
            string filePath = @"C:\temp\test.wav";
            WaveGenerator wave = new WaveGenerator(WaveExampleType.ExampleSineWave);
            wave.Save(filePath);

            SoundPlayer player = new SoundPlayer(filePath);
            player.Play();

            return View();
        }
    }
}