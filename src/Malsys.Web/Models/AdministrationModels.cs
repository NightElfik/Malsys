using System.ComponentModel.DataAnnotations;
using Malsys.Web.Entities;

namespace Malsys.Web.Models {

	public class UserModel {

		[Key]
		public string UserId { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		[Display(Name = "Email address")]
		public string Email { get; set; }

		public string Comment { get; set; }

		[Display(Name = "Is approved")]
		public bool IsApproved { get; set; }


		public static UserModel FromUser(User user) {
			var result = new UserModel() {
				UserId = user.UserName,
				Email = user.Email,
				Comment = user.Comment,
				IsApproved = user.IsApproved
			};

			return result;
		}

		public void UpdareUser(User user) {

			user.Email = Email;
			user.Comment = Comment;
			user.IsApproved = IsApproved;
		}

	}

}