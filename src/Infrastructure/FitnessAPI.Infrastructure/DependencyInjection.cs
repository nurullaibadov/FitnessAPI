using FitnessAPI.Application.Interfaces.Services;
using FitnessAPI.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FitnessAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
