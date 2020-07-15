using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EarTrainingLibrary.Utility
{
    public class PhraseGenerationException : Exception
    {
        public PhraseGenerationException() { }

        public PhraseGenerationException(string message) : base(message) { }
    }
}
