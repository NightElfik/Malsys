﻿using System;
using System.Security.Cryptography;

namespace Malsys {
	public class CryptographicRandomGenerator : IRandomGenerator, IDisposable {

		private RNGCryptoServiceProvider randomGenrator;

		private byte[] buffer = new byte[4];


		public CryptographicRandomGenerator() {
			randomGenrator = new RNGCryptoServiceProvider();
		}


		public void NextBytes(byte[] buffer) {
			randomGenrator.GetBytes(buffer);
		}

		public double NextDouble() {

			randomGenrator.GetBytes(buffer);

			uint value = BitConverter.ToUInt32(buffer, 0);

			return value * (1.0 / uint.MaxValue);
		}

		public void Dispose() {
			randomGenrator.Dispose();
			randomGenrator = null;
		}

	}
}
