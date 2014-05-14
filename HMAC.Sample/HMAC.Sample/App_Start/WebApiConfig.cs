using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using HMAC.Authorization;
using HMAC.Sample.Controllers;

namespace HMAC.Sample
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "SecureApi",
                routeTemplate: "api/values",
                constraints: null,
                handler: new HmacAuthenticationHandler(new ApiKeyRepository(),
                    new CanonicalRepresentationBuilder(), new HmacSignatureCalculator())
                {
                    InnerHandler = new HttpControllerDispatcher(config)
                },
                defaults: new { controller = "values" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
