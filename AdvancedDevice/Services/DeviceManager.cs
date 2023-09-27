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
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client.Exceptions;
using Message = Microsoft.Azure.Devices.Client.Message;

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

			Configuration.DeviceClient.SetMethodHandlerAsync("SetTelemetryInterval", SetTelemetryIntervalMethod, null)
				.Wait();
		}




		public void Start()
		{
			string deviceId = "Test_Device"; // Replace with the actual device ID you want to check
			string iotHubConnectionString = "HostName=iot-warrior.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=fUwugjRnWfRPHa5sB+yBDMO7Oqzg7yku6AIoTKh4Z5Q=";
			string apiBaseUrl = "https://deviceapi20230918091812.azurewebsites.net/api/devices";

			

		

				



			Task.WhenAll(
				SetTelemetryIntervalAsync()
				,
				SendTelemetryAsync(), ProcessDeviceRegistration(deviceId, iotHubConnectionString, apiBaseUrl));







			ListenForUserInput();


		}

		private async void InitializeIoTDevice()
		{
			try
			{
				string iotHubConnectionString =
					"HostName=iot-warrior.azure-devices.net;DeviceId=Lamp_Device;SharedAccessKey=et+aBpSlOWW3gZDIwajcw1HHNbXSo7Ss4Q0EYwe0IK0=";
				deviceClient = DeviceClient.CreateFromConnectionString(iotHubConnectionString);



				string deviceId = Guid.NewGuid().ToString();
				var twinProps = new TwinCollection();




				string message = "This is the last message1337";
				await InitializeIoTDeviceMessage(message);




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
				.UpdateReportedTwinAsync(Configuration.DeviceClient, "telemetryInterval",
					Configuration.TelemetryInterval);
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
				await DeviceTwinManager.UpdateReportedTwinAsync(Configuration.DeviceClient, "telemetryInterval",
					Configuration.TelemetryInterval);

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
					await DeviceTwinManager.UpdateReportedTwinAsync(Configuration.DeviceClient, "allowSending",
						Configuration.AllowSending);

					// Turn on the lamp simulator
					_lampService.TurnOn();

					return new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response)), 200);

				case "off":
					Configuration.AllowSending = false;
					await DeviceTwinManager.UpdateReportedTwinAsync(Configuration.DeviceClient, "allowSending",
						Configuration.AllowSending);

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

		static async Task<bool> IsDeviceRegisteredAsync(string deviceId, string iotHubConnectionString)
		{
			try
			{
				RegistryManager registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);

				Device device = await registryManager.GetDeviceAsync(deviceId);

				return device != null;
			}
			catch (DeviceNotFoundException)
			{
				// If DeviceNotFoundException is thrown, the device is not registered.
				return false;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error checking device registration: {ex.Message}");
				return false;
			}
		}

		async Task ProcessDeviceRegistration(string deviceId, string iotHubConnectionString, string apiBaseUrl)
		{
			bool isDeviceRegistered = await IsDeviceRegisteredAsync(deviceId, iotHubConnectionString);

			if (isDeviceRegistered)
			{
				Console.WriteLine($"Device {deviceId} is registered in Azure IoT Hub.");
			}
			else
			{
				Console.WriteLine($"Device {deviceId} is not registered in Azure IoT Hub.");

				// Register the device
				bool registrationSuccess = await RegisterDeviceAsync(deviceId, iotHubConnectionString, apiBaseUrl);

				if (registrationSuccess)
				{
					Console.WriteLine($"Device {deviceId} was registered successfully.");
					// Save the connection information locally
					// SaveConnectionInfoLocally(deviceId, iotHubConnectionString);
				}
				else
				{
					Console.WriteLine($"Device {deviceId} registration failed.");
				}
			}
		}


		// ... The rest of your code ...

		static async Task<bool> RegisterDeviceAsync(string deviceId, string iotHubConnectionString, string apiBaseUrl)
		{
			try
			{
				using (HttpClient httpClient = new HttpClient())
				{
					var registrationData = new
					{
						DeviceId = deviceId,
						// Add other properties as needed for device registration
					};

					var jsonContent = JsonConvert.SerializeObject(registrationData);

					var requestContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

					var response =
						await httpClient.PostAsync($"{apiBaseUrl}/register",
							requestContent); // Use the correct API endpoint

					if (response.IsSuccessStatusCode)
					{
						return true;
					}
					else
					{
						Console.WriteLine($"Device registration failed with status code: {response.StatusCode}");
						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error registering device: {ex.Message}");
				return false;
			}


		}
	}
}
