using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace Malsys.Web.Security {
	public class Sha512PasswordHasher : IPasswordHasher {

		public void CreatePasswordHash(string password, out byte[] hash, out byte[] salt) {

			Contract.Ensures(Contract.ValueAtReturn(out hash) != null);
			Contract.Ensures(Contract.ValueAtReturn(out hash).Length == 64);

			Contract.Ensures(Contract.ValueAtReturn(out salt) != null);
			Contract.Ensures(Contract.ValueAtReturn(out salt).Length == 64);

			salt = new byte[64];
			using (var rngCsp = new RNGCryptoServiceProvider()) {
				rngCsp.GetBytes(salt);
			}

			using (var hmac = new HMACSHA512(salt)) {
				hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
			}
		}

		public bool VerifyPasswordHash(string password, byte[] hash, byte[] salt) {

			using (var hmac = new HMACSHA512(salt)) {
				var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

				if (computedHash.Length != hash.Length) {
					return false;
				}

				for (int i = 0; i < computedHash.Length; i++) {
					if (computedHash[i] != hash[i]) {
						return false;
					}
				}
			}

			return true;
		}

	}
}