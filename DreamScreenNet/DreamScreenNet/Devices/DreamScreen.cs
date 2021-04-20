using System;
using System.Collections.Generic;
using System.Net;
using DreamScreenNet.Enum;

namespace DreamScreenNet.Devices {
	public class DreamScreen : DreamDevice {
		private static readonly byte[] Required4KEspFirmwareVersion = {1, 6};
		private static readonly byte[] Required4KPicVersionNumber = {5, 6};
		private static readonly byte[] RequiredHdEspFirmwareVersion = {1, 6};
		private static readonly byte[] RequiredHdPicVersionNumber = {1, 7};
		private static readonly byte[] RequiredSoloEspFirmwareVersion = {1, 6};
		private static readonly byte[] RequiredSoloPicVersionNumber = {6, 2};


		public DreamScreen(Payload payload, IPAddress address) {
			IpAddress = address;
			Name = payload.GetString(16);
			GroupName = payload.GetString(16);
			DeviceGroup = payload.GetUint8();
			DeviceMode = (DeviceMode) payload.GetUint8();
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
			if (payload.Length >= 137) {
				CecPowerEnable = payload.GetUint8();
			}

			if (payload.Length >= 138) {
				SkuSetup = payload.GetUint8();
			}

			if (payload.Length >= 139) {
				BootState = payload.GetUint8();
			}

			if (payload.Length >= 140) {
				PillarboxingEnable = payload.GetUint8();
			}

			if (payload.Length >= 141) {
				HdrToneRemapping = payload.GetUint8();
			}

			var encoded = payload.ToArray();
			Type = (DeviceType) encoded[encoded.Length - 1];
			Console.WriteLine("Rewound: " + Type);

			if (payload is null) {
				throw new ArgumentNullException(nameof(payload));
			}

			if (payload.Length < 132) {
				throw new ArgumentException($"Payload length is too short: {payload}");
			}

			Console.WriteLine("Good: " + Type);
		}

		public new byte[] EncodeState() {
			var espVersion = RequiredHdEspFirmwareVersion;
			var picVersion = RequiredHdPicVersionNumber;
			switch (Type) {
				case DeviceType.DreamScreen4K:
					espVersion = Required4KEspFirmwareVersion;
					picVersion = Required4KPicVersionNumber;
					break;
				case DeviceType.DreamScreenSolo:
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
				HdrToneRemapping,
				(byte) Type
			};


			return new Payload(args.ToArray()).ToArray();
		}
	}
}