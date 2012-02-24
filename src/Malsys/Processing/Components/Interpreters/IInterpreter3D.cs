﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Malsys.Evaluators;

namespace Malsys.Processing.Components.Interpreters {
	public interface IInterpreter3D : IInterpreter {


		void Nothing(ArgsStorage args);


		void MoveForward(ArgsStorage args);

		void DrawForward(ArgsStorage args);


		void Yaw(ArgsStorage args);

		void Pitch(ArgsStorage args);

		void Roll(ArgsStorage args);


		void StartBranch(ArgsStorage args);

		void EndBranch(ArgsStorage args);


		void StartPolygon(ArgsStorage args);

		void RecordPolygonVertex(ArgsStorage args);

		void EndPolygon(ArgsStorage args);




	}
}