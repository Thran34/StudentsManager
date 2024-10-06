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

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .WriteTo.EventCollector(
                await SecretAccessor.GetSecretAsync("splunk_host", "aj-dev-434320"),
                await SecretAccessor.GetSecretAsync("splunk_token", "aj-dev-434320"),
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

            var connectionString = await SecretAccessor.GetSecretAsync("conn_string", "aj-dev-434320");

            string redisConnection;
            if (builder.Environment.IsDevelopment())
            {
                connectionString = builder.Configuration.GetConnectionString("conn_string_local");
                redisConnection = "localhost:6379";
            }
            else
            {
                redisConnection = await SecretAccessor.GetSecretAsync("redis_ip", "aj-dev-434320");
            }

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

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

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