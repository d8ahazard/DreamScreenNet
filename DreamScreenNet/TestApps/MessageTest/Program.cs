using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using DreamScreenNet;
using DreamScreenNet.Devices;
using DreamScreenNet.Enum;
using Newtonsoft.Json;

namespace MessageTest {
	internal class Program {
		private static List<DreamDevice> _devices;

		private static async Task Main(string[] args) {
			var client = new DreamScreenClient();
			_devices = new List<DreamDevice>();
			client.DeviceDiscovered += AddDevice; // Handler for when a device is discovered
			client.CommandReceived += LogCommand; // Handle incoming messages
			client.StartDeviceDiscovery(); // Start discovery, wait for 5s
			await Task.Delay(5000);
			client.StopDeviceDiscovery(); // Stop discovery
			// Do stuff with found devices
			if (_devices.Count > 0) {
				Console.WriteLine($"Found {_devices.Count} devices.");
				var dev = _devices[0];
				if (dev != null) {
					var res = await client.SetMode(dev, DeviceMode.Video);
					Console.WriteLine("Mode Res: " + JsonConvert.SerializeObject(res));
					res = await client.SetAmbientColor(dev, Color.Crimson);
					Console.WriteLine("Col Res: " + JsonConvert.SerializeObject(res));
					res = await client.SetAmbientMode(dev, AmbientMode.Scene);
					Console.WriteLine("Mode Res: " + JsonConvert.SerializeObject(res));
					res = await client.SetAmbientShow(dev, AmbientShow.Forest);
					Console.WriteLine("Mode Res: " + JsonConvert.SerializeObject(res));
					while (true) {
						Console.WriteLine("Press ctrl+C to exit.");
					 // Hang out and wait for ctrl+c
					}

					await client.SetMode(dev, DeviceMode.Off);
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