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
    public class ImportDataController : ApiController
    {
        private ApiException exceptionToThrow = new ApiException();

        public ImportDataController()
        {
            var scheduleErrors = new Dictionary<string, object>();
            scheduleErrors.Add("66412","Invalid start date");
            var bookingResults = new Dictionary<string, object>();
            bookingResults.Add("55827","Invalid start date");
            var splitResults = new Dictionary<string, string>();
            splitResults.Add("1223", "Missing Insertion Rate for Bill / Pay");
            splitResults.Add("1224","OK");
            splitResults.Add("1225","OK");
            splitResults.Add("1226","OK");
            splitResults.Add("1227","OK");
            splitResults.Add("1228", "Missing Insertion Rate for Bill / Pay");
            bookingResults.Add("Split Results",splitResults);
            scheduleErrors.Add("Booking Results",bookingResults);

            exceptionToThrow.ImportErrors.Add("Schedules",scheduleErrors);
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
