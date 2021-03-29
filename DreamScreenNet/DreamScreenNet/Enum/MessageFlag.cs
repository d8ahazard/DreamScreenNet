namespace DreamScreenNet.Enum {
	public enum MessageFlag : byte {
		SubscriptionResponse = 0x10,
		Response = 0x41,
		WriteGroup = 0x11,
		WriteSomething = 0x17,
		GetData = 0x60,
		WriteIndividual = 0x21,
		MessageResponse = 0x3C,
		SystemMessage = 0x30,
		ColorData = 0x3D
	}
}