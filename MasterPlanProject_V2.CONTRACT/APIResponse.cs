using System.Net;

namespace ContractLibrary
{
	public class APIResponse
	{
		public HttpStatusCode StatusCode { get; set; }
		public bool IsSucces { get; set; } = true;
		public List<string> ErrorMessages { get; set; } = new List<string>();
		public object Result { get; set; }
		public APIErrorDescription ErrorDescription { get; set; }
	}
}
