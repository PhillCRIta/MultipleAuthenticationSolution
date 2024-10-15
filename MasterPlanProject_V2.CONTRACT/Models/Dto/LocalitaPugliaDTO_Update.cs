using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractLibrary
{
	public class LocalitaPugliaDTO_Update
	{
        [Required(ErrorMessage = ("Id obbligatorio"))]
        [Range (-1,100, ErrorMessage = "Valori id compresi fra -1 e 100")]
        public int Id { get; set; }
        public string Area { get; set; }
        public string Localita { get; set; }
    }
}
