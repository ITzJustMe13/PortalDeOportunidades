using BackEnd.Models.BackEndModels;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
            { }

        public DbSet<FavoritesModel> Favorites { get; set; }

        public DbSet<ImpulseModel> Impulses  { get; set; }

        public DbSet<OpportunityModel> Opportunities { get; set; }

        public DbSet<ReservationModel> Reservations { get; set; }

        public DbSet<ReviewModel> Reviews { get; set; }

        public DbSet<UserModel> Users { get; set; }
    }
}
