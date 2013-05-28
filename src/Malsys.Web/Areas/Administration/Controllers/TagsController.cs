// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System.Linq;
using System.Web.Mvc;
using Malsys.Web.Areas.Administration.Models;
using Malsys.Web.Entities;

namespace Malsys.Web.Areas.Administration.Controllers {
	public partial class TagsController : Controller {

		private readonly IInputDb inputDb;


		public TagsController(IInputDb inputDb) {
			this.inputDb = inputDb;
		}


		public virtual ActionResult Index() {
			return View(inputDb.Tags);
		}


		public virtual ActionResult Edit(int id) {
			var tagEntity = inputDb.Tags.Where(t => t.TagId == id).SingleOrDefault();
			if (tagEntity == null) {
				return HttpNotFound();
			}
			return View(TagModel.FromEntity(tagEntity));
		}


		[HttpPost]
		public virtual ActionResult Edit(TagModel model) {

			if (ModelState.IsValid) {
				var tagEntity = inputDb.Tags.Where(t => t.TagId == model.TagId).SingleOrDefault();
				if (tagEntity != null) {
					model.UpdateEntity(tagEntity);
					inputDb.SaveChanges();
					return RedirectToAction(Actions.Index());
				}
				else {
					ModelState.AddModelError("", "Unknown tag");
				}
			}

			return View(model);
		}


		public virtual ActionResult Delete(int id) {
			var tagEntity = inputDb.Tags.Where(t => t.TagId == id).SingleOrDefault();
			if (tagEntity == null) {
				return HttpNotFound();
			}
			return View(tagEntity);
		}

		[HttpPost]
		public virtual ActionResult Delete(int id, string delete) {
			var tagEntity = inputDb.Tags.Where(t => t.TagId == id).SingleOrDefault();
			if (tagEntity != null && delete.ToLower() == "yes") {
				inputDb.DeleteTag(tagEntity);
				inputDb.SaveChanges();
				return RedirectToAction(Actions.Index());
			}

			return RedirectToAction(Actions.Index());
		}
	}
}
