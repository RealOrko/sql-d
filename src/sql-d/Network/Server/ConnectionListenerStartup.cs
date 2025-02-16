using Microsoft.AspNetCore.Http.Features;
using OpenTelemetry.Metrics;
using SqlD.Network.Server.Middleware;
using SqlD.Network.Server.Workers;

namespace SqlD.Network.Server;

internal class ConnectionListenerStartup
{
    internal static ConnectionListener Listener;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder.AddPrometheusExporter();

                builder.AddMeter("Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel");
                builder.AddView("http.server.request.duration",
                    new ExplicitBucketHistogramConfiguration
                    {
                        Boundaries = [0, 0.005, 0.01, 0.025, 0.05, 0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10]
                    });
            });
        
        services.Configure<FormOptions>(options =>
        {
            options.BufferBody = false;
            options.KeyLengthLimit = int.MaxValue;
            options.ValueLengthLimit = int.MaxValue;
            options.ValueCountLimit = int.MaxValue;
            options.MultipartHeadersCountLimit = int.MaxValue;
            options.MultipartHeadersLengthLimit = int.MaxValue;
            options.MultipartBoundaryLengthLimit = 256;
            options.MultipartBodyLengthLimit = int.MaxValue; // 128 MiB
        });

        services.AddSingleton(Listener.DbConnectionFactory);
        services.AddSingleton(Listener.ServiceModel);
        services.AddSingleton((EndPoint)Listener.ServiceModel);
        services.AddSingleton<SynchronisationWorkerQueue>();
        services.AddHostedService<SynchronisationWorker>();
        services.AddHostedService<AutoSynchronisationWorker>();

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
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
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