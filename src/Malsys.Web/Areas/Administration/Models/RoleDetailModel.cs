using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Malsys.Web.Entities;

namespace Malsys.Web.Areas.Administration.Models {
	public class RoleDetailModel {

		[Key]
		public int RoleId { get; set; }

		[Required]
		[Display(Name = "Role name")]
		public string Name { get; set; }


		public static RoleDetailModel FromRole(Role role) {
			var result = new RoleDetailModel() {
				RoleId = role.RoleId,
				Name = role.Name,
			};

			return result;
		}

		public void UpdateRole(Role role) {

			Contract.Requires<InvalidOperationException>(role.RoleId == RoleId);

			role.Name = Name;
			role.NameLowercase = Name.ToLower();
		}

	}
}
