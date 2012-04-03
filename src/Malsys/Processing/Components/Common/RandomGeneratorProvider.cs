using System;
using System.Diagnostics.Contracts;
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	public class RandomGeneratorProvider : IComponent {

		private CryptographicRandomGenerator cryptoRandomInstance;

		private bool trueRandom = false;

		private IMessageLogger logger;

		private IRandomGenerator localRandomGenerator;


		[Alias("trueRandom")]
		[UserGettable]
		[UserSettable]
		public Constant TrueRandom {
			get { return trueRandom ? Constant.One : Constant.Zero; }
			set { trueRandom = !value.IsZero; }
		}

		[Alias("randomSeed")]
		[UserGettable(IsGettableBeforeInitialiation=true)]
		[UserSettable]
		public Constant RandomSeed { get; set; }


		public RandomGeneratorProvider(IMessageLogger logger) {
			this.logger = logger;
		}


		public void Initialize(ProcessContext ctxt) {

			logger = ctxt.Logger;

		}

		public void Cleanup() {

			localRandomGenerator = null;

			if (cryptoRandomInstance != null) {
				cryptoRandomInstance.Dispose();
			}

		}

		/// <summary>
		/// Resets random generator to initial state.
		/// </summary>
		public void Reset() {
			localRandomGenerator = null;
		}


		public IRandomGenerator GetRandomGenerator() {

			Contract.Ensures(Contract.Result<IRandomGenerator>() != null);

			if (trueRandom) {
				if (RandomSeed != null) {
					logger.LogMessage(Message.SeedWhileTrueRandom);
					RandomSeed = null;
				}

				if (cryptoRandomInstance == null) {
					cryptoRandomInstance = new CryptographicRandomGenerator();
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

		/// <summary>
		/// Returns random value from 0.0 (inclusive) to 1.0 (exclusive).
		/// </summary>
		[Alias(true, "Random", "random")]
		[UserCallableFunction(IsCallableBeforeInitialiation=true)]
		public Constant GetRandomValue(IValue[] args, IExpressionEvaluatorContext eec) {

			Contract.Ensures(Contract.Result<IValue>() != null);

			if (localRandomGenerator == null) {
				localRandomGenerator = GetRandomGenerator();
			}

			return localRandomGenerator.NextDouble().ToConst();

		}

		/// <summary>
		/// Returns random value within specified range.
		/// </summary>
		[Alias(true, "Random", "random")]
		[UserCallableFunction(2, ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant, IsCallableBeforeInitialiation = true)]
		public Constant GetRandomValueRange(IValue[] args, IExpressionEvaluatorContext eec) {

			Contract.Ensures(Contract.Result<IValue>() != null);

			if (localRandomGenerator == null) {
				localRandomGenerator = GetRandomGenerator();
			}

			double lower = ((Constant)args[0]).Value;
			double upper = ((Constant)args[1]).Value;

			return (localRandomGenerator.NextDouble() * (upper - lower) + lower).ToConst();

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
