namespace MasterPlanProject.WebApi.Repository.Interface
{
	public interface IUserRepository
	{
		Task<TokenDTO> LoginAsync(LoginRequestDTO loginRequestDTO);
		Task<LocalUsersDTO> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);
		bool IsUniqueUser(string username);

	}
}
