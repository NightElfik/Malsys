// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Data.Objects.DataClasses;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Malsys.Web.Entities {
	public interface IActionLogDb {

		IQueryable<ActionLog> ActionLogs { get; }

		void AddActionLog(ActionLog actionLog);


		int SaveChanges();

		void Detach(object entity);
		void Attach(IEntityWithKey entity);
	}

	public static class IActionLogDbExtensions {

		public static void Log(this IActionLogDb db, string action, ActionLogSignificance significance, string additionalInfo = null, User user = null) {

			Contract.Requires<ArgumentNullException>(db != null);
			Contract.Requires<ArgumentNullException>(action != null);

			db.AddActionLog(new ActionLog() {
				Action = action,
				Timestamp = DateTime.Now,
				Significance = (byte)significance,
				User = user,
				AdditionalInfo = additionalInfo
			});

			db.SaveChanges();

		}

	}

	public enum ActionLogSignificance : byte {

		High = 8,
		Medium = 16,
		Low = 32,

	}
}
