using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using HMAC.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HMAC.Sample.Tests
{
    [TestClass]
    public class SigningRequests
    {
        [TestMethod]
        public void Can_create_a_signed_message_using_the_helper_functions()
        {
            //TODO: Get this working (need to check sig is calculated as expected)
            //var signingHandler = new HmacSigningHandler(new ApiKeyRepository(),
            //    new CanonicalRepresentationBuilder(),
            //    new HmacSignatureCalculator());
            //signingHandler.UserId = "1234";

            //var client = new HttpClient(new RequestContentMd5Handler()
            //{
            //    InnerHandler = signingHandler
            //});

            //var response = client.PostAsJsonAsync("http://localhost:12036/api/values", "some content").Result;
            //var result = response.Content.ReadAsAsync<string>().Result;


            //var uri = "/api/somepath";
            //var request = new HttpRequestMessage(HttpMethod.Post, "http://myApi" + uri);
            //request.Headers.Date = new DateTimeOffset(DateTime.UtcNow);
            //var content = "{'A':'b'}";
            //request.Content = new StringContent(content);
            //request.Content.Headers.ContentMD5 = CalculateHash(content);

            //var userId = "1234";
            //var md5 = Convert.ToBase64String(request.Content.Headers.ContentMD5);

            //var signatureCaclulator = new HmacSignatureCalculator();
            //var messageSignature = signatureCaclulator.Signature(hashedApiKey, representation);

            //request.Headers.Authorization = new AuthenticationHeaderValue(
            //    Configuration.AuthenticationScheme,
            //    string.Format("{0}:{1}", userId, messageSignature)
            //);

            //var innerhandler = new FakeInnerHandler();
            //innerhandler.Message = new HttpResponseMessage(HttpStatusCode.OK);
            //var client = new HttpMessageInvoker(new HmacAuthenticationHandler(
            //            new ApiKeyRepository(),
            //            new CanonicalRepresentationBuilder(),
            //            signatureCaclulator)
            //{
            //    InnerHandler = innerhandler
            //});

            //var message = client.SendAsync(request, new CancellationToken(false)).Result;
            //Assert.AreEqual(message.StatusCode, HttpStatusCode.OK);


        }
    }
}
