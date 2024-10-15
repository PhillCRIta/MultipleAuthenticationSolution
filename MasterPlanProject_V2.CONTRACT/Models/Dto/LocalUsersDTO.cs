using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractLibrary.Models.Dto
{
	public class LocalUsersDTO
	{
        public string Id { get; set; }
		public string Name { get; set; }
		public string Username { get; set; }
		public List<string> Roles { get; set; } = new List<string>();
    }
}
