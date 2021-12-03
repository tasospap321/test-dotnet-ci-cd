using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWebApplication1.Tests
{
    [TestFixture]
    public class TestTest
    {
        [OneTimeSetUp]
        public void TestSetup()
        {

        }

        [Test]
        public void Test1()
        {
            Assert.True(true);
        }
    }
}
