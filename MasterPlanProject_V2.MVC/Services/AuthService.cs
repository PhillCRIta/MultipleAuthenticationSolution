namespace MasterPlanProject.Mvc.Services
{
	public class AuthService :  IAuthService
	{
		private readonly IConfiguration config;
		private readonly IHttpClientFactory httpClient;
		private readonly IBaseService baseService;
		private readonly string localitaPugliaServiceURL;

		public AuthService(IHttpClientFactory httpClient,  IConfiguration config, IBaseService baseService)  
		{
			this.httpClient = httpClient;
			this.baseService = baseService;
			localitaPugliaServiceURL = config.GetValue<string>("ServiceUrls:LocalitaPugliaAPI");
		}
		public Task<T> LoginAsync<T>(LoginRequestDTO objLogin)
		{
			return baseService.SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.POST,
				Data = objLogin,
				Url = localitaPugliaServiceURL+"/api/UserAuth/login"
			}, "AuthServiceClient", false);
		}
		public Task<T> RegisterAsync<T>(RegistrationRequestDTO objRegister)
		{
			return baseService.SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.POST,
				Data = objRegister,
				Url = localitaPugliaServiceURL+ "/api/UserAuth/register"
			}, "AuthServiceClient", false);
		}
	}
}
