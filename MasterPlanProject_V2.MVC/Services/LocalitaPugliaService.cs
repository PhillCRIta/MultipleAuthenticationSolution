namespace MasterPlanProject.Mvc.Services
{
	public class LocalitaPugliaService :  ILocalitaPugliaService
	{
		private readonly IHttpClientFactory clientFacotory;
		private readonly IBaseService baseService;
		private readonly string localitaPugliaServiceURL;

		public LocalitaPugliaService(IHttpClientFactory clientFacotory, IConfiguration config, IBaseService baseService)  
		{
			this.clientFacotory = clientFacotory;
			this.baseService = baseService;
			localitaPugliaServiceURL = config.GetValue<string>("ServiceUrls:LocalitaPugliaAPI");
		}
		public Task<T> CreateAsync<T>(LocalitaPugliaDTO_Insert localitaInsertDTO, string token)
		{
			return baseService.SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.POST,
				Data = localitaInsertDTO,
				Url = localitaPugliaServiceURL + "/api/BellezzePuglia"
			}, "LocalitaPugliaService");
		}
		public Task<T> DeleteAsync<T>(int id, string token)
		{
			return baseService.SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.DELETE,
				Url = localitaPugliaServiceURL + "/api/BellezzePuglia/" + id
			}, "LocalitaPugliaService");
		}
		public Task<T> GetAllAsync<T>(string token)
		{
			return baseService.SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.GET,
				Url = localitaPugliaServiceURL + "/api/BellezzePuglia/GetLocalita"
			}, "LocalitaPugliaService");
		}
		public Task<T> GetAsync<T>(int id , string token)
		{
			return baseService.SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.GET,
				Url = localitaPugliaServiceURL + "/api/BellezzePuglia/" + id
			}, "LocalitaPugliaService");
		}
		public Task<T> UpdateAsync<T>(LocalitaPugliaDTO_Update localitaUpdateDTO, string token)
		{
			return baseService.SendAsync<T>(new APIRequest()
			{
				ApiType = Constant.ApiType.PUT,
				Url = localitaPugliaServiceURL + "/api/BellezzePuglia/" + localitaUpdateDTO.Id
			}, "LocalitaPugliaService");
		}
	}
}
