using DataAnnotationsExtensions.ClientValidation;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Malsys.Web.App_Start.RegisterClientValidationExtensions), "Start")]

namespace Malsys.Web.App_Start {
	public static class RegisterClientValidationExtensions {
		public static void Start() {
			DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();
		}
	}
}
