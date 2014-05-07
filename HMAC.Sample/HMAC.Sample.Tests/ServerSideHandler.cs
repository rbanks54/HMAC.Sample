using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
        public void Should_accept_a_valid_request()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "http://myApi/api/somePath");
            request.Headers.Date = new DateTimeOffset(DateTime.UtcNow);
            request.Content = new StringContent("{'A':'b'}");
            request.Headers.Authorization = new AuthenticationHeaderValue(Configuration.AuthenticationScheme,
                string.Format("{0}:{1}","1234","bad hash value"));
            request.Content.Headers.ContentMD5 = Encoding.UTF8.GetBytes("anotherBadHash");

            var context = Substitute.For<HttpContextBase>();
            var innerhandler = new FakeInnerHandler();
            innerhandler.Message = new HttpResponseMessage(HttpStatusCode.OK);
            var client = new HttpMessageInvoker(new HmacAuthenticationHandler(
                        new SecretRepository(), 
                        new CanonicalRepresentationBuilder(),
                        new HmacSignatureCalculator())
            {
                InnerHandler = innerhandler
            });

            var message = client.SendAsync(request, new CancellationToken(false)).Result;
            Assert.AreEqual(message.StatusCode, HttpStatusCode.OK);
        }

        public class FakeInnerHandler : DelegatingHandler
        {
            public HttpResponseMessage Message { get; set; }
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (Message == null)
                {
                    return base.SendAsync(request, cancellationToken);
                }
                return Task.Factory.StartNew(() => Message);
            }
        }

    }
}
