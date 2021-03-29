using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using DreamScreenNet;
using DreamScreenNet.Devices;
using DreamScreenNet.Enum;
using Newtonsoft.Json;

namespace StreamTest {
	class Program {
		private static List<DreamDevice> _devices;
		static async Task Main(string[] args) {
			var client = new DreamScreenClient();
			_devices = new List<DreamDevice>();
			client.DeviceDiscovered += AddDevice;
			client.CommandReceived += LogCommand;
			client.StartDeviceDiscovery();
			await Task.Delay(5000);
			client.StopDeviceDiscovery();
			if (_devices.Count > 0) {
				Console.WriteLine($"Found {_devices.Count} devices.");
				var dev = _devices[0];
				if (dev != null) {
					var res = await client.SetMode(DeviceMode.Video, dev.IpAddress, dev.DeviceGroup);
					Console.WriteLine("Mode Res: " + JsonConvert.SerializeObject(res));
					res = await client.SetAmbientColor(Color.Crimson, dev.IpAddress, dev.DeviceGroup);
					Console.WriteLine("Col Res: " + JsonConvert.SerializeObject(res));
					res = await client.SetAmbientMode(AmbientMode.Scene, dev.IpAddress, dev.DeviceGroup);
					Console.WriteLine("Mode Res: " + JsonConvert.SerializeObject(res));
					res = await client.SetAmbientShow(AmbientShow.Forest, dev.IpAddress, dev.DeviceGroup);
					Console.WriteLine("Mode Res: " + JsonConvert.SerializeObject(res));
				}

				while (true) {
					
				}
			}

			
			
		}

		private static void LogCommand(object? sender, DreamScreenClient.MessageEventArgs e) {
			Console.WriteLine($"We got us a command from {e.Response.Target}: " + e.Response.Type);
		}

		private static void AddDevice(object? sender, DreamScreenClient.DeviceDiscoveryEventArgs e) {
			_devices.Add(e.Device);
		}
		
		
	}
}