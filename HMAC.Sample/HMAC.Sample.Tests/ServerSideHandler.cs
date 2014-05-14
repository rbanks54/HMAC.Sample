using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using HMAC.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace HMAC.Sample.Tests
{
    [TestClass]
    public class ServerSideHandler
    {
        [TestMethod]
        public void Should_reject_an_invalid_request()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://myApi/api/somePath");
            request.Headers.Date = new DateTimeOffset(DateTime.UtcNow);
            request.Content = new StringContent("{'A':'b'}");
            request.Headers.Authorization = new AuthenticationHeaderValue(Configuration.AuthenticationScheme,
                string.Format("{0}:{1}", "1234", "bad hash value"));

            request.Content.Headers.ContentMD5 = Encoding.UTF8.GetBytes("anotherBadHash");

            var innerhandler = new FakeInnerHandler
            {
                Message = new HttpResponseMessage(HttpStatusCode.OK)
            };
            var client = new HttpMessageInvoker(new HmacAuthenticationHandler(
                        new SecretRepository(),
                        new CanonicalRepresentationBuilder(),
                        new HmacSignatureCalculator())
            {
                InnerHandler = innerhandler
            });

            var message = client.SendAsync(request, new CancellationToken(false)).Result;
            Assert.AreEqual(message.StatusCode, HttpStatusCode.Unauthorized);
        }

        [TestMethod]
        public void Should_accept_a_valid_request()
        {
            var uri = "/api/somepath";
            var request = new HttpRequestMessage(HttpMethod.Post, "http://myApi" + uri);
            request.Headers.Date = new DateTimeOffset(DateTime.UtcNow);
            var content = "{'A':'b'}";
            request.Content = new StringContent(content);
            request.Content.Headers.ContentMD5 = CalculateHash(content);

            var userId = "1234";
            var md5 = Convert.ToBase64String(request.Content.Headers.ContentMD5);

            var hashedApiKey = ComputeSHA1("v87o2jh388d");
            var representation =   String.Join("\n", "POST",
                md5, 
                request.Headers.Date.Value.UtcDateTime.ToString(CultureInfo.InvariantCulture),
                "1234", 
                uri);

            var signatureCaclulator = new HmacSignatureCalculator();
            var messageSignature = signatureCaclulator.Signature(hashedApiKey, representation);

            request.Headers.Authorization = new AuthenticationHeaderValue(
                Configuration.AuthenticationScheme,
                string.Format("{0}:{1}", userId, messageSignature)
            );

            var context = Substitute.For<HttpContextBase>();
            var innerhandler = new FakeInnerHandler();
            innerhandler.Message = new HttpResponseMessage(HttpStatusCode.OK);
            var client = new HttpMessageInvoker(new HmacAuthenticationHandler(
                        new SecretRepository(),
                        new CanonicalRepresentationBuilder(),
                        signatureCaclulator)
            {
                InnerHandler = innerhandler
            });

            var message = client.SendAsync(request, new CancellationToken(false)).Result;
            Assert.AreEqual(message.StatusCode, HttpStatusCode.OK);

        }

        public byte[] CalculateHash(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            return hash;
        }

        private string ComputeSHA1(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashed = new SHA1CryptoServiceProvider().ComputeHash(inputBytes);
            return Convert.ToBase64String(hashed);
        }

    }
}
