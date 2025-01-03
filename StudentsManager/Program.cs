using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StudentsManager.Context;
using StudentsManager.Domain.Models;
using FluentValidation.AspNetCore;
using StudentsManager.Abstract.Repo;
using StudentsManager.Abstract.Service;
using StudentsManager.Concrete.Repo;
using StudentsManager.Concrete.Service;
using StudentsManager.Domain.Data;
using StudentsManager.Domain.Validators;
using StudentsManager.Infra;

namespace StudentsManager;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };
        var projectId = builder.Configuration["ProjectSettings:ProjectId"];
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.EventCollector(
                await SecretAccessor.GetSecretAsync("splunk_host", projectId),
                await SecretAccessor.GetSecretAsync("splunk_token", projectId),
                index: builder.Configuration["Serilog:WriteTo:0:Args:index"],
                batchSizeLimit: int.Parse(builder.Configuration["Serilog:WriteTo:0:Args:batchSizeLimit"]),
                messageHandler: handler
            )
            .CreateLogger();

        try
        {
            builder.Host.UseSerilog();
            // builder.WebHost.UseUrls("http://*:80");
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            string connectionString;
            string redisConnection;
            connectionString = builder.Configuration.GetConnectionString("conn_string_local");
            // connectionString = SecretAccessor.GetSecretAsync("conn_string", projectId).Result;
            redisConnection = await SecretAccessor.GetSecretAsync("redis_ip", projectId);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

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

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors(policyBuilder => policyBuilder.AllowAnyOrigin());
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chatHub");
            });

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    await RoleInitializer.InitializeAsync(userManager, roleManager, projectId);
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