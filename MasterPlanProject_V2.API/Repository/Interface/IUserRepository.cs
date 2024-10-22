namespace MasterPlanProject.WebApi.Repository.Interface
{
	public interface IUserRepository
	{
		Task<TokenDTO> LoginAsync(LoginRequestDTO loginRequestDTO);
		Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDto);
		Task<LocalUsersDTO> RegisterAsync(RegistrationRequestDTO registrationRequestDTO);
		bool IsUniqueUser(string username);
		Task RevokerefreshToken(TokenDTO tokenDTO);

	}
}
