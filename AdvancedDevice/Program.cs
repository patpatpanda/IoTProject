using AdvancedDevice.Data;
using AdvancedDevice.DeviceManager;
using AdvancedDevice.Services;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client.Exceptions;

var configurationBuilder = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var configuration = configurationBuilder.Build();


//var iotHubConnectionString = configuration.GetConnectionString("IoTHubConnectionString");

var lampService = new LampService();

var device = new DeviceManager(configuration, lampService);



device.Start();