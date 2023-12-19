﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using PokerPlanning.Application.src.Common.Interfaces.Authentication;
using PokerPlanning.Application.src.Common.Interfaces.Services;
using PokerPlanning.Infrastructure.src.Authentication;
using PokerPlanning.Infrastructure.src.Authentication.Services;
using PokerPlanning.Application.src.Common.Interfaces.Persistence;
using PokerPlanning.Infrastructure.src.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using PokerPlanning.Infrastructure.src.Persistence;
using PokerPlanning.Infrastructure.src.Persistence.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace PokerPlanning.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddPersistance(configuration);
        services.AddConfigurations(configuration);
        services.AddServices();
        services.AddIdentity(configuration);
        return services;
    }

    public static IServiceCollection AddPersistance(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var connectionString = configuration.GetSection(ConnectionStrings.SectionName).Get<ConnectionStrings>()?.PokerPlanningDbConnection;
        services.AddDbContext<PokerPlanningDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVotingSystemRepository, VotingSystemRepository>();
        return services;
    }

    public static IServiceCollection AddConfigurations(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        return services;
    }

    public static IServiceCollection AddIdentity(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();

        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<PokerPlanningDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = jwtSettings?.Audience ?? "",
                    ValidIssuer = jwtSettings?.Issuer ?? "",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Secret ?? ""))
                };
            });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
        });
        return services;
    }
}
