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
using AdvancedDevice.Data;
using AdvancedDevice.Models;

namespace AdvancedDevice.DeviceManager
{
	internal class DeviceManager
	{
		public DeviceConfig Configuration { get; set; }
		private LampService _lampService;

		private DeviceClient deviceClient;


		public DeviceManager(string connectionString, LampService lampService)
		{

			_lampService = lampService;
			Configuration = new DeviceConfig(connectionString);
			Configuration.DeviceClient.SetMethodDefaultHandlerAsync(DirectMethodCallback, null).Wait();
			InitializeIoTDevice();

			Configuration.DeviceClient.SetMethodHandlerAsync("SetTelemetryInterval", SetTelemetryIntervalMethod, null).Wait();
		}




		public void Start()
		{




			Task.WhenAll(
				SetTelemetryIntervalAsync()
				,
				SendTelemetryAsync());






			ListenForUserInput();


		}
		private async void InitializeIoTDevice()
		{
			try
			{
				string iotHubConnectionString = "HostName=iot-warrior.azure-devices.net;DeviceId=red;SharedAccessKey=Fu2Rgn+gGg3aNZoiFBhztVPtotfbxeifAR/Dmi4ZBhw=";
				deviceClient = DeviceClient.CreateFromConnectionString(iotHubConnectionString);

				var deviceTwin = await deviceClient.GetTwinAsync();
				if (!deviceTwin.Properties.Reported.Contains("deviceId"))
				{
					string deviceId = Guid.NewGuid().ToString();
					var twinProps = new TwinCollection();
					twinProps["deviceId"] = deviceId;
					await deviceClient.UpdateReportedPropertiesAsync(twinProps);
					Console.WriteLine($"Device registered with ID: {deviceId}");
				}
				else
				{
					string deviceId = deviceTwin.Properties.Reported["deviceId"].ToString();
					Console.WriteLine($"Device already registered with ID: {deviceId}");
					string message = "This is the last message";
					await InitializeIoTDeviceMessage(message);

				}

				// Här anropar du InitializeIoTDeviceMessage() för att hantera meddelanden

			}
			catch (Exception e)
			{
				Console.WriteLine($"Error checking device registration: {e.Message}");
			}

		}

		public async Task InitializeIoTDeviceMessage(string message)
		{
			try
			{

				var twinProps = new TwinCollection();
				twinProps["latestMessage"] = message;
				await deviceClient.UpdateReportedPropertiesAsync(twinProps);
				Console.WriteLine($"Latest message in Device Twin updated: {message}");
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error updating latest message in Device Twin: {e.Message}");
			}
		}




		public async Task SetTelemetryIntervalAsync()
		{
			var _telemetryInterval = await DeviceTwinManager
				.GetDesiredTwiPropnAsync(Configuration.DeviceClient, "telemetryInterval");

			if (_telemetryInterval != null)
				Configuration.TelemetryInterval = int.Parse(_telemetryInterval.ToString()!);

			await DeviceTwinManager
				.UpdateReportedTwinAsync(Configuration.DeviceClient, "telemetryInterval", Configuration.TelemetryInterval);
		}

		private async Task<MethodResponse> SetTelemetryIntervalMethod(MethodRequest methodRequest, object userContext)
		{
			try
			{
				// Hämta och deserialisera JSON-payloaden till TelemetryIntervalPayload
				var payloadJson = methodRequest.DataAsJson;
				var payload = JsonConvert.DeserializeObject<TelemetryIntervalPayload>(payloadJson);

				// Uppdatera konfigurationen med det nya telemetry-intervallet
				Configuration.TelemetryInterval = payload.Interval;

				// Uppdatera Device Twin för att reflektera det nya intervallet
				await DeviceTwinManager.UpdateReportedTwinAsync(Configuration.DeviceClient, "telemetryInterval", Configuration.TelemetryInterval);

				var responsePayload = new
				{
					message = $"Telemetry interval set to {Configuration.TelemetryInterval} seconds."
				};

				return new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responsePayload)), 200);
			}
			catch (Exception ex)
			{
				var errorPayload = new
				{
					message = $"Error: {ex.Message}"
				};
				return new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(errorPayload)), 500);
			}
		}



		private async Task SendTelemetryAsync()
		{
			while (true)
			{
				if (Configuration.AllowSending)
				{
					// Check the lamp state and include it in telemetry data
					bool lampState = _lampService.IsOn();

					string lampMessage = lampState ? "The lamp is on" : "The lamp is off";

					var telemetryData = new DeviceInfo()
					{

						Date = DateTime.Now,


						DeviceMessage = lampMessage,

					};

					var telemetryJson = JsonConvert.SerializeObject(telemetryData);

					await SendDataAsync(telemetryJson);

				}

				await Task.Delay(Configuration.TelemetryInterval);
			}
		}


		private async Task SendLampStatusAsync()
		{
			while (true)
			{
				if (Configuration.AllowSending)
				{
					// Check the lamp state
					bool lampState = _lampService.IsOn();

					// Create a message to represent the lamp status
					var lampStatusMessage = new LampStatusMessage()
					{
						Date = DateTime.Now,
						IsOn = lampState
					};

					// Serialize the message to JSON
					var lampStatusJson = JsonConvert.SerializeObject(lampStatusMessage);

					// Send the lamp status data
					await SendDataAsync(lampStatusJson);
				}

				await Task.Delay(Configuration.TelemetryInterval);
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
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.WriteLine("             LAMP DEVICE                 ");
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine();
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