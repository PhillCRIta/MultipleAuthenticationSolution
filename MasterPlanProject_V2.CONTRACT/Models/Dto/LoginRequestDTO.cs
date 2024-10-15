using System.ComponentModel.DataAnnotations.Schema;

namespace ContractLibrary
{
	public class LoginRequestDTO
	{
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
