namespace MasterPlanProject.WebApi.Models
{
	public class MappingConfigWebApi : Profile
	{
		public MappingConfigWebApi()
		{
			CreateMap<ApplicationIdentityUser, LocalUsersDTO>().ReverseMap();
		}
	}
}
