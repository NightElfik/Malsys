﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DataAnnotationsExtensions;

namespace Malsys.Web.Models {
	public class NewUserModel {

		[Required]
		[StringLength(64, MinimumLength = 4)]
		[Display(Name = "User name")]
		public string UserName { get; set; }

		[Required]
		[Email]
		[Display(Name = "E-mail address")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

	}
}
