using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Description;
using HMAC.Sample.Models;

namespace HMAC.Sample.Controllers
{
    /* 
     * We're only going to support GET for this sample code
     */

    //Could add a header such as [VndResourceType("contact")] and use that in ApiVersioningSelector
    //to ensure that the correct vendor type is used for this controller
    public class VersionedDataV2Controller : ApiController
    {
        private Contact LoadAContactFromTheDatabase()
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
        [ResponseType(typeof(ContactViewModel_v2))]
        public IHttpActionResult Get()
        {
            var contact = LoadAContactFromTheDatabase();
            var contactViewModel = new ContactViewModel_v2()
            {
                FullName = contact.FullName,
                Age = contact.Age,
                Address = contact.Address
            };
            return Ok(contactViewModel);
        }
    }
}
