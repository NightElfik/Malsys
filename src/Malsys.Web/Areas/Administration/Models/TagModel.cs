// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Web.Mvc;
using Malsys.Web.Entities;

namespace Malsys.Web.Areas.Administration.Models {
	public class TagModel {

		[Key]
		[HiddenInput(DisplayValue = false)]
		public int TagId { get; set; }

		[Required]
		[Display(Name = "Tag name")]
		public string Name { get; set; }

		[StringLength(10000)]
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }


		public static TagModel FromEntity(Tag tag) {
			var result = new TagModel() {
				TagId = tag.TagId,
				Name = tag.Name,
				Description = tag.Description
			};

			return result;
		}

		public void UpdateEntity(Tag tag) {

			Contract.Requires<InvalidOperationException>(tag.TagId == TagId);

			tag.Name = Name;
			tag.NameLowercase = Name.ToLower();
			tag.Description = Description;
		}

	}
}