using System;
using System.Collections.Generic;
using System.Net;

namespace DreamScreenNet.Devices {
	public class DreamScreen : DreamDevice {
		private const string DeviceTag4K = "Dreamscreen4K";
		private static readonly byte[] Required4KEspFirmwareVersion = {1, 6};
		private static readonly byte[] Required4KPicVersionNumber = {5, 6};
		private const string DeviceTagHd = "Dreamscreen";
		private static readonly byte[] RequiredHdEspFirmwareVersion = {1, 6};
		private static readonly byte[] RequiredHdPicVersionNumber = {1, 7};
		private const string DeviceTagSolo = "DreamscreenSolo";
		private static readonly byte[] RequiredSoloEspFirmwareVersion = {1, 6};
		private static readonly byte[] RequiredSoloPicVersionNumber = {6, 2};
		public static readonly byte[] DefaultSectorAssignment = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0};

		
		public DreamScreen(Payload payload, IPAddress address) {
			IpAddress = address;
			Name = payload.GetString(16);
			GroupName = payload.GetString(16);
			DeviceGroup = payload.GetUint8();
			DeviceMode = payload.GetUint8();
			Brightness = payload.GetUint8();
			Zones = payload.GetUint8();
			ZonesBrightness = payload.GetBytes(4);
			AmbientColor = payload.GetColor();
			Saturation = payload.GetColor();
			FlexSetup = payload.GetBytes(6);
			MusicModeType = payload.GetUint8();
			MusicModeColors = payload.GetBytes(3);
			MusicModeWeights = payload.GetBytes(3);
			MinimumLuminosity = payload.GetBytes(3);
			AmbientShowType = payload.GetUint8();
			FadeRate = payload.GetUint8();
			payload.Advance(6);
			IndicatorLightAutoOff = payload.GetUint8();
			UsbPowerEnable = payload.GetUint8();
			SectorBroadcastControl = payload.GetUint8();
			SectorBroadcastTiming = payload.GetUint8();
			HdmiInput = payload.GetUint8();
			MusicModeSource = payload.GetUint8();
			HdmiInputName1 = payload.GetString(16);
			HdmiInputName2 = payload.GetString(16);
			HdmiInputName3 = payload.GetString(16);
			CecPassthroughEnable = payload.GetUint8();
			CecSwitchingEnable = payload.GetUint8();
			HpdEnable = payload.GetUint8();
			VideoFrameDelay = payload.GetUint8();
			LetterboxingEnable = payload.GetUint8();
			HdmiActiveChannels = payload.GetUint8();
			payload.Advance(4);
			ColorBoost = payload.GetUint8();
			Console.WriteLine("CB");
			if (payload.Length >= 137) CecPowerEnable = payload.GetUint8();
			if (payload.Length >= 138) SkuSetup = payload.GetUint8();
			if (payload.Length >= 139) BootState = payload.GetUint8();
			if (payload.Length >= 140) PillarboxingEnable = payload.GetUint8();
			if (payload.Length >= 141) HdrToneRemapping = payload.GetUint8();
			var encoded = payload.ToArray();
			var devType = encoded[encoded.Length - 1];
			Console.WriteLine("Rewound: " + devType);

			DeviceTag = DeviceTag4K;
			if (devType == 1) {
				DeviceTag = DeviceTagSolo;
			} else if (devType == 2) {
				DeviceTag = DeviceTagHd;
			}
			if (payload is null) throw new ArgumentNullException(nameof(payload));
			if (payload.Length < 132)
				throw new ArgumentException($"Payload length is too short: {payload}");
			Console.WriteLine("Good: " + DeviceTag);
		}

		public new byte[] EncodeState() {
			var espVersion = RequiredHdEspFirmwareVersion;
			var picVersion = RequiredHdPicVersionNumber;
			switch (DeviceTag) {
				case DeviceTag4K:
					espVersion = Required4KEspFirmwareVersion;
					picVersion = Required4KPicVersionNumber;
					break;
				case DeviceTagSolo:
					espVersion = RequiredSoloEspFirmwareVersion;
					picVersion = RequiredSoloPicVersionNumber;
					break;
			}

			var args = new List<object> {
				Name,
				GroupName,
				DeviceGroup,
				DeviceMode,
				Brightness,
				Zones,
				ZonesBrightness,
				AmbientColor,
				Saturation,
				FlexSetup,
				MusicModeType,
				MusicModeColors,
				MusicModeWeights,
				MinimumLuminosity,
				AmbientShowType,
				FadeRate,
				new byte[5],
				IndicatorLightAutoOff,
				UsbPowerEnable,
				SectorBroadcastControl,
				SectorBroadcastTiming,
				HdmiInput,
				new byte[2],
				HdmiInputName1,
				HdmiInputName2,
				HdmiInputName3,
				CecPassthroughEnable,
				CecSwitchingEnable,
				HpdEnable,
				0x00,
				VideoFrameDelay,
				LetterboxingEnable,
				HdmiActiveChannels,
				espVersion,
				picVersion,
				ColorBoost,
				CecPowerEnable,
				SkuSetup,
				BootState,
				PillarboxingEnable,
				HdrToneRemapping
			};

			switch (DeviceTag) {
				// Device type
				case "DreamScreenHd":
					args.Add(0x01);
					break;
				case "DreamScreen4K":
					args.Add(0x02);
					break;
				//DS Solo
				default:
					args.Add(0x07);
					break;
			}

			return new Payload(args.ToArray()).ToArray();
		}
	}
}