using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractLibrary.Models
{
	public class LocalUsers
	{
        public int Id { get; set; }
		[Column(TypeName = "varchar(50)")]
		public string Username { get; set; }
		[Column(TypeName = "varchar(50)")]
		public string Password { get; set; }
		[Column(TypeName = "varchar(50)")]
		public string Role { get; set; }
		[Column(TypeName = "varchar(50)")]
		public string Name { get; set; }
		[Column(TypeName = "varchar(150)")]
		public string? Email { get; set; }
	}
}
