using System.Collections;
using System.Collections.Generic;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <summary>
	/// Argument storage to aviod creating small arrays to supply arguments for function invokes.
	/// </summary>
	public class ArgsStorage : IEnumerable<IValue> {

		public int ArgsCount { get; private set; }

		public IValue this[int i] { get { return args[i]; } }

		private IValue[] args = new IValue[8];


		public ArgsStorage() {
			Clear();
		}

		/// <summary>
		/// Drops all previously stored arguments and takes (pops) given number of new arguments from given stack.
		/// Top of stack is last agrument.
		/// Do not check weather is enough items in stack.
		/// </summary>
		public void PopArgs(int argsCount, Stack<IValue> stack) {
			// ensure internal capacity
			if (args.Length < argsCount) {
				args = new IValue[argsCount + 2];
			}

			ArgsCount = argsCount;

			for (int i = argsCount - 1; i >= 0; i--) {
				args[i] = stack.Pop();
			}
		}

		public void Clear() {
			ArgsCount = 0;
		}


		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() {
			return new Enumerator(this);
		}

		#endregion

		#region IEnumerable<IValue> Members

		public IEnumerator<IValue> GetEnumerator() {
			return new Enumerator(this);
		}

		#endregion


		private class Enumerator : IEnumerator<IValue> {

			private ArgsStorage argsStorage;
			private int index;


			public Enumerator(ArgsStorage args) {
				argsStorage = args;
				index = -1;
			}

			#region IEnumerator Members

			object IEnumerator.Current {
				get { return argsStorage[index]; }
			}

			public bool MoveNext() {
				index++;
				return index < argsStorage.ArgsCount;
			}

			public void Reset() {
				index = -1;
			}

			#endregion

			#region IEnumerator<IValue> Members

			public IValue Current {
				get { return argsStorage[index]; }
			}

			#endregion

			#region IDisposable Members

			public void Dispose() {
				argsStorage = null;
				index = -1;
			}

			#endregion
		}
	}
}
