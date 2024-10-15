using static ContractLibrary.Constant;

namespace ContractLibrary
{
	public class APIRequest
	{
		public ApiType ApiType { get; set; } = ApiType.GET;
		public string Url { get; set; }
		public object Data { get; set; }
		public string Token { get; set; }
	}
}
