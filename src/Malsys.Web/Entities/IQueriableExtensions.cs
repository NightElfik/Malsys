// Copyright © 2012-2013 Marek Fišer [malsys@marekfiser.cz]
// All rights reserved.
using System;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;

namespace Malsys.Web.Entities {
	public static class IQueriableExtensions {

		public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> value, string path) where TEntity : class {

			if (value is ObjectQuery<TEntity>) {
				return ((ObjectQuery<TEntity>)value).Include(path);
			}
			else {
				return value;
			}

		}

		public static IQueryable<TEntity> Include<TEntity, TValue>(
				this IQueryable<TEntity> value, Expression<Func<TEntity, TValue>> path) where TEntity : class {

			if (value is ObjectQuery<TEntity>) {
				var expression = (MemberExpression)path.Body;
				string name = expression.Member.Name;

				return ((ObjectQuery<TEntity>)value).Include(name);
			}
			else {
				return value;
			}

		}

	}
}