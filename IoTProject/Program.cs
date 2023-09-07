using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

DeviceClient deviceClient = DeviceClient.CreateFromConnectionString("HostName=iot-warrior.azure-devices.net;DeviceId=simple_device;SharedAccessKey=vWgTQqq1G5s087bSIXmL1KOKZtRgkvGIhWaDKhBwbR8=", TransportType.Mqtt);

var data = new
{
	Message = "Mitt meddelande",
	Created = DateTime.Now
};

await SendTelemetryAsync(JsonConvert.SerializeObject(data));
async Task SendTelemetryAsync(string payloadAsJson)
{
	while (true)
	{
		var message = new Message(Encoding.UTF8.GetBytes(payloadAsJson));
		await deviceClient.SendEventAsync(message);

		Console.WriteLine("Message sent" + DateTime.Now + "with " + payloadAsJson);

		await Task.Delay(5000);
	}
	

}