using System;
using System.Threading;
using DreamScreenNet;

namespace DiscoveryTest {
	internal class Program {
		private static void Main() {
			var client = new DreamScreenClient();
			var source = new CancellationTokenSource();
			client.DeviceDiscovered += ProcessDevice;
			source.CancelAfter(TimeSpan.FromSeconds(5));
			client.StartDeviceDiscovery();
			var token = source.Token;
			while (!token.IsCancellationRequested) {
			}

			client.StopDeviceDiscovery();
			client.Dispose();
		}

		private static void ProcessDevice(object? sender, DreamScreenClient.DeviceDiscoveryEventArgs e) {
			var dev = e.Device;
			Console.WriteLine("Device found: " + dev.Name);
		}
	}
}