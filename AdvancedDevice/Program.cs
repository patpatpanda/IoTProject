using AdvancedDevice.DeviceManager;
using AdvancedDevice.Services;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using Newtonsoft.Json;
using System.Net;

var device =
	new DeviceManager(
		"HostName=iot-warrior.azure-devices.net;DeviceId=red;SharedAccessKey=Fu2Rgn+gGg3aNZoiFBhztVPtotfbxeifAR/Dmi4ZBhw=");



device.Start();

Console.ReadKey();


