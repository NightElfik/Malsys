// Altairis Web Security Toolkit
// Copyright © Michal A. Valasek - Altairis, 2006-2011 | www.altairis.cz 
// Licensed under terms of Microsoft Permissive License (MS-PL)

using System.Linq;
using System.Web.Security;

namespace Altairis.Web.Security {
    internal static class ExtensionMethods {

        public static bool CheckPasswordPolicy(this MembershipProvider provider, string password) {
            // Check length
            if (string.IsNullOrWhiteSpace(password)) return false;
            if (password.Length < provider.MinRequiredPasswordLength) return false;

            // Check non-alphanumeric characters
            int count = password.ToCharArray().Count(x => !char.IsLetterOrDigit(x));
            if (count < provider.MinRequiredNonAlphanumericCharacters) return false;

            // Check regex if required
            if (string.IsNullOrWhiteSpace(provider.PasswordStrengthRegularExpression)) return true;
            return System.Text.RegularExpressions.Regex.IsMatch(password, provider.PasswordStrengthRegularExpression);
        }

    }
}
