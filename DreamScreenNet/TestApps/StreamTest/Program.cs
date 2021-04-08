using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DreamScreenNet;
using DreamScreenNet.Devices;
using DreamScreenNet.Enum;
using Newtonsoft.Json;

namespace StreamTest {
	internal class Program {
		private static List<DreamDevice> _devices;

		private static async Task Main(string[] args) {
			var client = new DreamScreenClient();
			_devices = new List<DreamDevice>();
			client.DeviceDiscovered += AddDevice;
			client.CommandReceived += LogCommand;
			client.StartDeviceDiscovery();
			await Task.Delay(5000);
			client.StopDeviceDiscovery();
			var subscribing = false;
			if (_devices.Count > 0) {
				Console.WriteLine($"Found {_devices.Count} devices.");
				foreach (var dev in _devices) {
					if (dev.Type == DeviceType.Connect || dev.Type == DeviceType.SideKick) {
						continue;
					}

					Console.WriteLine($"DreamScreen found, beginning subscribe test for {dev.IpAddress}");
					client.ColorsReceived += ColorsReceived;
					client.StartSubscribing(dev.IpAddress);
					subscribing = true;
					break;
				}
			}

			if (subscribing) {
				while (true) {
					// Log color messages
				}
			}
		}

		private static void ColorsReceived(object? sender, DreamScreenClient.DeviceColorEventArgs e) {
			Console.WriteLine("Colors received: " + JsonConvert.SerializeObject(e.Colors));
		}


		private static void LogCommand(object? sender, DreamScreenClient.MessageEventArgs e) {
			Console.WriteLine($"We got us a command from {e.Response.Target}: " + e.Response.Type);
		}

		private static void AddDevice(object? sender, DreamScreenClient.DeviceDiscoveryEventArgs e) {
			_devices.Add(e.Device);
		}
	}
}