using System.Data.Objects.DataClasses;
using System.Linq;

namespace Malsys.Web.Entities {
	public interface IFeedbackDb : IActionLogDb {

		IQueryable<Feedback> Feedbacks { get; }

		void AddFeedback(Feedback feedback);


		//int SaveChanges();

		//void Detach(object entity);
		//void Attach(IEntityWithKey entity);
	}
}
