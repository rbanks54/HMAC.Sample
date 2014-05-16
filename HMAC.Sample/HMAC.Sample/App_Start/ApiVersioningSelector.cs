//needed for .GetSubRoutes() extension method
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace HMAC.Sample
{
    public class ApiVersioningSelector : DefaultHttpControllerSelector
    {
        private HttpConfiguration httpConfiguration;
        public ApiVersioningSelector(HttpConfiguration httpConfiguration)
            : base(httpConfiguration)
        {
            this.httpConfiguration = httpConfiguration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            HttpControllerDescriptor controllerDescriptor = null;

            // get list of all controllers provided by the default selector
            var controllers = GetControllerMapping();

            var routeData = request.GetRouteData();

            if (routeData == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            //check if this route is actually an attribute route
            var attributeSubRoutes = routeData.GetSubRoutes();

            var apiVersion = ReadVersionFromMediaType(request);

            //If no attribute route specified ie [Route("xyz")] is not present...
            if (attributeSubRoutes == null)
            {
                var controllerName = GetRouteVariable<string>(routeData, "controller");
                if (controllerName == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                if (apiVersion != "1")
                {
                    //Convention - versioned controllers are named "{controllerName}V{apiVersion}Controller"
                    controllerName = String.Concat(controllerName, "V", apiVersion);
                }

                if (controllers.TryGetValue(controllerName, out controllerDescriptor))
                {
                    if (IsAcceptedMediaType(controllerDescriptor, request))
                        return controllerDescriptor;
                    throw new HttpResponseException(HttpStatusCode.NotAcceptable);
                }
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            // If we're using a [Route("xyz")] attribute we want to find all controller descriptors
            // whose controller type names end with the Vx suffix

            var newControllerNameSuffix = String.Concat("V", apiVersion);
            if (apiVersion != "1") newControllerNameSuffix = "";

            var filteredSubRoutes = attributeSubRoutes.Where(attrRouteData =>
                {
                    var currentDescriptor = GetControllerDescriptor(attrRouteData);

                    var match = currentDescriptor.ControllerName.EndsWith(newControllerNameSuffix);

                    if (match && (controllerDescriptor == null))
                    {
                        controllerDescriptor = currentDescriptor;
                    }

                    return match;
                });

            routeData.Values["MS_SubRoutes"] = filteredSubRoutes.ToArray();

            return controllerDescriptor;
        }

        private HttpControllerDescriptor GetControllerDescriptor(IHttpRouteData routeData)
        {
            return ((HttpActionDescriptor[])routeData.Route.DataTokens["actions"]).First().ControllerDescriptor;
        }

        // Get a value from the route data, if present.
        private static T GetRouteVariable<T>(IHttpRouteData routeData, string name)
        {
            object result = null;
            if (routeData.Values.TryGetValue(name, out result))
            {
                return (T)result;
            }
            return default(T);
        }


        //Accept: application/vnd.bccAdsystems.{yourresource}[.v{version}[+json|+bson]]
        //The regex: application\/vnd\.bccAdsystems\.([a-z]+)(\.v([0-9]+))*(\+json|\+bson)*
        //was manually tested at http://regexpal.com/
        private string ReadVersionFromMediaType(HttpRequestMessage request)
        {
            var acceptHeader = request.Headers.Accept;
            if (acceptHeader.Count == 0)
                return "1";
            
            var v1Headers = new[] {@"application/json", @"text/json"};
            var v2Headers = new[] { @"application/bson"};
            var defaultFormatVersions = new Dictionary<string, string>() {{"+json", "1"}, {"+bson", "2"}};

            var regularExpression = new Regex(@"application\/vnd\.bccAdsystems\.([a-z]+)(\.v([0-9]+))*(\+json|\+bson)*",
                RegexOptions.IgnoreCase);

            foreach (var mime in acceptHeader)
            {
                if (v1Headers.Contains(mime.MediaType)) return "1";
                if (v2Headers.Contains(mime.MediaType)) return "2";

                var match = regularExpression.Match(mime.MediaType);
                if (match.Success)
                {
                    var apiVersion = match.Groups[3].Value; //check for specific version
                    if (apiVersion != "") 
                        return apiVersion;
                    
                    var format = match.Groups[4].Value;
                    if (defaultFormatVersions.ContainsKey(format))
                        return defaultFormatVersions[format];
                }
            }
            return "1";
        }

        private bool IsAcceptedMediaType(HttpControllerDescriptor controllerDescriptor, HttpRequestMessage request)
        {
            var cxName = controllerDescriptor.ControllerName;
            var acceptHeader = request.Headers.Accept;
            if (acceptHeader.Count == 0)
                return true;

            var alwaysAccepted = new[] { "application/json", "text/json", "application/bson", "*/*" };

            var regularExpression = new Regex(@"application\/vnd\.bccAdsystems\.([a-z]+).*",
                RegexOptions.IgnoreCase);

            foreach (var mime in acceptHeader)
            {
                if (alwaysAccepted.Contains(mime.MediaType)) return true;

                var match = regularExpression.Match(mime.MediaType);
                if (match.Success)
                {
                    var resource = match.Groups[1].Value;
                    if (resource == "") return false;

                    //for this sample we want to always check that we're using 'contact'
                    return cxName.StartsWith("versioneddata",StringComparison.InvariantCultureIgnoreCase)
                        && resource.ToLowerInvariant().Equals("contact",StringComparison.InvariantCultureIgnoreCase);
                }
            }
            return false;
        }


    }
}