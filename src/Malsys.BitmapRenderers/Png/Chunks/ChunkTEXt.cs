
namespace Malsys.BitmapRenderers.Png.Chunks {
	public class ChunkTEXt {

		public static readonly string AuthorKeyword = "Author";
		public static readonly string DescriptionKeyword = "Description";
		public static readonly string CopyrightKeyword = "Copyright";
		public static readonly string CreationTimeKeyword = "Creation Time";
		public static readonly string SoftwareKeyword = "Software";
		public static readonly string DisclaimerKeyword = "Disclaimer";
		public static readonly string WarningKeyword = "Warning";
		public static readonly string SourceKeyword = "Source";
		public static readonly string CommentKeyword = "Comment";

		public static string PredefinedKwToString(PredefinedKeyword predefinedKw) {
			switch (predefinedKw) {
				case PredefinedKeyword.Author: return AuthorKeyword;
				case PredefinedKeyword.Description: return DescriptionKeyword;
				case PredefinedKeyword.Copyright: return CopyrightKeyword;
				case PredefinedKeyword.CreationTime: return CreationTimeKeyword;
				case PredefinedKeyword.Software: return SoftwareKeyword;
				case PredefinedKeyword.Disclaimer: return DisclaimerKeyword;
				case PredefinedKeyword.Warning: return WarningKeyword;
				case PredefinedKeyword.Source: return SourceKeyword;
				case PredefinedKeyword.Comment: return CommentKeyword;
				default: return null;
			}
		}


		public static readonly string Name = "tEXt";
		public uint Length { get { return (uint)keyword.Length + (uint)text.Length + 1u; } }

		public string Keyword {
			get { return keyword; }
			set { keyword = value.Replace("\r", ""); }
		}
		private string keyword;

		public string Text {
			get { return text; }
			set { text = value.Replace("\r", ""); }
		}
		private string text;


		public ChunkTEXt(string keyword, string text) {
			Keyword = keyword;
			Text = text;
		}

		public ChunkTEXt(PredefinedKeyword predefinedKw, string text) {
			keyword = PredefinedKwToString(predefinedKw);
			Text = text;
		}


		public enum PredefinedKeyword {
			Author,
			Description,
			Copyright,
			CreationTime,
			Software,
			Disclaimer,
			Warning,
			Source,
			Comment,
		}

	}
}
