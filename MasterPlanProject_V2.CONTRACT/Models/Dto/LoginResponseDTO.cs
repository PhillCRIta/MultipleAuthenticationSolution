using ContractLibrary.Models.Dto;

namespace ContractLibrary
{
	public class LoginResponseDTO
	{
		public LocalUsersDTO User { get; set; }
		public string Token { get; set; }
	}
}
