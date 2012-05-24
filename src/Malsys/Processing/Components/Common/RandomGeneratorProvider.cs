/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
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


		public IMessageLogger Logger { get; set; }


		/// <summary>
		/// If set to true as random generator will be used
		/// true-random (cryptographic random) generator.
		/// For this random generator can not be set any seed and numbers are
		/// always unpredictably random.
		/// If set to false as random generator will be used pseudo-random generator.
		/// </summary>
		/// <expected>true or false</expected>
		/// <default>false</default>
		[AccessName("trueRandom")]
		[UserGettable]
		[UserSettable]
		public Constant TrueRandom { get; set; }

		/// <summary>
		/// If set pseudo-random generator will generate always same sequence of random numbers.
		/// Do not work if TrueRandom property is set.
		/// </summary>
		/// <expected>Non-negative integer.</expected>
		/// <default>random</default>
		[AccessName("randomSeed")]
		[UserGettable(IsGettableBeforeInitialiation = true)]
		[UserSettable]
		public Constant RandomSeed { get; set; }


		public void Initialize(ProcessContext ctxt) { }

		public void Cleanup() {

			localRandomGenerator = null;

			if (cryptoRandomInstance != null) {
				cryptoRandomInstance.Dispose();
			}

			TrueRandom = Constant.False;
			RandomSeed = Constant.MinusOne;

		}

		/// <summary>
		/// Resets random generator to initial state.
		/// </summary>
		public void Reset(int? seed = null) {
			if (!TrueRandom.IsTrue) {
				if (seed == null) {
					ensureSeed();
				}
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
				if (RandomSeed.Value < 0) {
					ensureSeed();
				}

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

			Contract.Ensures(Contract.Result<IValue>() != null);

			if (localRandomGenerator == null) {
				localRandomGenerator = GetRandomGenerator();
			}

			return localRandomGenerator.NextDouble().ToConst();

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

			Contract.Ensures(Contract.Result<IValue>() != null);

			if (localRandomGenerator == null) {
				localRandomGenerator = GetRandomGenerator();
			}

			double lower = ((Constant)args[0]).Value;
			double upper = ((Constant)args[1]).Value;

			return (localRandomGenerator.NextDouble() * (upper - lower) + lower).ToConst();

		}

		private void ensureSeed() {
			if (RandomSeed != null && !double.IsInfinity(RandomSeed.Value) && RandomSeed.Value >= 0) {
				return;  // coveres also NaN
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
