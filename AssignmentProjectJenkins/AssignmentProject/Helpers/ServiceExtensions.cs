using AssignmentProject.Application.Interfaces;
using AssignmentProject.Application.Models;
using AssignmentProject.Application.Services;
using AssignmentProject.Core.Repositories;
using AssignmentProject.Infrastructure.Repository;
using LoggingService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net;
using System.Text;
using System.Text.Json;

namespace AssignmentProject.Helpers
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }
        public static void ConfigureApplicationServices(this IServiceCollection services)
        {
            // configure DI for application services

            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            services.AddScoped<IServiceWrapper, ServiceWrapper>();
            

            //services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            //services.AddScoped<IBookRepository, BookRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IBookService, BookService>();

        }
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration _configuration)
        {
            // configure strongly typed settings objects
            var appSettingsSection = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(AppSettings.SecretKey);
            //register JWT authenticatoin schema to perform authentication by extracting and validating a JWT token from the Authorization request header.
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            //configure JWT schema with JWR bearer options
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    //OnTokenValidated = context =>
                    //{
                    //    var userService = context.HttpContext.RequestServices.GetRequiredService<IServiceWrapper>();
                    //    int userId = int.Parse(context.Principal.Identity.Name);
                    //    UserModel user = userService.UserService.GetUserById(userId);
                    //    if (user == null)
                    //    {
                    //        // return unauthorized if user no longer exists
                    //        context.Fail("Unauthorized");
                    //    }
                    //    return Task.CompletedTask;
                    //},
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        // Ensure we always have an error and error description.
                        if (string.IsNullOrEmpty(context.Error))
                            context.Error = "invalid_token";
                        if (string.IsNullOrEmpty(context.ErrorDescription))
                            context.ErrorDescription = "This request requires a valid JWT access token to be provided";

                        // Add some extra context for expired tokens.
                        if (context.AuthenticateFailure != null && context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            var authenticationException = context.AuthenticateFailure as SecurityTokenExpiredException;
                            context.Response.Headers.Add("x-token-expired", authenticationException.Expires.ToString("o"));
                            context.ErrorDescription = $"The token expired on {authenticationException.Expires.ToString("o")}";
                        }

                        return context.Response.WriteAsync(JsonSerializer.Serialize(new
                        {
                            error = context.Error,
                            error_description = context.ErrorDescription
                        }));
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = AppSettings.Issuer,
                    ValidAudience = AppSettings.Issuer,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true
                };
            });
        }
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
        //public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerManager logger)
        //{
        //    app.UseExceptionHandler(appError =>
        //    {
        //        appError.Run(async context =>
        //        {
        //            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        //            context.Response.ContentType = "application/json";
        //            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        //            if (contextFeature != null)
        //            {
        //                logger.LogError($"Something went wrong: {contextFeature.Error}");
        //                await context.Response.WriteAsync(new ErrorDetails()
        //                {
        //                    StatusCode = context.Response.StatusCode,
        //                    Message = "Internal Server Error."
        //                }.ToString());
        //            }
        //        });
        //    });
        //}

    }
}
