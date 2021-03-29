using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DreamScreenNet.Devices;
using DreamScreenNet.Enum;

namespace DreamScreenNet {
	public partial class DreamScreenClient {
		private CancellationTokenSource? _discoverCancellationSource;
		private uint _discoverSourceId;
		private readonly Dictionary<string, DreamDevice> _discoveredDevices = new();

		private IList<DreamDevice> _devices = new List<DreamDevice>();
		/// <summary>
		/// Event fired when a DS dev is discovered on the network
		/// </summary>
		public event EventHandler<DeviceDiscoveryEventArgs>? DeviceDiscovered;

		/// <summary>
		/// Event fired when a DS dev hasn't been seen on the network for a while (for more than 5 minutes)
		/// </summary>
		public event EventHandler<DeviceDiscoveryEventArgs>? DeviceLost;
		private void ProcessDeviceDiscoveryMessage(IPAddress remoteAddress, DreamScreenResponse.StateResponse msg) {
			var id = msg.Target.ToString();
			msg.Identifier = 2;
			Console.WriteLine($"Processing device discovery message for {remoteAddress}: {id}");

			if (_discoveredDevices.ContainsKey(id)) {
				_discoveredDevices[id].LastSeen = DateTime.UtcNow; //Update datestamp
				_discoveredDevices[id].IpAddress = remoteAddress; //Update hostname in case IP changed
				Console.WriteLine("Device already discovered, skipping.");
				return;
			}

			if (msg.Identifier != _discoverSourceId || //did we request the discovery?
			    _discoverCancellationSource == null ||
			    _discoverCancellationSource.IsCancellationRequested) {
				Console.WriteLine($"Source mismatch or cancellation: {msg.Identifier} vs {_discoverSourceId}");
				return;
			}
			
			_discoveredDevices[id] = msg.Device;
			_devices.Add(msg.Device);
			Console.WriteLine("Device added...");
			DeviceDiscovered?.Invoke(this, new DeviceDiscoveryEventArgs(msg.Device));
		}
		
		/// <summary>
		/// Begins searching for bulbs.
		/// </summary>
		/// <seealso cref="DeviceDiscovered"/>
		/// <seealso cref="DeviceLost"/>
		/// <seealso cref="StopDeviceDiscovery"/>
		public void StartDeviceDiscovery() {
			// Reset our list of devices on discovery start
			_devices = new List<DreamDevice>();
			if (_discoverCancellationSource != null && !_discoverCancellationSource.IsCancellationRequested)
				return;
			_discoverCancellationSource = new CancellationTokenSource();
			var token = _discoverCancellationSource.Token;
			_discoverSourceId = MessageId.GetNextIdentifier();
			var discoPacket = new Message(_broadcastIp, MessageType.DeviceDiscovery, MessageFlag.SystemMessage, 0xFF);

			//Start discovery thread
			Task.Run(async () => {
				Debug.WriteLine("Sending GetServices...");
				await BroadcastMessageAsync(discoPacket);
				while (!token.IsCancellationRequested) {
					try {
						//await BroadcastMessageAsync<UnknownResponse>(null, header,
						//MessageType.DeviceGetService);
					} catch (Exception e) {
						Debug.WriteLine("Broadcast exception: " + e.Message + e.StackTrace);
					}

					await Task.Delay(1, token);
					var lostDevices = _devices.Where(d => (DateTime.UtcNow - d.LastSeen).TotalMinutes > 5).ToArray();
					if (!lostDevices.Any()) {
						continue;
					}

					foreach (var device in lostDevices) {
						_devices.Remove(device);
						_discoveredDevices.Remove(device.IpAddress.ToString());
						DeviceLost?.Invoke(this, new DeviceDiscoveryEventArgs(device));
					}
				}
			}, token);
		}

		/// <summary>
		/// Stops device discovery
		/// </summary>
		/// <seealso cref="StartDeviceDiscovery"/>
		public void StopDeviceDiscovery() {
			if (_discoverCancellationSource == null || _discoverCancellationSource.IsCancellationRequested)
				return;
			_discoverCancellationSource.Cancel();
			_discoverCancellationSource = null;
		}

		
		/// <summary>
		/// Event args for <see cref="DreamScreenClient.DeviceDiscovered"/> and <see cref="DreamScreenClient.DeviceLost"/> events.
		/// </summary>
		public sealed class DeviceDiscoveryEventArgs : EventArgs {
			/// <summary>
			/// The device the event relates to
			/// </summary>
			public DreamDevice Device { get; }

			internal DeviceDiscoveryEventArgs(DreamDevice device) => Device = device;
		}

	}
}