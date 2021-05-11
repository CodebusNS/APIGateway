using APIGateway.Core.Business;
using APIGateway.Core.DbRepository;
using APIGateway.Core.OcelotAddin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;

namespace APIGateway.Portal
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<GatewayDbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseMySQL(Configuration["MySql:ConnectionString"]);
            }, ServiceLifetime.Scoped);

            services.AddOcelot().AddConfigurationRepository(option =>
            {
                option.AutoUpdate = false;
                option.UpdateInterval = 10 * 1000;
            });

            services.AddScoped<IGatewayContract, GatewayServices>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(opt =>
            {
                opt.MapControllers();
            });

            app.UseGateway().Wait();
        }
    }
}
