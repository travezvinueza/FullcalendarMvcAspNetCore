using Fullcalendar.Models.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fullcalendar.Data
{
    public class DatabaseContext : IdentityDbContext<Usuario>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                 .HasMany(u => u.CalendarEvents)
                 .WithOne(e => e.Usuario)
                 .OnDelete(DeleteBehavior.Cascade);

        }

    }

}