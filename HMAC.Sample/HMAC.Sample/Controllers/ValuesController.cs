using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using HMAC.Authorization;

namespace HMAC.Sample.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public async Task<string> Get(int id)
        {
            var signingHandler = new HmacSigningHandler(new ApiKeyRepository(), 
                new CanonicalRepresentationBuilder(),
                new HmacSignatureCalculator());
            signingHandler.UserId = "89s8i2k";

            var client = new HttpClient(new RequestContentMd5Handler()
            {
                InnerHandler = signingHandler
            });

            var result = await client.GetAsync("http://localhost:12036/api/values");
            var json = await result.Content.ReadAsStringAsync();
            
            return "retrieved: " + json;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
