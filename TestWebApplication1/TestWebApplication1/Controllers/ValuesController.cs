using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace TestWebApplication1.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpGet]
        [ActionName("getvalues")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        [ActionName("getvaluebyid")]
        public string Get([FromUri] int id)
        {
            return "value";
        }

        [HttpPost]
        [ActionName("postvalue")]
        public void Post([FromBody] string value)
        {
        }

        [HttpGet]
        [ActionName("putvalue")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpGet]
        [ActionName("deletevaluebyid")]
        public void Delete(int id)
        {
        }
    }
}
