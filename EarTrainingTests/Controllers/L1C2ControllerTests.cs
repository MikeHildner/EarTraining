using Microsoft.VisualStudio.TestTools.UnitTesting;
using EarTraining.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EarTraining.Controllers.Tests
{
    [TestClass()]
    public class L1C2ControllerTests
    {
        [TestMethod()]
        public void AudioAndDictationTest()
        {
            ActionResult result =  new L1C2Controller().AudioAndDictation(1, "C", 60, 2, "Eighth");
        }
    }
}