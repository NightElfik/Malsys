using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Malsys.Web.Models {
	public class NewRoleModel {

		[Required]
		[Display(Name = "Role name")]
		public string RoleName { get; set; }

	}
}