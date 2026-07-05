using System.Text;
using Backend.Middlewares;
using Backend.Models;
using Backend.Services;
using Backend.Data;
using Backend.Data.Repositories;
using Backend.Data.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Backend
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<JwtSettings>(builder.Configuration);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CORS", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.Services.Configure<JwtSettings>(options =>
            {
                options.Secret = builder.Configuration["JWT_SECRET"] ?? "";
                options.Issuer = builder.Configuration["JWT_ISSUER"] ?? "meme-api";
                options.Audience = builder.Configuration["JWT_AUDIENCE"] ?? "meme-app";
                options.ExpirationMinutes = int.TryParse(builder.Configuration["JWT_EXPIRATION_MINUTES"], out var min) ? min : 15;
                options.RefreshExpirationDays = int.TryParse(builder.Configuration["JWT_REFRESH_EXPIRATION_DAYS"], out var days) ? days : 7;
            });

            var jwtSettings = new JwtSettings
            {
                Secret = builder.Configuration["JWT_SECRET"] ?? "",
                Issuer = builder.Configuration["JWT_ISSUER"] ?? "meme-api",
                Audience = builder.Configuration["JWT_AUDIENCE"] ?? "meme-app",
                ExpirationMinutes = int.TryParse(builder.Configuration["JWT_EXPIRATION_MINUTES"], out var expMin) ? expMin : 15,
                RefreshExpirationDays = int.TryParse(builder.Configuration["JWT_REFRESH_EXPIRATION_DAYS"], out var refDays) ? refDays : 7
            };
            var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddTransient<ImageService>();
            builder.Services.AddHttpClient<IAiService, CloudflareAiService>();
            builder.Services.AddMemoryCache();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IMemeTemplateRepository, MemeTemplateRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IMemeService, MemeService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header. Enter: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.Urls.Add("http://0.0.0.0:8080");

            if (app.Environment.IsDevelopment())
            {
                Console.WriteLine("-------------- DEVELOPMENT --------------");
            }
            else
            {
                Console.WriteLine("-------------- PRODUCTION --------------");
                app.UseHttpsRedirection();
            }

            app.UseCors("CORS");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.MapGet("/", () =>
            {
                return "server is online";
            });

            app.MapControllers();
            app.Run();
        }
    }
}
