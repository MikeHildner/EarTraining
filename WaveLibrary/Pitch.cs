using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveLibrary
{
    public class Pitch
    {
        public string PitchName { get; set; }
        public double Hertz { get; set; }

        public Pitch() { }
        public Pitch(string pitchName, double hertz)
        {
            this.PitchName = pitchName;
            this.Hertz = hertz;
        }
    }
}
