using System;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using DreamScreenNet.Enum;

namespace DreamScreenNet {
	public partial class DreamScreenClient {
		/// <summary>
		///     Set the device mode
		/// </summary>
		/// <param name="mode"><see cref="DeviceMode" />Device Mode to set</param>
		/// <param name="target">IP address of device</param>
		/// <param name="group">Group device belongs to</param>
		/// <returns></returns>
		public async Task<DreamScreenResponse> SetMode(DeviceMode mode, IPAddress target, int group) {
			var flag = Equals(group == 0) ? MessageFlag.WriteIndividual : MessageFlag.WriteGroup;
			var msg = new Message(target, MessageType.Mode, flag, group)
				{Payload = new Payload(new object[] {(byte) mode})};
			var response = await BroadcastMessageForResponse(msg);
			Console.WriteLine("Response here is " + response.Type);
			return response;
		}

		/// <summary>
		///     Set the device group number
		/// </summary>
		/// <param name="groupNumber">The group number to set the device to</param>
		/// <param name="target">Target group Ip</param>
		/// <param name="group">Current device group</param>
		/// <returns></returns>
		public async Task<DreamScreenResponse> SetGroupNumber(int groupNumber, IPAddress target, int group) {
			var flag = MessageFlag.WriteIndividual;
			var msg = new Message(target, MessageType.GroupNumber, flag, group) {
				Payload = new Payload(new object[] {groupNumber})
			};
			var response = await BroadcastMessageForResponse(msg);
			return response;
		}

		/// <summary>
		///     Set device group name
		/// </summary>
		/// <param name="groupName">The new name to set. Should be less than 16 chars.</param>
		/// <param name="target">The device IP to set</param>
		/// <param name="group">Device group number</param>
		/// <returns></returns>
		public async Task<DreamScreenResponse> SetGroupName(string groupName, IPAddress target, int group) {
			var flag = Equals(target, _broadcastIp) ? MessageFlag.WriteGroup : MessageFlag.WriteIndividual;
			var msg = new Message(target, MessageType.GroupNumber, flag, group) {
				Payload = new Payload(new object[] {groupName})
			};
			var response = await BroadcastMessageForResponse(msg);
			return response;
		}


		/// <summary>
		///     Set device ambient color
		/// </summary>
		/// <param name="color">System.Drawing.Color to set the device to</param>
		/// <param name="target">Device IP to set</param>
		/// <param name="group">Group number of target device</param>
		/// <returns></returns>
		public async Task<DreamScreenResponse> SetAmbientColor(Color color, IPAddress target, int group) {
			var flag = Equals(group == 0) ? MessageFlag.WriteIndividual : MessageFlag.WriteGroup;
			var msg = new Message(target, MessageType.AmbientColor, flag, group) {
				Payload = new Payload(new object[] {color})
			};
			var response = await BroadcastMessageForResponse(msg);
			return response;
		}

		/// <summary>
		///     Set ambient Show (scene)
		/// </summary>
		/// <param name="show"><see cref="AmbientShow" />Type of Ambient Show to set</param>
		/// <param name="target">Device IP</param>
		/// <param name="group">Device group</param>
		/// <returns></returns>
		public async Task<DreamScreenResponse> SetAmbientShow(AmbientShow show, IPAddress target, int group) {
			var flag = Equals(group == 0) ? MessageFlag.WriteIndividual : MessageFlag.WriteGroup;
			var msg = new Message(target, MessageType.AmbientScene, flag, group) {
				Payload = new Payload(new object[] {(byte) show})
			};
			var response = await BroadcastMessageForResponse(msg);
			return response;
		}

		/// <summary>
		///     Set Ambient Mode
		/// </summary>
		/// <param name="mode"><see cref="AmbientMode" />Type of Ambient Mode to set</param>
		/// <param name="target">Device IP</param>
		/// <param name="group">Device group</param>
		/// <returns></returns>
		public async Task<DreamScreenResponse> SetAmbientMode(AmbientMode mode, IPAddress target, int group) {
			var flag = Equals(group == 0) ? MessageFlag.WriteIndividual : MessageFlag.WriteGroup;
			var msg = new Message(target, MessageType.AmbientModeType, flag, group) {
				Payload = new Payload(new object[] {(byte) mode})
			};
			var response = await BroadcastMessageForResponse(msg);
			return response;
		}
	}
}