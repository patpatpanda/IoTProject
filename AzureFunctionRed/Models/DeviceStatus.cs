
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AzureFunctionRed.Models
{
	public class DeviceStatus
	{
		public int Id { get; set; }
		public DateTime? Date { get; set; }
		public string? DeviceMessage { get; set; }
	}
}
