using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using Microsoft.Ajax.Utilities;

namespace HMAC.Sample
{
    public class ApiException : ApplicationException
    {
        public ApiException()
        {
            ImportErrors = new Dictionary<string, object>();
        }

        public IDictionary<string,object> ImportErrors { get; private set; }
        public ImportResult ImportResult { get; set; }
    }

    public class CustomApiExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is ApiException)
            {
                var request = context.ActionContext.Request;
                var ex = context.Exception as ApiException;
                if (ex.ImportErrors.Count > 0)
                    context.Response = request.CreateResponse(HttpStatusCode.BadRequest, ex.ImportErrors);
                else
                    context.Response = request.CreateResponse(HttpStatusCode.BadRequest, ex.ImportResult);
                context.Response.ReasonPhrase = "Error thrown because current time has an even seconds value";
            }
        }
    }

    public class ImportResult
    {
        public ImportResult(int id, string errorMessage = null)
        {
            ID = id;
            WasSuccessful = string.IsNullOrWhiteSpace(errorMessage);
            ErrorMessage = errorMessage;
        }

        public int ID { get; set; }
        public bool WasSuccessful { get; set; }
        public String ErrorMessage { get; set; }
        public IList<ImportResult> InnerResults { get; set; }
    }
}