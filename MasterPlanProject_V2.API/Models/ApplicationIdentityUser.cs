namespace MasterPlanProject.WebApi.Models
{
	public class ApplicationIdentityUser : IdentityUser
	{
		[Column(TypeName = "varchar(150)")]
		public string Name { get; set; }
		[Column(TypeName = "varchar(150)")]
		public string? OtherResource { get; set; }	
    }
}
