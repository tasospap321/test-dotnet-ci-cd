using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TestWebApplication1.Controllers
{
    public class ValuesController : ApiController
    {
        private List<string> values = new List<string> { "value1", "value2" };

        [HttpGet]
        [ActionName("getvalues")]
        public IEnumerable<string> Get()
        {
            return values;
        }

        [HttpGet]
        [ActionName("getvaluebyid")]
        public string Get([FromUri] int id)
        {
            return values.ElementAt(id);
        }

        [HttpPost]
        [ActionName("postvalue")]
        public List<string> Post([FromBody] string value)
        {
            values.Add(value);
            return values;
        }

        [HttpGet]
        [ActionName("putvalue")]
        public void Put(int id, [FromBody] string value)
        {
            values.Insert(id, value);
        }

        [HttpGet]
        [ActionName("deletevaluebyid")]
        public void Delete(int id)
        {
            values.RemoveAt(id);
        }
    }
}
