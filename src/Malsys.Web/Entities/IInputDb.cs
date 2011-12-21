using System.Data.Objects.DataClasses;
using System.Linq;

namespace Malsys.Web.Entities {
	public interface IInputDb {

		IQueryable<CanonicInput> CanonicInputs { get; }

		void AddCanonicInput(CanonicInput canonicInput);


		IQueryable<InputProcess> InputProcesses { get; }

		void AddInputProcess(InputProcess inputProcess);


		IQueryable<ProcessOutput> ProcessOutputs { get; }

		void AddProcessOutput(ProcessOutput processOutput);

		void DeleteProcessOutput(ProcessOutput processOutput);


		int SaveChanges();

		void Detach(object entity);
		void Attach(IEntityWithKey entity);
	}
}
