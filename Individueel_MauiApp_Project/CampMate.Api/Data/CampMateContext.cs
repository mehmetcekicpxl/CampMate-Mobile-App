using Microsoft.EntityFrameworkCore;
using CampMate.Api.Models;

namespace CampMate.Api.Data
{
    public class CampMateContext : DbContext
    {
        public CampMateContext(DbContextOptions<CampMateContext> options) : base(options) { }

        public DbSet<CampSite> CampSites { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
