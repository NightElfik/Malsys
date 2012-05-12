/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

namespace Malsys.Web.Security {
	interface IPasswordHasher {

		void CreatePasswordHash(string password, out byte[] hash, out byte[] salt);

		bool VerifyPasswordHash(string password, byte[] hash, byte[] salt);

	}
}
