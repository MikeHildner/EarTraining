using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarTrainingLibrary.Utility
{
    public class Solfeg
    {
        private readonly double _doFrequency;

        public Solfeg(double doFrequency)
        {
            _doFrequency = doFrequency;
        }

        public double DoFrequency
        {
            get
            {
                return _doFrequency;
            }
        }

        public double ReFrequency
        {
            get
            {
                double reFrequency = GetFrequencyByHalfStepsFromDo(2);
                return reFrequency;
            }
        }

        public double MiFrequency
        {
            get
            {
                double reFrequency = GetFrequencyByHalfStepsFromDo(4);
                return reFrequency;
            }
        }

        public double FaFrequency
        {
            get
            {
                double reFrequency = GetFrequencyByHalfStepsFromDo(5);
                return reFrequency;
            }
        }

        public double SoFrequency
        {
            get
            {
                double reFrequency = GetFrequencyByHalfStepsFromDo(7);
                return reFrequency;
            }
        }

        public double LaFrequency
        {
            get
            {
                double reFrequency = GetFrequencyByHalfStepsFromDo(9);
                return reFrequency;
            }
        }

        public double TiFrequency
        {
            get
            {
                double reFrequency = GetFrequencyByHalfStepsFromDo(11);
                return reFrequency;
            }
        }

        public double HighDoFrequency
        {
            get
            {
                double reFrequency = GetFrequencyByHalfStepsFromDo(12);
                return reFrequency;
            }
        }

        public double LowTiFrequency
        {
            get
            {
                return GetFrequencyByHalfStepsFromDo(-1);
            }
        }

        public double GetFrequencyByHalfStepsFromDo(int halfStepsFromDo)
        {
            double frequencyRatio = Math.Pow(2, Math.Abs(halfStepsFromDo) / 12d);
            double thisFrequency;

            if (halfStepsFromDo >= 0)
            {
                thisFrequency = _doFrequency * frequencyRatio;
                return thisFrequency;
            }
            else
            {
                thisFrequency = _doFrequency / frequencyRatio;
                return thisFrequency;
            }
        }
    }
}
