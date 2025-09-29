using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class ScmVlxdContextFactory : IDesignTimeDbContextFactory<ScmVlxdContext>
    {
        public ScmVlxdContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../API"))
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ScmVlxdContext>();
            var connectionString = configuration.GetConnectionString("MyCnn");

            optionsBuilder.UseSqlServer(connectionString);

            return new ScmVlxdContext(optionsBuilder.Options);
        }
    }
}
