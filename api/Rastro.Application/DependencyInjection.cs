using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rastro.Application.Abstractions;

namespace Rastro.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
