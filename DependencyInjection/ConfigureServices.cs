using FileManager.Context;
using FileManager.Options;
using FileManager.Repositories.StorageFiles;
using FileManager.Repositories.Users;
using FileManager.Services.Data;
using FileManager.Services.Media;
using FileManager.Services.Tools;
using FileManager.Services.Users;
using Microsoft.EntityFrameworkCore;

namespace FileManager.DependencyInjection
{
    public static class ConfigureServices
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<FileManagerContext>(options =>
                options.EnableSensitiveDataLogging()
                    .UseSqlServer(configuration.GetConnectionString("FileManagerDatabase")));

            RegisterApplicationServices(services);
            RegisterRepositories(services);
            RegisterOptions(services, configuration);
        }

        public static void RegisterApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IDataService, DataService>();
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<IToolService, ToolService>();
            services.AddScoped<IUserService, UserService>();
        }

        public static void RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStorageFileRepository, StorageFileRepository>();
        }

        public static void RegisterOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DataOptions>(configuration.GetSection(nameof(DataOptions)));
            services.Configure<MediaOptions>(configuration.GetSection(nameof(MediaOptions)));
            services.Configure<ToolOptions>(configuration.GetSection(nameof(ToolOptions)));
        }
    }
}
