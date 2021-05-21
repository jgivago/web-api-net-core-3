using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using WebApi.Entities;

namespace WebApi.Data
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
		{
		}

		public DbSet<User> Users { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseMySQL(@"server=localhost;user=root;password=root;database=apidb");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>(entity =>
			{
				entity.HasKey(e => e.Id);
				entity.Property(e => e.FirstName)
					.IsRequired()
					.HasMaxLength(50)
					.HasColumnType("varchar(100)");
				entity.Property(e => e.LastName)
					.IsRequired()
					.HasMaxLength(50)
					.HasColumnType("varchar(100)");
				entity.Property(e => e.Username)
					.IsRequired()
					.HasMaxLength(50)
					.HasColumnType("varchar(50)");
				entity.Property(e => e.Password)
					.IsRequired()
					.HasMaxLength(50)
					.HasColumnType("varchar(50)");
			});
		}
	}
}