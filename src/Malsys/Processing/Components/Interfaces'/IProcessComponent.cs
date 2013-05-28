// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.

namespace Malsys.Processing.Components {
	/// <summary>
	///	Process components should be connected to other components and they
	///	receives signals of begin and end of processing.
	/// </summary>
	/// <name>Process component interface</name>
	/// <group>Common</group>
	/// <remarks>
	/// Process component interface is for components that can be used with other components to do some job. Processing
	/// is started by BeginProcessing method end ended by EndProcessing method. Processing can be repeated many times.
	///
	/// BeginProcessing and EndProcessing are called by component to which is current component connected.
	///
	/// If RequiresMeasure is set to true processing will have two stages, measure and run. Both stages will process
	/// identical inputs. Measure stage is started with BeginProcessing method with measuring parameter set to true.
	/// This stage is for measuring the result. No output should be produced during measure stage because after measure
	/// stage will be process stage which should produce outputs. IOutputProvider in ProcessContext offers  temporary
	/// outputs which can be used to save temporary data while measuring data if saving data in memory is not possible.
	/// Measure stage ends with EndProcessing method and all measured data should be saved by component. If
	/// RequiresMeasure property is set to false no measure stage will happen. Directly after measure stage will begin
	/// regular process stage with method BeginProcessing with measuring parameter set to false.
	///
	/// If component do not require measuring it must check if measure state is in progress and do not do anything.
	/// </remarks>
	public interface IProcessComponent : IComponent {

		/// <summary>
		/// If set to true L-system process will have two stages, measure and then run. Otherwise measure stage will be
		/// skipped.
		/// </summary>
		/// <remarks>
		/// Retrieved after component initialization.
		/// </remarks>
		bool RequiresMeasure { get; }


		void BeginProcessing(bool measuring);

		void EndProcessing();

	}

}
