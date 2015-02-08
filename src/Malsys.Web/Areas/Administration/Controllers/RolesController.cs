using System;
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Areas.Administration.Models;
using Malsys.Web.Entities;
using Malsys.Web.Models;
using MvcContrib.Pagination;

namespace Malsys.Web.Areas.Administration.Controllers {
	[Authorize(Roles = UserRoles.Administrator)]
	public partial class RolesController : Controller {

		private readonly IUsersRepository usersRepo;


		public RolesController(IUsersRepository usersRepo) {
			this.usersRepo = usersRepo;
		}

		public virtual ViewResult Index(int page = 1) {

			if (page <= 0) {
				page = 1;
			}

			return View(usersRepo.UsersDb.Roles.OrderBy(x => x.NameLowercase).AsPagination(page));
		}

		public virtual ViewResult Details(int id) {

			Role role = usersRepo.UsersDb.Roles.SingleOrDefault(r => r.RoleId == id);
			if (role == null) {
				return View(Views.Index);
			}

			return View(role);
		}


		public virtual ActionResult Create() {
			return View();
		}

		[HttpPost]
		public virtual ActionResult Create(NewRoleModel roleModel) {

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
					return RedirectToAction(Actions.Index());
				}
			}

			return View(roleModel);
		}


		public virtual ActionResult Edit(int id) {

			Role role = usersRepo.UsersDb.Roles.SingleOrDefault(r => r.RoleId == id);
			if (role == null) {
				return View(Views.Index);
			}

			return View(RoleDetailModel.FromRole(role));
		}

		[HttpPost]
		public virtual ActionResult Edit(RoleDetailModel roleModel) {

			if (ModelState.IsValid) {

				Role role = usersRepo.UsersDb.Roles.SingleOrDefault(r => r.RoleId == roleModel.RoleId);
				if (role != null) {
					roleModel.UpdateRole(role);
					usersRepo.UsersDb.SaveChanges();
					return RedirectToAction(Actions.Index());
				}
			}

			return View(roleModel);
		}
	}
}