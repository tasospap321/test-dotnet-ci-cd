using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using TestWebApplication1.Controllers;
using TestWebApplication1.DTOs;
using TestWebApplication1.Models;

namespace TestWebApplication1.Tests
{
    [TestFixture]
    //[Ignore("Ignore until there is a live Db")]
    public class UsersTest : CommonTest
    {

        private IQueryable<User> MockUsers;
        private Mock<TestWebApplicationDbEntities> MockContext;
        private UsersController usersController;

        [OneTimeSetUp]
        public void Setup()
        {
            /* Get users from Db */
            bool offline = true;
            DataTable data = GetFromDb(offline);
            /* DataTable to list */
            List<User> dataList = new List<User>();
            foreach (DataRow row in data.Rows)
            {
                dataList.Add(new User()
                {
                    Email = Convert.ToString(row["Email"]),
                    Firstname = Convert.ToString(row["Firstname"]),
                    Id = Convert.ToInt32(row["Id"]),
                    Lastname = Convert.ToString(row["Lastname"]),
                    Password = Convert.ToString(row["Password"])
                });
            }
            /* Initialize data */
            MockUsers = dataList.AsQueryable();

            /* Create Mock context */
            var mockSet = new Mock<DbSet<User>>();

            mockSet.As<IQueryable<User>>().Setup(x => x.Provider).Returns(MockUsers.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(MockUsers.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(MockUsers.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(MockUsers.GetEnumerator());

            MockContext = new Mock<TestWebApplicationDbEntities>();
            MockContext.Setup(x => x.Users).Returns(mockSet.Object);

            /* Initialize controller */
            usersController = new UsersController(MockContext.Object)
            {
                Request = new System.Net.Http.HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };
        }

        [Test]
        [TestCase(TestName = "GetUsers.OK")]
        public void GetUsersTest()
        {
            IHttpActionResult response = usersController.GetUsers();
            Assert.NotNull(response);
            var result = response as OkNegotiatedContentResult<List<User>>;
            Assert.Multiple(() =>
            {
                Assert.NotNull(result.Content);
                CollectionAssert.IsNotEmpty(result.Content);
                CollectionAssert.AreEqual(MockUsers, result.Content);
            });

        }

        [Test]
        [TestCase(0, TestName = "Get.OK")]
        [TestCase(1, TestName = "Get.OK")]
        [TestCase(2, TestName = "Get.OK")]
        [TestCase(3, TestName = "Get.OK")]
        public void GetTest(int id)
        {
            User responseUser = usersController.Get(id);
            Assert.NotNull(responseUser);
            Assert.Multiple(() =>
            {
                Assert.AreEqual(id, responseUser.Id);
            });
        }

        [Test, Order(1)]
        [TestCase(8, TestName = "RegisterUser.RegistrationSuccess", ExpectedResult = "RegistrationSuccess")]
        [TestCase(5, TestName = "RegisterUser.PasswordError", ExpectedResult = "PasswordError")]
        public async Task<string> TestRegisterUser(int passwordLength)
        {
            RegisterRequest req = new RegisterRequest()
            {
                Email = GetRandomString(7) + "@mail.com",
                Firstname = "unit-test",
                Lastname = "unit-test",
                Password = GetRandomString(passwordLength)
            };

            IHttpActionResult response = await usersController.Register(req);
            Assert.NotNull(response);
            var result = response as OkNegotiatedContentResult<BasicResponse>;
            if (result != null)
            {
                Assert.Multiple(() =>
                {
                    Assert.NotNull(result.Content);
                    Assert.NotNull(result.Content.Result);
                    Assert.AreEqual("RegistrationSuccess", result.Content.Message);
                });
                return result.Content.Message;
            }
            else
            {
                return "PasswordError";
            }

        }
    }
}
