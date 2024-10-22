using MasterPlanProject_V2.API.Models;

namespace MasterPlanProject.WebApi.Data
{
	public class MasterPlanDataDbContext : DbContext
	{
		public MasterPlanDataDbContext(DbContextOptions<MasterPlanDataDbContext> options) : base(options){}

		public DbSet<LocalitaPuglia> LocalitaPuglia { get; set; }
		public DbSet<LocalUsers> LocalUsers { get; set; }
		public DbSet<RefreshTokens> RefreshTokens { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<LocalitaPuglia>().HasData(
					new LocalitaPuglia
					{
						Id = 1,
						Area = "Salento",
						Localita = "Porto Cesareo",
						DataInserimento = new DateTime(1983, 9, 18)
					});
			modelBuilder.Entity<RefreshTokens>().Property(x => x.DataInserimento).HasDefaultValueSql<DateTime>("getdate()");
		}

		//COMANDI
		//get-migration ottiene l'elenco di tutte le migrations
		//Remove-Migration
		//script-migration per avere lo sript del db in anteprima
		//add-migration allineamento-post-identity -context MasterPlanDataDbContext 
		//add-migration allineamento-post-identity3 -context MasterPlanIdentityDbContext
		//update-database
		//update-database -context MasterPlanIdentityDbContext
		//script-migration -context MasterPlanIdentityDbContext
	}
}
