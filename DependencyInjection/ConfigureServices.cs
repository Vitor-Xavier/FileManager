using FileManager.Options;
using FileManager.Services.Data;
using FileManager.Services.Media;

namespace FileManager.DependencyInjection
{
    public static class ConfigureServices
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            RegisterApplicationServices(services);
            RegisterRepositories(services);
            RegisterOptions(services, configuration);
        }

        public static void RegisterApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IMediaService, MediaService>();
        }

        public static void RegisterRepositories(IServiceCollection services)
        {

        }

        public static void RegisterOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DataOptions>(configuration.GetSection(nameof(DataOptions)));
            services.Configure<MediaOptions>(configuration.GetSection(nameof(MediaOptions)));
        }
    }
}
