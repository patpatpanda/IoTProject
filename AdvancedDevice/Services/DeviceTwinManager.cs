using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

namespace AdvancedDevice.Services
{
	internal class DeviceTwinManager
	{
		public static async Task<object> GetDesiredTwiPropnAsync(DeviceClient deviceClient, string key)
		{
			try
			{
				var twin = await deviceClient.GetTwinAsync();
				if (twin.Properties.Desired.Contains(key))
					return twin.Properties.Desired[key];

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				
			}

			return null;
		}



		public static async Task UpdateReportedTwinAsync(DeviceClient deviceClient, string key, object value)
		{
			try
			{
				var twinProps = new TwinCollection();
				twinProps[key] = value;
				await deviceClient.UpdateReportedPropertiesAsync(twinProps);
				Console.WriteLine($"Reported property: {key} updated with value: {value}");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}


		private async Task UpdateDeviceIdInReportedProperties(string deviceId)
		{
			try
			{
				// Create or retrieve the DeviceClient
				// Replace with your actual connection string
				string iotHubConnectionString = "HostName=iot-warrior.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=fUwugjRnWfRPHa5sB+yBDMO7Oqzg7yku6AIoTKh4Z5Q=";
				DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(iotHubConnectionString);

				// Get the current device twin
				var deviceTwin = await deviceClient.GetTwinAsync();

				// Update the deviceId in reported properties
				var reportedProperties = new TwinCollection();
				reportedProperties["deviceId"] = "red"; // Replace with the desired deviceId
				await deviceClient.UpdateReportedPropertiesAsync(reportedProperties);

				Console.WriteLine("DeviceId in reported properties updated to 'red'");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error updating reported properties: {ex.Message}");
			}
		}
	}



}
