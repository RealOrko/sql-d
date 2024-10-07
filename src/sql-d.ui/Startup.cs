using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlD.UI.Services;
using SqlD.UI.Services.Client;
using SqlD.UI.Services.Query;

namespace SqlD.UI;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews(c => c.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
        services.AddResponseCompression();

        services.AddSingleton<QueryService>();
        services.AddSingleton<RegistryService>();
        services.AddSingleton<ServiceService>();
        services.AddSingleton<SurfaceService>();
        services.AddSingleton<ClientFactory>();
        services.AddSingleton<UnknownAction>();
        services.AddSingleton<DescribeAction>();
        services.AddSingleton<CommandAction>();
        services.AddSingleton<QueryAction>();

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
                "default",
                "{controller=Home}/{action=Index}/{id?}");
        });

        app.UseOpenApi();
        app.UseSwaggerUi();
    }
}