﻿using System.ComponentModel.DataAnnotations;

namespace Malsys.Web.Models {
	public class LogOnModel {

		[Required]
		[Display(Name = "User name")]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }

	}
}