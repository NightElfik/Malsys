using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Web.Mvc;
using Malsys.Web.Entities;

namespace Malsys.Web.Areas.Administration.Models {
	public class UserDetailModel {

		[Key]
		[HiddenInput(DisplayValue=false)]
		public int UserId { get; set; }

		[Required]
		[Display(Name = "User name")]
		public string Name { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "E-mail address")]
		public string Email { get; set; }


		public static UserDetailModel FromUser(User user) {
			var result = new UserDetailModel() {
				UserId = user.UserId,
				Name = user.Name,
				Email = user.Email
			};

			return result;
		}

		public void UpdateUser(User user) {

			Contract.Requires<InvalidOperationException>(user.UserId == UserId);

			user.Name = Name;
			user.Email = Email;
		}

	}
}