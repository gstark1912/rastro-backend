using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rastro.Application.Abstractions;
using Rastro.Domain;

namespace Rastro.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserScopedCrudService<Project>, UserScopedCrudService<Project>>();
            services.AddScoped<IUserScopedCrudService<Marker>, UserScopedCrudService<Marker>>();
            return services;
        }
    }
}
