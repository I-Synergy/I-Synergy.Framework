using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Data.SqlClient;
using System.Reflection;

namespace Sample.Synchronization.DataContext
{
    internal class AdventureWorksFactory : IDesignTimeDbContextFactory<AdventureWorksContext>
    {
        /// <summary>
        /// Creates a new instance of a derived context.
        /// </summary>
        /// <param name="args">Arguments provided by the design-time service.</param>
        /// <returns>An instance of <seealso name="DataContext" />.</returns>
        public AdventureWorksContext CreateDbContext(string[] args)
    {
        var connectionStringBuilder = new SqlConnectionStringBuilder("Server=(Localdb)\\MSSQLLocalDB;Database=AdventureWorks;Integrated Security=true");
        var optionsBuilder = new DbContextOptionsBuilder<AdventureWorksContext>();

        optionsBuilder
            .UseSqlServer(
                connectionStringBuilder.ConnectionString,
                opt => {
                    opt.MigrationsAssembly(Assembly.GetAssembly(typeof(AdventureWorksFactory))?.FullName);
                    opt.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds);
                });

        return new AdventureWorksContext(optionsBuilder.Options);
    }
}
}
