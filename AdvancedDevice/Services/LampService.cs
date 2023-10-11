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

		public void TurnOn()
		{
			if (!isOn)
			{
				isOn = true;
				Console.Clear();
				Console.WriteLine("Device is now on.");
			}
		}

		public void TurnOff()
		{
			if (isOn)
			{
				isOn = false;
				Console.Clear();
				Console.WriteLine("Device is now off.");
			}
		}

		public bool IsOn()
		{
			return isOn;
		}
	}

}
