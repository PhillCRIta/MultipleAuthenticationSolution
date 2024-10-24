﻿namespace MasterPlanProject.Mvc.Services.IServices
{
	public interface IBaseService
	{
		APIResponse ResponseModel { get; set; }
		Task<T> SendAsync<T>(APIRequest apiRequest, string nameClient, bool withBearer = true);

	}
}
