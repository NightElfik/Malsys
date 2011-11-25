using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Malsys.Web.Entities;
using System.Web.Security;

namespace Malsys.Web.Controllers.Administration {
	[Authorize(Roles = "admins")]
	public class RolesController : Controller {

		private MalsysDbEntities db = new MalsysDbEntities();


		public ViewResult Index() {
			return View(db.Roles.ToList());
		}

		public ViewResult Details(string id) {
			Role role = db.Roles.Single(r => r.RoleName == id);
			return View(role);
		}


		public ActionResult Create() {
			return View();
		}

		[HttpPost]
		public ActionResult Create(Role role) {
			if (ModelState.IsValid) {
				Roles.CreateRole(role.RoleName);
				return RedirectToAction("Index");
			}

			return View(role);
		}


		public ActionResult Delete(string id) {
			Role role = db.Roles.Single(r => r.RoleName == id);
			return View(role);
		}

		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(string id) {
			Roles.DeleteRole(id);
			return RedirectToAction("Index");
		}


		protected override void Dispose(bool disposing) {
			db.Dispose();
			base.Dispose(disposing);
		}
	}
}