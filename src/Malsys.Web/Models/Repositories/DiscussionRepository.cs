using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Malsys.Web.Entities;

namespace Malsys.Web.Models.Repositories {
	class DiscussionRepository : IDiscussionRepository {

		private readonly IDiscusDb discusDb;
		private readonly IUsersDb usersDb;
		private readonly IDateTimeProvider dateTimeProvider;


		public DiscussionRepository(IDiscusDb discusDb, IUsersDb usersDb, IDateTimeProvider dateTimeProvider) {

			this.discusDb = discusDb;
			this.usersDb = usersDb;
			this.dateTimeProvider = dateTimeProvider;

		}


		public IDiscusDb DiscusDb { get { return discusDb; } }


		public IQueryable<DiscusCategory> DiscusCategories { get { return discusDb.DiscusCategories; } }

		public IQueryable<DiscusThread> DiscusThreads { get { return discusDb.DiscusThreads; } }

		public IQueryable<DiscusMessage> DiscusMessages { get { return discusDb.DiscusMessages; } }


		public int SaveChanges() {
			return discusDb.SaveChanges();
		}


		public DiscusCategory CreateCategory(string name) {

			//Contract.Requires<ArgumentNullException>(name != null);

			if (string.IsNullOrWhiteSpace(name)) {
				return null;
			}
			name = name.Trim();


			var category = new DiscusCategory() {
				Name = name
			};

			discusDb.AddDiscusCategory(category);
			discusDb.SaveChanges();

			return category;
		}

		public DiscusCategory EnsureCategory(string name) {

			//Contract.Requires<ArgumentNullException>(name != null);

			if (string.IsNullOrWhiteSpace(name)) {
				return null;
			}
			name = name.Trim();

			var cat = discusDb.DiscusCategories.SingleOrDefault(x => x.Name == name);
			if (cat == null) {
				cat = CreateCategory(name);
			}

			return cat;
		}

		public DiscusThread CreateThread(int categoryId, string title, string authenticatedAuthorName, string nonRegisteredAuthorName, string threadName = null) {

			//Contract.Requires<ArgumentNullException>(title != null);

			var thread = createThreadEntity(categoryId, title, authenticatedAuthorName, nonRegisteredAuthorName, threadName);
			discusDb.AddDiscusThread(thread);
			discusDb.SaveChanges();

			return thread;
		}

		public DiscusThread CreateThreadWithFirstMessage(int categoryId, string title, string authenticatedAuthorName, string nonRegisteredAuthorName, string message, string threadName = null) {

			//Contract.Requires<ArgumentNullException>(title != null);

			var thread = createThreadEntity(categoryId, title, authenticatedAuthorName, nonRegisteredAuthorName, threadName);
			discusDb.AddDiscusThread(thread);

			var messageEntity = createMessageEntity(thread, message, authenticatedAuthorName, nonRegisteredAuthorName);
			discusDb.AddDiscusMessage(messageEntity);

			discusDb.SaveChanges();

			return thread;
		}

		public DiscusMessage AddMessage(int threadId, string text, string authenticatedAuthorName, string nonRegisteredAuthorName) {

			//Contract.Requires<ArgumentNullException>(text != null);
			//Contract.Requires<ArgumentException>(authenticatedAuthorName != null || nonRegisteredAuthorName != null);

			var threadEntity = discusDb.DiscusThreads.Where(x => x.ThreadId == threadId && !x.IsLocked).SingleOrDefault();

			if (threadEntity == null) {
				return null;
			}

			var message = createMessageEntity(threadEntity, text, authenticatedAuthorName, nonRegisteredAuthorName);
			discusDb.AddDiscusMessage(message);
			discusDb.SaveChanges();

			return message;
		}

		public DiscusMessage createMessageEntity(DiscusThread threadEntity, string text, string authenticatedAuthorName, string nonRegisteredAuthorName) {

			Contract.Requires<ArgumentNullException>(threadEntity != null);
			Contract.Requires<ArgumentNullException>(text != null);
			Contract.Requires<ArgumentException>(authenticatedAuthorName != null || nonRegisteredAuthorName != null);

			authenticatedAuthorName = StringHelper.TrimAndTreatEmptyAsNull(authenticatedAuthorName);
			nonRegisteredAuthorName = StringHelper.TrimAndTreatEmptyAsNull(nonRegisteredAuthorName);
			text = text.Trim();

			User user = null;
			if (authenticatedAuthorName != null) {
				user = usersDb.TryGetUserByName(authenticatedAuthorName);
				if (user != null) {
					nonRegisteredAuthorName = null;
				}
			}
			else if (nonRegisteredAuthorName == null) {
				return null;
			}

			var now = dateTimeProvider.Now;

			var message = new DiscusMessage() {
				DiscusThread = threadEntity,
				CreationDate = now,
				UpdateDate = now,
				User = user,
				AuthorNameNonRegistered = nonRegisteredAuthorName,
				Text = text
			};

			threadEntity.LastUpdateDate = now;

			return message;
		}

		private DiscusThread createThreadEntity(int categoryId, string title, string authenticatedAuthorName, string nonRegisteredAuthorName, string threadName = null) {

			Contract.Requires<ArgumentNullException>(title != null);

			authenticatedAuthorName = StringHelper.TrimAndTreatEmptyAsNull(authenticatedAuthorName);
			nonRegisteredAuthorName = StringHelper.TrimAndTreatEmptyAsNull(nonRegisteredAuthorName);

			if (string.IsNullOrWhiteSpace(title)) {
				return null;
			}
			title = title.Trim();
			threadName = StringHelper.TrimAndTreatEmptyAsNull(threadName);

			var cat = discusDb.DiscusCategories.Where(c => c.CategoryId == categoryId).SingleOrDefault();
			if (cat == null) {
				return null;
			}

			User user = null;
			if (authenticatedAuthorName != null) {
				user = usersDb.TryGetUserByName(authenticatedAuthorName);
				if (user != null) {
					nonRegisteredAuthorName = null;
				}
			}

			var now = dateTimeProvider.Now;

			var thread = new DiscusThread() {
				DiscusCategory = cat,
				ThreadName = threadName,
				IsLocked = false,
				CreationDate = now,
				LastUpdateDate = now,
				User = user,
				AuthorNameNonRegistered = nonRegisteredAuthorName,
				Title = title
			};

			return thread;
		}


	}
}