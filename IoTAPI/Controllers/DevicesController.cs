using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Threading.Tasks; // Added to allow asynchronous methods

[Route("api/devices")]
[ApiController]
public class DevicesController : ControllerBase
{
	[HttpGet("{deviceId}")]
	public async Task<IActionResult> IsDeviceRegistered(string deviceId)
	{
		// Check if the device is registered and return a response
		string iotHubConnectionString =
			"HostName=iot-warrior.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=fUwugjRnWfRPHa5sB+yBDMO7Oqzg7yku6AIoTKh4Z5Q=";

		// Initialize the IoT Hub RegistryManager:
		RegistryManager registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);

		try
		{
			// Try to retrieve the device by its ID.
			Device device = await registryManager.GetDeviceAsync(deviceId);

			// If the device is found, it's registered.
			bool isRegistered = device != null;

			if (isRegistered)
			{
				Console.WriteLine($"Device {deviceId} is registered in Azure IoT Hub.");
				return Ok(isRegistered); // Return a 200 OK response
			}
			else
			{
				Console.WriteLine($"Device {deviceId} is not registered in Azure IoT Hub.");
				return NotFound(); // Return a 404 Not Found response
			}
		}
		catch (DeviceNotFoundException)
		{
			// If DeviceNotFoundException is thrown, the device is not registered.
			Console.WriteLine($"Device {deviceId} is not registered in Azure IoT Hub.");
			return NotFound(); // Return a 404 Not Found response
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error checking device registration: {ex.Message}");
			return StatusCode(500, $"Internal Server Error: {ex.Message}"); // Return a 500 Internal Server Error response
		}
		finally
		{
			// Dispose of the RegistryManager when done.
			await registryManager.CloseAsync();
		}
	}


	[HttpPost("{deviceId}")]
	public IActionResult RegisterDevice(string deviceId)
	{
		try
		{
			// Initialize the Azure IoT Hub service client
			string iotHubConnectionString = "HostName=iot-warrior.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=fUwugjRnWfRPHa5sB+yBDMO7Oqzg7yku6AIoTKh4Z5Q=";
			RegistryManager registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);

			// Check if the device already exists
			Device existingDevice = registryManager.GetDeviceAsync(deviceId).Result;
			if (existingDevice != null)
			{
				return BadRequest("Device already exists in Azure IoT Hub");
			}

			// Create a new device
			var newDevice = new Device(deviceId);
			newDevice = registryManager.AddDeviceAsync(newDevice).Result;

			// Check if the registration was successful
			if (newDevice != null)
			{
				return Ok("Device registered successfully");
			}
			else
			{
				return BadRequest("Device registration failed");
			}
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal Server Error: {ex.Message}");
		}
	}

}
