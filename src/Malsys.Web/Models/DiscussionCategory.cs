using System;
using System.Linq;
using Malsys.Web.Entities;

namespace Malsys.Web.Models {

	public enum DiscussionCategory {

		General,
		Gallery,
		News,
		DevDiary,

	}

	public static class DiscussionCategoyExtensions {

		public static string ToName(this DiscussionCategory discusCat) {
			switch (discusCat) {
				default: return Enum.GetName(typeof(DiscussionCategory), discusCat);
			}
		}

		public static DiscusCategory GetKnownDiscussCat(this IDiscusDb discusDb, DiscussionCategory discusCat) {
			string name = discusCat.ToName();
			return discusDb.DiscusCategories.SingleOrDefault(x => x.Name == name);
		}

		public static DiscusCategory GetKnownDiscussCat(this IDiscussionRepository discusRepo, DiscussionCategory discusCat) {
			string name = discusCat.ToName();
			return discusRepo.DiscusCategories.SingleOrDefault(x => x.Name == name);
		}


		public static DiscussionCategory ToDiscussCatEnum(this DiscusCategory discusCat) {
			return (DiscussionCategory)Enum.Parse(typeof(DiscussionCategory), discusCat.Name, true);
		}

		public static bool Is(this DiscusCategory cat, DiscussionCategory catEnum) {
			return string.Compare(cat.Name, catEnum.ToName(), true) == 0;
		}

	}
}
