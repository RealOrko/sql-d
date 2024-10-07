using SqlD.Network.Server.Middleware;

namespace SqlD.Network.Server;

internal class ConnectionListenerStartup
{
    internal static ConnectionListener Listener;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Listener.DbConnectionFactory);
        services.AddSingleton(Listener.ServiceModel);
        services.AddSingleton(Listener.ServiceModel as EndPoint);

        services.AddCors();
        services.AddControllersWithViews(c => c.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true).AddNewtonsoftJson();
        services.AddResponseCompression();

        services.AddOpenApiDocument(settings =>
        {
            settings.DocumentName = "v1";
            settings.Title = "[ sql-d ]";
            settings.Version = "1.0.0";
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseStaticFiles();

        var middleware = new ForwardingMiddleware(Listener);
        app.Use(async (ctx, next) => await middleware.InvokeAsync(ctx, next));

        app.UseResponseCompression();
        app.UseRouting();
        app.UseCors(x => x.AllowAnyOrigin());
        app.UseEndpoints(opts => { opts.MapControllers(); });

        app.UseOpenApi();
        app.UseSwaggerUi();
    }
}