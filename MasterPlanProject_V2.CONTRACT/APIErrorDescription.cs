﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ContractLibrary
{
	public class APIErrorDescription
    {
        public string Message { get; set; }
        public List<string> ListMessage { get; set; } = new List<string>();
    }
}
