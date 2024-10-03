using Microsoft.EntityFrameworkCore;
using StudentsManager.Concrete.Service.Secrets;
using StudentsManager.Context;

namespace StudentsManager.Concrete.Factory;

public class ApplicationDbContextFactory
{
    public static async Task<ApplicationDbContext> CreateDbContextAsync()
    {
        // Initialize Secret Manager Service
        var secretManager = new SecretManagerService();
        var connectionString = await secretManager.GetSecretAsync("conn_string", "aj-dev-434320");

        // Use the retrieved connection string in DbContext
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new ApplicationDbContext(options);
    }
}