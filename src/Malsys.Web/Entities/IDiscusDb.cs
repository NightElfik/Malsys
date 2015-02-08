using System.Data.Objects.DataClasses;
using System.Linq;

namespace Malsys.Web.Entities {
	public interface IDiscusDb : IActionLogDb {

		IQueryable<DiscusCategory> DiscusCategories { get; }

		void AddDiscusCategory(DiscusCategory discusCategory);


		IQueryable<DiscusThread> DiscusThreads { get; }

		void AddDiscusThread(DiscusThread discusThread);

		void DeleteDiscusThread(DiscusThread discusThread);


		IQueryable<DiscusMessage> DiscusMessages { get; }

		void AddDiscusMessage(DiscusMessage discusMessage);

		void DeleteDiscusMessage(DiscusMessage discusMessage);


		//int SaveChanges();

		//void Detach(object entity);
		//void Attach(IEntityWithKey entity);
	}
}
