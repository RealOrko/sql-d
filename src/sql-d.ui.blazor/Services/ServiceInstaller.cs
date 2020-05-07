using Microsoft.Extensions.DependencyInjection;
using SqlD.UI.Services.Client;
using SqlD.UI.Services.Query;

namespace SqlD.UI.Services
{
    public static class ServiceInstaller
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddClient();
            services.AddQuery();
            services.AddTransient<ConfigService>();
            services.AddTransient<ContextService>();
            services.AddTransient<QueryService>();
            services.AddTransient<RegistryService>();
            services.AddTransient<ApplicationService>();
            services.AddTransient<SurfaceService>();
            services.AddTransient<StorageService>();
            services.AddSingleton<EventService>();
        }
    }
}