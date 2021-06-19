using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DreamScreenNet.Enum;

namespace DreamScreenNet {
	public partial class DreamScreenClient : IDisposable {
		private readonly IPAddress _broadcastIp = IPAddress.Parse("255.255.255.255");
		private readonly bool _disposeSender;
		private readonly UdpClient _sender;
		private readonly CancellationTokenSource _cts;
		private readonly UdpClient _listener;
		private readonly List<DreamScreenResponse> _messages;

		/// <summary>
		///     Initialize a new DreamScreen client
		/// </summary>
		public DreamScreenClient() {
			_disposeSender = true;
			_sender = new UdpClient();
			_sender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			_sender.EnableBroadcast = true;
			IPEndPoint end = new(IPAddress.Any, 8888);
			_listener = new UdpClient(end) {Client = {Blocking = false}};
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
				_listener.Client.DontFragment = true;
			}

			_listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			_cts = new CancellationTokenSource();
			_messages = new List<DreamScreenResponse>();
			Task.Run(async () => { await Listen(_cts.Token); });
			_subscribers = new Dictionary<IPAddress, int>();
		}

		/// <summary>
		///     Initialize a new DreamScreen client with an existing UDPClient as a sender
		/// </summary>
		/// <param name="sender">UDPClient to re-use</param>
		public DreamScreenClient(UdpClient sender) {
			_disposeSender = false;
			_sender = sender;
			IPEndPoint end = new(IPAddress.Any, 8888);
			_listener = new UdpClient(end) {Client = {Blocking = false}};
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
				_listener.Client.DontFragment = true;
			}

			_listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			_cts = new CancellationTokenSource();
			_messages = new List<DreamScreenResponse>();
			Task.Run(async () => { await Listen(_cts.Token); });
			_subscribers = new Dictionary<IPAddress, int>();
		}

		public void Dispose() {
			if (_disposeSender) {
				_sender.Dispose();
			}

			_listener.Dispose();
			_cts.Cancel();
		}

		/// <summary>
		///     Event fired when a command message is received
		/// </summary>
		public event EventHandler<MessageEventArgs>? CommandReceived;

		private async Task Listen(CancellationToken token) {
			while (!token.IsCancellationRequested) {
				var receivedResults = await _listener.ReceiveAsync();
				if (receivedResults.Buffer.Length > 0) {
					HandleIncomingMessages(receivedResults.Buffer, receivedResults.RemoteEndPoint);
				}
			}
		}

		private void HandleIncomingMessages(byte[] data, IPEndPoint endpoint) {
			var remote = endpoint;
			var msg = new Message(data, endpoint.Address);
			var response = DreamScreenResponse.Create(msg);
			//Debug.WriteLine("Msg: " + msg.Type);
			// Debug.WriteLine("{0}=>LOCAL::{1}: {2}", remote, msg.Type,
			// 	string.Join(",", (from a in data select a.ToString("X2")).ToArray()));
			switch (response.Type) {
				case MessageType.DeviceDiscovery:
					if (msg.Flag != MessageFlag.SystemMessage) {
						msg.Identifier = _discoverSourceId;
						ProcessDeviceDiscoveryMessage(remote.Address, (DreamScreenResponse.StateResponse) response);
					}

					break;
				case MessageType.Subscribe:
					if (msg.Flag == MessageFlag.SystemMessage) {
						SubscriptionRequested?.Invoke(this, new DeviceSubscriptionEventArgs(msg.Target));
						if (_subscribing && Equals(msg.Target, _subDevice)) {
							//Debug.WriteLine("Sending sub response...");
							var resp = new Message(msg.Target, MessageType.Subscribe, MessageFlag.SubscriptionResponse,
								msg.Group) {Payload = new Payload(new object[] {(byte) 0x01})};
							BroadcastMessageAsync(resp).ConfigureAwait(false);
						} else {
							//Debug.WriteLine($"Sub responses don't match or not subscribing: {msg.Target}, {_subDevice}");

						}
					}
					
					if (msg.Flag == MessageFlag.SubscriptionResponse) {
						if (_announceSubscription) {
							_subscribers[msg.Target] = 3;
						}
					}

					break;
				case MessageType.ColorData:
					var cResp = (DreamScreenResponse.ColorResponse) response;
					if (_subscribing) {
						ColorsReceived?.Invoke(msg.Target, new DeviceColorEventArgs(cResp.Colors));
					}

					break;
				default:
					switch (msg.Flag) {
						case MessageFlag.Response:
							_messages.Add(response);
							break;
						case MessageFlag.WriteGroup:
						case MessageFlag.WriteIndividual:
						case MessageFlag.WriteSomething:
							CommandReceived?.Invoke(msg.Target, new MessageEventArgs(response));
							break;
					}

					break;
			}

			
		}




		private async Task BroadcastMessageAsync(Message packet) {
			if (_sender == null) {
				throw new InvalidOperationException("No valid socket");
			}
			var data = packet.Encode();

			var ep = new IPEndPoint(packet.Target, 8888);
			// Debug.WriteLine($"LOCAL=>{ep}::{packet.Type}: "
			//                 +
			// 	                string.Join(",", (from a in data select a.ToString("X2")).ToArray()));

			await _sender.SendAsync(data, data.Length, ep);
		}


		private async Task<DreamScreenResponse> BroadcastMessageForResponse(Message packet) {
			var iotAddress = packet.Target;
			var iotEndpoint = new IPEndPoint(iotAddress, 8888);
			DreamScreenResponse msg = new DreamScreenResponse.UnknownResponse(packet);
			try {
				var bytes = packet.Encode();
				await _sender.SendAsync(bytes, bytes.Length, iotEndpoint).ConfigureAwait(false);

				var watch = new Stopwatch();
				watch.Start();
				while (watch.Elapsed < TimeSpan.FromSeconds(1)) {
					for (var i = 0; i < _messages.Count; i++) {
						var cMsg = _messages[i];
						if (cMsg.Type != packet.Type || !Equals(cMsg.Target, packet.Target)) {
							continue;
						}

						watch.Stop();
						_messages.RemoveAt(i);
						msg = cMsg;
					}
				}

				watch.Stop();
				return msg;
			} catch {
				//  Ignore
			}

			return new DreamScreenResponse.UnknownResponse(packet);
		}

		/// <summary>
		///     Event args for incoming command events.
		/// </summary>
		public sealed class MessageEventArgs : EventArgs {
			/// <summary>
			///     The device the event relates to
			/// </summary>
			public DreamScreenResponse Response { get; }

			internal MessageEventArgs(DreamScreenResponse response) {
				Response = response;
			}
		}
	}
}