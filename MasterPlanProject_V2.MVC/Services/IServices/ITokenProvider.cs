namespace MasterPlanProject_V2.MVC.Services.IServices
{
	public interface ITokenProvider
	{
		void SetToken(TokenDTO tokenDTO);
		TokenDTO? GetToken();
		void ClearToken();
	}
}
