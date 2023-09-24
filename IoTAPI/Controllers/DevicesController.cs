using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks; // Added to allow asynchronous methods

[Route("api/devices")]
[ApiController]
public class DevicesController : ControllerBase
{
	private readonly string iotHubConnectionString =
		"HostName=iot-warrior.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=fUwugjRnWfRPHa5sB+yBDMO7Oqzg7yku6AIoTKh4Z5Q=";

	[HttpGet("{deviceId}")]
	public async Task<IActionResult> IsDeviceRegistered(string deviceId)
	{
		try
		{
			RegistryManager registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);

			Device device = await registryManager.GetDeviceAsync(deviceId);

			bool isRegistered = device != null;

			if (isRegistered)
			{
				return Ok(new { IsRegistered = true, Message = $"Device {deviceId} is registered in Azure IoT Hub." });
			}
			else
			{
				return NotFound(new
					{ IsRegistered = false, Message = $"Device {deviceId} is not registered in Azure IoT Hub." });
			}
		}
		catch (DeviceNotFoundException)
		{
			// If DeviceNotFoundException is thrown, the device is not registered.
			return NotFound(new
				{ IsRegistered = false, Message = $"Device {deviceId} is not registered in Azure IoT Hub." });
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { IsRegistered = false, ErrorMessage = $"Internal Server Error: {ex.Message}" });
			string iotHubConnectionString =
				"HostName=your-iothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=your-shared-access-key";
		}
	}

	[HttpPost("register")]
	public async Task<IActionResult> RegisterDevice([FromBody] DeviceRegistrationModel registrationModel)
	{
		try
		{
			RegistryManager registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);

			// Check if the device already exists
			Device existingDevice = await registryManager.GetDeviceAsync(registrationModel.DeviceId);
			if (existingDevice != null)
			{
				return Conflict(new
				{
					IsRegistered = true,
					Message = $"Device {registrationModel.DeviceId} already exists in Azure IoT Hub."
				});
			}

			// Create a new device
			Device newDevice = new Device(registrationModel.DeviceId);
			newDevice = await registryManager.AddDeviceAsync(newDevice);

			return CreatedAtAction(nameof(IsDeviceRegistered), new { deviceId = newDevice.Id }, new
			{
				IsRegistered = true,
				Message = $"Device {registrationModel.DeviceId} registered successfully in Azure IoT Hub."
			});
		}
		catch (Exception ex)
		{
			return StatusCode(500, new
			{
				IsRegistered = false,
				ErrorMessage = $"Internal Server Error: {ex.Message}"
			});
		}
	}




	public class DeviceRegistrationModel
	{
		public string DeviceId { get; set; }
		// Add other properties as needed for device registration
	}


}



