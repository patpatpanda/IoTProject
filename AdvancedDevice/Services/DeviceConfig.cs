using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

namespace AdvancedDevice.DeviceManager
{
	internal class DeviceConfig
	{
		public DeviceConfig(string connectionString)
		{
			ConnectionString = connectionString;
			Initialize();
		}

		public string DeviceId { get; set; } = null!;
		public string ConnectionString { get; private set; } = null!;
		public string Color { get; set; }
		public DeviceClient DeviceClient { get; set; } = null!;

		
		
		public int TelemetryInterval { get; set; } = 100000;
		public bool AllowSending { get; set; } = true;

		
		



		public void Initialize()
		{
			if (ConnectionString != null)
			{

				DeviceId = ConnectionString.Split(";")[1].Split("=")[1];
				DeviceClient = DeviceClient.CreateFromConnectionString(ConnectionString, TransportType.Mqtt);
			}
		}
		

		
	}

}
