// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.FSharp.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Malsys.Tests.Prototypes {
	[TestClass]
	public class MetadataStoreComparison {

		/// <summary>
		/// This test shows that storing KeyValuePair array is probably the best way how to store key-value data.
		/// </summary>
		[TestMethod]
		public void DoTests() {

			var dict = new Dictionary<string, object>();
			var map = MapModule.Empty<string, object>();
			var arr = new KeyValuePair<string, object>[0];
			var tuple = new Tuple<string, object>[0];

			Console.WriteLine("i\tdict\tmap\tlist\ttuple");

			for (int i = 0; i < 32; i++) {

				long dictLen, mapLen, arrLen, tupleLen;
				var serDict = serializeDeserialize(dict, out dictLen);
				var serMap = serializeDeserialize(map, out mapLen);
				var serArr = serializeDeserialize(arr, out arrLen);
				var serTuple = serializeDeserialize(tuple, out tupleLen);

				Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", i, dictLen, mapLen, arrLen, tupleLen);

				CollectionAssert.AreEqual(dict, serDict);
				CollectionAssert.AreEqual(map.ToList(), serMap.ToList());
				CollectionAssert.AreEqual(arr, serArr);
				CollectionAssert.AreEqual(tuple, serTuple);

				dict.Add(i.ToString(), i * i);
				map = map.Add(i.ToString(), i * i);
				arr = map.ToArray();
				tuple = arr.Select(x => new Tuple<string, object>(x.Key, x.Value)).ToArray();
			}

		}


		private T serializeDeserialize<T>(T obj, out long length) {

			var serializer = new BinaryFormatter();

			var ms = new MemoryStream();
			serializer.Serialize(ms, obj);

			length = ms.Length;
			ms.Seek(0, SeekOrigin.Begin);

			return (T)serializer.Deserialize(ms);

		}
	}
}
