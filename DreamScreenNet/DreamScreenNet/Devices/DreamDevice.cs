﻿#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using Newtonsoft.Json;

#endregion

namespace DreamScreenNet.Devices {
	[Serializable]
	public class DreamDevice {
		private const string DeviceTag4K = "Dreamscreen4K";
		private static readonly byte[] Required4KEspFirmwareVersion = {1, 6};
		private static readonly byte[] Required4KPicVersionNumber = {5, 6};
		private const string DeviceTagHd = "Dreamscreen";
		private static readonly byte[] RequiredHdEspFirmwareVersion = {1, 6};
		private static readonly byte[] RequiredHdPicVersionNumber = {1, 7};
		private const string DeviceTagSolo = "DreamscreenSolo";
		private static readonly byte[] RequiredSoloEspFirmwareVersion = {1, 6};
		private static readonly byte[] RequiredSoloPicVersionNumber = {6, 2};
		private static readonly byte[] DefaultSectorAssignment = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0};
		

		[JsonProperty] public int[] EspSerialNumber = {0, 0};
		[JsonProperty] public string SerialNumber = "123ABC";

		[DefaultValue(new byte[0])]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] AppMusicData = Array.Empty<byte>();

		[DefaultValue(new[] {255, 255, 255})] [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] MusicModeColors = {255, 255, 255};

		[DefaultValue(new[] {100, 100, 100})] [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] MusicModeWeights = {100, 100, 100};

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int AmbientShowType { get; set; } = 0;

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int FadeRate { get; set; } = 0;

		[DefaultValue(4)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int ProductId { get; set; } = 4;

		[DefaultValue("000000")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public Color AmbientColor { get; set; } = Color.FromArgb(255, 255, 255);

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int AmbientMode { get; set; } = 0;

		[DefaultValue(new[] {8, 16, 48, 0, 7, 0})]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] FlexSetup { get; set; } = {8, 16, 48, 0, 7, 0};

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int SkuSetup { get; set; } = 0;

		[DefaultValue("Undefined       ")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string GroupName { get; set; } = "Undefined       ";

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int DeviceGroup { get; set; } = 0;

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int DeviceMode { get; set; } = 0;

		[DefaultValue("Dreamscreen4K")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string DeviceTag { get; set; }

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public Color Saturation { get; set; } = Color.FromArgb(255, 255, 255);
		
		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int AmbientLightAutoAdjustEnabled { get; set; }

		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int DisplayAnimationEnabled { get; set; }

		
		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int HdmiInput { get; set; }


		[DefaultValue(false)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public bool HueLifxSettingsReceived { get; set; }


		[DefaultValue(1)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int IrEnabled { get; set; } = 1;

		
		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int IrLearningMode { get; set; }

		[DefaultValue(new [] {
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0
		})]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] IrManifest { get; set; } = {
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0
		};

		
		[DefaultValue(false)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public bool IsDemo { get; set; }

		
		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int MicrophoneAudioBroadcastEnabled { get; set; }


		[DefaultValue(new [] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0})]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int[] SectorData { get; set; } = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0};


		[DefaultValue("")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string ThingName { get; set; } = "";

		
		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int BootState { get; set; }


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
		public byte HdmiActiveChannels { get; set; }


		[DefaultValue("HDMI 1          ")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string HdmiInputName1 { get; set; } = "HDMI 1          ";


		[DefaultValue("HDMI 2          ")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string HdmiInputName2 { get; set; } = "HDMI 2          ";


		[DefaultValue("HDMI 3          ")]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public string HdmiInputName3 { get; set; } = "HDMI 3          ";

		
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
		public int LetterboxingEnable { get; set; } = 1;


		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] MinimumLuminosity { get; set; } = {0};

		
		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int MusicModeSource { get; set; }

		
		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int MusicModeType { get; set; }


		[DefaultValue(1)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int PillarboxingEnable { get; set; } = 1;

		
		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int SectorBroadcastControl { get; set; }


		[DefaultValue(1)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int SectorBroadcastTiming { get; set; } = 1;

		
		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int UsbPowerEnable { get; set; }

		
		[DefaultValue(0)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public int VideoFrameDelay { get; set; }


		[DefaultValue(15)]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte Zones { get; set; } = 15;


		[DefaultValue(new[] {255, 255, 255})]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] ZonesBrightness { get; set; } = {255, 255, 255};

		[DefaultValue(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0})]
		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
		public byte[] SectorAssignment { get; set; } = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 0, 0, 0};

		public DreamDevice() {
			Name = "Dreamscreen4K";
			Tag = "Dreamscreen";
			DeviceTag = "Dreamscreen4K";
		}

		public DreamDevice(string devTag) {
			Name = "Dreamscreen4K";
			Tag = "Dreamscreen";
			DeviceTag = devTag;
		}
		
		public byte[] EncodeState() {
			switch (DeviceTag) {
				case "Dreamscreen":
				case "Dreamscreen4K":
				case "DreamscreenHD":
					var ds = (DreamScreen) this;
					return ds.EncodeState();
				case "Connect":
					var con = (Connect) this;
					return con.EncodeState();
				case "SideKick":
					var sk = (SideKick) this;
					return sk.EncodeState();
				default:
					return new byte[0];
			}
			
		}

		public string Name { get; set; }
		public string Id { get; set; }
		public string Tag { get; set; }
		public IPAddress IpAddress { get; set; }
		public int Brightness { get; set; }
		public bool Enable { get; set; }
		
		public DateTime LastSeen { get; set; }

		
	}
}