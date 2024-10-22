using MasterPlanProject_V2.MVC.Services.IServices;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace MasterPlanProject.Mvc.Services
{
	public class BaseService : IBaseService
	{
		private readonly IHttpClientFactory httpClient;
		private readonly ITokenProvider tokenProvider;
		private readonly IConfiguration config;
		private readonly IHttpContextAccessor context;

		public APIResponse ResponseModel { get; set; }

		public BaseService(IHttpClientFactory httpClient, ITokenProvider tokenProvider, IConfiguration config, IHttpContextAccessor context)
		{
			this.ResponseModel = new();
			this.httpClient = httpClient;
			this.tokenProvider = tokenProvider;
			this.config = config;
			this.context = context;
		}

		public async Task<T> SendAsync<T>(APIRequest apiRequest, string nameClient, bool withBearer = true)
		{
			try
			{
				HttpClient client = httpClient.CreateClient(nameClient);

				var messageFactory = () =>
				{
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
					return message;
				};

				//if (tokenProvider.GetToken() != null && withBearer == true)
				//{
				//	TokenDTO token = tokenProvider.GetToken();
				//	client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
				//}

				HttpResponseMessage apiResponse = null;
				//apiResponse = await client.SendAsync(messageFactory());
				apiResponse = await SendWithRefreshTokenAsync(client, messageFactory, withBearer);
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

		private async Task<HttpResponseMessage> SendWithRefreshTokenAsync(HttpClient httpClient, Func<HttpRequestMessage> httpRequestMessageFactory, bool withBearer = true)
		{
			if (withBearer == false)
			{
				return await httpClient.SendAsync(httpRequestMessageFactory());
			}
			else
			{
				TokenDTO tokenDto = tokenProvider.GetToken();
				if (tokenDto != null && string.IsNullOrEmpty(tokenDto.AccessToken) == false)
				{
					httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDto.AccessToken);
				}

				try
				{
					HttpResponseMessage response = await httpClient.SendAsync(httpRequestMessageFactory());
					if (response.IsSuccessStatusCode == true)
					{
						return response;
					}
					if (response.IsSuccessStatusCode == false && response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
					{
						await InvokeRefreshTokenEndPoint(httpClient, tokenDto.AccessToken, tokenDto.RefreshToken);
						response = await httpClient.SendAsync(httpRequestMessageFactory());
						return response;
					}
					return response;
				}
				catch (HttpRequestException httpEx)
				{
					if (httpEx.StatusCode == System.Net.HttpStatusCode.Unauthorized)
					{
						await InvokeRefreshTokenEndPoint(httpClient, tokenDto.AccessToken, tokenDto.RefreshToken);
						return await httpClient.SendAsync(httpRequestMessageFactory());
					}
					throw;
				}
			}
		}

		private async Task InvokeRefreshTokenEndPoint(HttpClient httpClient, string exisistingAccessToken, string existingRefreshToken)
		{
			HttpRequestMessage message = new();
			message.Headers.Add("Accept", "application/json");
			message.RequestUri = new Uri(config.GetValue<string>("ServiceUrls:LocalitaPugliaAPI") + "/api/UserAuth/refresh");
			message.Method = HttpMethod.Post;
			message.Content = new StringContent(JsonConvert.SerializeObject(new TokenDTO()
			{
				AccessToken = exisistingAccessToken,
				RefreshToken = existingRefreshToken
			}), Encoding.UTF8, "application/json");
			HttpResponseMessage response = await httpClient.SendAsync(message);
			string content = await response.Content.ReadAsStringAsync();
			APIResponse apiResponse = JsonConvert.DeserializeObject<APIResponse>(content);
			if (apiResponse?.IsSucces != true)
			{
				//if [apiResponse?.IsSucces] is null return null
				//null is different from true >> yes
				//prevent nullreference exception
				await context.HttpContext.SignOutAsync();
				tokenProvider.ClearToken();
			}
			else
			{
				TokenDTO tokenDTO = JsonConvert.DeserializeObject<TokenDTO>(JsonConvert.SerializeObject(apiResponse.Result));
				if (tokenDTO != null && string.IsNullOrEmpty(tokenDTO.AccessToken)== false)
				{
					await SignInWithNewToken(tokenDTO);
					httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenDTO.AccessToken);
				}
			}
		}

		private async Task SignInWithNewToken(TokenDTO model)
		{

			JwtSecurityTokenHandler handler = new();
			JwtSecurityToken jwt = handler.ReadJwtToken(model.AccessToken);

			string email = jwt.Claims.FirstOrDefault(c => c.Type == "email").Value;
			string userName = jwt.Claims.FirstOrDefault(c => c.Type == "unique_name").Value;

			List<string> roles = jwt.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();

			ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
			identity.AddClaim(new Claim(ClaimTypes.Name, userName));
			foreach (string role in roles)
			{
				identity.AddClaim(new Claim(ClaimTypes.Role, role));//se ci sono più ruoli posso passare un array, oppure inserire più volte la riga
			}
			identity.AddClaim(new Claim(ClaimTypes.Email, email));
			ClaimsPrincipal principal = new ClaimsPrincipal();
			principal.AddIdentity(identity);
			await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

			tokenProvider.SetToken(model);
		}
	}
}
