using Malsys.Reflection.Components;

namespace Malsys.Processing {

	public interface IComponentMetadataResolver : IComponentTypeResolver {

		ComponentMetadata ResolveComponentMetadata(string name, IMessageLogger logger);

	}

	public static class IComponentMetadataResolverExtensions {

		private static IMessageLogger dullLogger = new DullMessageLogger();

		/// <summary>
		/// Resolver component by its type name ignoring any errors occurred.
		/// This method should be used only if there is no effective way how to
		/// show possible logged errors to user.
		/// </summary>
		/// <remarks>
		/// Ignoring error messages is valid because metadata resolver should not cache result when error occurred
		/// so errors can be logged in some other call with logger.
		/// </remarks>
		public static ComponentMetadata ResolveComponentMetadata(this IComponentMetadataResolver metadataResolver, string name) {
			return metadataResolver.ResolveComponentMetadata(name, dullLogger);
		}

	}

}
