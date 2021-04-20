using System;
using System.Drawing;
using System.Threading.Tasks;
using DreamScreenNet.Devices;
using DreamScreenNet.Enum;

namespace DreamScreenNet {
	public partial class DreamScreenClient {
		/// <summary>
		///     Set the device mode
		/// </summary>
		/// /// <param name="target">Target device</param>
		/// <param name="mode"><see cref="DeviceMode" />Device Mode to set</param>
		/// <returns></returns>
		public async Task<DreamScreenResponse> SetMode(DreamDevice target, DeviceMode mode) {
			var flag = Equals(target.DeviceGroup == 0) ? MessageFlag.WriteIndividual : MessageFlag.WriteGroup;
			var msg = new Message(target.IpAddress, MessageType.Mode, flag, target.DeviceGroup)
				{Payload = new Payload(new object[] {(byte) mode})};
			var response = await BroadcastMessageForResponse(msg);
			return response;
		}

		/// <summary>
		///     Set the device group number
		/// </summary>
		/// /// <param name="target">Target group Ip</param>
		/// <param name="groupNumber">The group number to set the device to</param>
		/// <returns></returns>
		public async Task<DreamScreenResponse> SetGroupNumber(DreamDevice target, int groupNumber) {
			var flag = MessageFlag.WriteIndividual;
			var msg = new Message(target.IpAddress, MessageType.GroupNumber, flag, target.DeviceGroup) {
				Payload = new Payload(new object[] {groupNumber})
			};
			var response = await BroadcastMessageForResponse(msg);
			return response;
		}

		/// <summary>
		///     Set device group name
		/// </summary>
		/// <param name="groupName">The new name to set. Should be less than 16 chars.</param>
		/// <param name="target">The device to set</param>
		/// <returns></returns>
		public async Task<DreamScreenResponse> SetGroupName(DreamDevice target, string groupName) {
			var flag = Equals(target.IpAddress, _broadcastIp) ? MessageFlag.WriteGroup : MessageFlag.WriteIndividual;
			var msg = new Message(target.IpAddress, MessageType.GroupName, flag, target.DeviceGroup) {
				Payload = new Payload(new object[] {groupName})
			};
			var response = await BroadcastMessageForResponse(msg);
			return response;
		}


		/// <summary>
		///     Set device ambient color
		/// </summary>
		/// /// <param name="target">Target device</param>
		/// <param name="color">System.Drawing.Color to set the device to</param>
		/// <returns></returns>
		public async Task<DreamScreenResponse> SetAmbientColor(DreamDevice target, Color color) {
			var flag = Equals(target.DeviceGroup == 0) ? MessageFlag.WriteIndividual : MessageFlag.WriteGroup;
			var msg = new Message(target.IpAddress, MessageType.AmbientColor, flag, target.DeviceGroup) {
				Payload = new Payload(new object[] {color})
			};
			var response = await BroadcastMessageForResponse(msg);
			return response;
		}

		/// <summary>
		///     Set ambient Show (scene)
		/// </summary>
		/// <param name="target">Target device</param>
		/// <param name="show"><see cref="AmbientShow" />Type of Ambient Show to set</param>
		/// <returns></returns>
		public async Task<DreamScreenResponse> SetAmbientShow(DreamDevice target, AmbientShow show) {
			var flag = Equals(target.DeviceGroup == 0) ? MessageFlag.WriteIndividual : MessageFlag.WriteGroup;
			var msg = new Message(target.IpAddress, MessageType.AmbientScene, flag, target.DeviceGroup) {
				Payload = new Payload(new object[] {(byte) show})
			};
			var response = await BroadcastMessageForResponse(msg);
			return response;
		}

		/// <summary>
		///     Set Ambient Mode
		/// </summary>
		/// <param name="target">Target device</param>
		/// <param name="mode"><see cref="AmbientMode" />Type of Ambient Mode to set</param>
		/// <returns></returns>
		public async Task<DreamScreenResponse> SetAmbientMode(DreamDevice target, AmbientMode mode) {
			var flag = Equals(target.DeviceGroup == 0) ? MessageFlag.WriteIndividual : MessageFlag.WriteGroup;
			var msg = new Message(target.IpAddress, MessageType.AmbientModeType, flag, target.DeviceGroup) {
				Payload = new Payload(new object[] {(byte) mode})
			};
			var response = await BroadcastMessageForResponse(msg);
			return response;
		}
	}
}