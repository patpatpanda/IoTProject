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
		"HostName=iot-warrior.azure-devices.net;DeviceId=Lamp_Device;SharedAccessKey=et+aBpSlOWW3gZDIwajcw1HHNbXSo7Ss4Q0EYwe0IK0=", lampService);




device.Start();








