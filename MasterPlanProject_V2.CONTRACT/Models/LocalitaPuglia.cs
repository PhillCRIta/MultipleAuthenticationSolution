using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractLibrary.Models
{
    public class LocalitaPuglia : BaseDomainModel
    {
        public int Id { get; set; }
		[Column(TypeName = "varchar(150)")]
		public string Area { get; set; }
		[Column(TypeName = "varchar(150)")]
		public string Localita { get; set; }
    }
}
