﻿using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing {
	public class ProcessContext {

		public LsystemEvaled Lsystem { get; private set; }

		public FilesManager FilesManager { get; private set; }

		public InputBlock InputData { get; private set; }

		public ExpressionEvaluator ExpressionEvaluator { get; private set; }


		public ProcessContext(LsystemEvaled lsystem, FilesManager filesManager, InputBlock data, ExpressionEvaluator exprEal) {

			Lsystem = lsystem;
			FilesManager = filesManager;
			InputData = data;
			ExpressionEvaluator = exprEal;
		}
	}
}
