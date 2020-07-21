using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Magalu.API;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;

namespace Magalu.Test
{
    public class ClientApi_Tests
    {
        private readonly HttpClient Api;
        public ClientApi_Tests()
        {

            var server = new TestServer(new WebHostBuilder()
                //.UseEnvironment("Developement")
                .UseStartup<Startup>());

            Api = server.CreateClient();
        }

        [Theory]
        [InlineData("raphaelcarlosr@gmail.com", "Raphael Carlos Rego")]
        public async Task Register_Test(string email, string name)
        {
            //var request = new HttpRequestMessage(new HttpMethod("POST"), $"/client");
            var parameters = new Dictionary<string, string> { { "email", email }, { "name", name } };
            var encodedContent = new FormUrlEncodedContent(parameters);
            var response = await Api.PostAsync($"/api/client", encodedContent);
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}