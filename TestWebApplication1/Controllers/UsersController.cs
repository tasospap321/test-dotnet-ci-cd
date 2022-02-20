using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TestWebApplication1.Models;
using TestWebApplication1.DTOs;

namespace TestWebApplication1.Controllers
{
    public class UsersController : ApiController
    {

        private string x = "123";

        private TestWebApplicationDbEntities _context;

        public UsersController()
        {
            _context = new TestWebApplicationDbEntities();
        }

        public UsersController(TestWebApplicationDbEntities context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        [ActionName("getusers")]
        public IHttpActionResult/* IEnumerable<User>*/ GetUsers()
        {
            List<User> users = _context.Users.Select(x => x).ToList();

            return Ok(users);
        }

        // GET: api/Users/5
        public User Get(int id)
        {
            return _context.Users.Where(x => x.Id == id).ToList().FirstOrDefault();
        }

        // POST: api/Users
        [HttpPost]
        [ActionName("register")]
        public async Task<IHttpActionResult> Register([FromBody] RegisterRequest req)
        {
            if (req.Password.Length < 8)
            {
                return BadRequest("PasswordError");
            }

            var reqUser = _context.Users.FirstOrDefault(x => x.Email == req.Email);


            if (reqUser != null)
            {
                return BadRequest("UserAlreadyExists");
            }

            User user = new User()
            {
                Email = req.Email,
                Firstname = req.Firstname,
                Lastname = req.Lastname,
                Password = req.Password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new BasicResponse()
            {
                Message = "RegistrationSuccess",
                Result = user
            });
        }

        // PUT: api/Users/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Users/5
        public void Delete(int id)
        {
        }
    }
}
