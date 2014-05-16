using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace HMAC.Sample
{
    public class ApiException : ApplicationException
    {
        public ApiException()
        {
            ImportErrors = new Dictionary<string, string>();
        }

        public IDictionary<string,string> ImportErrors { get; private set; }
    }

    public class CustomApiExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is ApiException)
            {
                var request = context.ActionContext.Request;
                var ex = context.Exception as ApiException;
                context.Response = request.CreateResponse(HttpStatusCode.BadRequest, ex.ImportErrors);
                context.Response.ReasonPhrase = "Error thrown because current time has an even seconds value";
            }
        }
    }
}