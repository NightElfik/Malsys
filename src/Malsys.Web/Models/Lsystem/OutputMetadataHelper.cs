/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.FSharp.Collections;

namespace Malsys.Web.Models.Lsystem {
	public static class OutputMetadataHelper {

		private static byte[] emptyData = new byte[0];
		private static KeyValuePair<string, object>[] emptyMetadata = new KeyValuePair<string, object>[0];


		public static byte[] SerializeMetadata(FSharpMap<string, object> metadata) {

			if (metadata == null || metadata.Count == 0) {
				return emptyData;
			}

			KeyValuePair<string, object>[] metadataTransformed = metadata.ToArray();
			return new BinaryFormatter().SerializeToBytes(metadataTransformed);

		}

		public static KeyValuePair<string, object>[] DeserializeMetadata(byte[] data) {

			if (data == null || data.Length == 0) {
				return emptyMetadata;
			}

			var result = new BinaryFormatter().DeserializeFromBytes<KeyValuePair<string, object>[]>(data);
			if (result == null) {
				return emptyMetadata;
			}

			return result;

		}

		public static T TryGetValue<T>(KeyValuePair<string, object>[] metadata, string key, T defaultValue = default(T)) {

			var value = metadata.Where(x => x.Key == key).Select(x => x.Value).SingleOrDefault();
			if (value == null || !(value is T)) {
				return defaultValue;
			}

			return (T)value;

		}


	}
}