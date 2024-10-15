namespace MasterPlanProject.Mvc.Services.IServices
{
	public interface IAuthService
	{
		Task<T> LoginAsync<T>(LoginRequestDTO objLogin);
		Task<T> RegisterAsync<T>(RegistrationRequestDTO objRegister);
	}
}
