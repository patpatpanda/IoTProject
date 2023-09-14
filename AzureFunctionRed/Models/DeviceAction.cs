using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionRed.Models
{
	public class DeviceAction
	{
		public int Id { get; set; }
		public DateTime? Date { get; set; }
		public bool IsOn { get; set; }
	}
}
