﻿using Malsys.Evaluators;
using Malsys.SemanticModel.Evaluated;

namespace Malsys.Processing {
	public class ProcessContext {

		public LsystemEvaled Lsystem { get; private set; }

		public FilesManager FilesManager { get; private set; }

		public InputBlock InputData { get; private set; }

		public MalsysEvaluator Evaluator { get; private set; }

		public MessageLogger Messages { get; private set; }


		public ProcessContext(LsystemEvaled lsystem, FilesManager filesManager,
				InputBlock data, MalsysEvaluator evaluator, MessageLogger msgs) {

			Lsystem = lsystem;
			FilesManager = filesManager;
			InputData = data;
			Evaluator = evaluator;
			Messages = msgs;

			if (filesManager != null) {
				filesManager.CurrentLsystem = lsystem;
			}
		}
	}
}
