using MediumDevice.Services;
using Newtonsoft.Json;

var device = new DeviceManager("HostName=iot-warrior.azure-devices.net;DeviceId=blue;SharedAccessKey=PcCVzFjlMqhoqyhS2GOmWx0lhKX960MsimKpn0FMOcs=");


await device.StartAsync();