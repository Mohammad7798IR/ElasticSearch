using DapperExample.Models.Entites;
using Microsoft.EntityFrameworkCore;

namespace ElasticExample.Context
{
    public class ElasticDbContext : DbContext
    {
        public ElasticDbContext(DbContextOptions<ElasticDbContext> options) : base(options)
        {

        }


        public DbSet<User> Users { get; set; }
    }
}
