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
	//[Authorize(Roles = "admins")]
	public class RolesController : Controller {

		private readonly IUsersRepository usersRepo;


		public RolesController(IUsersRepository usersRepo) {
			this.usersRepo = usersRepo;
		}

		public ViewResult Index() {
			return View(usersRepo.Roles.ToList());
		}

		public ViewResult Details(int id) {

			Role role = usersRepo.Roles.SingleOrDefault(r => r.RoleId == id);
			if (role == null) {
				return View("Index");
			}

			return View(role);
		}


		public ActionResult Create() {
			return View();
		}

		[HttpPost]
		public ActionResult Create(NewRoleModel roleModel) {

			if (ModelState.IsValid) {
				bool success = false;
				try {
					usersRepo.CreateRole(roleModel);
					success = true;
				}
				catch (Exception ex) {
					ModelState.AddModelError("", ex.Message);
				}

				if (success) {
					return RedirectToAction("Index");
				}
			}

			return View(roleModel);
		}


		public ActionResult Edit(int id) {

			Role role = usersRepo.Roles.SingleOrDefault(r => r.RoleId == id);
			if (role == null) {
				return View("Index");
			}

			return View(RoleDetailModel.FromRole(role));
		}

		[HttpPost]
		public ActionResult Edit(RoleDetailModel roleModel) {

			if (ModelState.IsValid) {

				Role role = usersRepo.Roles.SingleOrDefault(r => r.RoleId == roleModel.RoleId);
				if (role != null) {
					roleModel.UpdateRole(role);
					usersRepo.SaveChanges();
					return RedirectToAction("Index");
				}
			}

			return View(roleModel);
		}
	}
}