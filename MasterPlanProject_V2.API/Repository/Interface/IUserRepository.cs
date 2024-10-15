namespace MasterPlanProject.WebApi.Repository.Interface
{
	public interface IUserRepository
	{
		Task<LoginResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO);
		Task<LocalUsersDTO> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);
		bool IsUniqueUser(string username);

	}
}
