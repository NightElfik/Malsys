using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Malsys;
using Malsys.Expressions;

namespace ExpressionsInteractive {
	public static class Program {

		public static string prompt = "> ";

		public const string commitSuffix = ";;";
		public const string specialCmdPrefix = "#";
		public const string helpCmd = "help";
		public const string exitCmd = "exit";
		public const string showVarsCmd = "vars";
		public const string showFunsCmd = "funs";
		public const string showAllVarsCmd = "allvars";
		public const string showAllFunsCmd = "allfuns";

		private static bool verbose = false;

		static void Main(string[] args) {
			if (args.Length == 1) {
				ushort port;
				if (ushort.TryParse(args[0], out port)) {
					Listen(port);
				}
				else {
					Console.WriteLine("Failed to parse `{0}` as port number.", args[0]);
				}
			}
			else if (args.Length == 2) {
				// just for Milan :)
				highlightFileToHtml(args[0], args[1]);
				return;
			}

			Start(Console.In, Console.Out);
		}

		public static void Listen(int port) {
			TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
			tcpListener.Start();
			if (verbose) {
				Console.WriteLine("Listening started at port `{0}`.", port);
			}

			while (true) {
				//blocks until a client has connected to the server
				TcpClient client = tcpListener.AcceptTcpClient();
				if (verbose) {
					Console.WriteLine("Client accepted from {0}.", client.Client.RemoteEndPoint.ToString());
				}

				Thread clientThread = new Thread(new ParameterizedThreadStart(handleTcpClient));
				clientThread.Start(client);
			}

		}

		private static void handleTcpClient(object client) {
			TcpClient tcpClient = (TcpClient)client;

			if (!tcpClient.GetStream().CanRead || !tcpClient.GetStream().CanWrite) {
				return;
			}

			try {
				Start(new StreamReader(tcpClient.GetStream()), new StreamWriter(tcpClient.GetStream()));
			}
			catch (Exception ex) {
				if (verbose) {
					Console.WriteLine("Exception aborted client's session: {0}.", ex.ToString());
				}
			}
		}

		public static void Start(TextReader reader, TextWriter writer) {

			writer.WriteLine("Expressions interactive by Marek Fišer 2011");
			writer.WriteLine("To commit input, end line with `{0}`.", commitSuffix);
			writer.WriteLine("To show help, write `{0}{1}{2}`.", specialCmdPrefix, helpCmd, commitSuffix);
			writer.WriteLine();

			Evaluator etor = new Evaluator();
			StringBuilder inputSb = new StringBuilder();
			bool running = true;

			while (running) {
				writer.Write(prompt);
				writer.Flush();
				string str = reader.ReadLine();
				if (str == null) {
					return;
				}

				str = str.Trim();

				if (str.StartsWith(specialCmdPrefix) && str.EndsWith(commitSuffix)) {
					string cmd = str.Substring(specialCmdPrefix.Length, str.Length - (commitSuffix.Length + specialCmdPrefix.Length)).ToLowerInvariant();
					switch (cmd) {
						case helpCmd: showHelp(writer); break;
						case exitCmd: running = false; break;
						case showVarsCmd: showVars(writer, etor); break;
						case showFunsCmd: showFuns(writer, etor); break;
						case showAllVarsCmd: showAllVars(writer, etor); break;
						case showAllFunsCmd: showAllFuns(writer, etor); break;
						default: showHelp(writer); break;
					}

					writer.Flush();
					inputSb.Clear();
					continue;
				}

				inputSb.AppendLine(str);

				if (str.EndsWith(commitSuffix)) {
#if !DEBUG
					try {
#endif
					writer.WriteLine(etor.EvaluateStr(inputSb.ToString(), "Interactive input"));
					writer.Flush();
#if !DEBUG
					}
					catch (Exception ex) {
						writer.WriteLine(ex.ToString());
					}
#endif

					inputSb.Clear();
				}

			}

			writer.WriteLine("... see you");
		}


		private static void showHelp(TextWriter writer) {
			writer.WriteLine();
			writer.WriteLine();
			writer.WriteLine("Expressions interactive by Marek Fišer 2011");
			writer.WriteLine();
			writer.WriteLine();
			writer.WriteLine("Basic usage -- expressions");
			writer.WriteLine();
			writer.WriteLine("try enter:");
			writer.WriteLine("2 * (5 - pi)" + commitSuffix);
			writer.WriteLine();
			writer.WriteLine();
			writer.WriteLine("Special commands");
			writer.WriteLine();
			writer.WriteLine("\t{0}{1}{2} \t shows this help", specialCmdPrefix, helpCmd, commitSuffix);
			writer.WriteLine("\t{0}{1}{2} \t ends program", specialCmdPrefix, exitCmd, commitSuffix);
			writer.WriteLine();
			writer.WriteLine("\t{0}{1}{2} \t shows all user-defined variables", specialCmdPrefix, showVarsCmd, commitSuffix);
			writer.WriteLine("\t{0}{1}{2} \t shows all user-defined functions", specialCmdPrefix, showFunsCmd, commitSuffix);
			writer.WriteLine();
			writer.WriteLine("\t{0}{1}{2} \t shows all variables including constants", specialCmdPrefix, showAllVarsCmd, commitSuffix);
			writer.WriteLine("\t{0}{1}{2} \t shows all functions including operators", specialCmdPrefix, showAllFunsCmd, commitSuffix);
			writer.WriteLine();
			writer.WriteLine();
			writer.WriteLine("Input syntax");
			writer.WriteLine();
			writer.WriteLine("<Expression> | <VariableDefinition> | <FunctionDefinition>");
			writer.WriteLine();
			writer.WriteLine("More statements can be written in one input divided by semicolon:");
			writer.WriteLine("5 + 9; 2 * pi; let x = 20; 20 - x{0}", commitSuffix);
			writer.WriteLine();
			writer.WriteLine("Any value can be also array.");
			writer.WriteLine("{{1},{1}}; {sqrt(10), {{1+2}, {}}}; {1,{2},3}[2]" + commitSuffix);
			writer.WriteLine();
			writer.WriteLine();
			writer.WriteLine("Variable definition sytax");
			writer.WriteLine();
			writer.WriteLine("let <Name> = <Expression>");
			writer.WriteLine();
			writer.WriteLine("More variable definitions can be divided by semicolon.");
			writer.WriteLine();
			writer.WriteLine();
			writer.WriteLine("Function definition sytax");
			writer.WriteLine();
			writer.WriteLine("fun <Name> (<Params>) { <OptionalVariableDefinitions> ; <Expression> }");
			writer.WriteLine();
			writer.WriteLine("Parameters <Params> can also have optional value:");
			writer.WriteLine("fun f(x, y = 10) { x - y } ; f(15) + f(5, 7)" + commitSuffix);
			writer.WriteLine("Delimiting semicolon is not mandatory.");
			writer.WriteLine();
			writer.WriteLine();
			writer.WriteLine("Comments sytax");
			writer.WriteLine();
			writer.WriteLine("/* multi-line");
			writer.WriteLine("comment */");
			writer.WriteLine("// single-line comment");
			writer.WriteLine();
			writer.WriteLine();

		}

		private static void showVars(TextWriter writer, Evaluator etor) {
			writer.WriteLine();
			writer.WriteLine("User-defined variables");
			writer.WriteLine();

			foreach (var var in etor.DefinedVariables) {
				writer.WriteLine("{0} = {1}", var.Key, var.Value);
			}

			writer.WriteLine();
		}

		private static void showFuns(TextWriter writer, Evaluator etor) {
			writer.WriteLine();
			writer.WriteLine("User-defined functions");
			writer.WriteLine();

			foreach (var var in etor.DefinedFunctions) {
				var fun = var.Value;

				writer.Write(fun.Name);
				writer.Write("(");

				int param;
				for (param = 0; param < fun.MandatoryParamsCount; param++) {
					if (param > 0) {
						writer.Write(", ");
					}
					writer.Write(fun.ParametersNames[param]);
				}

				for (; param < fun.ParametersCount; param++) {
					if (param > 0) {
						writer.Write(", ");
					}
					writer.Write(fun.ParametersNames[param]);
					writer.Write(" = ");
					writer.Write(fun.GetOptionalParamValue(param));
				}

				writer.WriteLine(")");
			}

			writer.WriteLine();
		}

		private static void showAllVars(TextWriter writer, Evaluator etor) {
			showVars(writer, etor);

			writer.WriteLine();
			writer.WriteLine("Built-in constants");
			writer.WriteLine();

			foreach (var c in KnownConstant.GetAllDefinedConstants()) {
				writer.WriteLine("{0} = {1}", c.Name, c.Value.ToStringInvariant());
			}

			writer.WriteLine();
		}

		private static void showAllFuns(TextWriter writer, Evaluator etor) {
			showFuns(writer, etor);

			writer.WriteLine();
			writer.WriteLine("Built-in functions");
			writer.WriteLine();

			foreach (var fun in FunctionCore.GetAllDefinedFunctions()) {
				writer.Write(fun.Name);
				writer.Write("(");

				if (fun.ParametersCount == FunctionCore.AnyParamsCount) {
					writer.Write("/*any number of arguments*/");
				}
				else {
					for (int param = 0; param < fun.ParametersCount; param++) {
						if (param > 0) {
							writer.Write(", ");
						}
						writer.Write(fun.ParamsTypes[param % fun.ParamsTypes.Length].ToTypeStringOneWord());
						writer.Write(param + 1);
					}
				}

				writer.WriteLine(")");
			}

			writer.WriteLine();
			writer.WriteLine("Built-in operators");
			writer.WriteLine();

			foreach (var op in OperatorCore.GetAllDefinedFunctions()) {
				if (op.Arity == 1) {
					writer.Write(op.Syntax);
					writer.Write(" ");
					writer.Write(op.ParamsTypes[0].ToTypeStringOneWord());
				}
				else if (op.Arity == 2) {
					writer.Write(op.ParamsTypes[0].ToTypeStringOneWord());
					writer.Write("1 ");
					writer.Write(op.Syntax);
					writer.Write(" ");
					writer.Write(op.ParamsTypes[1].ToTypeStringOneWord());
					writer.Write("2 ");
				}
				writer.WriteLine();
			}

			writer.WriteLine();
		}


		private static void highlightFileToHtml(string filePath, string outPath) {

			using (Stream outStream = File.Open(outPath, FileMode.Create, FileAccess.Write)) {
				using (StreamWriter writer = new StreamWriter(outStream)) {
					writer.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
					writer.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
					writer.WriteLine();
					writer.WriteLine("<head>");
					writer.WriteLine("<title>Highlighted L-system from `{0}`</title>".Fmt(filePath));
					writer.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"lsystem.css\" />");
					writer.WriteLine("</head>");
					writer.WriteLine();
					writer.WriteLine("<pre class=\"lsrc\">");

					var result = Malsys.SourceCode.Highlighters.HtmlHighlighter.HighlightFromString(File.ReadAllText(filePath), filePath);
					writer.WriteLine(result);

					writer.WriteLine("</pre>");
					writer.WriteLine();
					writer.WriteLine("</body>");
					writer.WriteLine("</html>");
				}
			}
		}
	}
}
