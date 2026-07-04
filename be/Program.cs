using Backend.Middlewares;
using Backend.Models;
using Backend.Services;
using Backend.Data;
using Backend.Data.Repositories;
using Backend.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Backend
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CORS", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:4200",
                        "https://blog-memes-generator.vercel.app",
                        "https://ai-memes.vercel.app"
                        )
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddTransient<ImageService>();
            builder.Services.AddHttpClient<IAiService, CloudflareAiService>();
            builder.Services.AddMemoryCache();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IMemeTemplateRepository, MemeTemplateRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IMemeService, MemeService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
            });

            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.Urls.Add("http://0.0.0.0:8080");

            // Configure the HTTP request pipeline.
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
