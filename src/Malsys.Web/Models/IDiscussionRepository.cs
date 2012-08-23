using System.Linq;
using Malsys.Web.Entities;

namespace Malsys.Web.Models {
	public interface IDiscussionRepository {


		IDiscusDb DiscusDb { get; }


		IQueryable<DiscusCategory> DiscusCategories { get; }

		IQueryable<DiscusThread> DiscusThreads { get; }

		IQueryable<DiscusMessage> DiscusMessages { get; }


		int SaveChanges();


		DiscusCategory CreateCategory(string name);

		DiscusCategory EnsureCategory(string name);

		DiscusThread CreateThread(int categoryId, string title, string authenticatedAuthorName, string nonRegisteredAuthorName, string threadName = null);

		DiscusThread CreateThreadWithFirstMessage(int categoryId, string title, string authenticatedAuthorName, string nonRegisteredAuthorName, string message, string threadName = null);

		DiscusMessage AddMessage(int threadId, string text, string authenticatedAuthorName, string nonRegisteredAuthorName);

	}
}
