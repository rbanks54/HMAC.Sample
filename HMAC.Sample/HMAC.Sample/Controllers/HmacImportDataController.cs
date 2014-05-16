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
            var result = new ImportResult(66412, "Invalid start date");
            var bookingResults = new ImportResult(55827, "Invalid start date");
            bookingResults.InnerResults = new ImportResult[]
            {
                new ImportResult(1223, "Missing Insertion Rate for Bill / Pay"),
                new ImportResult(1224),
                new ImportResult(1225),
                new ImportResult(1226),
                new ImportResult(1227),
                new ImportResult(1228, "Missing Insertion Rate for Bill / Pay")
            };
            result.InnerResults = new []{bookingResults};

            exceptionToThrow.ImportResult = result;
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
