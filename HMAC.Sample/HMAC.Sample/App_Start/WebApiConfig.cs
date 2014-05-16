using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using HMAC.Authorization;
using HMAC.Sample.Controllers;
using Newtonsoft.Json.Serialization;

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
                name: "SecureImportApi",
                routeTemplate: "api/hmacimportdata",
                constraints: null,
                handler: new HmacAuthenticationHandler(new ApiKeyRepository(),
                    new CanonicalRepresentationBuilder(), new HmacSignatureCalculator())
                {
                    InnerHandler = new HttpControllerDispatcher(config)
                },
                defaults: new { controller = "hmacimportdata" }
            );


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            /*
             * 
             * MIME TYPES AND CONTENT NEGOTATION STUFF
             * Should move this to a different method, but since it's a sample app.... well, yaknow...
             * 
             */

            //Drop XML as a content type since forward compatibility with it is a pain in the butt thanks to XML schemas
            var applicationXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes
               .FirstOrDefault(p => p.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(applicationXmlType);

            var xmlFormatter = config.Formatters.XmlFormatter;
            config.Formatters.Remove(xmlFormatter);

            //Set JSON output to camelCase identifiers to conform with standard JS conventions
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            CreateJsonMediaTypes(jsonFormatter);

            //BSON is supported in WebAPI v2.1+
            var bsonFormatter = new BsonMediaTypeFormatter();
            config.Formatters.Add(bsonFormatter);

            //Binary JSON serialisation - added in v2 of the API
            //Why? Maybe we wanted to include an embedded jpeg of the contact in the response 
            //so there are less server requests.
            bsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.bccAdsystems.contact+bson"));
            bsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.bccAdsystems.contact.v2+bson"));

            //Custom code to configure which controller is called
            config.Services.Replace(typeof(IHttpControllerSelector), new ApiVersioningSelector(config));

            config.MessageHandlers.Add(new NotAcceptableContentNegotiationHandler(config));

            config.Filters.Add(new CustomApiExceptionAttribute());
        }

        private static void CreateJsonMediaTypes(JsonMediaTypeFormatter jsonFormatter)
        {
            //You can publicly register your media types if you really want to.
            //Vendor specific media types http://www.iana.org/cgi-bin/mediatypes.pl
            var mediaTypes = new List<string> {
                "application/vnd.bccAdsystems.contact", //non-specific version - effectively v1.
                "application/vnd.bccAdsystems.contact+json", //non-specific version - specific content type.
                "application/vnd.bccAdsystems.contact.v1+json", //specific version and format type
                "application/vnd.bccAdsystems.contact.v2", 
                "application/vnd.bccAdsystems.contact.v2+json",
                "application/vnd.bccAdsystems.fred.v2+json"
            };

            //To make maintenance easier it wouldn't take much to programatically generate the media types
            //with different extensions.
            //Could also use reflection to indicate what 'resource' representations are allowed on a route
            //And check that in the ApiVersioningSelector class.

            foreach (var mediaType in mediaTypes)
            {
                jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue(mediaType));
            }
        }
    }
}
