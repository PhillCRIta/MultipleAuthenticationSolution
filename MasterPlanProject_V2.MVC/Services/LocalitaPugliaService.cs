namespace MasterPlanProject.Mvc.Services
{
	public class LocalitaPugliaService : BaseService, ILocalitaPugliaService
	{
		private readonly IHttpClientFactory clientFacotory;
		private readonly string localitaPugliaServiceURL;

		public LocalitaPugliaService(IHttpClientFactory clientFacotory, IConfiguration config) : base(clientFacotory, "LocalitaPugliaService")//devo passare alla classe base il client factory
		{
			this.clientFacotory = clientFacotory;
			localitaPugliaServiceURL = config.GetValue<string>("ServiceUrls:LocalitaPugliaAPI");
		}
		public Task<T> CreateAsync<T>(LocalitaPugliaDTO_Insert localitaInsertDTO, string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.POST,
				Data = localitaInsertDTO,
				Url = localitaPugliaServiceURL + "/api/BellezzePuglia",
				Token = token
			});
		}

		public Task<T> DeleteAsync<T>(int id, string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.DELETE,
				Url = localitaPugliaServiceURL + "/api/BellezzePuglia/" + id,
				Token = token
			});
		}

		public Task<T> GetAllAsync<T>(string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.GET,
				Url = localitaPugliaServiceURL + "/api/BellezzePuglia/GetLocalita",
				Token = token
			});
		}

		public Task<T> GetAsync<T>(int id , string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.GET,
				Url = localitaPugliaServiceURL + "/api/BellezzePuglia/" + id,
				Token = token
			});
		}

		public Task<T> UpdateAsync<T>(LocalitaPugliaDTO_Update localitaUpdateDTO, string token)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.PUT,
				Url = localitaPugliaServiceURL + "/api/BellezzePuglia/" + localitaUpdateDTO.Id,
				Token = token
			});
		}
	}
}
