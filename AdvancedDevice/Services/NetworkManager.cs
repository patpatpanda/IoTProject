using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedDevice.Services
{
	public class NetworkManager
	{
		private static readonly Ping ping = new();
		public static bool isConnected = false;

		private static int HeartbeatInterval = 100000;


		public static bool GetConnectedAsync() => isConnected;
		public static async Task CheckConnectivityAsync()
		{
			while (true)
			{
				isConnected = await SendPingAsync("8.8.8.8");
				Console.WriteLine(isConnected ? "Connected" : "Disconnected");
				await Task.Delay(HeartbeatInterval);
			}
		}


		private static async Task<bool> SendPingAsync(string ipAddress)
		{
			try
			{
				var reply = await ping.SendPingAsync(ipAddress);
				return reply.Status == IPStatus.Success;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

	}
}
