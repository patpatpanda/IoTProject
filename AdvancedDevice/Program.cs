using AdvancedDevice.Data;
using AdvancedDevice.DeviceManager;
using AdvancedDevice.Services;
using Microsoft.Azure.Devices.Client;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using Newtonsoft.Json;
using System.Net;using Microsoft.Azure.Devices;


var lampService = new LampService();



var device =
	new DeviceManager(
		"HostName=iot-warrior.azure-devices.net;DeviceId=red;SharedAccessKey=Fu2Rgn+gGg3aNZoiFBhztVPtotfbxeifAR/Dmi4ZBhw=",lampService);



device.Start();

Console.ReadKey();


