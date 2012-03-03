using System;
using System.Diagnostics.Contracts;
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	public class RandomGeneratorProvider : IRandomGeneratorProvider {

		private CryptographicRandomGenerator cryptoRandomInstance;

		private bool trueRandom = false;

		private IMessageLogger logger;

		private IRandomGenerator localRandomGenerator;


		[UserGettable]
		[UserSettable]
		public Constant TrueRandom {
			get { return trueRandom ? Constant.One : Constant.Zero; }
			set { trueRandom = !value.IsZero; }
		}

		[UserGettable]
		[UserSettable]
		public Constant RandomSeed { get; set; }



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


		public IRandomGenerator GetRandomGenerator() {

			Contract.Ensures(Contract.Result<IRandomGenerator>() != null);

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

		[Alias("Rand", "Random")]
		[UserCallableFunction]
		public IValue GetRandomValue(ArgsStorage args) {

			Contract.Ensures(Contract.Result<IValue>() != null);

			if (localRandomGenerator == null) {
				localRandomGenerator = GetRandomGenerator();
			}

			return localRandomGenerator.Next().ToConst();

		}


		public enum Message {

			[Message(MessageType.Info, "No random seed given, using value {0}.")]
			NoSeed,
			[Message(MessageType.Warning, "Random seed was set among with true random option. "
				+ "True random can not use the seed and it will generate always random unpredictable result.")]
			SeedWhileTrueRandom,

		}

	}
}
