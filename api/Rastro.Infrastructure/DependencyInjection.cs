using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Rastro.Infrastructure.Abstractions;

namespace Rastro.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddSingleton<IMongoClient>(sp =>
                new MongoClient(config.GetConnectionString("Server")));

            services.AddScoped(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var database = client.GetDatabase(config.GetConnectionString("DbName"));
                return database;
            });

            return services;
        }
    }
}
