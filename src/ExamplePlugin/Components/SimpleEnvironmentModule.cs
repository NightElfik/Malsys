using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Windows.Media.Media3D;
using Malsys;
using Malsys.Evaluators;
using Malsys.Processing;
using Malsys.Processing.Components;
using Malsys.Processing.Components.Interpreters;
using Malsys.SemanticModel;
using Malsys.SemanticModel.Evaluated;

using Symbol = Malsys.SemanticModel.Symbol<Malsys.SemanticModel.Evaluated.IValue>;

namespace ExamplePlugin.Components {
	public class SimpleEnvironmentModule : ISymbolProvider {

		private ProcessContext context;

		private Bitmap envMap;



		[UserConnectable]
		public ISymbolProvider SymbolProvider { get; set; }

		[UserConnectable]
		public TurtleInterpreter TurtleInterpreter { get; set; }

		[UserConnectable]
		public IInterpreterCaller InterpreterCaller { get; set; }


		/// <summary>
		/// Scale of environment.
		/// </summary>
		/// <expected>Positive number.</expected>
		/// <default>1</default>
		[AccessName("envScale")]
		[UserSettable]
		public Constant EnvScale { get; set; }


		[AccessName("canLive")]
		[UserCallableFunction]
		public Constant CanLive(IValue[] args, IExpressionEvaluatorContext eec) {

			Contract.Ensures(Contract.Result<Constant>() != null);

			Point3D position = TurtleInterpreter.CurrPosition;

			int envX = (int)Math.Round(position.X / EnvScale.Value);
			int envY = (int)Math.Round(position.Z / EnvScale.Value);

			if (envX < 0 || envX >= envMap.Width || envY < 0 || envY >= envMap.Height) {
				return Constant.False;
			}

			var color = envMap.GetPixel(envX, envY);
			double intensity = (color.R + color.G + color.B) / 3.0;

			return position.Y > 0 && intensity > position.Y ? Constant.True : Constant.False;
		}

		[AccessName("getEnvValue")]
		[UserCallableFunction]
		public Constant GetEnvValue(IValue[] args, IExpressionEvaluatorContext eec) {

			Contract.Ensures(Contract.Result<Constant>() != null);

			Point3D position = TurtleInterpreter.CurrPosition;

			int envX = (int)Math.Round(position.X / EnvScale.Value);
			int envY = (int)Math.Round(position.Z / EnvScale.Value);

			if (envX < 0 || envX >= envMap.Width || envY < 0 || envY >= envMap.Height) {
				return Constant.MinusOne;
			}

			var color = envMap.GetPixel(envX, envY);
			double intensity = (color.R + color.G + color.B) / 3.0;
			return intensity.ToConst();
		}

		[AccessName("getEnvPosition")]
		[UserCallableFunction]
		public ValuesArray GetEnvPosition(IValue[] args, IExpressionEvaluatorContext eec) {

			Contract.Ensures(Contract.Result<ValuesArray>() != null);

			Point3D position = TurtleInterpreter.CurrPosition;

			return new ValuesArray(position.X.ToConst(), position.Y.ToConst(), position.Z.ToConst());
		}

		#region IComponent Members

		public IMessageLogger Logger { get; set; }


		public void Reset() {
			context = null;
			Logger = null;
			EnvScale = Constant.One;
		}

		public void Initialize(ProcessContext context) {
			this.context = context;

			try {
				envMap = (Bitmap)Bitmap.FromFile("envMap.png");
			}
			catch {
				Logger.LogError("", "Failed to load environment map.");
				envMap = new Bitmap(1, 1);
			}
		}

		public void Cleanup() { }

		public void Dispose() { }

		#endregion

		#region IEnumerable Members

		public IEnumerator<Symbol<IValue>> GetEnumerator() {
			return new Enumerator(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return new Enumerator(this);
		}

		#endregion

		#region IProcessComponent Members

		public bool RequiresMeasure {
			get { return false; }
		}

		public void BeginProcessing(bool measuring) {
			SymbolProvider.BeginProcessing(measuring);
		}

		public void EndProcessing() {
			SymbolProvider.EndProcessing();
		}

		#endregion


		private class Enumerator : IEnumerator<Symbol> {

			ISymbolProvider symbolProvider;
			IEnumerator<Symbol> symbolProviderEnumerator;
			IInterpreterCaller interpreterCaller;

			Symbol current = null;

			public Enumerator(SimpleEnvironmentModule parent) {
				symbolProvider = parent.SymbolProvider;
				symbolProviderEnumerator = symbolProvider.GetEnumerator();
				interpreterCaller = parent.InterpreterCaller;
				interpreterCaller.BeginProcessing(false);
			}



			public Symbol Current {
				get { return current; }
			}


			public void Dispose() {
				symbolProvider.Dispose();
				interpreterCaller.EndProcessing();
			}


			object System.Collections.IEnumerator.Current {
				get { return current; }
			}

			public bool MoveNext() {
				bool nextExists = symbolProviderEnumerator.MoveNext();
				if (nextExists) {
					current = symbolProviderEnumerator.Current;

					// interpret
					interpreterCaller.ProcessSymbol(current);

					return true;
				}
				else {
					current = null;
					return false;
				}
			}

			public void Reset() {
				throw new NotImplementedException();
			}
		}
	}
}
