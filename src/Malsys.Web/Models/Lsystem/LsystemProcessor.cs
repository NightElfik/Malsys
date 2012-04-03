using System;
using Malsys.Processing;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;
using System.IO;
using Elmah;

namespace Malsys.Web.Models.Lsystem {
	public class LsystemProcessor {

		private readonly ProcessManager processManager;
		private readonly InputBlockEvaled stdLib;


		public LsystemProcessor(ProcessManager processManager, InputBlockEvaled stdLib) {
			this.processManager = processManager;
			this.stdLib = stdLib;
		}


		public bool TryProcess(string sourceCode, TimeSpan timeout, FileOutputProvider fileMgr, IMessageLogger logger, out InputBlockEvaled evaledInput,
			bool cleanupFilesOnError = true, bool compileOnly = false, bool dumpConstants = false) {

			evaledInput = processManager.CompileAndEvaluateInput(sourceCode, "webInput", logger);

			if (compileOnly || logger.ErrorOccurred) {
				return !logger.ErrorOccurred;
			}


			var inputAndStdLib = stdLib.JoinWith(evaledInput);

			if (inputAndStdLib.Lsystems.Count == 0) {
				if (dumpConstants) {
					logger.LogMessage(Message.NoLsysFoundDumpingConstants);
					processManager.DumpConstants(inputAndStdLib, fileMgr, logger);
				}
			}
			else if (inputAndStdLib.ProcessStatements.Count > 0) {
				processManager.ProcessInput(inputAndStdLib, fileMgr, logger, timeout);
			}

			fileMgr.CloseAllOutputStreams();

			if (cleanupFilesOnError && logger.ErrorOccurred) {
				// cleanup created files if error occurred
				foreach (var file in fileMgr.GetOutputFiles()) {
					try {
						File.Delete(file.FilePath);
					}
					catch (Exception) {
						// TODO
					}

				}
			}

			return !logger.ErrorOccurred;

		}



		public enum Message {

			[Message(MessageType.Info, "No L-systems found, dumping constants.")]
			NoLsysFoundDumpingConstants,

		}
	}
}