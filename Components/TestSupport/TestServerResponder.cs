using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace TestSupport
{
        public sealed class TestServerResponder : IStartup
        {
            private readonly string _response;

            public TestServerResponder(string response)
            {
                _response = response;
            }

            public IServiceProvider ConfigureServices(IServiceCollection services) =>
                services.BuildServiceProvider();

            public void Configure(IApplicationBuilder app)
            {
                app.Run(context =>
                {
                    context.Response.ContentLength = _response.Length;
                    return context.Response.WriteAsync(_response);
                });
            }
        }
}