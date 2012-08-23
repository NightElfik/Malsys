/**
 * Copyright © 2012 Marek Fišer [malsys@marekfiser.cz]
 * All rights reserved.
 */

/// <summary>
/// Well-known user roles.
/// </summary>
/// <remarks>
/// All user role names are in lower-case to allow direct comparison with NameLowercase column on Roles table.
///
/// All these roles should be in DB at any time.
/// </remarks>
public static class UserRoles {

	public const string Administrator = "administrator";
	public const string TrustedUser = "trusted";
	public const string ViewStats = "viewstats";
	public const string ViewFeedbacks = "viewfeedbacks";
	public const string DiscusModerator = "discusmoderator";

}