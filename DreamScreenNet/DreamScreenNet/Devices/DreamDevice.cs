#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using DreamScreenNet.Enum;
using Newtonsoft.Json;

#endregion

namespace DreamScreenNet.Devices {
	[Serializable]
	public class DreamDevice {
		[DefaultValue(false)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public bool HueLifxSettingsReceived { get; set; }


		[DefaultValue(false)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public bool IsDemo { get; set; }


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte HdmiActiveChannels { get; set; }


		[DefaultValue(15)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte Zones { get; set; } = 15;

		[DefaultValue(new[] {8, 16, 48, 0, 7, 0})]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] FlexSetup { get; set; } = {8, 16, 48, 0, 7, 0};

		[DefaultValue(new[] {
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0
		})]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] IrManifest { get; set; } = {
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0
		};


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] MinimumLuminosity { get; set; } = {0};

		[DefaultValue(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0})]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] SectorAssignment { get; set; } = DefaultSectorAssignment;


		[DefaultValue(new[] {255, 255, 255})]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] ZonesBrightness { get; set; } = {255, 255, 255};

		[DefaultValue("000000")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public Color AmbientColor { get; set; } = Color.FromArgb(255, 255, 255);

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public Color Saturation { get; set; } = Color.FromArgb(255, 255, 255);

		public DateTime LastSeen { get; set; }

		[DefaultValue(DeviceType.DreamScreen4K)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public DeviceType Type { get; set; }

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int AmbientLightAutoAdjustEnabled { get; set; }

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int AmbientMode { get; set; }

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int AmbientShowType { get; set; }


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int BootState { get; set; }

		public int Brightness { get; set; }


		[DefaultValue(1)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int CecPassthroughEnable { get; set; } = 1;


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int CecPowerEnable { get; set; }


		[DefaultValue(1)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int CecSwitchingEnable { get; set; } = 1;


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int ColorBoost { get; set; }

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int DeviceGroup { get; set; }

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int DeviceMode { get; set; }

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int DisplayAnimationEnabled { get; set; }

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int FadeRate { get; set; }


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int HdmiInput { get; set; }


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int HdrToneRemapping { get; set; }


		[DefaultValue(1)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int HpdEnable { get; set; } = 1;


		[DefaultValue(1)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int IndicatorLightAutoOff { get; set; } = 1;


		[DefaultValue(1)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int IrEnabled { get; set; } = 1;


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int IrLearningMode { get; set; }


		[DefaultValue(1)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int LetterboxingEnable { get; set; } = 1;


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int MicrophoneAudioBroadcastEnabled { get; set; }


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int MusicModeSource { get; set; }


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int MusicModeType { get; set; }


		[DefaultValue(1)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int PillarboxingEnable { get; set; } = 1;

		[DefaultValue(4)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int ProductId { get; set; } = 4;


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int SectorBroadcastControl { get; set; }


		[DefaultValue(1)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int SectorBroadcastTiming { get; set; } = 1;

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int SkuSetup { get; set; }


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int UsbPowerEnable { get; set; }


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int VideoFrameDelay { get; set; }


		[DefaultValue(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0})]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int[] SectorData { get; set; } = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0};

		public IPAddress IpAddress { get; set; } = IPAddress.None;

		[DefaultValue("Undefined       ")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string GroupName { get; set; } = "Undefined       ";


		[DefaultValue("HDMI 1          ")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string HdmiInputName1 { get; set; } = "HDMI 1          ";


		[DefaultValue("HDMI 2          ")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string HdmiInputName2 { get; set; } = "HDMI 2          ";


		[DefaultValue("HDMI 3          ")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string HdmiInputName3 { get; set; } = "HDMI 3          ";

		public string Id { get; set; } = string.Empty;

		public string Name { get; set; }


		[DefaultValue("")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string ThingName { get; set; } = "";

		private static readonly byte[] DefaultSectorAssignment = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0};

		[DefaultValue(new byte[0])] [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] AppMusicData = Array.Empty<byte>();

		[JsonProperty] public int[] EspSerialNumber = {0, 0};

		[DefaultValue(new[] {255, 255, 255})] [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] MusicModeColors = {255, 255, 255};

		[DefaultValue(new[] {100, 100, 100})] [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] MusicModeWeights = {100, 100, 100};

		[JsonProperty] public string SerialNumber = "123ABC";

		public DreamDevice() {
			Name = "DreamScreen4K";
			Type = DeviceType.DreamScreen4K;
		}

		public DreamDevice(string devTag) {
			Name = "DreamScreen4K";
			Type = DeviceType.DreamScreen4K;
		}

		public byte[] EncodeState() {
			switch (Type) {
				case DeviceType.DreamScreen4K:
				case DeviceType.DreamScreenHd:
				case DeviceType.DreamScreenSolo:
					var ds = (DreamScreen) this;
					return ds.EncodeState();
				case DeviceType.Connect:
					var con = (Connect) this;
					return con.EncodeState();
				case DeviceType.SideKick:
					var sk = (SideKick) this;
					return sk.EncodeState();
				default:
					return new byte[0];
			}
		}
	}
}