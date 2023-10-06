using System.Text;
using AdvancedDevice.Models;
using AdvancedDevice.Services;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Message = Microsoft.Azure.Devices.Client.Message;

namespace AdvancedDevice.DeviceManager;

internal class DeviceManager
{
	private readonly LampService _lampService;
	private DeviceClient deviceClient;
	private readonly IConfiguration _configuration;

	public DeviceManager(IConfiguration configuration, LampService lampService)
	{
		_lampService = lampService;
		_configuration = configuration;
		Configuration = new DeviceConfig(_configuration.GetConnectionString("IoTHubConnectionString"));
	}

	public DeviceConfig Configuration { get; set; }

	public void Start()
	{
		Configuration.DeviceClient.SetMethodDefaultHandlerAsync(DirectMethodCallback, null).Wait();

		Configuration.DeviceClient.SetMethodHandlerAsync("SetTelemetryInterval", SetTelemetryIntervalMethod, null).Wait();

		var deviceId = "LampDevice";
		var iotHubOwnerConnectionString = _configuration.GetConnectionString("IoTHubOwnerConnectionString");
		var apiBaseUrl = _configuration["ApiBaseUrl"];

		Task.WhenAll(
			SetTelemetryIntervalAsync(),
			SendTelemetryAsync(),
			ProcessDeviceRegistration(deviceId, iotHubOwnerConnectionString, apiBaseUrl),
			InitializeIoTDevice());

		ListenForUserInput();
	}

	private async Task InitializeIoTDevice()
	{
		try
		{
			var iotHubConnectionString = _configuration.GetConnectionString("IoTHubConnectionString");
			deviceClient = DeviceClient.CreateFromConnectionString(iotHubConnectionString);

			var deviceId = Guid.NewGuid().ToString();
			var twinProps = new TwinCollection();

			var message = "This is the last message1337";
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
			var payloadJson = methodRequest.DataAsJson;
			var payload = JsonConvert.DeserializeObject<TelemetryIntervalPayload>(payloadJson);


			Configuration.TelemetryInterval = payload.Interval;


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
				var lampState = _lampService.IsOn();

				var lampMessage = lampState ? "The lamp is on" : "The lamp is off";

				var telemetryData = new DeviceInfo
				{
					Date = DateTime.Now,


					DeviceMessage = lampMessage
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
				var lampState = _lampService.IsOn();


				var lampStatusMessage = new LampStatusMessage
				{
					Date = DateTime.Now,
					IsOn = lampState
				};


				var lampStatusJson = JsonConvert.SerializeObject(lampStatusMessage);


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
		}
	}

	private async Task<MethodResponse> DirectMethodCallback(MethodRequest methodRequest, object userContext)
	{
		var response = new
		{
			messsage = $"Executed Direct Method: {methodRequest.Name}"
		};

		switch (methodRequest.Name.ToLower())
		{
			case "on":
				Configuration.AllowSending = true;
				await DeviceTwinManager.UpdateReportedTwinAsync(Configuration.DeviceClient, "allowSending",
					Configuration.AllowSending);


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


			var userInput = Console.ReadLine()?.ToLower();
			Console.Clear();
			if (userInput == "1")
			{
				_lampService.TurnOn();

				DeviceTwinManager.UpdateReportedTwinAsync(Configuration.DeviceClient, "lampStatus", "on").Wait();
			}
			else if (userInput == "2")
			{
				_lampService.TurnOff();

				DeviceTwinManager.UpdateReportedTwinAsync(Configuration.DeviceClient, "lampStatus", "off").Wait();
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

	private static async Task<bool> IsDeviceRegisteredAsync(string deviceId, string iotHubConnectionString)
	{
		try
		{
			var registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);

			var device = await registryManager.GetDeviceAsync(deviceId);

			return device != null;
		}
		catch (DeviceNotFoundException)
		{
			return false;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error checking device registration: {ex.Message}");
			return false;
		}
	}

	private async Task ProcessDeviceRegistration(string deviceId, string iotHubConnectionString, string apiBaseUrl)
	{
		var isDeviceRegistered = await IsDeviceRegisteredAsync(deviceId, iotHubConnectionString);

		if (isDeviceRegistered)
		{
			Console.WriteLine($"Device {deviceId} is registered in Azure IoT Hub.");
		}
		else
		{
			Console.WriteLine($"Device {deviceId} is not registered in Azure IoT Hub.");

			var registrationSuccess = await RegisterDeviceAsync(deviceId, iotHubConnectionString, apiBaseUrl);

			if (registrationSuccess)
				Console.WriteLine($"Device {deviceId} was registered successfully.");
			else
				Console.WriteLine($"Device {deviceId} registration failed.");
		}
	}


	private static async Task<bool> RegisterDeviceAsync(string deviceId, string iotHubConnectionString,
		string apiBaseUrl)
	{
		try
		{
			using (var httpClient = new HttpClient())
			{
				var registrationData = new
				{
					DeviceId = deviceId
				};

				var jsonContent = JsonConvert.SerializeObject(registrationData);

				var requestContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

				var response =
					await httpClient.PostAsync($"{apiBaseUrl}/register",
						requestContent);

				if (response.IsSuccessStatusCode)
				{
					return true;
				}

				Console.WriteLine($"Device registration failed with status code: {response.StatusCode}");
				return false;
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error registering device: {ex.Message}");
			return false;
		}
	}
}