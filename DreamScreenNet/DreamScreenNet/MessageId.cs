namespace DreamScreenNet {
	public static class MessageId {
		private static uint _identifier = 1;
		private static readonly object IdentifierLock = new object();

		public static uint GetNextIdentifier() {
			lock (IdentifierLock) {
				_identifier++;
			}

			return _identifier;
		}
	}
}