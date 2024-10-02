using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Splunk;
using StudentsManager.Abstract.Repo;
using StudentsManager.Abstract.Service;
using StudentsManager.Concrete.Repo;
using StudentsManager.Concrete.Service;
using StudentsManager.Context;
using StudentsManager.Domain.Data;
using StudentsManager.Domain.Models;
using StudentsManager.Domain.Validators;
using System.Net.Http;

namespace StudentsManager;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Load configuration from appsettings.json
        var builder = WebApplication.CreateBuilder(args);

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.EventCollector(
                builder.Configuration["Serilog:WriteTo:1:Args:splunkHost"],
                builder.Configuration["Serilog:WriteTo:1:Args:eventCollectorToken"],
                index: builder.Configuration["Serilog:WriteTo:1:Args:index"],
                batchSizeLimit: int.Parse(builder.Configuration["Serilog:WriteTo:1:Args:batchSizeLimit"]),
                messageHandler: handler
            )
            .CreateLogger();

        try
        {
            builder.Host.UseSerilog();

            if (!builder.Environment.IsDevelopment())
                builder.WebHost.UseUrls("http://*:80");

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();

            string connectionString;
            string redisConnection;

            if (builder.Environment.IsDevelopment())
            {
                connectionString = builder.Configuration.GetConnectionString("conn_string_local");
                redisConnection = "localhost:6379";
            }
            else
            {
                connectionString = builder.Configuration.GetConnectionString("conn_string_gcp");
                redisConnection = builder.Configuration.GetConnectionString("redis_gcp");
            }

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                {
                    builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(origin => true)
                        .AllowCredentials();
                }));

            builder.Services.AddSignalR().AddStackExchangeRedis(redisConnection,
                options => { options.Configuration.ChannelPrefix = "SignalR"; });

            // Dependency Injection
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IClassGroupService, ClassGroupService>();
            builder.Services.AddScoped<IStudentService, StudentService>();
            builder.Services.AddScoped<ITeacherService, TeacherService>();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<ILessonPlanService, LessonPlanService>();

            builder.Services.AddScoped<IClassGroupRepository, ClassGroupRepository>();
            builder.Services.AddScoped<ILessonPlanRepository, LessonPlanRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
            builder.Services.AddScoped<IChatRepository, ChatRepository>();

            builder.Services.AddFluentValidation(fv =>
                fv.RegisterValidatorsFromAssemblyContaining<ClassGroupValidator>());
            builder.Services.AddFluentValidation(fv =>
                fv.RegisterValidatorsFromAssemblyContaining<ApplicationUserValidator>());

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors(policyBuilder => policyBuilder.AllowAnyOrigin());

            app.UseRouting(); // This must be before UseAuthentication and UseAuthorization

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chatHub");
            });

            // Initialize roles
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await RoleInitializer.InitializeAsync(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "The application failed to start correctly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}