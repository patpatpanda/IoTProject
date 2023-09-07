using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace MediumDevice.Services
{
	internal class DeviceManager
	{
		public  DeviceConfig Configuration { get; set; }
		public DeviceManager(string connectionString)
		{
			Configuration = new DeviceConfig(connectionString);
		}

		public async Task StartAsync()
		{
			while (true)

			{
				var data = new
				{
					Temperature = 20,
					Humidty = 44,
					Created = DateTime.Now
				};

				await SendDataAsync(JsonConvert.SerializeObject(data));
				await Task.Delay(Configuration.TelemetryInterval);
			}

		}

		public async Task SendDataAsync(string dataAsJson)
		{
			if (!string.IsNullOrEmpty(dataAsJson))
			{
				var message = new Message(Encoding.UTF8.GetBytes(dataAsJson));
				await Configuration.DeviceClient.SendEventAsync(message);
			}
		}

	}
}
