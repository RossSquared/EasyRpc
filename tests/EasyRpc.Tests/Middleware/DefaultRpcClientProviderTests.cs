﻿using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EasyRpc.DynamicClient;
using Xunit;

namespace EasyRpc.Tests.Middleware
{
    public class DefaultRpcClientProviderTests
    {
        [Fact]
        public async Task DefaultRpcClientProviderTest()
        {
            using (var provider = new DefaultRpcClientProvider("http://google.com"))
            {
                var client = provider.GetHttpClient("SomeNamespace", "Test");

                var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, ""));

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
