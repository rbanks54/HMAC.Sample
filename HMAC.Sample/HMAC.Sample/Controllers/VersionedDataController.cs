using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using HMAC.Sample.Models;

namespace HMAC.Sample.Controllers
{
    /* Best practices for versioning an API
     * 
     * 0. API version != Software version (never, ever. not even once)
     * 1. Keep compatible changes out of names (i.e. avoid versions in URIs)
     * 2. Avoid new major versions, as all client apps will be affected
     * 3. Make changes backward compatible wherever possible
     * 4. Design for forward compatibility (XML and XSDs vs JSON)
     * 
     * See http://pivotallabs.com/api-versioning/ for more info (sample code is in Ruby)
     * Also https://www.mnot.net/blog/2012/12/04/api-evolution
     * 
     */

    /* For our API we will pretend our contact API evolves as follows:
     * V1.0: Just the 'FirstName' & 'LastName' fields
     * V1.1: Added 'Age' field
     * V1.2: Added 'Address' field
     * v2.0: Replaced 'FirstName' & 'LastName' fields with a 'FullName' field
     */

    public class VersionedDataController : ApiController
    {
        public Contact LoadAContactFromTheDatabase()
        {
            var contact = new Contact()
            {
                FirstName = "Bruce",
                LastName = "Wayne",
                Address = new Address()
                {
                    Line1 = "Wayne Manor",
                    City = "Gotham",
                    PostCode = "91131"
                }
            };
            return contact;
        }

        // GET: api/VersionedData
        [ResponseType(typeof(ContactViewModel))]
        public virtual IHttpActionResult Get()
        {
            var contact = LoadAContactFromTheDatabase();
            var contactViewModel = new ContactViewModel()
            {
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Age = contact.Age,
                Address = contact.Address
            };
            return Ok(contactViewModel);
        }

        // GET: api/VersionedData/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/VersionedData
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/VersionedData/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/VersionedData/5
        public void Delete(int id)
        {
        }
    }
}
