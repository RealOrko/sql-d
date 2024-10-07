using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SqlD.Configuration;
using SqlD.Configuration.Model;
using SqlD.Network;

namespace SqlD.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = SqlDConfig.Get(typeof(Startup).Assembly);

            services.AddSingleton(configuration);
            services.AddSingleton(EndPoint.FromUri("http://localhost:5000"));
            services.AddSingleton(x => SqlDStart.NewDb().ConnectedTo("sql-d/ui", "sql-d.ui.db", SqlDPragmaModel.Default));

            services.AddControllersWithViews();
            
            services.AddOpenApiDocument(settings =>
            {
                settings.DocumentName = "v1";
                settings.Title = "[ sql-d/ui ]";
                settings.Version = "1.0.0";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(opts =>
            {
                opts.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseOpenApi();
            app.UseSwaggerUi();
        }
    }
}
