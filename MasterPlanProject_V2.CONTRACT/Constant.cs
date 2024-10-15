using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractLibrary
{
	public static class Constant
	{
		public static string SessioneToken { get; set; } = "JWTToken";//il nome della sessione usato nella parte web

		public enum ApiType { GET, POST, PUT, DELETE }

		public enum AuthType { BUILTIN, IDENTITY, IDENTITYSERVER}
	}
}
