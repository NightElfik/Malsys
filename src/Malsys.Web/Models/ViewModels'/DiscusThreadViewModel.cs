using System;
using System.Collections.Generic;
using Malsys.Web.Entities;

namespace Malsys.Web.Models {
	public class DiscusThreadViewModel {

		public DiscusThread Thread { get; set; }

		public IEnumerable<DiscusMessage> Messages { get; set; }

	}
}