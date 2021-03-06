﻿using System;
using System.IO;
using Elmah;
using Malsys.Processing;
using Malsys.Processing.Output;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Web.Models.Lsystem {
	public class LsystemProcessor {

		private readonly ProcessManager processManager;
		private readonly InputBlockEvaled stdLib;


		public LsystemProcessor(ProcessManager processManager, InputBlockEvaled stdLib) {
			this.processManager = processManager;
			this.stdLib = stdLib;
		}


		public bool TryProcess(string sourceCode, TimeSpan timeout, FileOutputProvider fileMgr, IMessageLogger logger,
				out InputBlockEvaled evaledInput, out InputBlockEvaled evaledInputNoStdlib, bool cleanupFilesOnError = true, bool compileOnly = false) {

#if !DEBUG
			try {
#endif
				evaledInput = processManager.CompileAndEvaluateInput(sourceCode, "webInput", logger);
#if !DEBUG
			}
			catch (Exception ex) {
				ErrorSignal.FromCurrentContext().Raise(ex);  // log exception by Elmah
				logger.LogMessage(Message.ExceptionThrownWhileProcessingInput, ex.GetType().Name);
				evaledInput = null;
				evaledInputNoStdlib = null;
				return false;
			}
#endif

			if (compileOnly || logger.ErrorOccurred) {
				evaledInputNoStdlib = null;
				return !logger.ErrorOccurred;
			}

			evaledInputNoStdlib = evaledInput.ShallowClone();
			evaledInput.Append(stdLib);

			if (evaledInput.ProcessStatements.Count > 0) {
#if !DEBUG
				try {
#endif
					processManager.ProcessInput(evaledInput, fileMgr, logger, timeout);
#if !DEBUG
				}
				catch (Exception ex) {
					ErrorSignal.FromCurrentContext().Raise(ex);  // log exception by Elmah
					logger.LogMessage(Message.ExceptionThrownWhileProcessingInput, ex.GetType().Name);
					return false;
				}
#endif
			}

			fileMgr.CloseAllOutputStreams();

			if (cleanupFilesOnError && logger.ErrorOccurred) {
				// cleanup created files if error occurred
				foreach (var file in fileMgr.GetOutputFiles()) {
					try {
						File.Delete(file.FilePath);
					}
					catch (Exception ex) {
						ErrorSignal.FromCurrentContext().Raise(ex);
					}

				}
			}

			return !logger.ErrorOccurred;

		}



		public enum Message {

			[Message(MessageType.Error, "Failed to process input, `{0}` was thrown.")]
			ExceptionThrownWhileProcessingInput,

		}
	}
}
