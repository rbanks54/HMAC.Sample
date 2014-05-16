using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.Ajax.Utilities;

namespace HMAC.Sample.Controllers
{
    public class HmacImportDataController : ApiController
    {
        private ApiException exceptionToThrow = new ApiException();

        public HmacImportDataController()
        {
            exceptionToThrow.ImportErrors.Add("Row1","OK");
            exceptionToThrow.ImportErrors.Add("Row2", "Failed - incorrect time stamp");
            exceptionToThrow.ImportErrors.Add("Row3", "Failed - index out of range");
        }

        public IHttpActionResult Post([FromBody]dynamic data)
        {
            var throwAnError = DateTime.Now.Second % 2 == 0;
            if (throwAnError)
            {
                throw exceptionToThrow;
            }
            return Ok("cool");
        }
    }
}
