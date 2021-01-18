using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi3.Models
{
	public class Distribuidor
	{
		public string code { get; set; }
		public List<Asociado> asoc { get; set; }
	}
}