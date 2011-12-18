using System;
using Malsys.SemanticModel;

namespace Malsys.Processing.Components.Common {
	public class RandomGeneratorProvider : IRandomGeneratorProvider {

		private CryptographicRandomGenerator cryptoRandomInstance;

		private bool trueRandom = false;

		private IMessageLogger logger;


		[UserSettable]
		public Constant TrueRandom { set { trueRandom = !value.IsZero; } }

		[UserSettable]
		public Constant RandomSeed { get; set; }


		public IRandomGenerator GetRandomGenerator() {

			if (trueRandom) {
				if (RandomSeed != null) {
					logger.LogMessage(Message.SeedWhileTrueRandom);
					RandomSeed = null;
				}

				return cryptoRandomInstance;
			}
			else {
				if (RandomSeed == null) {
					Random r = new Random();
					int seed = r.Next();

					logger.LogMessage(Message.NoSeed, seed);
					RandomSeed = seed.ToConst();
				}

				return new PseudoRandomGenerator(RandomSeed.RoundedIntValue);
			}
		}


		public bool RequiresMeasure {
			get { return false; }
		}

		public void Initialize(ProcessContext ctxt) {

			logger = ctxt.Logger;

			if (trueRandom) {
				cryptoRandomInstance = new CryptographicRandomGenerator();
			}
		}

		public void Cleanup() {
			if (cryptoRandomInstance != null) {
				cryptoRandomInstance.Dispose();
			}
		}

		public void BeginProcessing(bool measuring) {
		}

		public void EndProcessing() {
		}

		public enum Message {

			[Message(MessageType.Info, "No random seed given, using value {0}.")]
			NoSeed,
			[Message(MessageType.Warning, "Random seed was set among with true random option. "
				+"True random can not use the seed and it will generate always random unpredictable result.")]
			SeedWhileTrueRandom,

		}

	}
}
