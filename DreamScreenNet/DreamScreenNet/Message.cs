﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using DreamScreenNet.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DreamScreenNet {
	[Serializable]
	public class Message {
		[JsonProperty] public int Group { get; }

		[JsonProperty] public int Length => 5 + Payload.Length;

		[IgnoreDataMember] public IPAddress Target { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public MessageFlag Flag { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public MessageType Type { get; set; }

		[JsonProperty] public Payload Payload { get; set; }

		[JsonProperty] public uint Identifier { get; set; }

		private static readonly byte[] Crc8Table = {
			0x00, 0x07, 0x0E, 0x09, 0x1C, 0x1B,
			0x12, 0x15, 0x38, 0x3F, 0x36, 0x31,
			0x24, 0x23, 0x2A, 0x2D, 0x70, 0x77,
			0x7E, 0x79, 0x6C, 0x6B, 0x62, 0x65,
			0x48, 0x4F, 0x46, 0x41, 0x54, 0x53,
			0x5A, 0x5D, 0xE0, 0xE7, 0xEE, 0xE9,
			0xFC, 0xFB, 0xF2, 0xF5, 0xD8, 0xDF,
			0xD6, 0xD1, 0xC4, 0xC3, 0xCA, 0xCD,
			0x90, 0x97, 0x9E, 0x99, 0x8C, 0x8B,
			0x82, 0x85, 0xA8, 0xAF, 0xA6, 0xA1,
			0xB4, 0xB3, 0xBA, 0xBD, 0xC7, 0xC0,
			0xC9, 0xCE, 0xDB, 0xDC, 0xD5, 0xD2,
			0xFF, 0xF8, 0xF1, 0xF6, 0xE3, 0xE4,
			0xED, 0xEA, 0xB7, 0xB0, 0xB9, 0xBE,
			0xAB, 0xAC, 0xA5, 0xA2, 0x8F, 0x88,
			0x81, 0x86, 0x93, 0x94, 0x9D, 0x9A,
			0x27, 0x20, 0x29, 0x2E, 0x3B, 0x3C,
			0x35, 0x32, 0x1F, 0x18, 0x11, 0x16,
			0x03, 0x04, 0x0D, 0x0A, 0x57, 0x50,
			0x59, 0x5E, 0x4B, 0x4C, 0x45, 0x42,
			0x6F, 0x68, 0x61, 0x66, 0x73, 0x74,
			0x7D, 0x7A, 0x89, 0x8E, 0x87, 0x80,
			0x95, 0x92, 0x9B, 0x9C, 0xB1, 0xB6,
			0xBF, 0xB8, 0xAD, 0xAA, 0xA3, 0xA4,
			0xF9, 0xFE, 0xF7, 0xF0, 0xE5, 0xE2,
			0xEB, 0xEC, 0xC1, 0xC6, 0xCF, 0xC8,
			0xDD, 0xDA, 0xD3, 0xD4, 0x69, 0x6E,
			0x67, 0x60, 0x75, 0x72, 0x7B, 0x7C,
			0x51, 0x56, 0x5F, 0x58, 0x4D, 0x4A,
			0x43, 0x44, 0x19, 0x1E, 0x17, 0x10,
			0x05, 0x02, 0x0B, 0x0C, 0x21, 0x26,
			0x2F, 0x28, 0x3D, 0x3A, 0x33, 0x34,
			0x4E, 0x49, 0x40, 0x47, 0x52, 0x55,
			0x5C, 0x5B, 0x76, 0x71, 0x78, 0x7F,
			0x6A, 0x6D, 0x64, 0x63, 0x3E, 0x39,
			0x30, 0x37, 0x22, 0x25, 0x2C, 0x2B,
			0x06, 0x01, 0x08, 0x0F, 0x1A, 0x1D,
			0x14, 0x13, 0xAE, 0xA9, 0xA0, 0xA7,
			0xB2, 0xB5, 0xBC, 0xBB, 0x96, 0x91,
			0x98, 0x9F, 0x8A, 0x8D, 0x84, 0x83,
			0xDE, 0xD9, 0xD0, 0xD7, 0xC2, 0xC5,
			0xCC, 0xCB, 0xE6, 0xE1, 0xE8, 0xEF,
			0xFA, 0xFD, 0xF4, 0xF3
		};

		private byte[] _data;

		public Message(Message message) {
			Target = message.Target;
			Type = message.Type;
			Flag = message.Flag;
			Group = message.Group;
			Payload = message.Payload;
			_data = message.Encode();
		}

		public Message(IPAddress target, MessageType type, MessageFlag flag, int group = 0) {
			Identifier = MessageId.GetNextIdentifier();
			Target = target;
			Type = type;
			Flag = flag;
			Group = group;
			Payload = new Payload();
			_data = Encode();
		}

		public Message(MessageType type, IPAddress target) {
			Type = type;
			Target = target;
			Flag = MessageFlag.MessageResponse;
			Group = 0;
			Payload = new Payload();
			_data = Encode();
		}

		public Message(byte[] input, IPAddress sender) {
			_data = input;
			Target = sender;
			var ms = new MemoryStream(input);
			var br = new BinaryReader(ms);
			var magic = br.ReadByte();
			br.ReadByte(); // Len
			Group = br.ReadByte();
			var fByte = br.ReadByte();
			Flag = (MessageFlag) fByte;


			var c1 = br.ReadByte();
			var c2 = br.ReadByte();

			var bytes = new[] {c2, c1};
			var iv2 = BitConverter.ToInt16(bytes, 0);
			Type = (MessageType) iv2;
			if (!CheckCrc(input) || magic != 0xFC || input.Length < 7) {
				Console.WriteLine("invalid msg.");
				throw new InvalidDataException("Invalid message.");
			}

			var plBytes = new List<byte>();
			for (var i = 6; i < input.Length - 1; i++) {
				plBytes.Add(input[i]);
			}

			Payload = new Payload(plBytes.ToArray());
		}

		public byte[] Encode() {
			var output = new List<byte> {0xFC, (byte) Length, (byte) Group, (byte) Flag};
			output.AddRange(BitConverter.GetBytes((ushort) Type).Reverse());
			output.AddRange(Payload.ToArray());
			var crc = CalculateCrc(output.ToArray());
			output.Add(crc);
			return output.ToArray();
		}

		public override string ToString() {
			return string.Join(",", (from a in _data select a.ToString("X2")).ToArray());
		}

		private static byte CalculateCrc(byte[] data) {
			if (data == null) {
				throw new ArgumentNullException(nameof(data));
			}

			var size = (byte) (data[1] + 1);
			byte crc = 0;
			for (var counter = 0; counter < size; counter++) {
				var cByte = (byte) counter;
				crc = Crc8Table[(byte) (data[cByte] ^ crc) & 255];
			}

			return crc;
		}

		private static bool CheckCrc(byte[] data) {
			if (data is null) {
				throw new ArgumentNullException(nameof(data));
			}

			var checkCrc = data[data.Length - 1];
			data = data.Take(data.Length - 1).ToArray();
			var size = (byte) (data[1] + 1);
			byte crc = 0;
			for (byte counter = 0; counter < size; counter = (byte) (counter + 1)) {
				crc = Crc8Table[(byte) (data[counter] ^ crc) & 255];
			}

			return crc == checkCrc;
		}
	}
}