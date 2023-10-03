
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
		
		public DeviceClient DeviceClient { get; set; } = null!;

		
		
		public int TelemetryInterval { get; set; } = 10000;
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
