using System.Reflection;
using SqlD.Configuration;
using SqlD.Configuration.Model;
using SqlD.Network.Server.Middleware;

namespace SqlD.Network.Server
{
    internal class ConnectionListenerStartup
    {
        public static Assembly StartAssembly;
        public static EndPoint ListenerAddress;
        public static DbConnection DbConnection;
        public static EndPoint[] ForwardAddresses;
        
        public string DbConnectionName = DbConnection.Name;
        public string DbConnectionDbName = DbConnection.DatabaseName;
        public SqlDPragmaModel PragmaOptions = DbConnection.PragmaOptions;

        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = SqlDConfig.Get(StartAssembly);

            services.AddSingleton(configuration);
            services.AddSingleton(ListenerAddress);
            services.AddSingleton(x => SqlDStart.NewDb().ConnectedTo(DbConnectionName, DbConnectionDbName, PragmaOptions));

            services.AddCors();
            services.AddControllersWithViews().AddNewtonsoftJson();
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

            var middleware = new ForwardingMiddleware(ForwardAddresses);
            app.Use(async (ctx, next) => await middleware.InvokeAsync(ctx, next));

            app.UseResponseCompression();
            
            app.UseRouting();
            app.UseCors(x => x.AllowAnyOrigin());
            app.UseEndpoints(opts =>
            {
                opts.MapControllers();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}