using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskManagementApplication.Entities.DependencyInjection;
using TaskManagementApplication.Entities.Infrastructure.Repositories;
using TaskManagementApplication.Entities.Interfaces;
using TaskManagementApplication.Entities.Services;

namespace TaskManagementApplication
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var host = CreateHostBuilder().Build();
            ServiceProvider = host.Services;

            // Ensure database is created
            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.EnsureCreated();
            }

            Application.Run(ServiceProvider.GetRequiredService<LoginForm>());
        }

        static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Configuration
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

                    // Database
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                    // Repositories
                    services.AddScoped<IUserRepository, UserRepository>();
                    services.AddScoped<ITaskRepository, TaskRepository>();

                    // Services
                    services.AddScoped<IUserService, UserService>();
                    services.AddScoped<ITaskService, TaskService>();
                    services.AddScoped<IReminderPersonalityService, ReminderPersonalityService>();
                    services.AddScoped<IValidationService, ValidationService>();
                    services.AddScoped<INotificationService, NotificationService>();

                    // Forms
                    services.AddTransient<LoginForm>();
                    services.AddTransient<RegistrationForm>();
                    services.AddTransient<DashboardForm>();
                });
        }
    }
}