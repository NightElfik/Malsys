﻿/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */
using System.Data.Objects.DataClasses;
using System.Linq;

namespace Malsys.Web.Entities {
	public interface IFeedbackDb {

		IQueryable<Feedback> Feedbacks { get; }

		void AddFeedback(Feedback feedback);


		int SaveChanges();

		void Detach(object entity);
		void Attach(IEntityWithKey entity);
	}
}
