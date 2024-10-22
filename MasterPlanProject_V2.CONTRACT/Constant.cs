using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractLibrary
{
	public static class Constant
	{
		public static string AccessTokenSession { get; set; } = "JWTToken"; 
		public static string RefreshToken { get; set; } = "RefreshToken"; 

		public enum ApiType { GET, POST, PUT, DELETE }

		public enum AuthType { BUILTIN, IDENTITY, IDENTITYSERVER}
	}
}
