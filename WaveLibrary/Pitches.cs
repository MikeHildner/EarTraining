using System;
using System.Collections.Generic;
using System.Linq;

namespace WaveLibrary
{
    public class Pitches
    {
        public List<Pitch> PitchesList { get; set; }

        public Pitches()
        {
            List<Pitch> pitchesList = new List<Pitch>();
            //Pitch pitch = new Pitch("A4", 440);
            //);pitchesList.Add(pitch);
            //pitch = new Pitch("C4 - Middle C", 261.625565);
            //);pitchesList.Add(pitch);

            Pitch pitch;
            //pitch = new Pitch("C0", 16.35); pitchesList.Add(pitch);
            //pitch = new Pitch("C#0/Db0", 17.32); pitchesList.Add(pitch);
            //pitch = new Pitch("D0", 18.35); pitchesList.Add(pitch);
            //pitch = new Pitch("D#0/Eb0", 19.45); pitchesList.Add(pitch);
            //pitch = new Pitch("E0", 20.6); pitchesList.Add(pitch);
            //pitch = new Pitch("F0", 21.83); pitchesList.Add(pitch);
            //pitch = new Pitch("F#0/Gb0", 23.12); pitchesList.Add(pitch);
            //pitch = new Pitch("G0", 24.5); pitchesList.Add(pitch);
            //pitch = new Pitch("G#0/Ab0", 25.96); pitchesList.Add(pitch);
            //pitch = new Pitch("A0", 27.5); pitchesList.Add(pitch);
            //pitch = new Pitch("A#0/Bb0", 29.14); pitchesList.Add(pitch);
            //pitch = new Pitch("B0", 30.87); pitchesList.Add(pitch);
            //pitch = new Pitch("C1", 32.7); pitchesList.Add(pitch);
            //pitch = new Pitch("C#1/Db1", 34.65); pitchesList.Add(pitch);
            //pitch = new Pitch("D1", 36.71); pitchesList.Add(pitch);
            //pitch = new Pitch("D#1/Eb1", 38.89); pitchesList.Add(pitch);
            //pitch = new Pitch("E1", 41.2); pitchesList.Add(pitch);
            //pitch = new Pitch("F1", 43.65); pitchesList.Add(pitch);
            //pitch = new Pitch("F#1/Gb1", 46.25); pitchesList.Add(pitch);
            //pitch = new Pitch("G1", 49); pitchesList.Add(pitch);
            //pitch = new Pitch("G#1/Ab1", 51.91); pitchesList.Add(pitch);
            //pitch = new Pitch("A1", 55); pitchesList.Add(pitch);
            //pitch = new Pitch("A#1/Bb1", 58.27); pitchesList.Add(pitch);
            //pitch = new Pitch("B1", 61.74); pitchesList.Add(pitch);
            //pitch = new Pitch("C2", 65.41); pitchesList.Add(pitch);
            //pitch = new Pitch("C#2/Db2", 69.3); pitchesList.Add(pitch);
            //pitch = new Pitch("D2", 73.42); pitchesList.Add(pitch);
            //pitch = new Pitch("D#2/Eb2", 77.78); pitchesList.Add(pitch);
            //pitch = new Pitch("E2", 82.41); pitchesList.Add(pitch);
            //pitch = new Pitch("F2", 87.31); pitchesList.Add(pitch);
            //pitch = new Pitch("F#2/Gb2", 92.5); pitchesList.Add(pitch);
            //pitch = new Pitch("G2", 98); pitchesList.Add(pitch);
            //pitch = new Pitch("G#2/Ab2", 103.83); pitchesList.Add(pitch);
            //pitch = new Pitch("A2", 110); pitchesList.Add(pitch);
            //pitch = new Pitch("A#2/Bb2", 116.54); pitchesList.Add(pitch);
            //pitch = new Pitch("B2", 123.47); pitchesList.Add(pitch);
            pitch = new Pitch("C3", 130.81); pitchesList.Add(pitch);
            pitch = new Pitch("C#3/Db3", 138.59); pitchesList.Add(pitch);
            pitch = new Pitch("D3", 146.83); pitchesList.Add(pitch);
            pitch = new Pitch("D#3/Eb3", 155.56); pitchesList.Add(pitch);
            pitch = new Pitch("E3", 164.81); pitchesList.Add(pitch);
            pitch = new Pitch("F3", 174.61); pitchesList.Add(pitch);
            pitch = new Pitch("F#3/Gb3", 185); pitchesList.Add(pitch);
            pitch = new Pitch("G3", 196); pitchesList.Add(pitch);
            pitch = new Pitch("G#3/Ab3", 207.65); pitchesList.Add(pitch);
            pitch = new Pitch("A3", 220); pitchesList.Add(pitch);
            pitch = new Pitch("A#3/Bb3", 233.08); pitchesList.Add(pitch);
            pitch = new Pitch("B3", 246.94); pitchesList.Add(pitch);
            pitch = new Pitch("C4", 261.63); pitchesList.Add(pitch);
            pitch = new Pitch("C#4/Db4", 277.18); pitchesList.Add(pitch);
            pitch = new Pitch("D4", 293.66); pitchesList.Add(pitch);
            pitch = new Pitch("D#4/Eb4", 311.13); pitchesList.Add(pitch);
            pitch = new Pitch("E4", 329.63); pitchesList.Add(pitch);
            pitch = new Pitch("F4", 349.23); pitchesList.Add(pitch);
            pitch = new Pitch("F#4/Gb4", 369.99); pitchesList.Add(pitch);
            pitch = new Pitch("G4", 392); pitchesList.Add(pitch);
            pitch = new Pitch("G#4/Ab4", 415.3); pitchesList.Add(pitch);
            pitch = new Pitch("A4", 440); pitchesList.Add(pitch);
            pitch = new Pitch("A#4/Bb4", 466.16); pitchesList.Add(pitch);
            pitch = new Pitch("B4", 493.88); pitchesList.Add(pitch);
            //pitch = new Pitch("C5", 523.25); pitchesList.Add(pitch);
            //pitch = new Pitch("C#5/Db5", 554.37); pitchesList.Add(pitch);
            //pitch = new Pitch("D5", 587.33); pitchesList.Add(pitch);
            //pitch = new Pitch("D#5/Eb5", 622.25); pitchesList.Add(pitch);
            //pitch = new Pitch("E5", 659.25); pitchesList.Add(pitch);
            //pitch = new Pitch("F5", 698.46); pitchesList.Add(pitch);
            //pitch = new Pitch("F#5/Gb5", 739.99); pitchesList.Add(pitch);
            //pitch = new Pitch("G5", 783.99); pitchesList.Add(pitch);
            //pitch = new Pitch("G#5/Ab5", 830.61); pitchesList.Add(pitch);
            //pitch = new Pitch("A5", 880); pitchesList.Add(pitch);
            //pitch = new Pitch("A#5/Bb5", 932.33); pitchesList.Add(pitch);
            //pitch = new Pitch("B5", 987.77); pitchesList.Add(pitch);
            //pitch = new Pitch("C6", 1046.5); pitchesList.Add(pitch);
            //pitch = new Pitch("C#6/Db6", 1108.73); pitchesList.Add(pitch);
            //pitch = new Pitch("D6", 1174.66); pitchesList.Add(pitch);
            //pitch = new Pitch("D#6/Eb6", 1244.51); pitchesList.Add(pitch);
            //pitch = new Pitch("E6", 1318.51); pitchesList.Add(pitch);
            //pitch = new Pitch("F6", 1396.91); pitchesList.Add(pitch);
            //pitch = new Pitch("F#6/Gb6", 1479.98); pitchesList.Add(pitch);
            //pitch = new Pitch("G6", 1567.98); pitchesList.Add(pitch);
            //pitch = new Pitch("G#6/Ab6", 1661.22); pitchesList.Add(pitch);
            //pitch = new Pitch("A6", 1760); pitchesList.Add(pitch);
            //pitch = new Pitch("A#6/Bb6", 1864.66); pitchesList.Add(pitch);
            //pitch = new Pitch("B6", 1975.53); pitchesList.Add(pitch);
            //pitch = new Pitch("C7", 2093); pitchesList.Add(pitch);
            //pitch = new Pitch("C#7/Db7", 2217.46); pitchesList.Add(pitch);
            //pitch = new Pitch("D7", 2349.32); pitchesList.Add(pitch);
            //pitch = new Pitch("D#7/Eb7", 2489.02); pitchesList.Add(pitch);
            //pitch = new Pitch("E7", 2637.02); pitchesList.Add(pitch);
            //pitch = new Pitch("F7", 2793.83); pitchesList.Add(pitch);
            //pitch = new Pitch("F#7/Gb7", 2959.96); pitchesList.Add(pitch);
            //pitch = new Pitch("G7", 3135.96); pitchesList.Add(pitch);
            //pitch = new Pitch("G#7/Ab7", 3322.44); pitchesList.Add(pitch);
            //pitch = new Pitch("A7", 3520); pitchesList.Add(pitch);
            //pitch = new Pitch("A#7/Bb7", 3729.31); pitchesList.Add(pitch);
            //pitch = new Pitch("B7", 3951.07); pitchesList.Add(pitch);
            //pitch = new Pitch("C8", 4186.01); pitchesList.Add(pitch);
            //pitch = new Pitch("C#8/Db8", 4434.92); pitchesList.Add(pitch);
            //pitch = new Pitch("D8", 4698.63); pitchesList.Add(pitch);
            //pitch = new Pitch("D#8/Eb8", 4978.03); pitchesList.Add(pitch);
            //pitch = new Pitch("E8", 5274.04); pitchesList.Add(pitch);
            //pitch = new Pitch("F8", 5587.65); pitchesList.Add(pitch);
            //pitch = new Pitch("F#8/Gb8", 5919.91); pitchesList.Add(pitch);
            //pitch = new Pitch("G8", 6271.93); pitchesList.Add(pitch);
            //pitch = new Pitch("G#8/Ab8", 6644.88); pitchesList.Add(pitch);
            //pitch = new Pitch("A8", 7040); pitchesList.Add(pitch);
            //pitch = new Pitch("A#8/Bb8", 7458.62); pitchesList.Add(pitch);
            //pitch = new Pitch("B8", 7902.13); pitchesList.Add(pitch);


            PitchesList = pitchesList;
        }

        public Pitch Random()
        {
            Random r = new Random();
            int randomInt = r.Next(0, PitchesList.Count); //for ints
            Pitch pitch = PitchesList.ElementAt(randomInt);
            return pitch;
        }
    }
}
