using NUnit.Framework;

namespace TestWebApplication1.Tests
{
    public class ValuesTest : CommonTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(TestName = "Test_ValuesController_Get")]
        public void Test_ValuesController_Get()
        {
            var response = valuesController.Get();
            Assert.NotNull(response);
        }

        [Test]
        [TestCase("value3", TestName = "Test_ValuesController_Post.value3")]
        [TestCase("value4", TestName = "Test_ValuesController_Post.value4")]
        [TestCase("value5", TestName = "Test_ValuesController_Post.value5")]
        [TestCase("value6", TestName = "Test_ValuesController_Post.value6")]
        [TestCase("value7", TestName = "Test_ValuesController_Post.value7")]
        [TestCase("value8", TestName = "Test_ValuesController_Post.value8")]
        public void Test_ValuesController_Post(string value)
        {
            var response = valuesController.Post(value);
            Assert.NotNull(response);
        }
    }
}