using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace DataBase.AppDbContexts
{
    public class MainDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>, IDbContextFactory<MainDbContext>
    {
        /// <param name="args">Is connection jsons files </param>
        public MainDbContext CreateDbContext(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder();
            foreach(var jsonfiles in args)
            {
                configurationBuilder.AddJsonFile(jsonfiles);
            }
            var configuration = configurationBuilder.Build();
            var contextOptionsBuilder = new DbContextOptionsBuilder<MainDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new NullReferenceException("Connection string 'DefaultConnection' not found.");
            contextOptionsBuilder.UseNpgsql(connectionString);
            return new MainDbContext(contextOptionsBuilder.Options);
        }

        public MainDbContext CreateDbContext()
        {
            throw new NotImplementedException();
        }
    }
}
