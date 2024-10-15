namespace MasterPlanProject.Mvc.Services
{
	public class AuthService : BaseService, IAuthService
	{
		private readonly IConfiguration config;
		private readonly IHttpClientFactory httpClient;
		private readonly string localitaPugliaServiceURL;

		public AuthService(IHttpClientFactory httpClient,  IConfiguration config) : base(httpClient, "LocalitaPugliaService")
		{
			this.httpClient = httpClient;
			localitaPugliaServiceURL = config.GetValue<string>("ServiceUrls:LocalitaPugliaAPI");
		}

		public Task<T> LoginAsync<T>(LoginRequestDTO objLogin)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.POST,
				Data = objLogin,
				Url = localitaPugliaServiceURL+"/api/UserAuth/login"
			});
		}

		public Task<T> RegisterAsync<T>(RegistrationRequestDTO objRegister)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.POST,
				Data = objRegister,
				Url = localitaPugliaServiceURL+ "/api/UserAuth/register"
			});
		}
	}
}
