using EarTrainingLibrary.Enums;
using EarTrainingLibrary.NAudio;
using EarTrainingLibrary.Utility;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WaveLibrary;

namespace EarTraining.Controllers
{
    public class L1C1Controller : BaseController
    {
        public L1C1Controller()
        {
            Pitch pitch = new Pitches().Random();
            ViewBag.Pitch = pitch;
            ViewBag.ShowDo = true;
        }

        public ActionResult SolfegResolutionsDO()
        {
            return View();
        }

        public ActionResult SolfegResolutionsNoDO()
        {
            return View();
        }

        public ActionResult PitchIdentification()
        {
            return View();
        }

        public ActionResult Experimental()
        {
            return View();
        }

        public ActionResult GetResolutionDO(string doNoteName, int type)
        {
            ResolutionType rt = (ResolutionType)type;
            MemoryStream wavStream = GetResolutionDO(doNoteName, rt);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");

        }

        private MemoryStream GetResolutionDO(string doNoteName, ResolutionType resolutionType)
        {
            double bpm = double.Parse(ConfigurationManager.AppSettings["MelodicDrillBPM"]);
            double quarterNotemillis = (60 / bpm) * 1000;
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNotemillis);
            TimeSpan halfNoteDuration = quarterNoteDuration.Add(quarterNoteDuration);
            TimeSpan wholeNoteDuration = halfNoteDuration.Add(halfNoteDuration);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1;
            ISampleProvider note2;
            ISampleProvider note3;
            ISampleProvider note4;

            switch (resolutionType)
            {

                case ResolutionType.DoDoReDo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, halfNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoFaMi:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoLaSo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoTiHighDo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber + 12, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoLowTiDo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, quarterNoteDuration);
                    note3 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, halfNoteDuration);
                    note4 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                default:
                    throw new NotSupportedException($"ResolutionType '{resolutionType}' is not supported.");
            }

            ISampleProvider phrase = note1
                .FollowedBy(note2)
                .FollowedBy(note3)
                .FollowedBy(note4);

            SampleToWaveProvider stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            return wavStream;
        }

        public ActionResult GetResolutionNoDO(string doNoteName, int type)
        {
            ResolutionType rt = (ResolutionType)type;
            MemoryStream wavStream = GetResolutionNoDO(doNoteName, rt);

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");

        }

        private MemoryStream GetResolutionNoDO(string doNoteName, ResolutionType resolutionType)
        {
            double bpm = double.Parse(ConfigurationManager.AppSettings["MelodicDrillBPM"]);
            double quarterNotemillis = (60 / bpm) * 1000;
            TimeSpan quarterNoteDuration = TimeSpan.FromMilliseconds(quarterNotemillis);
            TimeSpan halfNoteDuration = quarterNoteDuration.Add(quarterNoteDuration);
            TimeSpan wholeNoteDuration = halfNoteDuration.Add(halfNoteDuration);

            string doFileName = NAudioHelper.GetFileNameFromNoteName(doNoteName);
            doFileName = Path.GetFileName(doFileName);
            int doNoteNumber = int.Parse(doFileName.Split('.')[0]);

            ISampleProvider note1;
            ISampleProvider note2;

            switch (resolutionType)
            {

                case ResolutionType.DoDoReDo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor2nd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoFaMi:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect4th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor3rd, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoLaSo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor6th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpPerfect5th, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoTiHighDo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.UpMajor7th, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber + 12, wholeNoteDuration);
                    break;

                case ResolutionType.DoDoLowTiDo:
                    note1 = NAudioHelper.GetSampleProvider(doNoteNumber + Interval.DownMinor2nd, halfNoteDuration);
                    note2 = NAudioHelper.GetSampleProvider(doNoteNumber, wholeNoteDuration);
                    break;

                default:
                    throw new NotSupportedException($"ResolutionType '{resolutionType}' is not supported.");
            }

            ISampleProvider phrase = note1
                .FollowedBy(note2);

            SampleToWaveProvider stwp = new SampleToWaveProvider(phrase);

            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;
            return wavStream;
        }

        public ActionResult GetNoteEx(string doNoteName, int type)
        {
            int doNoteNumber = NoteHelper.NoteNumberFromNoteName(doNoteName);
            int thisNoteNumber = doNoteNumber + type;
            ISampleProvider note = NAudioHelper.GetSampleProvider(thisNoteNumber, TimeSpan.FromSeconds(3));

            var stwp = new SampleToWaveProvider(note);
            MemoryStream wavStream = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wavStream, stwp);
            wavStream.Position = 0;

            wavStream.WavToMp3File(out string fileName);
            return Redirect($"~/Temp/{fileName}");
        }
    }
}