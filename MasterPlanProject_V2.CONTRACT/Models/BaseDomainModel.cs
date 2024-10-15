using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractLibrary.Models
{
    public abstract class BaseDomainModel
    {
		[DataType(DataType.DateTime)]
		[Column(TypeName = "DateTime2(0)")]
		public DateTime DataInserimento { get; set; } = DateTime.Now;
    }
}
