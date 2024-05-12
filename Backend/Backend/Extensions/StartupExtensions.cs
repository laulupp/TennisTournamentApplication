using Backend.Persistence.Context;
using Backend.Services.Interfaces;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Backend.Persistence.Repositories;
using Backend.Persistence.Repositories.Interfaces;

namespace Backend.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration config)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<ITournamentParticipantRepository, TournamentParticipantRepository>();
        services.AddScoped<ITournamentRepository, TournamentRepository>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
    {
        // Application service registrations
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMatchService, MatchService>();
        services.AddScoped<ITournamentService, TournamentService>();
        services.AddSingleton<IPasswordEncryptionService, PasswordEncryptionService>();
        services.AddSingleton<ITokenService, TokenService>();


        services.AddTransient<IEmailService>(provider => new EmailService(
            config.GetSection("EmailSettings:SMTPServer").Value!,
            Int32.Parse(config.GetSection("EmailSettings:SMTPServerPort").Value!),
            config.GetSection("EmailSettings:ApiKey").Value!,
            config.GetSection("EmailSettings:ApiSecret").Value!
        ));

        return services;
    }
}
