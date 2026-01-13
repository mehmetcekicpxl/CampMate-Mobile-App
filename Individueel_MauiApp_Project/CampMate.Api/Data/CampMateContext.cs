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

        public DbSet<Favorite> Favorites { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           
            // "In de relatie tussen een opmerking en een gebruiker zeggen we: als de gebruiker wordt verwijderd, raak de opmerking dan niet aan (beperken). Zo wordt het verwijderen van het kamp niet gecombineerd met het verwijderen van de opmerking."

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

          
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Geef een foutmelding of verwijder de gebruiker (breekt de lus)

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.CampSite)
                .WithMany()
                .HasForeignKey(f => f.CampSiteId)
                .OnDelete(DeleteBehavior.Cascade); // Als het kamp wordt verwijderd, moet ook de favoriet worden verwijderd.
        }
    }
}
