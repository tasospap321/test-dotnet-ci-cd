using NUnit.Framework;

namespace TestWebApplication1.Tests
{
    public class Tests : CommonTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            Assert.True(true);
        }

        [Test]
        public void Test_ValuesController_Get()
        {
            var response = valuesController.Get();
            Assert.NotNull(response);
        }
    }
}