// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Linq;
using Malsys.Web.Entities;

namespace Malsys.Web.Areas.Administration.Models {
	public class InputStatisticsModel {

		public IQueryable<InputProcess> InputProcesses;

		public IQueryable<SavedInput> SavedInputs;

	}
}