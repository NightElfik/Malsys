using System.IO;
using Malsys.SemanticModel.Evaluated;
using ConstsMap = Microsoft.FSharp.Collections.FSharpMap<string, Malsys.SemanticModel.Evaluated.IValue>;

namespace Malsys.Processing.Components.Common {
	public class ConstantsDumper {

		public void DumpConstants(ConstsMap constants, IFilesManager storageManager, MessageLogger logger) {

			if (constants.Count == 0) {
				return;
			}

			using (TextWriter writer = new StreamWriter(storageManager.GetOutputStream<ConstantsDumper>(".txt"))) {

				foreach (var c in constants) {
					writer.Write(c.Key);
					writer.Write(" = ");
					writer.Write(c.Value);
					writer.WriteLine(";");
				}
			}

		}
	}
}
