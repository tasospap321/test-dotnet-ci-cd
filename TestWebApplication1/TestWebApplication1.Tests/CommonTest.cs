using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWebApplication1.Controllers;

namespace TestWebApplication1.Tests
{
    public class CommonTest
    {

        protected ValuesController valuesController;

        [OneTimeSetUp]
        public void Setup()
        {
            valuesController = new ValuesController();
        }
        [OneTimeTearDown]
        public void Teardown()
        {

        }
    }
}
