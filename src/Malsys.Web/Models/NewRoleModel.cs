/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.ComponentModel.DataAnnotations;

namespace Malsys.Web.Models {
	public class NewRoleModel {

		[Required]
		[StringLength(64, MinimumLength = 4)]
		[Display(Name = "Role name")]
		public string RoleName { get; set; }

	}
}