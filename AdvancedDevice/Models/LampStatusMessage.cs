using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedDevice.Models
{
	public class LampStatusMessage
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public bool IsOn { get; set; } 
	}
}
