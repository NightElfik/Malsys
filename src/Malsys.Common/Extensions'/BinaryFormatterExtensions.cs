using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Malsys {
	public static class BinaryFormatterExtensions {

		public static byte[] SerializeToBytes(this BinaryFormatter serializer, object item) {

			using (var stream = new MemoryStream()) {
				serializer.Serialize(stream, item);
				return stream.GetBuffer();
			}

		}

		public static T DeserializeFromBytes<T>(this BinaryFormatter serializer, byte[] data) where T : class {

			using (var ms = new MemoryStream(data)) {
				return serializer.Deserialize(ms) as T;
			}

		}

	}
}
