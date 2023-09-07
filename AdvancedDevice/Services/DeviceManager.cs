using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedDevice.Services;
using Microsoft.Azure.Devices.Shared;

namespace AdvancedDevice.DeviceManager
{
	internal class DeviceManager 
	{
		public DeviceConfig Configuration { get; set; }
		private LampService _lampService = new LampService();
		
		public DeviceManager(string connectionString)
		{
			Configuration = new DeviceConfig(connectionString);
			Configuration.DeviceClient.SetMethodDefaultHandlerAsync(DirectMethodCallback, null).Wait();
		}

		public async void StartAsync()
		{
			
		}
		

		public void Start()
		{

			
			Task.WhenAll(NetworkManager.CheckConnectivityAsync(),
				SetTelemetryIntervalAsync()
				,
				SendTelemetryAsync());

			ListenForUserInput();
			
		}

	  public	async Task Twin()
		{
			
		}

		private async Task SetTelemetryIntervalAsync()
		{
			var _telemetryInterval = await DeviceTwinManager
				.GetDesiredTwiPropnAsync(Configuration.DeviceClient, "telemetryInterval");

			if (_telemetryInterval != null)
				Configuration.TelemetryInterval = int.Parse(_telemetryInterval.ToString()!);

			await DeviceTwinManager
				.UpdateReportedTwinAsync(Configuration.DeviceClient, "telemetryInterval", Configuration.TelemetryInterval);
		}



		private async Task SendTelemetryAsync()
		{
		
			while (true)
			{
				
				// Check the lamp state and include it in telemetry data
				bool lampState = _lampService.IsOn();

				if (Configuration.AllowSending)
				{
					var data = new LampService()
					{
						DeviceName = "Mr Lampa",
						Created = DateTime.Now,
						DeviceColor = "Yellow",
						
						
					};
					
					var json = JsonConvert.SerializeObject(data);
					await SendDataAsync(json);
					await Task.Delay(Configuration.TelemetryInterval);

					
				}
			}
		}

		public async Task SendDataAsync(string dataAsJson)
		{
			
			
				if (!string.IsNullOrEmpty(dataAsJson))
				{
					var message = new Message(Encoding.UTF8.GetBytes(dataAsJson));
					await Configuration.DeviceClient.SendEventAsync(message);
					//Console.WriteLine($"Message sent at {DateTime.Now} with data {dataAsJson}");

}
			
			
		}

		private async Task<MethodResponse> DirectMethodCallback(MethodRequest methodRequest, object userContext)
		{
			var response = new
			{
				messsage = $"Executed Direct Method: {methodRequest.Name}",
			};

			switch (methodRequest.Name.ToLower())
			{
				case "on":
					Configuration.AllowSending = true;
					await DeviceTwinManager.UpdateReportedTwinAsync(Configuration.DeviceClient, "allowSending", Configuration.AllowSending);

					// Turn on the lamp simulator
					_lampService.TurnOn();

					return new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response)), 200);

				case "off":
					Configuration.AllowSending = false;
					await DeviceTwinManager.UpdateReportedTwinAsync(Configuration.DeviceClient, "allowSending", Configuration.AllowSending);

					// Turn off the lamp simulator
					_lampService.TurnOff();

					return new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response)), 200);

				default:
					return new MethodResponse(400);
			}
		}
		private void ListenForUserInput()
		{
			while (true)
			{
				
				Console.WriteLine("-----------------------------------------");
				Console.WriteLine("-----------------------------------------");
				Console.WriteLine("║   1 = ON                               ║");
				Console.WriteLine("║   2 = OFF                              ║");
				Console.WriteLine("║   3 = EXIT                             ║");
				Console.WriteLine();
				Console.WriteLine();
				Console.Write("Input : ");
				
				
				
					string userInput = Console.ReadLine()?.ToLower();
				Console.Clear();
				if (userInput == "1")
				{
					_lampService.TurnOn();
					// Uppdatera senaste status i Device Twin
					DeviceTwinManager.UpdateReportedTwinAsync(Configuration.DeviceClient, "lampStatus", "On").Wait();
					
				}
				else if (userInput == "2")
				{
					
						_lampService.TurnOff();
					// Uppdatera senaste status i Device Twin
					DeviceTwinManager.UpdateReportedTwinAsync(Configuration.DeviceClient, "lampStatus", "Off").Wait();
					
				}
				else if (userInput == "3")
				{
					Environment.Exit(0);
				}
				else
				{
					Console.WriteLine("Invalid command. Try again.");
				}
			}
		}
		

	}
		}

	

	

