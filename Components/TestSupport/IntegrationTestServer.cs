using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace TestSupport
{
    public static class IntegrationTestServer
    {
        public static TestServer Start(string response, int port)
        {
            var url = $"http://localhost:{port}/";

            var testServer = new TestServer(
                WebHost.CreateDefaultBuilder(new string[] { })
                    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/hosting?tabs=aspnetcore2x#troubleshooting-systemargumentexception
                    .Configure(_ => { })
                    .ConfigureServices(services => services.AddSingleton<IStartup>(new TestServerResponder(response)))
                    .UseUrls(url)
            ) {BaseAddress = new Uri(url)};

            return testServer;
        }
    }
}