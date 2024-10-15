namespace MasterPlanProject.Mvc.Services
{
	public class BaseService : IBaseService
	{
		private readonly IHttpClientFactory httpClient;
		private readonly string nameClient;

		public APIResponse ResponseModel { get; set; }

		public BaseService(IHttpClientFactory httpClient, string nameClient)
		{
			this.ResponseModel = new();
			this.httpClient = httpClient;
			this.nameClient = nameClient;
		}

		public async Task<T> SendAsync<T>(APIRequest apiRequest)
		{
			try
			{
				var client = httpClient.CreateClient(nameClient);
				HttpRequestMessage message = new HttpRequestMessage();
				message.Headers.Add("Accept", "application/json");
				message.RequestUri = new Uri(apiRequest.Url);
				if (apiRequest.Data != null)
				{
					message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
				}
				switch (apiRequest.ApiType)
				{
					case Constant.ApiType.POST:
						message.Method = HttpMethod.Post;
						break;
					case Constant.ApiType.PUT:
						message.Method = HttpMethod.Put;
						break;
					case Constant.ApiType.DELETE:
						message.Method = HttpMethod.Delete;
						break;
					default:
						message.Method = HttpMethod.Get;
						break;
				}
				if (string.IsNullOrEmpty(apiRequest.Token)==false)
				{
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.Token);
				}
				HttpResponseMessage apiResponse = null;
				apiResponse = await client.SendAsync(message);
				string responseContent = await apiResponse.Content.ReadAsStringAsync();
				T responseOut = default(T);
				if (apiResponse.StatusCode != System.Net.HttpStatusCode.OK)
				{
					APIResponse rispostaErrore = new()
					{
						StatusCode = apiResponse.StatusCode,
						ErrorMessages = new List<string>() { apiResponse.ReasonPhrase },
						ErrorDescription = JsonConvert.DeserializeObject<APIErrorDescription>(responseContent),
						IsSucces = false,
						Result = null
					};
					responseOut = (T)Convert.ChangeType(rispostaErrore, typeof(T));
				}
				else
				{
					responseOut = JsonConvert.DeserializeObject<T>(responseContent);
				}
				return responseOut;
			}
			catch (Exception ex)
			{
				var error = new APIResponse
				{
					ErrorMessages = new List<string> { "Errore di chiamata al servizio. ", Convert.ToString(ex.Message) },
					IsSucces = false
				};
				string errorConvert = JsonConvert.SerializeObject(error);
				T KOresponse = JsonConvert.DeserializeObject<T>(errorConvert);
				return KOresponse;
			}
		}
	}
}
