using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DreamScreenNet.Devices;
using DreamScreenNet.Enum;

namespace DreamScreenNet {
	public partial class DreamScreenClient {
		private bool _announceSubscription;
		private IPAddress? _subDevice;
		private Dictionary<IPAddress, int> _subscribers;
		private bool _subscribing;

		/// <summary>
		///     Event fired when a Subscription request is received
		/// </summary>
		public event EventHandler<DeviceSubscriptionEventArgs>? SubscriptionRequested;

		/// <summary>
		///     Event fired when color data is received
		/// </summary>
		public event EventHandler<DeviceColorEventArgs>? ColorsReceived;

		/// <summary>
		///     Begin subscribing to the designated device group
		/// </summary>
		/// <param name="target">Target Ip address to subscribe to</param>
		public void StartSubscribing(IPAddress target) {
			_subscribing = true;
			_subDevice = target;
		}

		/// <summary>
		///     Stop responding to subscription requests
		/// </summary>
		public void StopSubscribing() {
			_subscribing = false;
			_subDevice = null;
		}

		/// <summary>
		///     Begin sending a subscription announcement every 5 seconds until ended
		/// </summary>
		/// <param name="subGroup">Device Group to broadcast to</param>
		/// <returns></returns>
		public async Task BeginAnnouncement(int subGroup) {
			_announceSubscription = true;
			_subscribers = new Dictionary<IPAddress, int>();
			while (_announceSubscription) {
				await AnnounceSubscription(subGroup);
				await Task.Delay(TimeSpan.FromSeconds(5));
				var keys = _subscribers.Keys;
				foreach (var key in keys) {
					// If the subscribers haven't replied in three messages, remove them, otherwise, count down one
					if (_subscribers[key] <= 0) {
						_subscribers.Remove(key);
					} else {
						_subscribers[key] -= 1;
					}
				}
			}

			_subscribers = new Dictionary<IPAddress, int>();
		}

		/// <summary>
		///     Stop announcing subscription message
		/// </summary>
		public void EndAnnouncement() {
			_announceSubscription = false;
		}


		private async Task AnnounceSubscription(int group, bool isRequest = false) {
			var target = _broadcastIp;
			var flag = MessageFlag.SubscriptionResponse;
			if (isRequest) {
				flag = MessageFlag.SystemMessage;
			}

			var msg = new Message(target, MessageType.Subscribe, flag, group);
			if (flag == MessageFlag.SystemMessage) {
				msg.Payload = new Payload(new object[] {(byte) 0x01});
			}

			await BroadcastMessageAsync(msg);
		}

		/// <summary>
		///     Send colors to target IP address
		/// </summary>
		/// <param name="target">Target device to send to</param>
		/// <param name="colors">An array of 12 System.Drawing>Colors</param>
		/// <returns></returns>
		public async Task SendColors(DreamDevice target, IEnumerable<Color> colors) {
			var msg = new Message(target.IpAddress, MessageType.ColorData, MessageFlag.ColorData, target.DeviceGroup) {
				Payload = new Payload(colors.Cast<object>().ToArray())
			};
			await BroadcastMessageAsync(msg);
		}

		/// <summary>
		///     Event args for <see cref="DreamScreenClient.DeviceDiscovered" /> and <see cref="DreamScreenClient.DeviceLost" />
		///     events.
		/// </summary>
		public sealed class DeviceSubscriptionEventArgs : EventArgs {
			/// <summary>
			///     The device the event relates to
			/// </summary>
			public IPAddress Target { get; }

			internal DeviceSubscriptionEventArgs(IPAddress target) {
				Target = target;
			}
		}

		/// <summary>
		///     Event args for <see cref="DreamScreenClient.DeviceDiscovered" /> and <see cref="DreamScreenClient.DeviceLost" />
		///     events.
		/// </summary>
		public sealed class DeviceColorEventArgs : EventArgs {
			/// <summary>
			///     The device the event relates to
			/// </summary>
			public Color[] Colors { get; set; }

			internal DeviceColorEventArgs(Color[] colors) {
				Colors = colors;
			}
		}
	}
}