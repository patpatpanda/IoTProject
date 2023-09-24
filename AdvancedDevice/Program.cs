using AdvancedDevice.Data;
using AdvancedDevice.DeviceManager;
using AdvancedDevice.Services;
using Microsoft.Azure.Devices.Client;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client.Exceptions;


var lampService = new LampService();



var device =
	new DeviceManager(
		"HostName=iot-warrior.azure-devices.net;DeviceId=red;SharedAccessKey=Fu2Rgn+gGg3aNZoiFBhztVPtotfbxeifAR/Dmi4ZBhw=",lampService);


string deviceId = "red"; // Replace with the actual device ID you want to check
string iotHubConnectionString = "HostName=iot-warrior.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=fUwugjRnWfRPHa5sB+yBDMO7Oqzg7yku6AIoTKh4Z5Q=";
string apiBaseUrl = "https://deviceapi20230918091812.azurewebsites.net/api/devices";

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


device.Start();
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

            var response = await httpClient.PostAsync($"{apiBaseUrl}/register", requestContent); // Use the correct API endpoint

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






