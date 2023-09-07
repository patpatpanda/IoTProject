using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedDevice.Services
{
	public class LampService
	{
		
		private bool isOn;
		public string DeviceName { get; set; } = null!;
		public string DeviceColor { get; set; } = null!;
		public DateTime Created { get; set; }




		public LampService()
		{
			isOn = false; // Lamp is initially off
		}

		public void TurnOn()
		{
			Console.Clear();
			isOn = true;
			Console.WriteLine("Lamp is now on.");
		}

		public void TurnOff()
		{
			Console.Clear();
			isOn = false;
			Console.WriteLine("Lamp is now off.");
		}

		public bool IsOn()
		{
			return isOn;
		}
	}
}
