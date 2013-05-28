// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Areas.Administration.Models;
using Malsys.Web.Entities;
using Malsys.Web.Models;
using MvcContrib.Pagination;

namespace Malsys.Web.Areas.Administration.Controllers {
	[Authorize(Roles = UserRoles.Administrator)]
	public partial class UsersController : Controller {

		private readonly IUsersRepository usersRepo;


		public UsersController(IUsersRepository usersRepo) {
			this.usersRepo = usersRepo;
		}


		public virtual ViewResult Index(int page = 1) {

			if (page <= 0) {
				page = 1;
			}

			return View(usersRepo.UsersDb.Users.OrderBy(u => u.NameLowercase).AsPagination(page));
		}

		public virtual ViewResult Details(int id) {
			User user = usersRepo.UsersDb.Users.SingleOrDefault(u => u.UserId == id);
			if (user == null) {
				return View(Views.Index);
			}

			return View(user);
		}


		public virtual ActionResult Edit(int id) {

			User user = usersRepo.UsersDb.Users.SingleOrDefault(u => u.UserId == id);
			if (user == null) {
				return View(Views.Index);
			}

			return View(UserDetailModel.FromUser(user));
		}

		[HttpPost]
		public virtual ActionResult Edit(UserDetailModel userModel) {

			if (ModelState.IsValid) {

				User user = usersRepo.UsersDb.Users.SingleOrDefault(u => u.UserId == userModel.UserId);
				if (user != null) {
					userModel.UpdateUser(user);
					usersRepo.UsersDb.SaveChanges();
					return RedirectToAction(Actions.Index());
				}
				else {
					ModelState.AddModelError("", "Unknown user");
				}
			}

			return View(userModel);
		}


		public virtual ActionResult Roles(int id) {

			var user = usersRepo.UsersDb.Users.SingleOrDefault(u => u.UserId == id);
			if (user == null) {
				return View(Views.Index);
			}

			var aviableRoles = usersRepo.UsersDb.Roles.ToList();
			var aviableRolesList = aviableRoles.Except(user.Roles.ToList()).ToList();

			return View(new Tuple<User, IEnumerable<Role>>(user, aviableRolesList));
		}

		[HttpPost]
		public virtual ActionResult Roles(int userId, string roleName, bool add) {

			var user = usersRepo.UsersDb.Users.SingleOrDefault(u => u.UserId == userId);
			if (user == null) {
				return View(Views.Index);
			}

			var role = usersRepo.UsersDb.Roles.SingleOrDefault(r => r.NameLowercase == roleName.Trim().ToLower());
			if (role == null) {
				return View(Views.Index);
			}

			bool success = false;
			try {
				if (add) {
					usersRepo.AddUserToRole(userId, role.RoleId);
				}
				else {
					usersRepo.RemoveUserFromRole(userId, role.RoleId);
				}
				success = true;
			}
			catch (Exception ex) {
				ModelState.AddModelError("", ex.Message);
			}

			if (success) {
				return RedirectToAction(Actions.Details(userId));
			}

			return Roles(userId);
		}

	}
}