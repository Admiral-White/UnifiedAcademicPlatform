using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace UAP.Authentication.Infrastructure.Data;

public class AuthenticationDbContextFactory : IDesignTimeDbContextFactory<AuthenticationDbContext>
{
    public AuthenticationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthenticationDbContext>();
        
        // Use a default connection string for migrations
        // Update this to match your actual connection string
        var connectionString = "Server=localhost\\SQLEXPRESS;Database=UAP_Authentication;Integrated Security=true;TrustServerCertificate=True;";
        
        optionsBuilder.UseSqlServer(connectionString);

        return new AuthenticationDbContext(optionsBuilder.Options);
    }
}
