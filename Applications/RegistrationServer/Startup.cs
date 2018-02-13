using Accounts;
using DatabaseSupport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pivotal.Discovery.Client;
using Projects;
using Users;

namespace RegistrationServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddSingleton<IDataSourceConfig, VcapDataSourceConfig>();
            services.AddSingleton<IDatabaseTemplate, DatabaseTemplate>();
            services.AddSingleton<IAccountDataGateway, AccountDataGateway>();
            services.AddSingleton<IProjectDataGateway, ProjectDataGateway>();
            services.AddSingleton<IUserDataGateway, UserDataGateway>();
            services.AddSingleton<IRegistrationService, RegistrationService>();

            services.AddDiscoveryClient(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
            app.UseDiscoveryClient();
        }
    }
}