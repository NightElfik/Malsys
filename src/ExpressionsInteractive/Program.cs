using System;
using System.Text;

namespace ExpressionsInteractive {
	public static class Program {

		public static string prompt = "> ";
		public static string commitSuffix = ";;";

		static void Main(string[] args) {
			Evaluator eTor = new Evaluator();

			Console.WriteLine("Expressions interactive");
			Console.WriteLine("To commit input, end line with `;;` (two semicolons).");

			StringBuilder inputSb = new StringBuilder();

			while (true) {
				Console.Write(prompt);
				string str = Console.ReadLine().TrimEnd();
				inputSb.AppendLine(str);

				if (str.EndsWith(commitSuffix)) {
#if !DEBUG
					try {
#endif
					Console.WriteLine(eTor.EvaluateStr(inputSb.ToString()));
#if !DEBUG
					}
					catch (Exception ex) {
						Console.WriteLine(ex.ToString());

						throw;

					}
#endif

					inputSb.Clear();
				}

			}


		}

	}
}
