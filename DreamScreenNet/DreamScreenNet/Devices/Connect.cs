using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using DreamScreenNet.Enum;

namespace DreamScreenNet.Devices {
	public class Connect : DreamDevice {
		public Connect(Payload payload, IPAddress address) {
			IpAddress = address;
			try {
				var name = payload.GetString(16);
				if (name.Length == 0) {
					name = "Connect";
				}

				Name = name;
				var groupName = payload.GetString(16);
				if (groupName.Length == 0) {
					groupName = "Group";
				}

				GroupName = groupName;
			} catch (IndexOutOfRangeException) {
				Console.WriteLine($@"Index out of range, payload length is {payload.Length}.");
			}

			DeviceGroup = payload.GetUint8();
			DeviceMode = (DeviceMode) payload.GetUint8();
			Brightness = payload.GetUint8();
			AmbientColor = payload.GetColor();
			Saturation = payload.GetColor();
			FadeRate = payload.GetUint8();
			payload.Advance(18);
			AmbientMode = payload.GetUint8();
			AmbientShowType = payload.GetUint8();
			HdmiInput = payload.GetUint8();
			DisplayAnimationEnabled = payload.GetUint8();
			AmbientLightAutoAdjustEnabled = payload.GetUint8();
			MicrophoneAudioBroadcastEnabled = payload.GetUint8();
			IrEnabled = payload.GetUint8();
			IrLearningMode = payload.GetUint8();
			var irBytes = payload.GetBytes(40);
			IrManifest = irBytes;
			if (payload.Length > 115) {
				try {
					ThingName = payload.GetString(63);
				} catch (IndexOutOfRangeException) {
					ThingName = "";
				}
			}
		}


		public new byte[] EncodeState() {
			var args = new object[] {
				Name,
				GroupName,
				DeviceGroup,
				DeviceMode,
				Brightness,
				AmbientColor,
				Saturation,
				FadeRate,
				AmbientMode,
				AmbientShowType,
				HdmiInput,
				DisplayAnimationEnabled,
				AmbientLightAutoAdjustEnabled,
				MicrophoneAudioBroadcastEnabled,
				IrEnabled,
				IrLearningMode,
				IrManifest,
				StringBytePad(ThingName, 64),
				(byte) DeviceType.SideKick
			};
			return new Payload(args).ToArray();
		}

		private static IEnumerable<byte> StringBytePad(string toPad, int len) {
			if (toPad is null) {
				throw new ArgumentNullException(nameof(toPad));
			}

			var outBytes = new byte[len];
			var output = toPad.Length > len ? toPad.Substring(0, len) : toPad;
			var encoding = new ASCIIEncoding();

			var myBytes = encoding.GetBytes(output);
			for (var bb = 0; bb < len; bb++) {
				if (bb < myBytes.Length) {
					outBytes[bb] = myBytes[bb];
				} else {
					outBytes[bb] = 0;
				}
			}

			return outBytes;
		}
	}
}