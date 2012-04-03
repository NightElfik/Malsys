
/// <summary>
/// Helper class to avoid typos in app settings keys.
/// </summary>
public class AppSettingsKeys {

	/// <summary>
	/// Working directory for input processing.
	/// </summary>
	public const string WorkDir = "WorkDir";

	/// <summary>
	/// Maximum number of process outputs before outputs are packed (zipped) into one file.
	/// This number is regulating maximum number of outputs pre request.
	/// </summary>
	public const string AutoPackTreshold = "Process_AutoPackTreshold";

	/// <summary>
	/// Maximum number of files in working directory.
	/// </summary>
	public const string MaxFilesInWorkDir = "WorkDir_MaxFilesCount";

	/// <summary>
	/// How many files are cleaned up when maximum number of files is reached.
	/// </summary>
	public const string WorkDirCleanAmount = "WorkDir_CleanAmount";

	/// <summary>
	/// Working directory for gallery (for caching thumbnails and results).
	/// </summary>
	public const string GalleryWorkDir = "GalleryWorkDir";

	/// <summary>
	/// Maximum process time for unregistered users in seconds.
	/// </summary>
	public const string UnregisteredUserProcessTime = "ProcessTime_Unregistered";

	/// <summary>
	/// Maximum process time for registered users in seconds.
	/// </summary>
	public const string RegisteredUserProcessTime = "ProcessTime_Registered";

	/// <summary>
	/// Maximum process time for gallery inputs.
	/// </summary>
	public const string GalleryProcessTime = "ProcessTime_Gallery";

	/// <summary>
	/// Maximum process time for trusted users in seconds.
	/// </summary>
	public const string TrustedUserProcessTime = "ProcessTime_Trusted";


	public const string ReCaptchaPublicKey = "ReCaptcha_PublicKey";

	public const string ReCaptchaPrivateKey = "ReCaptcha_PrivateKey";

}