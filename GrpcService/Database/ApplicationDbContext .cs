using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GrpcService.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // đổi kiểu dữ liệu
            var converter = new ValueConverter<Timestamp, DateTime>(
                                    v => DateTime.SpecifyKind(v.ToDateTime(), DateTimeKind.Local),
                                    v => Timestamp.FromDateTime(DateTime.SpecifyKind(v, DateTimeKind.Utc)));

            modelBuilder.Entity<User>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<User>()
                .Property(e => e.CreatedDate)
                .HasConversion(converter);

            modelBuilder.Entity<User>()
                .Property(e => e.LastActive)
                .HasConversion(converter);
        }

        public DbSet<User> Users { get; set; }
    }
}