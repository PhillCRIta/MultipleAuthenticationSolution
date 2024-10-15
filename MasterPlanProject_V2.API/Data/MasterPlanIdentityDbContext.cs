namespace MasterPlanProject.WebApi.Data
{
	public class MasterPlanIdentityDbContext : IdentityDbContext<ApplicationIdentityUser>
	{
		public MasterPlanIdentityDbContext(DbContextOptions<MasterPlanIdentityDbContext> options) : base(options)
		{
		}
		public DbSet<ApplicationIdentityUser> ApplicationIdentityUsers { get; set; }
	}
}
