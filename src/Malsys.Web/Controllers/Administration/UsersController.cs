using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Malsys.Web.Entities;
using System.Web.Security;
using Malsys.Web.Models;

namespace Malsys.Web.Controllers.Administration {
	[Authorize(Roles="admins")]
	public class UsersController : Controller {

		private MalsysDbEntities db = new MalsysDbEntities();


		public ViewResult Index() {
			return View(db.Users.ToList());
		}

		public ViewResult Details(string id) {
			User user = db.Users.Single(u => u.UserName == id);
			return View(user);
		}


		public ActionResult Edit(string id) {
			User user = db.Users.Single(u => u.UserName == id);
			return View(UserModel.FromUser(user));
		}

		[HttpPost]
		public ActionResult Edit(UserModel user) {
			if (ModelState.IsValid) {
				User userDb = db.Users.Single(u => u.UserName == user.UserId);
				user.UpdareUser(userDb);
				db.SaveChanges();
				return RedirectToAction("Index");
			}

			return View(user);
		}


		public ActionResult Roles(string id) {

			User user = db.Users.Single(u => u.UserName == id);
			var aviableRoles = db.Roles.ToList();
			var aviableRolesList = aviableRoles.Except(user.Roles.ToList());

			return View(new Tuple<User, IEnumerable<Role>>(user, aviableRolesList));
		}

		public ActionResult AddUserRole(string userId, string roleId) {

			userId = userId.ToLower();
			roleId = roleId.ToLower();

			User user = db.Users.Single(u => u.UserName == userId);
			Role role = db.Roles.Single(r => r.RoleName == roleId);

			if (user.Roles.Where(r => r.RoleName == roleId).Count() == 0) {
				user.Roles.Add(role);
				db.SaveChanges();
			}

			return RedirectToAction("Details", new { id = userId });
		}

		public ActionResult DeleteUserRole(string userId, string roleId) {

			userId = userId.ToLower();
			roleId = roleId.ToLower();

			User user = db.Users.Single(u => u.UserName == userId);
			Role role = user.Roles.Single(r => r.RoleName == roleId);
			user.Roles.Remove(role);
			db.SaveChanges();

			return RedirectToAction("Details", new { id = userId });
		}

		protected override void Dispose(bool disposing) {
			db.Dispose();
			base.Dispose(disposing);
		}
	}
}