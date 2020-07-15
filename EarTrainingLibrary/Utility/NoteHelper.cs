using EarTrainingLibrary.NAudio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EarTrainingLibrary.Utility
{
    public static class NoteHelper
    {
        public static int NoteNumberFromNoteName(string noteName)
        {
            string thisFileName = NAudioHelper.GetFileNameFromNoteName(noteName);
            thisFileName = Path.GetFileName(thisFileName);
            int noteNumber = int.Parse(thisFileName.Split('.')[0]);
            return noteNumber;
        }

        public static int[] TransposeScaleNoteNumbers(int[] scaleNoteNumbers, string keySignature)
        {
            switch (keySignature)
            {
                case "F#":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += 6;
                    }
                    break;

                case "F":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += 5;
                    }
                    break;

                case "E":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += 4;
                    }
                    break;

                case "Eb":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += 3;
                    }
                    break;

                case "D":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += 2;
                    }
                    break;

                case "Db":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += 1;
                    }
                    break;

                case "C":
                    break;  // Do nothing as (hopefully!) we're passed in the C major scale notes number.

                case "B":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += -1;
                    }
                    break;

                case "Bb":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += -2;
                    }
                    break;

                case "A":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += -3;
                    }
                    break;

                case "Ab":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += -4;
                    }
                    break;

                case "G":
                    for (int i = 0; i < scaleNoteNumbers.Length; i++)
                    {
                        scaleNoteNumbers[i] += -5;
                    }
                    break;

                default:
                    throw new NotSupportedException($"Key signature '{keySignature}' is not supported.");
            }

            return scaleNoteNumbers;
        }

        public static int GetRandomInt(int inclusiveLowerBound, int exclusiveUpperBound)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] buffer = new byte[4];

                rng.GetBytes(buffer);
                int result = BitConverter.ToInt32(buffer, 0);
                int index = new Random(result).Next(inclusiveLowerBound, exclusiveUpperBound);
                return index;
            }
        }

        public static int[] PopulateNoteNumbersFromQueue(int numberOfNotes, Queue<int> noteNumberQueue)
        {
            int[] measureNoteNumbers = new int[numberOfNotes];
            for (int i = 0; i < measureNoteNumbers.Length; i++)
            {
                measureNoteNumbers[i] = noteNumberQueue.Dequeue();
            }

            return measureNoteNumbers;
        }

        public static void AdjustNoteNamesForKey(string keySignature, int[] measureNoteNumbers, string[] noteNames)
        {
            for (int i = 0; i < noteNames.Length; i++)
            {
                noteNames[i] = NAudioHelper.GetNoteNameFromNoteNumber(measureNoteNumbers[i]);
                if ((keySignature == "G" || keySignature == "A" || keySignature == "D" || keySignature == "E" || keySignature == "B") && noteNames[i].Contains("b"))
                {
                    noteNames[i] = noteNames[i].FlatToNaturalForSharpKeys();
                }
                if ((keySignature == "F" || keySignature == "Bb" || keySignature == "Eb" || keySignature == "Ab" || keySignature == "Db") && noteNames[i].Contains("b"))
                {
                    noteNames[i] = noteNames[i].FlatToNaturalForFlatKeys();
                }
                if (keySignature == "F#")
                {
                    noteNames[i] = noteNames[i].AdjustForFSharp();
                }
            }
        }

        // TODO: Refactor this to only receive a whole note duration, and calculate the others from there.
        public static void CreateSamplesFromRhythmsAndNoteNames(TimeSpan eighthNoteDuration, TimeSpan quarterNoteDuration, TimeSpan halfNoteDuration, TimeSpan dottedHalfNoteDuration, TimeSpan wholeNoteDuration, string[] measureRhythmSplit, ISampleProvider[] notes, int[] measureNoteNumbers)
        {
            for (int i = 0; i < notes.Length; i++)
            {
                TimeSpan duration;
                switch (measureRhythmSplit[i])
                {
                    case "1":
                        duration = wholeNoteDuration;
                        break;

                    case "2":
                        duration = halfNoteDuration;
                        break;

                    case "2.":
                        duration = dottedHalfNoteDuration;
                        break;

                    case "4":
                        duration = quarterNoteDuration;
                        break;

                    case "8":
                        duration = eighthNoteDuration;
                        break;

                    default:
                        throw new NotSupportedException($"Duration '{measureRhythmSplit[i]}' is not supported.");
                }

                notes[i] = NAudioHelper.GetSampleProvider(measureNoteNumbers[i], duration);
            }
        }

        public static string GetEasyScoreScript(string elementId, string[] noteNames, string[] measureRhythmSplit, string keySignature, bool showTimeSignature)
        {
            string timeSignature = string.Empty;
            if (showTimeSignature)
            {
                timeSignature = ".addTimeSignature('4/4')";
            }

            var sb = new StringBuilder();
            for (int i = 0; i < noteNames.Length; i++)
            {
                sb.Append($"{noteNames[i]}/{measureRhythmSplit[i]},");
            }

            var easyScoreNotes = sb.ToString();
            easyScoreNotes = easyScoreNotes.TrimEnd(',');
            //easyScoreNotes += " |";

            string script = $@"
            const vf = new Vex.Flow.Factory({{
                renderer: {{ elementId: '{elementId}' }}
            }});

            const score = vf.EasyScore();
            //const system = vf.System();
            var system = vf.System({{width: 320}});

            system.addStave({{
                voices: [
                    score.voice(score.notes('{easyScoreNotes}', {{ stem: 'up' }})),
                ]
            }}).addClef('treble'){timeSignature}.addKeySignature('{keySignature}');
            system.addConnector('singleLeft');
            system.addConnector('singleRight');

            vf.draw();
            ";

            return script;
        }

        public static string GetEasyScoreScript3(string elementId, string[] noteNames, string[] measureRhythms, string keySignature, bool showTimeSignature)
        {
            string timeSignature = string.Empty;
            if (showTimeSignature)
            {
                timeSignature = ".addTimeSignature('4/4')";
            }

            var sb = new StringBuilder();
            for (int i = 0; i < noteNames.Length; i++)
            {
                // Beam if a pair of eighth notes.
                if (measureRhythms[i] == "8" && measureRhythms[i + 1] == "8")
                {
                    sb.Append($".concat(score.beam(score.notes('{noteNames[i]}/{measureRhythms[i]},{noteNames[i + 1]}/{measureRhythms[i + 1]}'), {{ autoStem: true }}))");
                    i++;
                }
                else
                {
                    sb.Append($".concat(score.notes('{noteNames[i]}/{measureRhythms[i]},'))");
                }
            }

            var easyScoreNotes = sb.ToString();
            easyScoreNotes = easyScoreNotes.TrimEnd(',');

            string script = $@"
            const vf = new Vex.Flow.Factory({{
                renderer: {{ elementId: '{elementId}' }}
            }});

            const score = vf.EasyScore();
            var system = vf.System({{width: 320}});

            system.addStave({{
                voices: [
                    score.voice(
                      score.notes('')
                      {easyScoreNotes}
                      ),
                ]
            }}).addClef('treble'){timeSignature}.addKeySignature('{keySignature}');
            system.addConnector('singleLeft');
            system.addConnector('singleRight');

            vf.draw();
            ";

            return script;
        }
        public static string GetMusicXmlScript(string elementId, string[] noteNames, string[] measureRhythmSplit, string keySignature, bool showTimeSignature)
        {
            int fifths = GetFifthsFromKey(keySignature);
            string timeSignature = showTimeSignature ? "<time><beats>4</beats><beat-type>4</beat-type></time>" : string.Empty;

            string xml = $@"
                <?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
                <!DOCTYPE score-partwise PUBLIC
                    ""-//Recordare//DTD MusicXML 3.1 Partwise//EN""
                    ""http://www.musicxml.org/dtds/partwise.dtd"">
                <score-partwise version=""3.1"">
                  <work>
                    <work-title> </work-title>
                  </work>
                  <part-list>
                    <score-part id=""P1"">
                      <part-name>Music</part-name>
                    </score-part>
                  </part-list>
                  <part id=""P1"">
                    <measure number=""1"">
                      <attributes>
                        <divisions>1</divisions>
                        <key>
                          <fifths>{fifths}</fifths>
                        </key>
                        {timeSignature}
                        <clef>
                          <sign>G</sign>
                          <line>2</line>
                        </clef>
                      </attributes>
                      <note>
                        <pitch>
                          <step>D</step>
                          <octave>4</octave>
                        </pitch>
                        <duration>4</duration>
                        <type>whole</type>
                      </note>
                    </measure>
                  </part>
                </score-partwise>
";

            xml = xml.Replace(Environment.NewLine, string.Empty);
            var script = $@"
            var osmd = new opensheetmusicdisplay.OpenSheetMusicDisplay({elementId});
            var loadPromise = osmd.load('{xml}');
            loadPromise.then(function() {{
                osmd.render();
            }});
            ";

            return script;
        }

        private static int GetFifthsFromKey(string keySignature)
        {
            switch (keySignature)
            {
                case "C":
                    return 0;

                case "G":
                    return 1;

                default:
                    throw new NotSupportedException($"keySignature '{keySignature}' is not supported.");
            }
        }
    }
}
