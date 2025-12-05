using DonutsVikets.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DonutsVikets.DAL
{
    public class DonutsVikets3ContextFactory : IDesignTimeDbContextFactory<DonutsVikets3Context>
    {
        public DonutsVikets3Context CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DonutsVikets3Context>();

            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=DonutsVikets3Context;Trusted_Connection=True;");

            return new DonutsVikets3Context(optionsBuilder.Options);
        }
    }
}
