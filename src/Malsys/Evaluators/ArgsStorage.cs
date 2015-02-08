using System;
using System.Collections;
using System.Collections.Generic;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Evaluators {
	/// <summary>
	/// Argument storage to avoid creating small arrays to supply arguments for function invokes.
	/// </summary>
	public class ArgsStorage : IEnumerable<IValue> {

		public int ArgsCount { get; private set; }

		public IValue this[int i] { get { return args[i]; } }

		private IValue[] args = new IValue[8];



		public bool IsEmpty { get { return ArgsCount == 0; } }


		/// <summary>
		/// Clears all previously stored arguments and takes (pops) given number of new arguments from given stack.
		/// Top of stack is last argument.
		/// Do not check weather is enough items in stack.
		/// </summary>
		public void PopArgs(int argsCount, Stack<IValue> stack) {

			ArgsCount = argsCount;
			ensureCapacity(ArgsCount);

			for (int i = argsCount - 1; i >= 0; i--) {
				args[i] = stack.Pop();
			}
		}

		/// <summary>
		/// Clears all previously stored arguments and copies all arguments from given first list.
		/// </summary>
		public void AddArgs(List<IValue> arguments) {

			ArgsCount = arguments.Count;
			ensureCapacity(ArgsCount);

			for (int i = 0; i < arguments.Count; i++) {
				args[i] = arguments[i];
			}
		}

		/// <summary>
		/// Clears all previously stored arguments and copies all arguments from first list and if second is longer,
		/// rest from second.
		/// </summary>
		public void AddArgs(List<IValue> arguments, List<IValue> defaultValues) {

			ArgsCount = Math.Max(arguments.Count, defaultValues.Count);
			ensureCapacity(ArgsCount);

			int i = 0;

			for (; i < arguments.Count; i++) {
				args[i] = arguments[i];
			}

			for (; i < defaultValues.Count; i++) {
				args[i] = defaultValues[i];
			}
		}


		IEnumerator IEnumerable.GetEnumerator() {
			return new Enumerator(this);
		}

		public IEnumerator<IValue> GetEnumerator() {
			return new Enumerator(this);
		}


		private void ensureCapacity(int capacity) {
			if (args.Length < capacity) {
				args = new IValue[capacity + 4];
			}
		}


		private class Enumerator : IEnumerator<IValue> {

			private ArgsStorage argsStorage;
			private int index;


			public Enumerator(ArgsStorage args) {
				argsStorage = args;
				index = -1;
			}


			public IValue Current {
				get { return argsStorage[index]; }
			}

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


			public void Dispose() {
				argsStorage = null;
				index = -1;
			}

		}
	}
}
