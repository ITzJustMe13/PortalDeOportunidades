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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração do relacionamento de Users com Favorites
            modelBuilder.Entity<FavoritesModel>()
                .HasKey(f => new { f.UserId, f.OpportunityId });

            modelBuilder.Entity<FavoritesModel>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Restringe exclusão para evitar cascatas múltiplas

            modelBuilder.Entity<FavoritesModel>()
                .HasOne(f => f.Opportunity)
                .WithMany(o => o.Favorites)
                .HasForeignKey(f => f.OpportunityId)
                .OnDelete(DeleteBehavior.Cascade); // Exclui favoritos em cascata quando Opportunity for excluído

            // Configuração do relacionamento de Users com Reservations
            modelBuilder.Entity<ReservationModel>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.userID)
                .OnDelete(DeleteBehavior.Restrict); // Restringe exclusão para evitar cascatas múltiplas

            modelBuilder.Entity<ReservationModel>()
                .HasOne(r => r.Opportunity)
                .WithMany(o => o.Reservations)
                .HasForeignKey(r => r.opportunityID)
                .OnDelete(DeleteBehavior.Cascade); // Exclui reservas em cascata quando Opportunity for excluído

            // Configuração do relacionamento de Reservations com Reviews
            modelBuilder.Entity<ImpulseModel>()
                .HasOne(r => r.User)
                .WithMany(u => u.Impulses)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
