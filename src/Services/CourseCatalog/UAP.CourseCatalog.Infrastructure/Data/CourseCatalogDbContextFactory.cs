using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UAP.CourseCatalog.Infrastructure.Data;

public class CourseCatalogDbContextFactory : IDesignTimeDbContextFactory<CourseCatalogDbContext>
{
    public CourseCatalogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CourseCatalogDbContext>();
        
        var connectionString = "Server=localhost\\SQLEXPRESS;Database=UAP_CourseCatalog;Integrated Security=true;TrustServerCertificate=True;";
        
        optionsBuilder.UseSqlServer(connectionString);

        return new CourseCatalogDbContext(optionsBuilder.Options);
    }
}
