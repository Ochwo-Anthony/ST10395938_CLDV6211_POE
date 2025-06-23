using Microsoft.EntityFrameworkCore;

namespace EventEase.Models
{
    public class ApplicationDbContext : DbContext
    {
        // The code was adapted from "MVC, Entity Framework & SQL Azure" by Adeol Adisa
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Venues> Venues { get; set; }

        public DbSet<Events> Events { get; set; }

        public DbSet<Bookings> Bookings { get; set; }

        public DbSet<EventTypes> EventTypes { get; set; }
    }
}
