using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using DreamScreenNet.Devices;
using DreamScreenNet.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DreamScreenNet {
	public abstract class DreamScreenResponse : Message {
		protected DreamScreenResponse(IPAddress target, MessageType type, MessageFlag flag, int group = 0) : base(
			target, type, flag, group) {
		}

		protected DreamScreenResponse(MessageType type, IPAddress target) : base(type, target) {
		}

		protected DreamScreenResponse(byte[] input, IPAddress sender) : base(input, sender) {
		}

		protected DreamScreenResponse(Message message) : base(message) {
		}

		internal static DreamScreenResponse Create(Message message) {
			DreamScreenResponse response = new AcknowledgementResponse(message);
			switch (message.Type) {
				case MessageType.Brightness:
					response = new BrightnessResponse(message);
					break;
				case MessageType.Mode:
					response = new ModeResponse(message);
					break;
				case MessageType.Name:
					response = new DeviceNameResponse(message);
					break;
				case MessageType.AmbientColor:
					response = new AmbientColorResponse(message);
					break;
				case MessageType.AmbientScene:
					response = new AmbientSceneResponse(message);
					break;
				case MessageType.AmbientModeType:
					response = new AmbientModeResponse(message);
					break;
				case MessageType.GetSerial:
					break;
				case MessageType.GroupName:
					response = new GroupNameResponse(message);
					break;
				case MessageType.GroupNumber:
					response = new GroupNumberResponse(message);
					break;
				case MessageType.ColorData:
					response = new ColorResponse(message);
					break;
				case MessageType.DeviceDiscovery:
					response = new StateResponse(message);
					break;
			}

			return response;
		}

		/// <summary>
		///     Response to any message sent with ack_required set to 1.
		/// </summary>
		public class AcknowledgementResponse : DreamScreenResponse {
			internal AcknowledgementResponse(Message message) : base(message) {
			}
		}

		public class UnknownResponse : DreamScreenResponse {
			internal UnknownResponse(Message message) : base(message) {
			}
		}

		[Serializable]
		public class ColorResponse : DreamScreenResponse {
			[JsonProperty] public Color[] Colors { get; set; }

			internal ColorResponse(Message message) : base(message) {
				var cols = new List<Color>();
				for (var i = 0; i < 12; i++) {
					cols.Add(message.Payload.GetColor());
				}

				Colors = cols.ToArray();
			}
		}

		[Serializable]
		public class StateResponse : DreamScreenResponse {
			[JsonProperty] public DreamDevice? Device { get; private set; }

			internal StateResponse(Message message) : base(message) {
				var bytes = message.Encode();
				var lastByte = bytes[bytes.Length - 2];
				var type = (DeviceType) lastByte;
				message.Payload.Reset();
				switch (type) {
					case DeviceType.Connect:
						Device = new Connect(message.Payload, message.Target);
						break;
					case DeviceType.SideKick:
						Device = new SideKick(message.Payload, message.Target);
						break;
					case DeviceType.DreamScreenSolo:
					case DeviceType.DreamScreenHd:
					case DeviceType.DreamScreen4K:
						var ds = new DreamScreen(message.Payload, message.Target);
						Debug.WriteLine("Created dreamscreen: " + ds.Name);
						Device = ds;
						break;
				}

				if (Device != null) {
					Debug.WriteLine("Device set: " + Device.Name);
				} else {
					Debug.WriteLine("Device is still freaking null??");
				}
			}
		}
	}

	[Serializable]
	public class BrightnessResponse : DreamScreenResponse {
		[JsonProperty] public int Brightness { get; set; }

		internal BrightnessResponse(Message message) : base(message) {
			Brightness = message.Payload.GetUint8();
		}
	}

	[Serializable]
	public class ModeResponse : DreamScreenResponse {
		[JsonConverter(typeof(StringEnumConverter))]
		public DeviceMode Mode { get; set; }

		internal ModeResponse(Message message) : base(message) {
			Mode = (DeviceMode) message.Payload.GetUint8();
		}
	}

	[Serializable]
	public class DeviceNameResponse : DreamScreenResponse {
		[JsonProperty] public string DeviceName { get; set; }

		internal DeviceNameResponse(Message message) : base(message) {
			DeviceName = message.Payload.GetString(16);
		}
	}

	[Serializable]
	public class GroupNumberResponse : DreamScreenResponse {
		[JsonProperty] public int GroupNumber { get; set; }

		internal GroupNumberResponse(Message message) : base(message) {
			GroupNumber = message.Payload.GetUint8();
		}
	}

	[Serializable]
	public class AmbientColorResponse : DreamScreenResponse {
		[JsonProperty] public Color AmbientColor { get; set; }

		internal AmbientColorResponse(Message message) : base(message) {
			AmbientColor = message.Payload.GetColor();
		}
	}

	[Serializable]
	public class AmbientModeResponse : DreamScreenResponse {
		[JsonConverter(typeof(StringEnumConverter))]
		public int AmbientMode { get; set; }

		internal AmbientModeResponse(Message message) : base(message) {
			AmbientMode = message.Payload.GetUint8();
		}
	}

	[Serializable]
	public class AmbientSceneResponse : DreamScreenResponse {
		[JsonConverter(typeof(StringEnumConverter))]
		public int AmbientScene { get; set; }

		internal AmbientSceneResponse(Message message) : base(message) {
			AmbientScene = message.Payload.GetUint8();
		}
	}

	[Serializable]
	public class GroupNameResponse : DreamScreenResponse {
		[JsonProperty] public string GroupName { get; set; }

		internal GroupNameResponse(Message message) : base(message) {
			GroupName = message.Payload.GetString(16);
		}
	}
}