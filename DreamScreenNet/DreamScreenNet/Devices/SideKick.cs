#region

using System;
using System.Net;
using DreamScreenNet.Enum;

#endregion

namespace DreamScreenNet.Devices {
	public class SideKick : DreamDevice {
		public SideKick(Payload payload, IPAddress address) {
			IpAddress = address;
			var dd = new DreamDevice {Type = DeviceType.SideKick};
			if (payload is null) {
				throw new ArgumentNullException(nameof(payload));
			}

			var name = payload.GetString(16);
			if (name.Length == 0) {
				name = dd.Type.ToString();
			}

			dd.Name = name;
			var groupName = payload.GetString(16);
			if (groupName.Length == 0) {
				groupName = "unassigned";
			}

			dd.GroupName = groupName;
			dd.DeviceGroup = payload.GetUint8();
			dd.DeviceMode = (DeviceMode) payload.GetUint8();
			dd.Brightness = payload.GetUint8();
			dd.AmbientColor = payload.GetColor();
			dd.Saturation = payload.GetColor();
			dd.FadeRate = payload.GetUint8();
			dd.SectorAssignment = payload.GetBytes(16);
			if (payload.Length != 62) {
				return;
			}

			dd.AmbientMode = payload.GetUint8();
			dd.AmbientShowType = payload.GetUint8();
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
				new byte[16],
				AmbientMode,
				AmbientShowType,
				(byte) DeviceType.SideKick
			};
			return new Payload(args).ToArray();
		}
	}
}