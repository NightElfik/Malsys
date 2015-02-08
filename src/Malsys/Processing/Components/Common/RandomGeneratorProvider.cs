using System;
using System.Diagnostics.Contracts;
using Malsys.Evaluators;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing.Components.Common {
	/// <summary>
	/// This component offers both, random and pseudo-random generators.
	/// It provides a callable function called random which can be called even in the L-system definition
	/// (not only at run-time).
	/// The pseudo-random number generator is used by default.
	/// If no random seed is set it will be generated randomly and a message with its value will be sent to the user
	/// to be possible to reproduce generated result.
	/// Truly-random generation should be used only by experienced users because other components will not be able to
	/// measure a generated model and results may be strange.
	/// </summary>
	/// <name>Random generator provider</name>
	/// <group>General</group>
	public class RandomGeneratorProvider : IComponent {

		private CryptographicRandomGenerator cryptoRandomInstance;

		private IRandomGenerator localRandomGenerator;


		/// <summary>
		/// If set to true, as internal random generator will be used true-random (cryptographic random) generator.
		/// For this random generator can not be set any seed and numbers are always unpredictably random.
		/// This will cause that measuring and process pass will not be the same and can cause unexpected behavior.
		/// Default random generator is pseudo-random generator.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[AccessName("trueRandom")]
		[UserGettable]
		[UserSettable]
		public Constant TrueRandom { get; set; }

		/// <summary>
		/// Random seed for pseudo-random generator to be able to reproduce random L-systems.
		/// Do not work if TrueRandom property is set.
		/// </summary>
		/// <expected>Non-negative integer.</expected>
		/// <default>random</default>
		[AccessName("randomSeed")]
		[UserGettable(IsGettableBeforeInitialiation = true)]
		[UserSettable]
		public Constant RandomSeed { get; set; }


		public IMessageLogger Logger { get; set; }


		public void Reset() {

			localRandomGenerator = null;

			TrueRandom = Constant.False;
			RandomSeed = Constant.MinusOne;
		}

		public void Initialize(ProcessContext ctxt) {
			localRandomGenerator = GetRandomGenerator();
		}

		public void Cleanup() { }

		/// <summary>
		/// Disposes true random generator if used.
		/// </summary>
		/// <remarks>
		/// Once "true random" generator is created there is no need to dispose and recreate id after every usage, it
		/// can be reused but it should be disposed at the very end.
		/// </remarks>
		public void Dispose() {
			if (cryptoRandomInstance != null) {
				cryptoRandomInstance.Dispose();
				cryptoRandomInstance = null;
			}
		}


		/// <summary>
		/// Resets random generator to initial state.
		/// </summary>
		public void ResetRandomGenerator(int? seed = null) {
			if (!TrueRandom.IsTrue) {
				ensureSeed();
				localRandomGenerator = new PseudoRandomGenerator(seed ?? RandomSeed.RoundedIntValue); ;
			}
		}


		public IRandomGenerator GetRandomGenerator() {

			Contract.Ensures(Contract.Result<IRandomGenerator>() != null);

			if (TrueRandom.IsTrue) {
				if (RandomSeed.Value < 0) {
					Logger.LogMessage(Message.SeedWhileTrueRandom);
				}

				if (cryptoRandomInstance == null) {
					cryptoRandomInstance = new CryptographicRandomGenerator();
				}

				return cryptoRandomInstance;
			}
			else {
				ensureSeed();
				return new PseudoRandomGenerator(RandomSeed.RoundedIntValue);
			}
		}

		public double Random() {

			if (localRandomGenerator == null) {
				localRandomGenerator = GetRandomGenerator();
			}

			return localRandomGenerator.NextDouble();

		}

		/// <summary>
		/// Returns random value from 0.0 (inclusive) to 1.0 (exclusive).
		/// </summary>
		[AccessName("random")]
		[UserCallableFunction(IsCallableBeforeInitialiation = true)]
		public Constant GetRandomValue(IValue[] args, IExpressionEvaluatorContext eec) {

			Contract.Ensures(Contract.Result<Constant>() != null);

			return Random().ToConst();
		}

		/// <summary>
		/// Returns random value within specified range.
		/// </summary>
		/// <parameters>
		/// The inclusive lower bound of the random number returned.
		/// The exclusive upper bound of the random number returned.
		/// </parameters>
		[AccessName("random")]
		[UserCallableFunction(2, ExpressionValueTypeFlags.Constant, ExpressionValueTypeFlags.Constant, IsCallableBeforeInitialiation = true)]
		public Constant GetRandomValueRange(IValue[] args, IExpressionEvaluatorContext eec) {

			Contract.Ensures(Contract.Result<Constant>() != null);

			double lower = ((Constant)args[0]).Value;
			double upper = ((Constant)args[1]).Value;
			return (Random() * (upper - lower) + lower).ToConst();

		}


		private void ensureSeed() {
			if (RandomSeed != null && !double.IsInfinity(RandomSeed.Value) && RandomSeed.Value >= 0) {
				return;  // covers also NaN
			}

			// no random seed set, generate random one
			Random r = new Random();
			int seed = r.Next();

			Logger.LogMessage(Message.NoSeed, seed);
			RandomSeed = seed.ToConst();
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
