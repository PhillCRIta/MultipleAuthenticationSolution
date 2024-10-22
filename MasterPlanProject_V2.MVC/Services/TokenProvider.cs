using MasterPlanProject_V2.MVC.Services.IServices;

namespace MasterPlanProject_V2.MVC.Services
{
	public class TokenProvider : ITokenProvider
	{
		private readonly IHttpContextAccessor contextAccessor;

		public TokenProvider(IHttpContextAccessor contextAccessor )
        {
			this.contextAccessor = contextAccessor;
		}

        public void ClearToken()
		{
			contextAccessor.HttpContext?.Response.Cookies.Delete(Constant.AccessTokenSession);
			contextAccessor.HttpContext?.Response.Cookies.Delete(Constant.RefreshToken);
		}

		public TokenDTO? GetToken()
		{
			try
			{
				bool hasAccessToken = contextAccessor.HttpContext.Request.Cookies.TryGetValue(Constant.AccessTokenSession, out string accessToken);
				bool hasRefreshToken = contextAccessor.HttpContext.Request.Cookies.TryGetValue(Constant.RefreshToken, out string refreshToken);
				TokenDTO token = new()
				{
					AccessToken = accessToken,
					RefreshToken = refreshToken
				};
				return hasAccessToken ? token : null;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public void SetToken(TokenDTO tokenDTO)
		{
			CookieOptions co = new CookieOptions { Expires = DateTime.UtcNow.AddDays(60) };
			contextAccessor.HttpContext?.Response.Cookies.Append(Constant.AccessTokenSession, tokenDTO.AccessToken, co);
			contextAccessor.HttpContext?.Response.Cookies.Append(Constant.RefreshToken, tokenDTO.RefreshToken, co);
		}
	}
}
