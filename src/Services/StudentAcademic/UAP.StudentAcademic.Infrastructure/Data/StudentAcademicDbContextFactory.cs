using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UAP.StudentAcademic.Infrastructure.Data;

public class StudentAcademicDbContextFactory : IDesignTimeDbContextFactory<StudentAcademicDbContext>
{
    public StudentAcademicDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StudentAcademicDbContext>();
        
        var connectionString = "Server=localhost\\SQLEXPRESS;Database=UAP_StudentAcademic;Integrated Security=true;TrustServerCertificate=True;";
        
        optionsBuilder.UseSqlServer(connectionString);

        return new StudentAcademicDbContext(optionsBuilder.Options);
    }
}
