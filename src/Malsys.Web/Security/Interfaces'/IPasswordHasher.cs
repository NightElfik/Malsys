﻿
namespace Malsys.Web.Security {
	internal interface IPasswordHasher {

		void CreatePasswordHash(string password, out byte[] hash, out byte[] salt);

		bool VerifyPasswordHash(string password, byte[] hash, byte[] salt);

	}
}
