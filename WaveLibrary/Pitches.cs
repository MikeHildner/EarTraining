using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveLibrary
{
    public class Pitches
    {
        public List<Pitch> PitchesList { get; set; }

        public Pitches()
        {
            List<Pitch> pitchesList = new List<Pitch>();
            Pitch pitch = new Pitch("A4", 440);
            pitchesList.Add(pitch);
            pitch = new Pitch("C4 - Middle C", 261.625565);
            pitchesList.Add(pitch);

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
