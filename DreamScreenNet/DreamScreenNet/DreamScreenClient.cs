using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DreamScreenNet.Devices;
using DreamScreenNet.Enum;

namespace DreamScreenNet {
	public partial class DreamScreenClient : IDisposable {
		private readonly UdpClient _sender;
		private UdpClient _listener;
		private readonly bool _disposeSender;
		private readonly IPAddress _broadcastIp = IPAddress.Parse("255.255.255.255");
		private List<DreamScreenResponse> _messages;
		private Task _listenTask;
		private CancellationTokenSource _cts;
		private readonly Dictionary<uint, Action<DreamScreenResponse>> _taskCompletions = new();
		private readonly ManualResetEvent _receiveDone = new(false);

		/// <summary>
		/// Event fired when a command message is received
		/// </summary>
		public event EventHandler<MessageEventArgs>? CommandReceived;

		/// <summary>
		/// Initialize a new DreamScreen client
		/// </summary>
		public DreamScreenClient() {
			_disposeSender = true;
			_sender = new UdpClient();
			_sender.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			_sender.EnableBroadcast = true;
			Initialize();
		}
		
		/// <summary>
		/// Initialize a new DreamScreen client with an existing UDPClient as a sender
		/// </summary>
		/// <param name="sender">UDPClient to re-use</param>
		
		public DreamScreenClient(UdpClient sender) {
			_disposeSender = false;
			_sender = sender;
			Initialize();
		}

		private void Initialize() {
			IPEndPoint end = new IPEndPoint(IPAddress.Any, 8888);
			_listener = new UdpClient(end) {Client = {Blocking = false}};
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
				_listener.Client.DontFragment = true;
			}
			_listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			_cts = new CancellationTokenSource();
			_messages = new List<DreamScreenResponse>();
			_listenTask = Task.Run(async () => {
				await Listen(_cts.Token);
			});
		}

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
						if (_subscribing) {
							var resp = new Message(msg.Target, MessageType.Subscribe, MessageFlag.SubscriptionResponse,
								_subscriptionGroup) {Payload = new Payload(new object[] {(byte) 0x01})};
							BroadcastMessageAsync(resp).ConfigureAwait(false);
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
					ColorsReceived?.Invoke(msg.Target,new DeviceColorEventArgs(cResp.Colors));
					break;
				default:
					switch (msg.Flag) {
						case MessageFlag.Response:
							_messages.Add(response);
							break;
						case MessageFlag.WriteGroup:
						case MessageFlag.WriteIndividual:
						case MessageFlag.WriteSomething:
							CommandReceived?.Invoke(msg.Target,new MessageEventArgs(response));
							break;
					}

					break;
			}

			if (remote.Port == 8888)
				Debug.WriteLine("Received {0} from {1}:{2}", msg.Type, remote,
					string.Join(",", (from a in data select a.ToString("X2")).ToArray()));
		}

		
        private async Task<DreamScreenResponse> BroadcastMessageAsync(Message packet) {
	        Debug.WriteLine($"LOCAL=>{packet.Target}::{packet.Type}: " + packet);
	        return await BroadcastPayloadAsync<DreamScreenResponse>(packet);
        }
        
        
        private async Task<T> BroadcastPayloadAsync<T>(Message packet)
	        where T : DreamScreenResponse {
	        if (_sender == null)
		        throw new InvalidOperationException("No valid socket");
	        var data = packet.Encode();
	        Debug.WriteLine(
		        string.Join(",", (from a in data select a.ToString("X2")).ToArray()));


	        TaskCompletionSource<T>? tcs = null;
	        if (packet.Identifier > 0 &&
	            typeof(T) != typeof(DreamScreenResponse.UnknownResponse)) {
		        tcs = new TaskCompletionSource<T>();

		        void Action(DreamScreenResponse r) {
			        if (r.GetType() == typeof(T)) tcs.TrySetResult((T) r);
		        }

		        _taskCompletions[packet.Identifier] = Action;
	        }

	        var msg = packet.Encode();
	        await _sender.SendAsync(msg, msg.Length, packet.Target.ToString(), 8888);

	        T result;
	        if (tcs == null) {
		        return null;
	        }

	        var _ = Task.Delay(1000).ContinueWith(t => {
		        if (!t.IsCompleted)
			        tcs.TrySetException(new TimeoutException());
	        });
	        try {
		        result = await tcs.Task.ConfigureAwait(false);
	        } finally {
		        _taskCompletions.Remove(packet.Identifier);
	        }

	        return result;
        }


        private async Task<DreamScreenResponse> BroadcastMessageForResponse(Message packet) {

	        var iotAddress = packet.Target;
	        var iotEndpoint = new IPEndPoint(iotAddress, 8888);

	        try {
		        var bytes = packet.Encode();
		        await _sender.SendAsync(bytes, bytes.Length,iotEndpoint).ConfigureAwait(false);

		        var watch = new Stopwatch();
		        watch.Start();
		        while (watch.Elapsed < TimeSpan.FromSeconds(1)) {
			        for (var i = 0; i < _messages.Count; i++) {
				        var msg = _messages[i];
				        if (msg.Type != packet.Type || !Equals(msg.Target, packet.Target)) {
					        continue;
				        }

				        watch.Stop();
				        _messages.RemoveAt(i);
				        return msg;
			        }
		        }
		        watch.Stop();
		        return null;

	        } catch {
		        //  Ignore
	        }

	        return new DreamScreenResponse.UnknownResponse(packet);
        }
        
        /// <summary>
        /// Event args for incoming command events.
        /// </summary>
        public sealed class MessageEventArgs : EventArgs {
	        /// <summary>
	        /// The device the event relates to
	        /// </summary>
	        public DreamScreenResponse Response { get; set; }

	        internal MessageEventArgs(DreamScreenResponse response) => Response = response;
        }
        
		public void Dispose() {
			if (_disposeSender) _sender?.Dispose();
			_listener?.Dispose();
			_cts.Cancel();
		}
	}
}